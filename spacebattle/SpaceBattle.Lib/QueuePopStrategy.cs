namespace SpaceBattle.Lib;
using Hwdtech;

public class QueuePopStrategy : ISimpleStrategy
{
    public object Run()
    {
        var queue = IoC.Resolve<IQueue>("Game.Queue");
        return queue.Take();
    }
}
