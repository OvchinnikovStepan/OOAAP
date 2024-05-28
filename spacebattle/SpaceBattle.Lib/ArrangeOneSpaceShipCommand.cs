namespace SpaceBattle.Lib;
using Hwdtech;

public class ArrangeOneSpaceShipCommand : ICommand
{
    private readonly IUObject spaceship;
    private readonly IEnumerator<object> positionIterator;

    public ArrangeOneSpaceShipCommand(IUObject spaceship, IEnumerator<object> positionIterator)
    {
        this.spaceship = spaceship;
        this.positionIterator = positionIterator;
    }

    public void Execute()
    {
        IoC.Resolve<ICommand>("Game.UObject.SetProperty", spaceship, "Position", positionIterator.Current).Execute();

        positionIterator.MoveNext();
    }
}
