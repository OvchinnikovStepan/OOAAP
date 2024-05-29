using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateStartMovementCommandStrategy : IStrategy
{
    public object Run(object[] args)
    {
        var obj = args[0];
        return new StartCommand(IoC.Resolve<IStartCommand>("Game.IUObject.Adapter.IStartCommand", obj));
    }
}
