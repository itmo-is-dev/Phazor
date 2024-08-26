using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Phazor.Reactive.Generators.Models.Effects.CollectionEffectActions;
using Phazor.Reactive.Generators.Tools;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects.PropertyEffects;

public record CollectionPropertyEffect(IPropertySymbol Property, ICollectionEffectAction Action)
    : IEntityPropertyEffect
{
    public IEnumerable<StatementSyntax> ToStatementSyntax(EffectGenerationContext context, EntityEffect entityEffect)
    {
        var eventIdentifier = IdentifierName("evt");

        var factory = context.GetFactory(entityEffect.EntityType);
        var entity = context.GetEntity(entityEffect.EntityType);
        var property = entity.GetProperty(Property);

        var entityFullIdentifier = IdentifierName(entity.FullyQualifiedName);
        var fieldIdentifier = IdentifierName(property.BackingField.Name);

        var identifierExpression = LambdaParameterReferenceSyntaxRewriter
            .RewriteToInocationResult(entityEffect.GetIdFromEventExpression, eventIdentifier);

        var factoryMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(factory.Field.Name),
            IdentifierName("Create"));

        var getEntityInvocation = InvocationExpression(factoryMemberAccess)
            .AddArgumentListArguments(Argument(identifierExpression));

        var concreteEntity = CastExpression(entityFullIdentifier, getEntityInvocation);

        var filedMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            ParenthesizedExpression(concreteEntity),
            fieldIdentifier);

        var invocation = Action.CreateInvocation(filedMemberAccess, eventIdentifier);

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