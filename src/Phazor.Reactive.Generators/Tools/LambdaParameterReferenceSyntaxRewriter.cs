using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Tools;

public class LambdaParameterReferenceSyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly IdentifierNameSyntax _sourceIdentifier;
    private readonly IdentifierNameSyntax _targetIdentifier;

    public LambdaParameterReferenceSyntaxRewriter(
        IdentifierNameSyntax sourceIdentifier,
        IdentifierNameSyntax targetIdentifier)
    {
        _sourceIdentifier = sourceIdentifier;
        _targetIdentifier = targetIdentifier;
    }

    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        if (node.Expression is not IdentifierNameSyntax expressionIdentifier)
            return base.VisitMemberAccessExpression(node);

        if (expressionIdentifier.Identifier.Text != _sourceIdentifier.Identifier.Text)
            return base.VisitMemberAccessExpression(node);

        return node.WithExpression(_targetIdentifier);
    }

    public static ExpressionSyntax RewriteToInocationResult(
        SimpleLambdaExpressionSyntax lambda,
        IdentifierNameSyntax to)
    {
        var rewriter = new LambdaParameterReferenceSyntaxRewriter(
            IdentifierName(lambda.Parameter.Identifier),
            to);

        return (ExpressionSyntax)lambda.Body.Accept(rewriter)!;
    }
}