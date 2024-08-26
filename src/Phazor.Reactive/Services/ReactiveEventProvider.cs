using Phazor.Reactive.Abstractions;
using System.Reactive.Subjects;

namespace Phazor.Reactive.Services;

internal class ReactiveEventProvider : IReactiveEventProvider, IDisposable
{
    private readonly Dictionary<Type, IProviderSubject> _subjects = [];

    public IObservable<TEvent> Observe<TEvent>()
        where TEvent : IReactiveEvent<TEvent>
    {
        return GetOrCreateSubject<TEvent>().ToObservable<TEvent>();
    }

    public void Publish<TEvent>(TEvent evt)
        where TEvent : IReactiveEvent<TEvent>
    {
        GetOrCreateSubject<TEvent>().Publish(evt);
    }

    public void Dispose()
    {
        foreach (IProviderSubject observable in _subjects.Values)
            observable.Dispose();
    }

    private IProviderSubject GetOrCreateSubject<T>()
        where T : IReactiveEvent<T>
    {
        if (_subjects.TryGetValue(typeof(T), out IProviderSubject? subject))
            return subject;

        subject = _subjects[typeof(T)] = new ProviderSubject<T>();
        return subject;
    }

    private interface IProviderSubject : IDisposable
    {
        IObservable<T> ToObservable<T>()
            where T : IReactiveEvent<T>;

        void Publish<T>(T evt)
            where T : IReactiveEvent<T>;
    }

    private class ProviderSubject<TEvent> : IProviderSubject
        where TEvent : IReactiveEvent<TEvent>
    {
        private readonly ReplaySubject<TEvent> _subject = new(1);

        public IObservable<T> ToObservable<T>()
            where T : IReactiveEvent<T>
        {
            return _subject as IObservable<T> ?? throw new InvalidCastException(
                $"Failed to convert observable from {typeof(TEvent)} to {typeof(T)}");
        }

        public void Publish<T>(T evt)
            where T : IReactiveEvent<T>
        {
            if (evt is TEvent typedEvent)
            {
                _subject.OnNext(typedEvent);
            }
            else
            {
                throw new InvalidCastException($"Failed to convert event from {typeof(TEvent)} to {typeof(T)}");
            }
        }

        public void Dispose()
            => _subject.Dispose();
    }
}