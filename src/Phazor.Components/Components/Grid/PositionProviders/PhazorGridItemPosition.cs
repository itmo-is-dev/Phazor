namespace Phazor.Components;

public class PhazorGridItemPosition
{
    private readonly PhazorGridDimension _dimension;
    private readonly string _majorDimensionIndex;
    private readonly string _minorDimensionIndex;

    public PhazorGridItemPosition(PhazorGridDimension dimension, string majorDimensionIndex, string minorDimensionIndex)
    {
        _dimension = dimension;
        _majorDimensionIndex = majorDimensionIndex;
        _minorDimensionIndex = minorDimensionIndex;
    }

    public PhazorGridPosition Value => _dimension.MakePosition(_majorDimensionIndex, _minorDimensionIndex);
}