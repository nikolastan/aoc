namespace Utility.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class VectorAttribute(int di, int dj) : Attribute
{
    public int Di { get; private set; } = di;
    public int Dj { get; private set; } = dj;
}
