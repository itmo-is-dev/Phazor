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
    public string Name { get; } = GenerateTypeName(InterfaceType);

    public string FullyQualifiedName { get; } =
        $"{InterfaceType.ContainingNamespace.ToDisplayString()}.{GenerateTypeName(InterfaceType)}";

    public IReactiveProperty GetProperty(IPropertySymbol property)
        => Properties.Single(x => x.PropertySymbol.EqualsDefault(property));

    public TypeDeclarationSyntax ToSyntax()
    {
        MemberDeclarationSyntax[] members = Properties.SelectMany(x => x.ToMemberSyntax())
            .Prepend(GenerateIdentifierProperty())
            .Append(GenerateIdentifierConstructor())
            .Append(GenerateDisposeMethod())
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

        return ClassDeclaration(Name)
            .AddModifiers(
                Token(SyntaxKind.InternalKeyword),
                Token(SyntaxKind.SealedKeyword),
                Token(SyntaxKind.PartialKeyword))
            .AddBaseListTypes(SimpleBaseType(IdentifierName(InterfaceType.GetFullyQualifiedName())))
            .AddMembers(members);
    }

    private static string GenerateTypeName(INamedTypeSymbol interfaceName)
        => interfaceName.Name[0] is 'I' ? interfaceName.Name[1..] : $"{interfaceName.Name}Impl";

    private ConstructorDeclarationSyntax GenerateIdentifierConstructor()
    {
        ParameterSyntax parameter = Parameter(Identifier("id")).WithType(IdentifierName(IdentifierType.GetFullyQualifiedName()));

        AssignmentExpressionSyntax assignment = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName("Id"),
            IdentifierName("id"));

        return ConstructorDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(parameter)
            .AddBodyStatements(ExpressionStatement(assignment));
    }

    private PropertyDeclarationSyntax GenerateIdentifierProperty()
    {
        AccessorDeclarationSyntax accessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        return PropertyDeclaration(IdentifierName(IdentifierType.GetFullyQualifiedName()), "Id")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(accessor);
    }

    private MethodDeclarationSyntax GenerateDisposeMethod()
    {
        return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "Dispose")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBodyStatements(Properties.Select(GenerateDisposeInvocation).ToArray());
    }

    private StatementSyntax GenerateDisposeInvocation(IReactiveProperty property)
    {
        IdentifierNameSyntax fieldIdentifier = IdentifierName(property.BackingField.Name);
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
}