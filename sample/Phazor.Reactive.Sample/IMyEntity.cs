using Phazor.Reactive.Abstractions;
using System;
using System.Collections.Generic;

namespace Phazor.Reactive.Sample;

public interface IMyEntity : IReactiveEntity<long>
{
    Guid GuidValue { get; }
    IObservable<int> Value { get; }
    IObservable<int?> NullableValue { get; }
    IObservable<string?> NullableReferenceValue { get; }
    IObservable<IEnumerable<int>> Collection { get; }
    IObservable<IEnumerable<IOtherEntity>> OtherEntities { get; }
}
