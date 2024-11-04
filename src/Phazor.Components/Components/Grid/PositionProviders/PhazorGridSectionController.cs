namespace Phazor.Components;

public class PhazorGridSectionController
{
    private readonly PhazorGridDimension _dimension;
    private int _currentMinorIndex;

    public PhazorGridSectionController(PhazorGridDimension dimension, int majorDimensionIndex)
    {
        MajorDimensionIndex = majorDimensionIndex;
        MajorDimensionString = majorDimensionIndex.ToString();

        _dimension = dimension;
        _currentMinorIndex = 1;
    }

    public int MajorDimensionIndex { get; }

    public string MajorDimensionString { get; }

    public PhazorGridItemPosition NextItem()
    {
        return new PhazorGridItemPosition(
            _dimension,
            MajorDimensionString,
            minorDimensionIndex: _currentMinorIndex++.ToString());
    }

    public PhazorGridPosition MakePosition(string minorDimension)
        => _dimension.MakePosition(MajorDimensionString, minorDimension);
}