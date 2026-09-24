using System;

[Serializable]
public class FourDirectional<T>
{
    public readonly T[] AllDirections = new T[4];
    
    public T up;
    public T down;
    public T left;
    public T right;

    public FourDirectional()
    {
        AllDirections[0] = up;
        AllDirections[1] = down;
        AllDirections[2] = left;
        AllDirections[3] = right;
    }
}