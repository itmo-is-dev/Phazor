namespace Phazor.Components;

public class PhazorGridController
{
    private readonly PhazorGridDimension _dimension;
    private readonly Dictionary<int, PhazorGridSectionController> _sections;
    private int _currentMajorIndex;

    public PhazorGridController(PhazorGridDimension dimension)
    {
        _dimension = dimension;
        _sections = [];
        _currentMajorIndex = 1;
    }

    public PhazorGridSectionController NextSection()
    {
        return SectionAt(_currentMajorIndex++);
    }

    public PhazorGridSectionController SectionAt(int majorIndex)
    {
        return _sections.TryGetValue(majorIndex, out PhazorGridSectionController? controller)
            ? controller
            : _sections[majorIndex] = new PhazorGridSectionController(_dimension, majorIndex);
    }

    public PhazorGridPosition MakePosition(int majorIndex, int minorIndex)
        => _dimension.MakePosition(majorIndex, minorIndex);
}