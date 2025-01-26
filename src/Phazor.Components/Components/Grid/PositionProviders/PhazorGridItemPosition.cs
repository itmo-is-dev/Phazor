namespace Phazor.Components;

public class PhazorGridItemPosition
{
    private readonly PhazorGridDimension _dimension;

    public PhazorGridItemPosition(PhazorGridDimension dimension, int majorDimensionIndex, int minorDimensionIndex)
    {
        _dimension = dimension;
        MajorDimensionIndex = majorDimensionIndex;
        MinorDimensionIndex = minorDimensionIndex;
    }

    public int MajorDimensionIndex { get; }
    public int MinorDimensionIndex { get; }

    public PhazorGridPosition MakePosition(int? majorSpan, int? minorSpan)
        => _dimension.MakePosition(MajorDimensionIndex, MinorDimensionIndex, majorSpan, minorSpan);
}
