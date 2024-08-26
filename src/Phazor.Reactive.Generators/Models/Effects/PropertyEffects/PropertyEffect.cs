using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Models.Entities.Properties;
using Phazor.Reactive.Generators.Tools;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects.PropertyEffects;

public record PropertyEffect(IPropertySymbol Property, SimpleLambdaExpressionSyntax GetValueExpression)
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

        ExpressionSyntax valueExpression = LambdaParameterReferenceSyntaxRewriter
            .RewriteToInocationResult(GetValueExpression, eventIdentifier);

        MemberAccessExpressionSyntax changeToMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            filedMemberAccess,
            IdentifierName("ChangeTo"));

        InvocationExpressionSyntax changeToInvocation = InvocationExpression(changeToMemberAccess)
            .AddArgumentListArguments(Argument(valueExpression));

        yield return ExpressionStatement(changeToInvocation).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}