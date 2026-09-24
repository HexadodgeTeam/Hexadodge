using System;

public static class Control
{
    public enum Type
    {
        Classic,
        Modern
    }

    public static string GetInputScheme(this Type type)
    {
        return type switch
        {
            Type.Classic => "Gamepad",
            Type.Modern => "Touch",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}