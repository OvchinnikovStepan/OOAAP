using Hwdtech;

namespace SpaceBattle.Lib;

public class QueuePushCommand : Hwdtech.ICommand
{
    private readonly ICommand command;
    public QueuePushCommand(ICommand command)
    {
        this.command = command;
    }
    public void Execute()
    {
        var queue = IoC.Resolve<IQueue>("Game.Queue");
        queue.Add(command);
    }
}
