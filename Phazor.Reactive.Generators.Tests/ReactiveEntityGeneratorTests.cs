using Microsoft.CodeAnalysis.Testing;
using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Generators.Generators;
using Phazor.Reactive.Generators.Tests.Common;
using Xunit;

namespace Phazor.Reactive.Generators.Tests;

public class ReactiveEntityGeneratorTests : GeneratorTestBase<ReactiveEntityGenerator>
{
    [Fact]
    public async Task GenerateEntity()
    {
        var source = new SourceFile(
            Name: "IMyEntity.cs",
            Content: """
             using Phazor.Reactive.Abstractions;
             using System;
             using System.Collections.Generic;
             
             namespace Phazor.Reactive.Sample;
             
             public interface IMyEntity : IReactiveEntity<long>
             {
                 IObservable<int> Value { get; }
                 IObservable<IEnumerable<int>> Collection { get; }
             }
             """);

        await GeneratorTest
            .WithSource(source)
            .WithAdditionalReference(typeof(IReactiveEntity<>).Assembly)
            .WithAdditionalReference(typeof(ReactiveProperty<>).Assembly)
            .WithReferenceAssemblies(ReferenceAssemblies.Net.Net60)
            .Build()
            .RunAsync();
    }

    [Fact]
    public async Task GenerateEventHandler()
    {
        var entitySource = new SourceFile(
            Name: "IMyEntity.cs",
            Content: """
             using Phazor.Reactive.Abstractions;
             using System;
             using System.Collections.Generic;
             
             namespace Phazor.Reactive.Sample;
             
             public interface IMyEntity : IReactiveEntity<long>
             {
                 IObservable<int> Value { get; }
                 IObservable<IEnumerable<int>> Collection { get; }
             }
             """);
        
        var source = new SourceFile(
            Name: "MyEvent.cs",
            Content: """
             using Phazor.Reactive.Abstractions;
             
             namespace Phazor.Reactive.Sample;
             
             public record MyEvent(long Id, int Value) : IReactiveEvent<MyEvent>
             {
                 public static void Handle(ReactiveEventHandler<MyEvent> handler)
                 {
                     handler
                         .Affects<IMyEntity, long>()
                         .SelectedBy(x => x.Id)
                         .AndItsProperty(x => x.Value)
                         .ByChangingTo(x => x.Value);
                 }
             }
             """);

        await GeneratorTest
            .WithSource(entitySource)
            .WithSource(source)
            .WithAdditionalReference(typeof(IReactiveEntity<>).Assembly)
            .WithAdditionalReference(typeof(ReactiveProperty<>).Assembly)
            .WithReferenceAssemblies(ReferenceAssemblies.Net.Net60)
            .Build()
            .RunAsync();
    }
}