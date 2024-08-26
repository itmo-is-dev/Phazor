using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Phazor.Reactive.Generators.Models.Effects.CollectionEffectActions;
using Phazor.Reactive.Generators.Models.Entities;
using Phazor.Reactive.Generators.Models.Entities.Properties;
using Phazor.Reactive.Generators.Tools;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects.PropertyEffects;

public record CollectionPropertyEffect(IPropertySymbol Property, ICollectionEffectAction Action)
    : IEntityPropertyEffect
{
    public IEnumerable<StatementSyntax> ToStatementSyntax(EffectGenerationContext context, EntityEffect entityEffect)
    {
        IdentifierNameSyntax eventIdentifier = IdentifierName("evt");

        EntityFactory factory = context.GetFactory(entityEffect.EntityType);
        Entities.ReactiveEntity entity = context.GetEntity(entityEffect.EntityType);
        IReactiveProperty property = entity.GetProperty(Property);

        IdentifierNameSyntax entityFullIdentifier = IdentifierName(entity.FullyQualifiedName);
        IdentifierNameSyntax fieldIdentifier = IdentifierName(property.BackingField.Name);

        ExpressionSyntax identifierExpression = LambdaParameterReferenceSyntaxRewriter
            .RewriteToInocationResult(entityEffect.GetIdFromEventExpression, eventIdentifier);

        MemberAccessExpressionSyntax factoryMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(factory.Field.Name),
            IdentifierName("Create"));

        InvocationExpressionSyntax getEntityInvocation = InvocationExpression(factoryMemberAccess)
            .AddArgumentListArguments(Argument(identifierExpression));

        CastExpressionSyntax concreteEntity = CastExpression(entityFullIdentifier, getEntityInvocation);

        MemberAccessExpressionSyntax filedMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            ParenthesizedExpression(concreteEntity),
            fieldIdentifier);

        InvocationExpressionSyntax invocation = Action.CreateInvocation(filedMemberAccess, eventIdentifier);

        yield return ExpressionStatement(invocation).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    public static bool TryGetEffectAction(
        IInvocationOperation actionOperation,
        Compilation compilation,
        [NotNullWhen(true)] out ICollectionEffectAction? action)
    {
        if (ByAddingAction.TryGetAction(actionOperation, out action))
            return true;

        if (ClearAction.TryGetAction(actionOperation, out action))
            return true;

        if (ByRemovingAction.TryGetAction(actionOperation, out action))
            return true;

        if (ByReplacingAction.TryGetAction(actionOperation, out action))
            return true;

        return false;
    }
}