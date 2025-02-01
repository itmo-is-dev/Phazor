using System.Runtime.CompilerServices;

namespace Phazor.Components.Tools;

/// <summary>
///     <see cref="WeakReference"/> wrapper that allows to retain value based on it's usage
/// </summary>
/// <typeparam name="T">
///     Type of the value
/// </typeparam>
internal class AdaptiveWeakReference<T>
    where T : class
{
    private const ushort DefaultWeaknessBreakpoint = 15;

    private readonly Func<T> _valueFactory;

    /// <summary>
    ///     <see cref="_weaknessFactor"/> less than <see cref="_weaknessBreakpoint"/>: reference must be weak
    ///     <see cref="_weaknessFactor"/> greater or equal to <see cref="_weaknessBreakpoint"/>: reference must be strong
    /// </summary>
    private readonly ushort _weaknessBreakpoint;

    /// <summary>
    ///     Container for weak reference. If value is initialized, always contains reference to it. When strong
    ///     reference is also initialized, weak reference will retrain its value because GC would not collect target
    ///     object.
    /// </summary>
    private readonly WeakReference<T?> _reference;

    /// <summary>
    ///     Strong reference to value. Initialized when <see cref="_weaknessFactor"/> greater or equal
    ///     to <see cref="_weaknessBreakpoint"/>
    /// </summary>
    private T? _value;

    private ushort _weaknessFactor;
    private ushort _strengthenStreak;
    private ushort _weakenStreak;

    public AdaptiveWeakReference(Func<T> valueFactory, ushort weaknessBreakpoint = DefaultWeaknessBreakpoint)
    {
        _valueFactory = valueFactory;
        _weaknessBreakpoint = weaknessBreakpoint;

        _value = null;
        _reference = new WeakReference<T?>(null);
    }

    /// <summary>
    ///     Gets or sets wrapped value. When getting an uninitialized value, it will be created with provided value
    ///     factory. When value is initialized in getter or set via setter it will either be weak or strong reference
    ///     based on current <see cref="_weaknessFactor"/>.
    /// </summary>
    public T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetValue();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SetValue(value);
    }

    /// <summary>
    ///     Checks whether value is initialized either with weak or strong reference.
    /// </summary>
    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value is not null || _reference.TryGetTarget(out _);
    }

    /// <summary>
    ///     Removes both weak and strong reference 
    /// </summary>
    public void Clear()
    {
        _value = null;
        _reference.SetTarget(null);
    }

    /// <summary>
    ///     Increases the <see cref="_weaknessFactor"/>, changing weak to strong reference if required
    /// </summary>
    public void Strengthen()
    {
        if (_weaknessFactor is ushort.MaxValue)
            return;

        if (_strengthenStreak is not ushort.MaxValue)
            _strengthenStreak++;

        _weakenStreak = 0;

        _weaknessFactor = ushort.MaxValue - _weaknessFactor < _strengthenStreak
            ? ushort.MaxValue
            : (ushort)(_weaknessFactor + _strengthenStreak);

        if (_weaknessFactor >= _weaknessBreakpoint)
            ConvertToStrongReference();
    }

    /// <summary>
    ///     Decreases the <see cref="_weaknessFactor"/>, changing strong to weak reference if required
    /// </summary>
    public void Weaken()
    {
        if (_weaknessFactor is 0)
            return;

        if (_weakenStreak is not ushort.MaxValue)
            _weakenStreak++;

        _strengthenStreak = 0;

        _weaknessFactor = _weaknessFactor < _weakenStreak
            ? ushort.MinValue
            : (ushort)(_weaknessFactor - _weakenStreak);

        if (_weaknessFactor < _weaknessBreakpoint)
            ConvertToWeakReference();
    }

    /// <summary>
    ///     If current value is initialized – returns it, otherwise – creates and sets the value, and then returns it 
    /// </summary>
    private T GetValue()
    {
        if (_value is not null)
            return _value;

        if (_reference.TryGetTarget(out T? value))
            return value;

        value = _valueFactory.Invoke();
        SetValue(value);

        return value;
    }

    private void SetValue(T value)
    {
        _value = _weaknessFactor < _weaknessBreakpoint ? null : value;
        _reference.SetTarget(value);
    }

    private void ConvertToWeakReference()
    {
        _reference.SetTarget(_value);
        _value = null;
    }

    private void ConvertToStrongReference()
    {
        _reference.TryGetTarget(out _value);
    }
}
