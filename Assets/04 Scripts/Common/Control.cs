using System;

public static class Control
{
    public enum Type
    {
        Modern,
        Classic
    }

    public static string GetInputScheme(this Type type)
    {
        return type switch
        {
            Type.Modern => "Touch",
            Type.Classic => "Gamepad",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}