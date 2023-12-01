namespace SpaceBattle.Lib;

public class Rotate_Vector
{
    private int[] Cordinates;
    private int Number_Of_Positions;
    public Rotate_Vector(int Number_Of_Positions, params int[] Cordinates)
    {
        this.Number_Of_Positions = Number_Of_Positions;
        this.Cordinates = Cordinates.Select(p => p % Number_Of_Positions).ToArray();
    }

    public static Rotate_Vector operator +(Rotate_Vector vector1, Rotate_Vector vector2)
    {
        vector1.Cordinates = vector1.Cordinates.Select((p, ind) => (p + vector2.Cordinates[ind])).ToArray();
        Rotate_Vector vectorsum = new Rotate_Vector(vector1.Number_Of_Positions, vector1.Cordinates);
        return vectorsum;
    }
    public override bool Equals(object obj)
    {
        return Number_Of_Positions == ((Rotate_Vector)obj).Number_Of_Positions && Cordinates.SequenceEqual(((Rotate_Vector)obj).Cordinates);
    }
    public override int GetHashCode()
    {
        return Cordinates.GetHashCode();
    }
}
