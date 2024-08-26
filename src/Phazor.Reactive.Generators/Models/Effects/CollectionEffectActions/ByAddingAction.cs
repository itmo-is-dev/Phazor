using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Phazor.Reactive.Generators.Tools;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects.CollectionEffectActions;

public record ByAddingAction(SimpleLambdaExpressionSyntax GetValueExpression) : ICollectionEffectAction
{
    public InvocationExpressionSyntax CreateInvocation(ExpressionSyntax field, IdentifierNameSyntax eventIdentifier)
    {
        var valueExpression = LambdaParameterReferenceSyntaxRewriter
            .RewriteToInocationResult(GetValueExpression, eventIdentifier);

        var addMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            field,
            IdentifierName("Add"));

        return InvocationExpression(addMemberAccess).AddArgumentListArguments(Argument(valueExpression));
    }

    public static bool TryGetAction(
        IInvocationOperation actionOperation,
        [NotNullWhen(true)] out ICollectionEffectAction? action)
    {
        action = null;

        if (actionOperation.TargetMethod.Name is not "ByAdding")
            return false;

        if (actionOperation.Arguments is not
            [{ Syntax: ArgumentSyntax { Expression: SimpleLambdaExpressionSyntax valuesExpression } }])
        {
            return false;
        }

        action = new ByAddingAction(valuesExpression);
        return true;
    }
}