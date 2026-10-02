public static class DataCurves
{
    public static float Random(this FloatCurve curve, float random)
    {
        return curve.Eval(1f, random);
    }

    public static int Random(this IntCurve curve, float random)
    {
        return curve.Eval(1f, random);
    }
}