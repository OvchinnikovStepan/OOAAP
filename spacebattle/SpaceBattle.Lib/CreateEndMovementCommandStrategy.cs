using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateEndMovementCommandStrategy : IStrategy
{
    public object Run(object[] args)
    {
        var obj = args[0];
        return new EndMovementCommand(IoC.Resolve<IEndable>("Game.IUObject.Adapter.IEndable", obj));
    }
}
