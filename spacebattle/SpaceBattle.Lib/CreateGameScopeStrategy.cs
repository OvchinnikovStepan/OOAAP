namespace SpaceBattle.Lib;

using Hwdtech;

public class NewGameScopeStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var gameId = (string)args[0];
        var parentScope = (object)args[1];
        var gameQueue = (object)args[2];
        var quantum = (double)args[3];

        var gameScope = IoC.Resolve<object>("Scopes.New", parentScope);

        var gameScopeMap = IoC.Resolve<IDictionary<string, object>>("Game.Scope.Map");
        gameScopeMap.Add(gameId, gameScope);

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Quant", (object[] args) => (object)quantum).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => gameQueue).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Pop", (object[] args) => { return new QueuePopStrategy().Run(args); }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Push", (object[] args) => { return new QueuePushCommand((ICommand)args[0]); }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", parentScope).Execute();

        return gameScope;
    }
}
