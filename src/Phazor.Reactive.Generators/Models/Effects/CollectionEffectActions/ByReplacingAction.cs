using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Phazor.Reactive.Generators.Tools;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects.CollectionEffectActions;

public record ByReplacingAction(SimpleLambdaExpressionSyntax GetValueExpression) : ICollectionEffectAction
{
    public InvocationExpressionSyntax CreateInvocation(ExpressionSyntax field, IdentifierNameSyntax eventIdentifier)
    {
        ExpressionSyntax valueExpression = LambdaParameterReferenceSyntaxRewriter
            .RewriteToInocationResult(GetValueExpression, eventIdentifier);

        MemberAccessExpressionSyntax addMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            field,
            IdentifierName("ReplaceBy"));

        return InvocationExpression(addMemberAccess).AddArgumentListArguments(Argument(valueExpression));
    }

    public static bool TryGetAction(
        IInvocationOperation actionOperation,
        [NotNullWhen(true)] out ICollectionEffectAction? action)
    {
        action = null;

        if (actionOperation.TargetMethod.Name is not "ByReplacingWith")
            return false;

        if (actionOperation.Arguments is not
            [{ Syntax: ArgumentSyntax { Expression: SimpleLambdaExpressionSyntax valuesExpression } }])
        {
            return false;
        }

        action = new ByReplacingAction(valuesExpression);
        return true;
    }
}