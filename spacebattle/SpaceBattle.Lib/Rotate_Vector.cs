namespace SpaceBattle.Lib;

public class Rotate_Vector
{
    private int[] cordinates;
    private int number_of_positions;
    public Rotate_Vector(int number_of_positions, params int[] cordinates)
    {
        this.number_of_positions = number_of_positions;
        this.cordinates = cordinates.Select(p => p % number_of_positions).ToArray();
    }

    public static Rotate_Vector operator +(Rotate_Vector vector1, Rotate_Vector vector2)
    {
        vector1.cordinates = vector1.cordinates.Select((p, ind) => (p + vector2.cordinates[ind])).ToArray();
        Rotate_Vector vectorsum = new Rotate_Vector(vector1.number_of_positions, vector1.cordinates);
        return vectorsum;
    }
    public override bool Equals(object obj)
    {
        return number_of_positions == ((Rotate_Vector)obj).number_of_positions && cordinates.SequenceEqual(((Rotate_Vector)obj).cordinates);
    }
    public override int GetHashCode()
    {
        return cordinates.GetHashCode();
    }
}
