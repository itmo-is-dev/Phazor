using Phazor.Reactive.Sample.Inner;
using System;

namespace Phazor.Reactive.Sample;

internal partial class MyEntity
{
    private readonly IServiceProvider _provider;

    public MyEntity(long id, IServiceProvider provider,  IInnerEntityFactory entityFactory)
    {
        Id = id;
        _provider = provider;
    }

    public object Aboba()
    {
        return _provider;
    }
}