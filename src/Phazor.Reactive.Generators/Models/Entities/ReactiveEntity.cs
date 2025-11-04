using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Entities.Properties;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Entities;

public record ReactiveEntity(
    INamedTypeSymbol InterfaceType,
    INamedTypeSymbol IdentifierType,
    IReadOnlyCollection<IReactiveProperty> Properties)
{
    private static readonly SyntaxToken DisposableFieldName = Identifier("_disposable");

    public string Name { get; } = GenerateTypeName(InterfaceType);

    public string FullyQualifiedName { get; } =
        $"{InterfaceType.ContainingNamespace.ToDisplayString()}.{GenerateTypeName(InterfaceType)}";

    public IReactiveProperty GetProperty(IPropertySymbol property)
        => Properties.Single(x => x.PropertySymbol.EqualsDefault(property));

    public TypeDeclarationSyntax ToSyntax(GeneratorExecutionContext context)
    {
        MemberDeclarationSyntax[] members = Properties
            .SelectMany(x => x.ToMemberSyntax(context))
            .Prepend(GenerateIdentifierProperty())
            .Concat(GenerateIdentifierConstructor(context))
            .Append(GenerateDisposableField())
            .Append(GenerateDisposeMethod())
            .Append(GenerateTypedEqualsMethod())
            .Append(GenerateUntypedEqualsMethod())
            .Append(GenerateGetHashCodeMethod())
            .Append(GenerateDestructorSyntax())
            .OrderBy(member => member switch
            {
                FieldDeclarationSyntax => 10,
                ConstructorDeclarationSyntax => 20,
                DestructorDeclarationSyntax => 21,
                PropertyDeclarationSyntax => 30,
                MethodDeclarationSyntax => 40,
                _ => int.MaxValue,
            })
            .ToArray();

        GenericNameSyntax equatableType = GenericName(Constants.EquatableIdentifier)
            .AddTypeArgumentListArguments(IdentifierName(Name));

        return ClassDeclaration(Name)
            .AddModifiers(
                Token(SyntaxKind.InternalKeyword),
                Token(SyntaxKind.SealedKeyword),
                Token(SyntaxKind.PartialKeyword))
            .AddBaseListTypes(
                SimpleBaseType(IdentifierName(InterfaceType.GetFullyQualifiedName())),
                SimpleBaseType(equatableType))
            .AddMembers(members);
    }

    private static string GenerateTypeName(INamedTypeSymbol interfaceName)
        => interfaceName.Name[0] is 'I' ? interfaceName.Name[1..] : $"{interfaceName.Name}Impl";

    private IEnumerable<ConstructorDeclarationSyntax> GenerateIdentifierConstructor(GeneratorExecutionContext context)
    {
        INamedTypeSymbol? existingType = context.Compilation.GetTypeByMetadataName(FullyQualifiedName);

        if (existingType is { Constructors: not [] })
            yield break;

        ParameterSyntax parameter = Parameter(Identifier("id")).WithType(IdentifierType.ToNameSyntax());

        AssignmentExpressionSyntax assignment = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName("Id"),
            IdentifierName("id"));

        yield return ConstructorDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(parameter)
            .AddBodyStatements(ExpressionStatement(assignment));
    }

    private PropertyDeclarationSyntax GenerateIdentifierProperty()
    {
        AccessorDeclarationSyntax accessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        return PropertyDeclaration(IdentifierType.ToNameSyntax(), "Id")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(accessor);
    }

    private FieldDeclarationSyntax GenerateDisposableField()
    {
        MemberAccessExpressionSyntax disposableEmpty = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(Constants.Disposable),
            IdentifierName("Empty"));

        EqualsValueClauseSyntax assignment = EqualsValueClause(disposableEmpty);
        VariableDeclaratorSyntax declarator = VariableDeclarator(DisposableFieldName).WithInitializer(assignment);

        VariableDeclarationSyntax declaration = VariableDeclaration(IdentifierName(Constants.IDisposable))
            .AddVariables(declarator);

        return FieldDeclaration(declaration).AddModifiers(Token(SyntaxKind.PrivateKeyword));
    }

    private MethodDeclarationSyntax GenerateDisposeMethod()
    {
        StatementSyntax[] invocations = Properties
            .Select(prop => prop.BackingField)
            .Where(field => field.IsDisposable)
            .Select(field => GenerateDisposeInvocation(IdentifierName(field.Name)))
            .Append(GenerateDisposeInvocation(IdentifierName(DisposableFieldName)))
            .ToArray();

        return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "Dispose")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBodyStatements(invocations);
    }

    private StatementSyntax GenerateDisposeInvocation(IdentifierNameSyntax fieldIdentifier)
    {
        IdentifierNameSyntax methodIdentifier = IdentifierName("Dispose");

        MemberAccessExpressionSyntax memberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            fieldIdentifier,
            methodIdentifier);

        return ExpressionStatement(InvocationExpression(memberAccess));
    }

    private DestructorDeclarationSyntax GenerateDestructorSyntax()
    {
        return DestructorDeclaration(Identifier(Name))
            .AddBodyStatements(ExpressionStatement(InvocationExpression(IdentifierName("Dispose"))));
    }

    private MethodDeclarationSyntax GenerateTypedEqualsMethod()
    {
        const string parameterName = "other";

        ParameterSyntax parameter = Parameter(Identifier(parameterName)).WithType(NullableType(IdentifierName(Name)));

        BinaryExpressionSyntax expression = BinaryExpression(
            SyntaxKind.EqualsExpression,
            IdentifierName("Id"),
            ConditionalAccessExpression(IdentifierName(parameterName), MemberBindingExpression(IdentifierName("Id"))));

        return MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("Equals"))
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(parameter)
            .AddBodyStatements(ReturnStatement(expression));
    }

    private MethodDeclarationSyntax GenerateUntypedEqualsMethod()
    {
        const string parameterName = "obj";

        NullableTypeSyntax parameterType = NullableType(PredefinedType(Token(SyntaxKind.ObjectKeyword)));

        BinaryExpressionSyntax expression = BinaryExpression(
            SyntaxKind.AsExpression,
            IdentifierName("obj"),
            IdentifierName(Name));

        InvocationExpressionSyntax invocation = InvocationExpression(IdentifierName("Equals"))
            .AddArgumentListArguments(Argument(expression));

        return MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("Equals"))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(Parameter(Identifier(parameterName)).WithType(parameterType))
            .AddBodyStatements(ReturnStatement(invocation));
    }

    private MethodDeclarationSyntax GenerateGetHashCodeMethod()
    {
        MemberAccessExpressionSyntax memberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("Id"),
            IdentifierName("GetHashCode"));

        return MethodDeclaration(PredefinedType(Token(SyntaxKind.IntKeyword)), Identifier("GetHashCode"))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
            .AddBodyStatements(ReturnStatement(InvocationExpression(memberAccess)));
    }
}
