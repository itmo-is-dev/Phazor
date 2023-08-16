using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Phazor.Components.Extensions;

internal static class ParameterViewExtensions
{
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        string parameterName,
        T oldValue,
        [NotNullWhen(true)] out T? value)
    {
        return parameters.TryGetValue(parameterName, out value) && oldValue?.Equals(value) is false;
    }

    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        string parameterName,
        T oldValue,
        [NotNullWhen(true)] out T? value) where T : struct, IEquatable<T>
    {
        return parameters.TryGetValue(parameterName, out value) && oldValue.Equals(value) is false;
    }
}