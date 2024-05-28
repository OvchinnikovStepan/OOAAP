namespace SpaceBattle.Lib;

public class Vector
{
    private readonly int CordinatesCount;
    private int[] Cordinates;
    public Vector(params int[] cordinates)
    {
        Cordinates = cordinates;
        CordinatesCount = cordinates.Length;
    }

    public static Vector operator +(Vector vector1, Vector vector2)
    {
        var sum_vector = new Vector(new int[vector1.CordinatesCount]);
        sum_vector.Cordinates = vector1.Cordinates.Select((p, ind) => p + vector2.Cordinates[ind]).ToArray();
        return sum_vector;
    }
    public override bool Equals(object? obj)
    {
        return obj != null && Cordinates.SequenceEqual(((Vector)obj).Cordinates);
    }
    public override int GetHashCode()
    {
        return Cordinates.GetHashCode();
    }
}
