using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Phazor.Reactive.Generators.Models.Effects.CollectionEffectActions;

public interface ICollectionEffectAction
{
    InvocationExpressionSyntax CreateInvocation(ExpressionSyntax field, IdentifierNameSyntax eventIdentifier);
}