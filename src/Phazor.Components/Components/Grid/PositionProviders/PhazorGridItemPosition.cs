namespace Phazor.Components;

public class PhazorGridItemPosition
{
    private readonly PhazorGridDimension _dimension;
    private readonly int _majorDimensionIndex;
    private readonly int _minorDimensionIndex;

    public PhazorGridItemPosition(PhazorGridDimension dimension, int majorDimensionIndex, int minorDimensionIndex)
    {
        _dimension = dimension;
        _majorDimensionIndex = majorDimensionIndex;
        _minorDimensionIndex = minorDimensionIndex;
    }

    public PhazorGridPosition Value => _dimension.MakePosition(_majorDimensionIndex, _minorDimensionIndex);
}