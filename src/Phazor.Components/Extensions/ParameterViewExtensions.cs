using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Phazor.Components.Extensions;

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

    // Non-nullable value types
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        string parameterName,
        T oldValue,
        [NotNullWhen(true)] out T? value) where T : struct, IEquatable<T>
    {
        return parameters.TryGetValue(parameterName, out value) && oldValue.Equals(value) is false;
    }

    // Nullable value types
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        string parameterName,
        T? oldValue,
        [NotNullWhen(true)] out T? value) where T : struct, IEquatable<T>
    {
        return parameters.TryGetValue(parameterName, out value) && oldValue.Equals(value) is false;
    }
}