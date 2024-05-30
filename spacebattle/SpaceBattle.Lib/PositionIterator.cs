namespace SpaceBattle.Lib;
using Hwdtech;

public class PositionIterator : IEnumerator<object>
{
    private readonly IList<Vector> positions;
    private int position_index = 0;

    public PositionIterator()
    {
        positions = IoC.Resolve<List<Vector>>("Game.StartShipPositions");
    }

    public object Current => positions[position_index];

    public bool MoveNext()
    {
        position_index = position_index + 1;
        return position_index < positions.Count;
    }

    public void Reset() { position_index = 0; }

    public void Dispose() { throw new NotImplementedException(); }
}
