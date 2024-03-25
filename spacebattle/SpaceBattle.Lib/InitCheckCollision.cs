namespace SpaceBattle.Lib;
using Hwdtech;

public class InitCheckCollision : ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.CheckCollision", (object[] args) => new CollisionCheckCommand((IUObject)args[0], (IUObject)args[1])).Execute();
    }
}
