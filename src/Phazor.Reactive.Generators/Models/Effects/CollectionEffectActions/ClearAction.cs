using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects.CollectionEffectActions;

public record ClearAction : ICollectionEffectAction
{
    public InvocationExpressionSyntax CreateInvocation(ExpressionSyntax field, IdentifierNameSyntax eventIdentifier)
    {
        var addMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            field,
            IdentifierName("Clear"));

        return InvocationExpression(addMemberAccess);
    }

    public static bool TryGetAction(
        IInvocationOperation actionOperation,
        [NotNullWhen(true)] out ICollectionEffectAction? action)
    {
        action = actionOperation.TargetMethod.Name is "ByClearing"
            ? new ClearAction()
            : null;

        return action is not null;
    }
}