using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateRotationCommandStrategy : IStrategy
{
    public object Run(object[] args)
    {
        var obj = args[0];
        return new RotateCommand(IoC.Resolve<IRotatable>("Game.IUObject.Adapter.IRotatable", obj));
    }
}
