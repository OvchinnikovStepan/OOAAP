using Hwdtech;

namespace SpaceBattle.Lib;

public class IncomingCommand : Hwdtech.ICommand
{
    private readonly IMessage _message;
    public IncomingCommand(IMessage message)
    {
        _message = message;
    }
    public void Execute()
    {
        var obj = IoC.Resolve<IUObject>("Game.IUObject.Get", _message.GameItemID);
        var properties = _message.Properties;
        properties.ToList().ForEach(property => IoC.Resolve<Hwdtech.ICommand>("Game.IUObject.SetProprty", obj, property.Key, property.Value).Execute());
        IoC.Resolve<ICommand>("Game.Command." + _message.OrderType, obj).Execute();
    }
}
