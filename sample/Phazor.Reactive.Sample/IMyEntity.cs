using Phazor.Reactive.Abstractions;
using System;
using System.Collections.Generic;

namespace Phazor.Reactive.Sample;

public interface IMyEntity : IReactiveEntity<long>
{
    IObservable<int> Value { get; }
    IObservable<IEnumerable<int>> Collection { get; }
    IObservable<IEnumerable<IOtherEntity>> OtherEntities { get; }
}