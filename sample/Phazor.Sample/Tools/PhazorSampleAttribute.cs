namespace Phazor.Sample.Tools;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PhazorSampleAttribute : Attribute
{
    public PhazorSampleAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}