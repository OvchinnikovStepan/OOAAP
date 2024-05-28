namespace SpaceBattle.Lib;
using Hwdtech;

public class ArrangeSpaceShipsCommand : Hwdtech.ICommand
{
    private readonly IEnumerable<IUObject> spaceships;

    public ArrangeSpaceShipsCommand(IEnumerable<IUObject> spaceships) { this.spaceships = spaceships; }

    public void Execute()
    {
        var positionIterator = IoC.Resolve<IEnumerator<object>>("Game.Iterators.Position");

        spaceships.ToList().ForEach(spaceship =>
            IoC.Resolve<Hwdtech.ICommand>("Game.Arrange.SpaceShip", spaceship, positionIterator).Execute());

        positionIterator.Reset();
    }
}
