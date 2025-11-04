using Phazor.Reactive.Abstractions;
using System.Collections.Generic;

namespace Phazor.Reactive.Sample;

public record MyEvent(long Id, int Value, Guid GuidValue) : IReactiveEvent<MyEvent>
{
    public IEnumerable<int> Values { get; } = [Value];

    public static void Handle(ReactiveEventHandler<MyEvent> handler)
    {
        handler
            .Affects<IMyEntity, long>()
            .SelectedBy(x => x.Id)
            .AndItsProperty(x => x.Collection)
            .ByAdding(x => x.Value);

        handler
            .Affects<IMyEntity, long>()
            .SelectedBy(x => x.Id)
            .AndItsProperty(x => x.Value)
            .ByChangingTo(x => x.Value);

        handler
            .Affects<IMyEntity, long>()
            .SelectedBy(x => x.Id)
            .AndItsProperty(x => x.GuidValue)
            .ByChangingTo(x => x.GuidValue);
    }
}