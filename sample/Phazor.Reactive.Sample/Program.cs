using Microsoft.Extensions.DependencyInjection;
using Phazor.Reactive;
using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Sample;
using System;
using Console = System.Console;

var collection = new ServiceCollection();

collection.AddPhazorReactive(reactive => reactive
    .AddPhazorReactiveFromPhazorReactiveSample());

var provider = collection.BuildServiceProvider();

var factory = provider.GetRequiredService<IReactiveEntityFactory<IMyEntity, long>>();
var publisher = provider.GetRequiredService<IReactiveEventPublisher>();
var eventProvider = provider.GetRequiredService<IReactiveEventProvider>();

const long id = 1;

var entity = factory.Create(id);
using var _ = entity.Collection.Subscribe(x => Console.WriteLine(string.Join(", ", x)));
using var _1 = eventProvider.Observe<MyEvent>().Subscribe(Console.WriteLine);

var evt = new MyEvent(id, 1);
await publisher.PublishAsync(evt, default);
await publisher.PublishAsync(evt, default);
await publisher.PublishAsync(evt, default);
await publisher.PublishAsync(evt, default);