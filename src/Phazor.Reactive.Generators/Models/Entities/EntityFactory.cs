using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Entities;

public record EntityFactory(ReactiveEntity Entity)
{
    private readonly IdentifierNameSyntax _entityInterfaceType =
        IdentifierName(Entity.InterfaceType.GetFullyQualifiedName());

    private readonly IdentifierNameSyntax _identifierType =
        IdentifierName(Entity.IdentifierType.GetFullyQualifiedName());

    public BackingField Field { get; } = BackingField.ForEntityFactory(Entity);

    public string Name { get; } = $"{Entity.Name}Factory";
    public string FullyQualifiedName { get; } = $"{Entity.FullyQualifiedName}Factory";

    public string AliasName { get; } = $"{Entity.InterfaceType.Name}Factory";

    public string AliasFullyQualifiedName { get; } = $"{Entity.InterfaceType.GetFullyQualifiedName()}Factory";

    public ClassDeclarationSyntax ToFactorySyntax(GeneratorExecutionContext context)
    {
        GenericNameSyntax baseType = GenericName(Constants.ReactiveFactoryBaseIdentifier)
            .AddTypeArgumentListArguments(_entityInterfaceType)
            .AddTypeArgumentListArguments(_identifierType);


        return ClassDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.SealedKeyword))
            .AddBaseListTypes(SimpleBaseType(baseType))
            .AddBaseListTypes(SimpleBaseType(IdentifierName(AliasFullyQualifiedName)))
            .AddMembers(GenerateFields(context).ToArray())
            .AddMembers(GenerateConstructor(context).ToArray())
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

    private IEnumerable<MemberDeclarationSyntax> GenerateFields(GeneratorExecutionContext context)
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
            string typeName = parameter.Type.GetFullyQualifiedName();
            VariableDeclaratorSyntax declarator = VariableDeclarator(Identifier(parameter.Name));

            VariableDeclarationSyntax declaration = VariableDeclaration(IdentifierName(typeName))
                .AddVariables(declarator);

            yield return FieldDeclaration(declaration)
                .AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));
        }
    }

    private IEnumerable<MemberDeclarationSyntax> GenerateConstructor(GeneratorExecutionContext context)
    {
        INamedTypeSymbol? existingType = context.Compilation.GetTypeByMetadataName(Entity.FullyQualifiedName);

        if (existingType is not { Constructors: [{ } constructor, ..] })
            yield break;

        ParameterSyntax[] parameters = constructor.Parameters
            .Where(x => x.Type.EqualsDefault(Entity.IdentifierType) is false)
            .Select(parameter => parameter.ToParameterSyntax())
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
}