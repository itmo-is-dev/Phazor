using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators;

public static class Constants
{
    public const string ReactiveEntityMetadataName = "Phazor.Reactive.Abstractions.IReactiveEntity`1";
    public const string ReactiveEventMetadataName = "Phazor.Reactive.Abstractions.IReactiveEvent`1";
    public const string HandlerBuilderMetadataName = "Phazor.Reactive.Abstractions.ReactiveEventHandler`1";

    public const string ObservableMetadataName = "System.IObservable`1";

    public const string EnumerableMetadataName = "System.Collections.Generic.IEnumerable`1";

    public static readonly SyntaxToken EventHandlerIdentifier =
        Identifier("Phazor.Reactive.Abstractions.IReactiveEventHandler");

    public static readonly SyntaxToken EntityFactoryIdentifier =
        Identifier("Phazor.Reactive.Abstractions.IReactiveEntityFactory");

    public static readonly SyntaxToken ReactivePropertyIdentifier =
        Identifier("Phazor.Reactive.ReactiveProperty");

    public static readonly SyntaxToken ReactiveCollectionPropertyIdentifier =
        Identifier("Phazor.Reactive.ReactiveCollectionProperty");

    public static readonly SyntaxToken ReactiveFactoryBaseIdentifier =
        Identifier("Phazor.Reactive.ReactiveEntityFactory");

    public static readonly SyntaxToken ConfiguratorIdentifier =
        Identifier("Phazor.Reactive.IPhazorReactiveConfigurator");

    public static readonly SyntaxToken ObservableIdentifier =
        Identifier("System.IObservable");

    public static readonly SyntaxToken EnumerableIdentifier =
        Identifier("System.Collections.Generic.IEnumerable");

    public static readonly SyntaxToken ValueTaskIdentifier =
        Identifier("System.Threading.Tasks.ValueTask");

    public static readonly SyntaxToken CancellationTokenParameter =
        Identifier("System.Threading.CancellationToken");
}