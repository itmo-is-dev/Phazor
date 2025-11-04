using Microsoft.Extensions.DependencyInjection;
using Phazor.Reactive;
using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Sample;
using System;
using Console = System.Console;

var collection = new ServiceCollection();

collection.AddPhazorReactive(reactive => reactive
    .AddPhazorReactiveFromPhazorReactiveSample());

ServiceProvider provider = collection.BuildServiceProvider();

IReactiveEntityFactory<IMyEntity, long> factory = provider.GetRequiredService<IReactiveEntityFactory<IMyEntity, long>>();
IReactiveEventPublisher publisher = provider.GetRequiredService<IReactiveEventPublisher>();
IReactiveEventProvider eventProvider = provider.GetRequiredService<IReactiveEventProvider>();

const long id = 1;

IMyEntity entity = factory.Create(id);
using IDisposable _ = entity.Collection.Subscribe(x => Console.WriteLine(string.Join(", ", x)));
using IDisposable _1 = eventProvider.Observe<MyEvent>().Subscribe(Console.WriteLine);

var evt = new MyEvent(id, 1, Guid.NewGuid());
await publisher.PublishAsync(evt, default);
await publisher.PublishAsync(evt, default);
await publisher.PublishAsync(evt, default);
await publisher.PublishAsync(evt, default);