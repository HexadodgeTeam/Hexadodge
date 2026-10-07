using UnityEngine;

public static class RandomUtils
{
    public static T RandomElement<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}