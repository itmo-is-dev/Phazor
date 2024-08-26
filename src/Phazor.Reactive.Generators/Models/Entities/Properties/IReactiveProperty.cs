using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Phazor.Reactive.Generators.Models.Entities.Properties;

public interface IReactiveProperty
{
    IPropertySymbol PropertySymbol { get; }

    BackingField BackingField { get; }

    IEnumerable<MemberDeclarationSyntax> ToMemberSyntax();
}