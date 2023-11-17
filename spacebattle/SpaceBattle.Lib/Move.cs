namespace SpaceBattle.Lib;

public interface IMovable
{
    public int[] Position { get; set; }
    public int[] Velocity { get; }
}

public class MoveCommand : ICommand
{
    private readonly IMovable movable;
    public MoveCommand(IMovable movable)
    {
        this.movable = movable;
    }
    public void Execute()
    {
        movable.Position = new int[]{
            movable.Position[0] +  movable.Velocity[0],
            movable.Position[1] +  movable.Velocity[1],
        };
    }
}