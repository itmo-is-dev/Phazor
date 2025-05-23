using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Phazor.Reactive.Generators.Models.Effects.PropertyEffects;

public interface IEntityPropertyEffect
{
    IEnumerable<StatementSyntax> ToStatementSyntax(EffectGenerationContext context, EntityEffect entityEffect);
}