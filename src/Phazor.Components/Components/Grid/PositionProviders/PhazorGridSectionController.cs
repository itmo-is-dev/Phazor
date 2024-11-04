namespace Phazor.Components;

public class PhazorGridSectionController
{
    private readonly PhazorGridDimension _dimension;
    private int _currentMinorIndex;

    public PhazorGridSectionController(PhazorGridDimension dimension, int majorDimensionIndex)
    {
        MajorDimensionIndex = majorDimensionIndex;
        _dimension = dimension;
        _currentMinorIndex = 1;
    }

    public int MajorDimensionIndex { get; }

    public PhazorGridItemPosition NextItem()
    {
        return new PhazorGridItemPosition(_dimension, MajorDimensionIndex, _currentMinorIndex++);
    }

    public PhazorGridPosition MakePosition(int minorDimension, int? majorSpan, int? minorSpan)
        => _dimension.MakePosition(MajorDimensionIndex, minorDimension, majorSpan, minorSpan);
}