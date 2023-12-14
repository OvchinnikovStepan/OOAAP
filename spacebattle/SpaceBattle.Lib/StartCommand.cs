using Hwdtech;

namespace SpaceBattle.Lib;

public interface IStartCommand{
    IUObject Target {get;}
    IDictionary <string,object> Properties {get;}
}

public class StartCommand : ICommand
{
    private readonly IStartCommand _startable;
    public StartCommand(IStartCommand startable)
    {
        _startable = startable;
    }

    public void Execute()
    {
        _startable.Properties.ToList().ForEach(property => IoC.Resolve<object>(
            "Game.IUObject.SetProperty",
            _startable.Target,
            property.Key,
            property.Value
        ));

        var cmd = IoC.Resolve<ICommand>(
            "Game.Commands.LongMove",
            _startable.Target
        );

        IoC.Resolve<object>(
            "Game.IUObject.SetProperty",
            _startable.Target,
            "Game.Commands.LongMove",
            cmd
        );

        IoC.Resolve<IQueue>("Game.Queue").Add(cmd);
    }
}