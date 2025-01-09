using Utility.Attributes;

namespace Utility.Enums;
public enum Direction
{
    [Vector(-1, 0)]
    North,
    [Vector(-1, 1)]
    Northeast,
    [Vector(0, 1)]
    East,
    [Vector(1, 1)]
    Southeast,
    [Vector(1, 0)]
    South,
    [Vector(1, -1)]
    Southwest,
    [Vector(0, -1)]
    West,
    [Vector(-1, -1)]
    Northwest
}
