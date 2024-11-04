namespace Phazor.Components;

public sealed class PhazorGridDimension
{
    private readonly string _stringRepresentation;
    private readonly Func<int, int, int?, int?, PhazorGridPosition> _func;

    private PhazorGridDimension(string stringRepresentation, Func<int, int, int?, int?, PhazorGridPosition> func)
    {
        _stringRepresentation = stringRepresentation;
        _func = func;
    }

    public static readonly PhazorGridDimension Rows = new(
        "rows",
        (majorIndex, minorIndex, majorSpan, minorSpan) => new PhazorGridPosition(
            Row: majorIndex,
            Column: minorIndex,
            RowSpan: majorSpan ?? 1,
            ColumnSpan: minorSpan ?? 1));

    public static readonly PhazorGridDimension Columns = new(
        "columns",
        (majorIndex, minorIndex, majorSpan, minorSpan) => new PhazorGridPosition(
            Row: minorIndex,
            Column: majorIndex,
            RowSpan: minorSpan ?? 1,
            ColumnSpan: majorSpan ?? 1));

    public PhazorGridPosition MakePosition(int majorIndex, int minorIndex, int? majorSpan, int? minorSpan)
        => _func.Invoke(majorIndex, minorIndex, majorSpan, minorSpan);

    public override string ToString() => _stringRepresentation;
}