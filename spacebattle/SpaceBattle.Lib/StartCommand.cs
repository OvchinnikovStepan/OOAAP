using Hwdtech;

namespace SpaceBattle.Lib;

public interface IStartCommand{
    IUObject Target {get;}
    IDictionary <string,object> Properties {get;}
}

public class StartCommand : ICommand
{
    private readonly IStartCommand starting;
    public StartCommand(IStartCommand starting)
    {
        this.starting = starting;
    }

    public void Execute()
    {
        starting.Properties.ToList().ForEach(prop => IoC.Resolve<object>("Game.IUObject.SetProperty",
        starting.Target,prop.Key,prop.Value));

        var cmd = IoC.Resolve<ICommand>("Game.Commands.LongOperation",starting.Target);

        var injectable = IoC.Resolve<ICommand>("Commands.Injectable",cmd);

        IoC.Resolve<object>("Game.IUObject.SetProperty",starting.Target,"Game.Commands.LongOperation",injectable);

        IoC.Resolve<IQueue>("Game.Queue").Add(injectable);
    }
}
