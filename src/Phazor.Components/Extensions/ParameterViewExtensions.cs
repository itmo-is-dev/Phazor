using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Phazor.Components.Extensions;

#pragma warning disable CA1021

public static class ParameterViewExtensions
{
    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        string parameterName,
        T oldValue,
        [NotNullWhen(true)] out T? value)
    {
        return parameters.TryGetValue(parameterName, out value)
               && EqualityComparer<T>.Default.Equals(oldValue, value) is false;
    }

    public static bool TryGetUpdatedValue<T>(
        this ParameterView parameters,
        T oldValue,
        [NotNullWhen(true)] out T? value,
        [CallerArgumentExpression(nameof(oldValue))]
        string? parameterName = null)
    {
        return parameters.TryGetValue(parameterName ?? string.Empty, out value)
               && EqualityComparer<T>.Default.Equals(oldValue, value) is false;
    }
}
