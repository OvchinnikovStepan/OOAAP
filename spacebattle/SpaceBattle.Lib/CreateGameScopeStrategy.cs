namespace SpaceBattle.Lib;

using Hwdtech;

public class NewGameScopeStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var gameId = (string)args[0];
        var parentScope = (object)args[1];
        var gameQueue = (object)args[2];
        var quant = (double)args[3];

        var gameScope = IoC.Resolve<object>("Scopes.New", parentScope);

        var gameScopeList = IoC.Resolve<IDictionary<string, object>>("Game.Scope.List");
        gameScopeList.Add(gameId, gameScope);

        var currentScope = IoC.Resolve<object>("Scopes.Current");

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Quant", (object[] args) => (object)quant).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => gameQueue).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) => new Dictionary<int, IUObject>()).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Pop", (object[] args) => { return new QueuePopStrategy().Run(); }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Push", (object[] args) => { return new QueuePushCommand((ICommand)args[0]); }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.Delete", (object[] args) => { return new DeleteObjectCommand((int)args[0]); }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.Get", (object[] args) => { return new GetObjectStrategy().Run(args); }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", currentScope).Execute();

        return gameScope;
    }
}
