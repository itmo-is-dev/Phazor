using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Effects;
using Phazor.Reactive.Generators.Models.Events;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models;

public record RegistrationExtension(
    IAssemblySymbol Assembly,
    IReadOnlyCollection<EntityFactory> Factories,
    IReadOnlyCollection<ReactiveEventHandler> EventHandlers)
{
    private const string ClassName = "PhazorReactiveConfiguratorExtensions";

    public string Name { get; } = ClassName;

    public string FullyQualifiedName { get; } = $"{Assembly.Name}.{ClassName}";

    public ClassDeclarationSyntax ToSyntax()
    {
        IdentifierNameSyntax configuratorType = IdentifierName(Constants.ConfiguratorIdentifier);

        ParameterSyntax parameter = Parameter(Identifier("configurator"))
            .WithType(configuratorType)
            .AddModifiers(Token(SyntaxKind.ThisKeyword));

        MethodDeclarationSyntax method = MethodDeclaration(configuratorType, $"AddPhazorReactiveFrom{Assembly.Name.Replace(".", "")}")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(parameter)
            .AddBodyStatements(Factories.Select(x => ToRegistrationSyntax(x, parameter.Identifier)).ToArray())
            .AddBodyStatements(EventHandlers.Select(x => ToRegistrationSyntax(x, parameter.Identifier)).ToArray())
            .AddBodyStatements(ReturnStatement(IdentifierName(parameter.Identifier)));

        return ClassDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddMembers(method);
    }

    private static StatementSyntax ToRegistrationSyntax(EntityFactory factory, SyntaxToken parameterIdentifier)
    {
        IdentifierNameSyntax entityType = IdentifierName(factory.Entity.InterfaceType.GetFullyQualifiedName());
        IdentifierNameSyntax identifierType = IdentifierName(factory.Entity.IdentifierType.GetFullyQualifiedName());
        IdentifierNameSyntax factoryType = IdentifierName(factory.FullyQualifiedName);

        GenericNameSyntax methodName = GenericName("AddEntityFactory")
            .AddTypeArgumentListArguments(entityType, identifierType, factoryType);

        MemberAccessExpressionSyntax memberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(parameterIdentifier),
            methodName);

        return ExpressionStatement(InvocationExpression(memberAccess));
    }

    private static StatementSyntax ToRegistrationSyntax(ReactiveEventHandler handler, SyntaxToken parameterIdentifier)
    {
        IdentifierNameSyntax eventType = IdentifierName(handler.Event.FullyQualifiedName);
        IdentifierNameSyntax handlerType = IdentifierName(handler.FullyQualifiedName);

        GenericNameSyntax methodName = GenericName("AddEventHandler").AddTypeArgumentListArguments(eventType, handlerType);

        MemberAccessExpressionSyntax memberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(parameterIdentifier),
            methodName);

        return ExpressionStatement(InvocationExpression(memberAccess));
    }
}