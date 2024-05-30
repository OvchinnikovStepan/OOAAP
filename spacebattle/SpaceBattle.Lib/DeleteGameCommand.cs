namespace SpaceBattle.Lib;
using Hwdtech;

public class DeleteGameCommand : ICommand
{
    private readonly string gameId;

    public DeleteGameCommand(string gameId)
    {
        this.gameId = gameId;
    }
    public void Execute()
    {
        var gameList = IoC.Resolve<IDictionary<string, IInjectable>>("Game.GetGamesList");
        gameList[gameId].Inject(new EmptyCommand());

        var gameScopeList = IoC.Resolve<IDictionary<string, object>>("Game.Scope.List");
        gameScopeList.Remove(gameId);
    }
}
