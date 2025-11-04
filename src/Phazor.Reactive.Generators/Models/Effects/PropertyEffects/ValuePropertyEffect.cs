using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Models.Entities;
using Phazor.Reactive.Generators.Tools;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects.PropertyEffects;

public sealed record ValuePropertyEffect(IPropertySymbol Property, SimpleLambdaExpressionSyntax GetValueExpression)
    : IEntityPropertyEffect
{
    public IEnumerable<StatementSyntax> ToStatementSyntax(EffectGenerationContext context, EntityEffect entityEffect)
    {
        IdentifierNameSyntax eventIdentifier = IdentifierName("evt");

        EntityFactory factory = context.GetFactory(entityEffect.EntityType);
        ReactiveEntity entity = context.GetEntity(entityEffect.EntityType);

        IdentifierNameSyntax entityFullIdentifier = IdentifierName(entity.FullyQualifiedName);
        IdentifierNameSyntax propertyIdentifier = IdentifierName(Property.Name);

        ExpressionSyntax identifierExpression = LambdaParameterReferenceSyntaxRewriter
            .RewriteToInocationResult(entityEffect.GetIdFromEventExpression, eventIdentifier);

        MemberAccessExpressionSyntax factoryMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(factory.Field.Name),
            IdentifierName("Create"));

        InvocationExpressionSyntax getEntityInvocation = InvocationExpression(factoryMemberAccess)
            .AddArgumentListArguments(Argument(identifierExpression));

        CastExpressionSyntax concreteEntity = CastExpression(entityFullIdentifier, getEntityInvocation);

        MemberAccessExpressionSyntax propertyMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            ParenthesizedExpression(concreteEntity),
            propertyIdentifier);

        ExpressionSyntax valueExpression = LambdaParameterReferenceSyntaxRewriter
            .RewriteToInocationResult(GetValueExpression, eventIdentifier);

        AssignmentExpressionSyntax assignmentExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            propertyMemberAccess,
            valueExpression);

        yield return ExpressionStatement(assignmentExpression);
    }
}
