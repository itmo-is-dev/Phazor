using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Phazor.Components.Extensions;

#pragma warning disable CA1021

public static class ParameterViewExtensions
{
    // Reference types
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        string parameterName,
        T oldValue,
        [NotNullWhen(true)] out T? value)
    {
        return parameters.TryGetValue(parameterName, out value) && ReferenceEquals(oldValue, value) is false;
    }

    // Reference types (simplified)
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        T oldValue,
        [NotNullWhen(true)] out T? value,
        [CallerArgumentExpression(nameof(oldValue))]
        string? parameterName = null)
    {
        return parameters.TryGetValue(parameterName ?? string.Empty, out value)
               && ReferenceEquals(oldValue, value) is false;
    }

    // Non-nullable value types
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        string parameterName,
        T oldValue,
        [NotNullWhen(true)] out T? value)
        where T : struct, IEquatable<T>
    {
        return parameters.TryGetValue(parameterName, out value) && oldValue.Equals(value) is false;
    }

    // Non-nullable value types (simplified)
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        T oldValue,
        [NotNullWhen(true)] out T? value,
        [CallerArgumentExpression(nameof(oldValue))]
        string? parameterName = null)
        where T : struct, IEquatable<T>
    {
        return parameters.TryGetValue(parameterName ?? string.Empty, out value) && oldValue.Equals(value) is false;
    }

    // Nullable value types
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        string parameterName,
        T? oldValue,
        [NotNullWhen(true)] out T? value)
        where T : struct, IEquatable<T>
    {
        return parameters.TryGetValue(parameterName, out value) && oldValue.Equals(value) is false;
    }

    // Nullable value types (simplified)
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        T? oldValue,
        [NotNullWhen(true)] out T? value,
        [CallerArgumentExpression(nameof(oldValue))]
        string? parameterName = null)
        where T : struct, IEquatable<T>
    {
        return parameters.TryGetValue(parameterName ?? string.Empty, out value) && oldValue.Equals(value) is false;
    }
}
