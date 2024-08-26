using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Effects;
using Phazor.Reactive.Generators.Models.Entities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Events;

public record ReactiveEventHandler(ReactiveEvent Event)
{
    public string Name { get; } = $"{Event.Name}Handler";

    public string FullyQualifiedName { get; } = $"{Event.FullyQualifiedName}Handler";

    public ClassDeclarationSyntax ToSyntax(IEnumerable<ReactiveEntity> entities)
    {
        var baseType = GenericName(Constants.EventHandlerIdentifier)
            .AddTypeArgumentListArguments(IdentifierName(Event.FullyQualifiedName));

        var context = new EffectGenerationContext(entities);

        var method = GenerateHandleMethod(context);

        return ClassDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.InternalKeyword))
            .AddBaseListTypes(SimpleBaseType(baseType))
            .AddMembers(context.ToMemberSyntax(this).ToArray())
            .AddMembers(method);
    }

    private MemberDeclarationSyntax GenerateHandleMethod(EffectGenerationContext context)
    {
        var eventParameter = Parameter(Identifier("evt")).WithType(IdentifierName(Event.FullyQualifiedName));

        var cancellationTokenParameter = Parameter(Identifier("cancellationToken"))
            .WithType(IdentifierName(Constants.CancellationTokenParameter));

        var completedTask = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(Constants.ValueTaskIdentifier),
            IdentifierName("CompletedTask"));

        return MethodDeclaration(IdentifierName(Constants.ValueTaskIdentifier), "HandleAsync")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(eventParameter, cancellationTokenParameter)
            .AddBodyStatements(Event.Effects.SelectMany(x => x.ToStatementSyntax(context)).ToArray())
            .AddBodyStatements(ReturnStatement(completedTask));
    }
}