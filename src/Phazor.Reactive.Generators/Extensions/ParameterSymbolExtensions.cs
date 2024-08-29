using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Extensions;

public static class ParameterSymbolExtensions
{
    public static ParameterSyntax ToParameterSyntax(this IParameterSymbol parameter)
        => Parameter(Identifier(parameter.Name)).WithType(IdentifierName(parameter.Type.GetFullyQualifiedName()));
}