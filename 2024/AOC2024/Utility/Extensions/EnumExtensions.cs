using Utility.Enums;

namespace Utility.Extensions;
public static class EnumExtensions
{
    public static TAttribute GetAttribute<TAttribute>(this Enum value)
       where TAttribute : Attribute
    {
        var enumType = value.GetType();
        var name = Enum.GetName(enumType, value);
        return enumType.GetField(name!)!.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault()!;
    }

    public static Direction GetOpposite(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            Direction.Northeast => Direction.Southwest,
            Direction.Southwest => Direction.Northeast,
            Direction.Northwest => Direction.Southeast,
            Direction.Southeast => Direction.Northwest,
            _ => throw new NotImplementedException()
        };
    }
}
