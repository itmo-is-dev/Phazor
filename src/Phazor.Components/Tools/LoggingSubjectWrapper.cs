using Microsoft.Extensions.Logging;
using System.Reactive.Subjects;

namespace Phazor.Components.Tools;

internal class LoggingSubjectWrapper<T> : SubjectBase<T>
{
    private readonly ILogger _logger;
    private readonly SubjectBase<T> _subject;

    public LoggingSubjectWrapper(ILogger logger, SubjectBase<T> subject)
    {
        _logger = logger;
        _subject = subject;
    }

    public override bool HasObservers => _subject.HasObservers;

    public override bool IsDisposed => _subject.IsDisposed;

    public override void OnCompleted()
    {
        _logger.LogInformation("Subject completed");
        _subject.OnCompleted();
    }

    public override void OnError(Exception error)
    {
        _logger.LogError(error, "Subject received error");
        _subject.OnError(error);
    }

    public override void OnNext(T value)
    {
        _logger.LogInformation("Subject received value = {Value}", value);
        _subject.OnNext(value);
    }

    public override IDisposable Subscribe(IObserver<T> observer)
    {
        _logger.LogInformation("Subject subscribed to");
        return _subject.Subscribe(observer);
    }

    public override void Dispose()
    {
        _logger.LogInformation("Subject disposed");
        _subject.Dispose();
    }
}