using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Effects;
using Phazor.Reactive.Generators.Models.Entities;
using Phazor.Reactive.Generators.Models.Events;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models;

public record RegistrationExtension(
    IAssemblySymbol Assembly,
    IReadOnlyList<EntityFactory> Factories,
    IReadOnlyList<ReactiveEventHandler> EventHandlers)
{
    private const string ClassName = "PhazorReactiveConfiguratorExtensions";

    public string Name { get; } = ClassName;

    public string FullyQualifiedName { get; } = $"{Assembly.Name}.{ClassName}";

    public bool TryGetSyntax([NotNullWhen(true)] out ClassDeclarationSyntax? syntax)
    {
        if (Factories is [] && EventHandlers is [])
        {
            syntax = null;
            return false;
        }

        IdentifierNameSyntax configuratorType = IdentifierName(Constants.ConfiguratorIdentifier);

        ParameterSyntax parameter = Parameter(Identifier("configurator"))
            .WithType(configuratorType)
            .AddModifiers(Token(SyntaxKind.ThisKeyword));

        string methodIdentifier = $"AddPhazorReactiveFrom{Assembly.Name.Replace(".", string.Empty)}";

        MethodDeclarationSyntax method = MethodDeclaration(configuratorType, methodIdentifier)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(parameter)
            .AddBodyStatements(Factories.Select(x => ToRegistrationSyntax(x, parameter.Identifier)).ToArray())
            .AddBodyStatements(EventHandlers.Select(x => ToRegistrationSyntax(x, parameter.Identifier)).ToArray())
            .AddBodyStatements(ReturnStatement(IdentifierName(parameter.Identifier)));

        syntax = ClassDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddMembers(method);

        return true;
    }

    private static StatementSyntax ToRegistrationSyntax(EntityFactory factory, SyntaxToken parameterIdentifier)
    {
        IdentifierNameSyntax entityType = IdentifierName(factory.Entity.InterfaceType.GetFullyQualifiedName());
        IdentifierNameSyntax identifierType = IdentifierName(factory.Entity.IdentifierType.GetFullyQualifiedName());
        IdentifierNameSyntax aliasType = IdentifierName(factory.AliasFullyQualifiedName);
        IdentifierNameSyntax factoryType = IdentifierName(factory.FullyQualifiedName);

        GenericNameSyntax methodName = GenericName("AddEntityFactory")
            .AddTypeArgumentListArguments(entityType, identifierType, aliasType, factoryType);

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

        GenericNameSyntax methodName =
            GenericName("AddEventHandler").AddTypeArgumentListArguments(eventType, handlerType);

        MemberAccessExpressionSyntax memberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(parameterIdentifier),
            methodName);

        return ExpressionStatement(InvocationExpression(memberAccess));
    }
}