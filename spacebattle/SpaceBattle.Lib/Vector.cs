namespace  SpaceBattle.Lib;

public class Vector
{
    private int cordinates_count;
    private int[] cordinates;
    public Vector (params int[] cordinates)
    {
        this.cordinates=cordinates;
        cordinates_count=cordinates.Length;
    }

    public static Vector operator +(Vector vector1,Vector vector2)
    {
        Vector vectorsum = new Vector(new int[vector1.cordinates_count]);
        vectorsum.cordinates= vector1.cordinates.Select((p, ind) => p+vector2.cordinates[ind]).ToArray();
        return vectorsum;
    }
    public override bool Equals(object obj)
    {
        return cordinates.SequenceEqual( ( (Vector)obj ).cordinates);
    }
    public override int GetHashCode()
    {
        return cordinates.GetHashCode();
    }
}