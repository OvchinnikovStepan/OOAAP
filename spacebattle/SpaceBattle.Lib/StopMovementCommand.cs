using Hwdtech;

namespace SpaceBattle.Lib;

public class StartCommand : ICommand
{
private IStopCommand starttable;

public StartCommand(IStopCommand starttable)
{
this.starttable = starttable;
}
public void Execute()
{
starttable.Property.ToList().ForEach(a => IoC.Resolve<ICommand>("Game.Property.Set", starttable.Target, a.Key, a.Value).Execute());
var cmd = IoC.Resolve<ICommand>("Game.Operations.Movement", starttable.Target);
IoC.Resolve<ICommand>("IUObject.Property.Set", starttable.Target, "Movement", cmd).Execute();
IoC.Resolve<IQueue>("Game.Queue.Push").Add(cmd);
}
}