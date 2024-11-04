namespace Phazor.Components;

public sealed class PhazorGridDimension
{
    private readonly string _stringRepresentation;
    private readonly Func<string, string, PhazorGridPosition> _func;

    private PhazorGridDimension(string stringRepresentation, Func<string, string, PhazorGridPosition> func)
    {
        _stringRepresentation = stringRepresentation;
        _func = func;
    }

    public static readonly PhazorGridDimension Rows = new(
        "rows",
        (majorIndex, minorIndex) => new PhazorGridPosition(Row: majorIndex, Column: minorIndex));

    public static readonly PhazorGridDimension Columns = new(
        "columns",
        (majorIndex, minorIndex) => new PhazorGridPosition(Row: minorIndex, Column: majorIndex));

    public PhazorGridPosition MakePosition(string majorIndex, string minorIndex)
        => _func.Invoke(majorIndex, minorIndex);

    public override string ToString() => _stringRepresentation;
}