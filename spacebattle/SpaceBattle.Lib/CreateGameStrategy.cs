namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateGameStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var gameId = (string)args[0];
        var parentScope = (object)args[1];
        var quant = (double)args[2];

        var gameQueue = IoC.Resolve<object>("Game.Queue.New");
        var gameScope = IoC.Resolve<object>("Game.Scope.New", gameId, parentScope, gameQueue, quant);
        var gameCommand = IoC.Resolve<Hwdtech.ICommand>("Game.Command", gameQueue, gameScope);

        var commandsList = new List<Hwdtech.ICommand> { gameCommand };
        var macroCommand = IoC.Resolve<Hwdtech.ICommand>("Game.Command.Macro", commandsList);
        var injectCommand = IoC.Resolve<Hwdtech.ICommand>("Game.Command.Inject", macroCommand);
        var repeatCommand = IoC.Resolve<Hwdtech.ICommand>("Command.Command.Repeat", injectCommand);
        commandsList.Add(repeatCommand);

        var gameList = IoC.Resolve<IDictionary<string, Hwdtech.ICommand>>("Game.GetGamesList");
        gameList.Add(gameId, injectCommand);

        return injectCommand;
    }
}
