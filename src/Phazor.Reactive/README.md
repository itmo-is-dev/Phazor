# Phazor.Reactive

A package that allows to define and generate reactive entities and events to use in Blazor applications with Phazor
components.

## Idea

Reactive components are useful when you want your UI to respond to some state changes in the application. Such
components are often represent some entities in your application, so, if you want to represent entity state reactively –
your entities must also be reactive.

In .NET and Blazor realities, for entity to be reactive means that it's data should be represented as `IObservable`, so
your code can subscribe to its data changes. Hand rolling a reactive entity is not that hard, but, as number of entities
and their properties grow, it can become complex and challenging to maintain.

This is when Phazor.Reactive comes into play. It provides abstractions to describe your reactive entities, events
that can affect them as well as source generators, that implement all the entities and event handlers. When using this
library, creating reactive entities and their behaviour boils down to defining interfaces and describing what effects
are made by events.

## Glossary

- Reactive entity (entity) – an interface that derives from `IReactiveEntity<TIdentifier>`
- Reactive event (event) – a type that implements `IReactiveEvent<TSelf>` and describes how it should be handled
- Effect – a part of event handling description, that results to entity change
- Entity factory – an abstraction used to create entity instances

## Defining an entity

In Phazor.Reactive concept, a reactive entity is a binding to some instance of an entity. What does that mean? As all
the state of an entity is reactive, you do not need all the data to create it, it can be acquired later with events. The
only required data is its identity – identifier. So, being a binging means that it represents an instance which data may
not be loaded yet, but it will be there eventually.

As Phazor.Reactive generates an implementations for your entities, reactive entities are defined as interfaces, that
derive from `IReactiveEntity<TIdentifier>`. Base interface itself provides an `TIdentifier Id` property.

```csharp
public interface IMyEntity : IReactiveEntity<long>;
```

### Reactive properties

To define reactive state you must add a property with `IObservable<>` type, Phazor.Reactive also allows for reactive
collection properties with `IObservable<IEnumerable<>>` type (no other collection type is supported). These properties
must be defined with get accessor only.

```csharp
public interface IMyEntity : IReactiveEntity<long>
{
    IObservable<int> Value { get; }
    
    IObservable<IEnumerable<string>> Collection { get; }
}
```

Behind the scenes, Phazor.Reactive will generate implementation for this type, wiring all the observables. The library
will only generate observable properties in the implementation, but if you need to implement any custom properties or
methods, you can add them by creating another part of a class as it is generated as `partial`.

### Defining your own parts

As Phazor.Reactive generates entity classes as partial, you can define your own parts of implementation (for example for
implementing some logic). In case of you needing some additional state (some services for example) you can add a custom
constructor in your part of an entity. Be aware that in that case, the library won't generate a constructor, so you
would have to initialize `Id` property manually.

All the parameters beside the id will be injected from DI, see [factory section](#factories-for-partial-entities) for
more
info.

## Reactive events

After you defined your reactive entity, you need some way to update its state, and this (and only) way to do that is
reactive events. Reactive event is a type, that implements an `IReactiveEvent<TSelf>` interface. Event can be of any
type – class, struct, record, record struct. This interface requires you to implement a single method to describe how
event should be handled.

```csharp
public record MyEvent(long Id, int Value) : IReactiveEvent<MyEvent>
{
    public static void Handle(ReactiveEventHandler<MyEvent> handler) { }
}
```

This method has a single argument of type `ReactiveEventHandler<>` that is used to describe which effects event has on
an application state. This type is abstract, and does not have any implementations, its method calls will be used by
generators.

Defining an effect requires several method calls:

1. Selecting affected entity using `.Affects<TEntity, TIdentifier>` method
2. Specifying entity selector using `.SelectedBy` method. This method accepts an expression that gets an identifier
   value from event
3. Selecting entity property using `.AndItsProperty` method. This method accepts an expression that gets a property from
   entity. You should only pass a simple member access expressions to this method.

```csharp
public static void Handle(ReactiveEventHandler<MyEvent> handler)
{
    handler
        .Affects<IMyEntity, long>()
        .SelectedBy(x => x.Id)
        .AndItsProperty(x => x.Collection);
}
```

After you specify a target for your event effect, you have to specify the effect itself. There are two kind of effects:
property effects and collection property effects.

### Property effects

A property effect is an effect on non-collection property. This kind of effect allows only changing property value, it
may be interpreted as a setter of some sort.

To define a property effect, you should select a non-collection property and call `.ByChangingTo` method. This method
accepts an expression that gets a value from event.

```csharp
public static void Handle(ReactiveEventHandler<MyEvent> handler)
{
    handler
        .Affects<IMyEntity, long>()
        .SelectedBy(x => x.Id)
        .AndItsProperty(x => x.Value)
        .ByChangingTo(x => x.Value);
}
```

### Collection effects

Collection effects are effects on collection properties (ei `IObservable<IEnumerable<>>` properties). This kind of
effect allows several collection transformation

- Add value: using `.ByAdding` method. This method accepts an expression that gets either a single value or collection
  from event.
- Remove value: using `.ByRemoving` method. This method accepts an expression that gets either a single value or
  collection from event.
- Replace values: using `.ByReplacingWith` method. This method accepts an expression that gets either single value or
  collection from event.
- Clear values: using `.ByClearing` method.

```csharp
public static void Handle(ReactiveEventHandler<MyEvent> handler)
{
    handler
        .Affects<IMyEntity, long>()
        .SelectedBy(x => x.Id)
        .AndItsProperty(x => x.Collection)
        .ByAdding(x => x.Value);
}
```

### Event handlers

When you define an event and its effects, Phazor.Reactive will generate a handler for it. This handler will execute all
configured effects. Handlers must be registered in `IServiceCollection`, method for it is also generated by the library.

## Entity factories

Entity factories are used to create entity instances. They cache entity objects, meaning every time that you create
instances with same id, you will get same object. These factories are used internally in event handler implementations,
but you also can and should use them to gain access to reactive entities.

Factories are presented with `IReactiveEntityFactory<TEntity, TIdentifier>` interface, its implementation is generated
by the library. You can use a `.Create` method that accepts an identifier and returns an instance of entity.

Factories must be registered in `IServiceCollection`, method for it is also generated by library.

### Factories for partial entities

When you have defined a part of entity with a custom constructor, all its parameters (except for id) will be taken from
DI. Library will generate a fields and constructor for all these parameters.

## Event publishers and providers

To work with events you can use `IReactiveEventPublisher` and `IReactiveEventProvider` interfaces.

Publisher allows you to send events to handlers, which will run registered effects. To publish event use `.PublishAsync`
method.

You can also observe and handle events manually, not with entities. To do that, use `.Observe<TEvent>` method on event
provider.

## Registration

To register Phazor.Reactive in `ServiceCollection` call a `.AddPhazorReactive` extension method on it. It will register
all the basic implementations for the library.This method accepts a single argument a delegate action on
`IPhazorReactiveConfigurator` object. This configurator allows you to register an event handlers and entity factories.

Phazor.Reactive generates an extension method on `IPhazorReactiveConfigurator` to register all generated types. This
generated method has name schema `AddPhazorReactiveFrom{AssemblyName}` with assembly name without dots (ex:
AddPhazorReactiveFromMyReactiveApp, for My.Reactive.App)

```csharp
collection.AddPhazorReactive(reactive => reactive
    .AddPhazorReactiveFromMyReactiveApp());
```