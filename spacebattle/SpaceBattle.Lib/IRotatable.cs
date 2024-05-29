namespace SpaceBattle.Lib;

public interface IRotatable
{
    public Rotate_Vector Position { get; set; }
    public Rotate_Vector Velocity { get; }
}

public class RotateCommand : ICommand
{
    private readonly IRotatable rotatable;
    public RotateCommand(IRotatable rotatable)
    {
        this.rotatable = rotatable;
    }
    public void Execute()
    {
        rotatable.Position = rotatable.Position + rotatable.Velocity;

    }
}
