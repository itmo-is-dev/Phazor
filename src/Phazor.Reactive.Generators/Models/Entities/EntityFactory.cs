using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Entities;

public record EntityFactory(ReactiveEntity Entity)
{
    private readonly TypeSyntax _entityInterfaceType = Entity.InterfaceType.ToNameSyntax();
    private readonly TypeSyntax _identifierType = Entity.IdentifierType.ToNameSyntax();

    public BackingField Field { get; } = BackingField.ForEntityFactory(Entity);

    public string Name { get; } = $"{Entity.Name}Factory";
    public string FullyQualifiedName { get; } = $"{Entity.FullyQualifiedName}Factory";

    public string AliasName { get; } = $"{Entity.InterfaceType.Name}Factory";

    public string AliasFullyQualifiedName { get; } = $"{Entity.InterfaceType.GetFullyQualifiedName()}Factory";

    public ClassDeclarationSyntax ToFactorySyntax(
        GeneratorExecutionContext context,
        IReadOnlyCollection<ReactiveEntityContext> entityContexts)
    {
        GenericNameSyntax baseType = GenericName(Constants.ReactiveFactoryBaseIdentifier)
            .AddTypeArgumentListArguments(_entityInterfaceType)
            .AddTypeArgumentListArguments(_identifierType);

        return ClassDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.SealedKeyword))
            .AddBaseListTypes(SimpleBaseType(baseType))
            .AddBaseListTypes(SimpleBaseType(IdentifierName(AliasFullyQualifiedName)))
            .AddMembers(GenerateFields(context, entityContexts).ToArray())
            .AddMembers(GenerateConstructor(context, entityContexts).ToArray())
            .AddMembers(GenerateCreateMethod(context));
    }

    public InterfaceDeclarationSyntax ToFactoryAliasSyntax()
    {
        GenericNameSyntax baseType = GenericName(Constants.EntityFactoryIdentifier)
            .AddTypeArgumentListArguments(IdentifierName(Entity.InterfaceType.GetFullyQualifiedName()))
            .AddTypeArgumentListArguments(IdentifierName(Entity.IdentifierType.GetFullyQualifiedName()));

        return InterfaceDeclaration(AliasName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SimpleBaseType(baseType));
    }

    private IEnumerable<MemberDeclarationSyntax> GenerateFields(
        GeneratorExecutionContext context,
        IReadOnlyCollection<ReactiveEntityContext> entityContexts)
    {
        INamedTypeSymbol? existingType = context.Compilation.GetTypeByMetadataName(Entity.FullyQualifiedName);

        if (existingType is not { Constructors: [{ } constructor, ..] })
            yield break;

        IParameterSymbol[] parameters = constructor.Parameters
            .Where(x => x.Type.EqualsDefault(Entity.IdentifierType) is false)
            .ToArray();

        if (parameters is [])
            yield break;

        foreach (IParameterSymbol parameter in parameters)
        {
            VariableDeclaratorSyntax declarator = VariableDeclarator(Identifier(parameter.Name));

            TypeSyntax fieldType =
                parameter.Type is IErrorTypeSymbol errorTypeSymbol
                && TryFindEntityTypeName(errorTypeSymbol, entityContexts, out TypeSyntax? typeName)
                    ? typeName
                    : parameter.Type.ToNameSyntax();

            VariableDeclarationSyntax declaration = VariableDeclaration(fieldType)
                .AddVariables(declarator);

            yield return FieldDeclaration(declaration)
                .AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));
        }
    }

    private IEnumerable<MemberDeclarationSyntax> GenerateConstructor(
        GeneratorExecutionContext context,
        IReadOnlyCollection<ReactiveEntityContext> entityContexts)
    {
        INamedTypeSymbol? existingType = context.Compilation.GetTypeByMetadataName(Entity.FullyQualifiedName);

        if (existingType is not { Constructors: [{ } constructor, ..] })
            yield break;

        ParameterSyntax[] parameters = constructor.Parameters
            .Where(x => x.Type.EqualsDefault(Entity.IdentifierType) is false)
            .Select(MapToParameterSyntax)
            .ToArray();

        StatementSyntax[] statements = constructor.Parameters
            .Where(x => x.Type.EqualsDefault(Entity.IdentifierType) is false)
            .Select(ToAssignment)
            .Select(StatementSyntax (x) => ExpressionStatement(x))
            .ToArray();

        yield return ConstructorDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(parameters)
            .AddBodyStatements(statements);

        yield break;

        static ExpressionSyntax ToAssignment(IParameterSymbol parameter)
        {
            MemberAccessExpressionSyntax memberAccess = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                ThisExpression(),
                IdentifierName(parameter.Name));

            return AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                memberAccess,
                IdentifierName(parameter.Name));
        }

        ParameterSyntax MapToParameterSyntax(IParameterSymbol parameter)
        {
            TypeSyntax fieldType =
                parameter.Type is IErrorTypeSymbol errorTypeSymbol
                && TryFindEntityTypeName(errorTypeSymbol, entityContexts, out TypeSyntax? typeName)
                    ? typeName
                    : parameter.Type.ToNameSyntax();

            return Parameter(Identifier(parameter.Name)).WithType(fieldType);
        }
    }

    private MemberDeclarationSyntax GenerateCreateMethod(GeneratorExecutionContext context)
    {
        INamedTypeSymbol? existingType = context.Compilation.GetTypeByMetadataName(Entity.FullyQualifiedName);

        ArgumentSyntax[] arguments;

        if (existingType is { Constructors: [{ } constructor, ..] })
        {
            arguments = constructor.Parameters
                .Select(parameter =>
                {
                    if (parameter.Type.EqualsDefault(Entity.IdentifierType))
                        return Argument(IdentifierName("id"));

                    MemberAccessExpressionSyntax memberAccess = MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName(parameter.Name));

                    return Argument(memberAccess);
                })
                .ToArray();
        }
        else
        {
            arguments = [Argument(IdentifierName("id"))];
        }

        ObjectCreationExpressionSyntax objectCreation = ObjectCreationExpression(IdentifierName(Entity.Name))
            .AddArgumentListArguments(arguments);

        return MethodDeclaration(_entityInterfaceType, "CreateInternal")
            .AddModifiers(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(Parameter(Identifier("id")).WithType(_identifierType))
            .AddBodyStatements(ReturnStatement(objectCreation));
    }

    private static bool TryFindEntityTypeName(
        IErrorTypeSymbol symbol,
        IReadOnlyCollection<ReactiveEntityContext> entityContexts,
        [NotNullWhen(true)] out TypeSyntax? typeName)
    {
        ReactiveEntityContext? context = entityContexts
            .SingleOrDefault(x => symbol.Name.Equals(x.Factory.AliasName, StringComparison.OrdinalIgnoreCase));

        if (context is null)
        {
            typeName = null;
            return false;
        }

        typeName = IdentifierName(context.Factory.AliasFullyQualifiedName);
        return true;
    }
}