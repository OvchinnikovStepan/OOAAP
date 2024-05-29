using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateShootCommandStrategy : IStrategy
{
    public object Run(object[] args)
    {
        var obj = args[0];
        return new ShootCommand(IoC.Resolve<IShootable>("Game.IUObject.Adapter.IShootable", obj));
    }
}
