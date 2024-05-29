namespace SpaceBattle.Lib;
using Hwdtech;

public class QueuePopStrategy : IStrategy
{
    public object Run(object[] args)
    {
        var queue = IoC.Resolve<IQueue>("Game.Queue");
        return queue.Take();
    }
}
