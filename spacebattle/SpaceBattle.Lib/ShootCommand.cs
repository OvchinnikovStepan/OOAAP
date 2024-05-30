namespace SpaceBattle.Lib;
using Hwdtech;

public class ShootCommand : ICommand
{
    private readonly IShootable shootable;

    public ShootCommand(IShootable shootable)
    {
        this.shootable = shootable;
    }

    public void Execute()
    {
        var bullet = IoC.Resolve<object>("Game.Create.Bullet", shootable);
        var cmd = IoC.Resolve<Hwdtech.ICommand>("Game.Command.Bullet.Act", bullet);
        IoC.Resolve<Hwdtech.ICommand>("Game.Queue.Push", cmd).Execute();
    }
}
