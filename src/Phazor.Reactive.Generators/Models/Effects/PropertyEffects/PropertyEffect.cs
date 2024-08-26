using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Tools;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects.PropertyEffects;

public record PropertyEffect(IPropertySymbol Property, SimpleLambdaExpressionSyntax GetValueExpression)
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

        var valueExpression = LambdaParameterReferenceSyntaxRewriter
            .RewriteToInocationResult(GetValueExpression, eventIdentifier);

        var changeToMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            filedMemberAccess,
            IdentifierName("ChangeTo"));

        var changeToInvocation = InvocationExpression(changeToMemberAccess)
            .AddArgumentListArguments(Argument(valueExpression));

        yield return ExpressionStatement(changeToInvocation).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}