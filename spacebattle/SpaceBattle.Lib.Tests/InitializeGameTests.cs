using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace SpaceBattle.Lib.Test;
public class InitializeGameTests
{
    public InitializeGameTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void GetObjectStrategy_Test_Success()
    {
        var objectDict = new Dictionary<int, IUObject>();
        var obj = new Mock<IUObject>();
        objectDict.Add(1, obj.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) => objectDict).Execute();
        var strategy = new GetObjectStrategy();
        var result = strategy.Run(1);
        Assert.Equal(obj.Object, result);
    }
    [Fact]
    public void GetObjectStrategy_Test_DisabbilityToGetIUObjectListCauseException()
    {
        var objectDict = new Dictionary<int, IUObject>();
        var obj = new Mock<IUObject>();
        objectDict.Add(1, obj.Object);
        var badcmd = new Mock<Hwdtech.ICommand>();
        badcmd.Setup(x => x.Execute()).Throws(new Exception());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) =>
        {
            badcmd.Object.Execute();
            return objectDict;
        }).Execute();
        var strategy = new GetObjectStrategy();
        Assert.Throws<Exception>(() => strategy.Run(1));
    }
    [Fact]
    public void GetObjectStrategy_Test_NoObjectIdCauseException()
    {
        var objectDict = new Dictionary<int, IUObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) => objectDict).Execute();
        var strategy = new GetObjectStrategy();
        Assert.Throws<KeyNotFoundException>(() => strategy.Run(1));
    }
    [Fact]
    public void DeleteObjectStrategy_Test_Success()
    {
        var objectDict = new Dictionary<int, IUObject>();
        var obj = new Mock<IUObject>();
        objectDict.Add(1, obj.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) => objectDict).Execute();
        new DeleteObjectCommand(1).Execute();
        Assert.Empty(objectDict);
    }
    [Fact]
    public void DeleteObjectStrategy_Test_DisabbilityToGetIUObjectListCauseException()
    {
        var objectDict = new Dictionary<int, IUObject>();
        var obj = new Mock<IUObject>();
        objectDict.Add(1, obj.Object);
        var badcmd = new Mock<Hwdtech.ICommand>();
        badcmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) =>
        {
            badcmd.Object.Execute();
            return objectDict;
        }).Execute();
        Assert.Throws<Exception>(() => { new DeleteObjectCommand(1).Execute(); });
    }
    [Fact]
    public void QueuePopStrategy_Test_Success()
    {
        var queue = new Mock<IQueue>();
        var cmd = new Mock<ICommand>();
        queue.Setup(x => x.Take()).Returns(cmd.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => queue.Object).Execute();
        var strategy = new QueuePopStrategy();
        Assert.Equal(cmd.Object, strategy.Run());
    }
    [Fact]
    public void QueuePopStrategy_Test_DisabilityToGetQueueCauseException()
    {
        var queue = new Mock<IQueue>();
        var cmd = new Mock<ICommand>();
        var badcmd = new Mock<Hwdtech.ICommand>();
        badcmd.Setup(x => x.Execute()).Throws(new Exception());
        queue.Setup(x => x.Take()).Returns(cmd.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) =>
        {
            badcmd.Object.Execute();
            return queue.Object;
        }).Execute();
        var strategy = new QueuePopStrategy();
        Assert.Throws<Exception>(() => strategy.Run());
    }
    [Fact]
    public void QueuePopStrategy_Test_DisabilityToTakeCommandCauseException()
    {
        var queue = new Mock<IQueue>();
        queue.Setup(x => x.Take()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) =>
        {
            return queue.Object;
        }).Execute();
        var strategy = new QueuePopStrategy();
        Assert.Throws<Exception>(() => strategy.Run());
    }
    [Fact]
    public void QueuePushStrategy_Test_Success()
    {
        var queue = new Mock<IQueue>();
        var cmd = new Mock<ICommand>();
        var realqueue = new Queue<ICommand>();
        queue.Setup(x => x.Add(It.IsAny<ICommand>())).Callback<ICommand>(cmd => { realqueue.Enqueue(cmd); });
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => queue.Object).Execute();
        var pushCommand = new QueuePushCommand(cmd.Object);
        pushCommand.Execute();
        Assert.Equal(cmd.Object, realqueue.Peek());
    }
    [Fact]
    public void QueuePushStrategy_Test_DisabilityToGetQueueCauseException()
    {
        var queue = new Mock<IQueue>();
        var cmd = new Mock<ICommand>();
        var badcmd = new Mock<Hwdtech.ICommand>();
        badcmd.Setup(x => x.Execute()).Throws(new Exception());
        var realqueue = new Queue<ICommand>();
        queue.Setup(x => x.Add(It.IsAny<ICommand>())).Callback<ICommand>(cmd => { realqueue.Enqueue(cmd); });
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) =>
        {
            badcmd.Object.Execute();
            return queue.Object;
        }).Execute();
        var pushCommand = new QueuePushCommand(cmd.Object);

        Assert.Throws<Exception>(() => { pushCommand.Execute(); });
    }
    [Fact]
    public void QueuePushStrategy_Test_DisabilityToAddCommandCauseException()
    {
        var queue = new Mock<IQueue>();
        var cmd = new Mock<ICommand>();
        queue.Setup(x => x.Add(It.IsAny<ICommand>())).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => queue.Object).Execute();
        var pushCommand = new QueuePushCommand(cmd.Object);

        Assert.Throws<Exception>(() => { pushCommand.Execute(); });
    }
    [Fact]
    public void DeleteGameCommand_Test_Success()
    {
        var gameList = new Mock<IDictionary<string, IInjectable>>();
        var injectCommand = new Mock<IInjectable>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetGamesList", (object[] args) => gameList.Object).Execute();
        gameList.SetupGet(x => x[It.IsAny<string>()]).Returns(injectCommand.Object);
        injectCommand.Setup(x => x.Inject(It.IsAny<ICommand>())).Verifiable();
        var scopeList = new Mock<IDictionary<string, object>>();
        scopeList.Setup(x => x.Remove(It.IsAny<string>())).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scope.List", (object[] args) => scopeList.Object).Execute();

        var command = new DeleteGameCommand("1");
        command.Execute();

        scopeList.Verify(x => x.Remove(It.IsAny<string>()), Times.Once());
        injectCommand.Verify(x => x.Inject(It.IsAny<EmptyCommand>()), Times.Once());
    }
    [Fact]
    public void DeleteGameCommand_Test_DisabilityToGetGameListCauseException()
    {
        var gameList = new Mock<IDictionary<string, IInjectable>>();
        var injectCommand = new Mock<IInjectable>();
        var badcmd = new Mock<Hwdtech.ICommand>();
        badcmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetGamesList", (object[] args) =>
        {
            badcmd.Object.Execute();
            return gameList.Object;
        }).Execute();

        gameList.SetupGet(x => x[It.IsAny<string>()]).Returns(injectCommand.Object);
        injectCommand.Setup(x => x.Inject(It.IsAny<ICommand>())).Verifiable();
        var scopeList = new Mock<IDictionary<string, object>>();
        scopeList.Setup(x => x.Remove(It.IsAny<string>())).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scope.List", (object[] args) => scopeList.Object).Execute();

        var command = new DeleteGameCommand("1");

        Assert.Throws<Exception>(() => { command.Execute(); });
    }
    [Fact]
    public void DeleteGameCommand_Test_DisabilityToGetScopeListCauseException()
    {
        var gameList = new Mock<IDictionary<string, IInjectable>>();
        var injectCommand = new Mock<IInjectable>();
        var badcmd = new Mock<Hwdtech.ICommand>();
        badcmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetGamesList", (object[] args) => gameList.Object).Execute();

        gameList.SetupGet(x => x[It.IsAny<string>()]).Returns(injectCommand.Object);
        injectCommand.Setup(x => x.Inject(It.IsAny<ICommand>())).Verifiable();
        var scopeList = new Mock<IDictionary<string, object>>();
        scopeList.Setup(x => x.Remove(It.IsAny<string>())).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scope.List", (object[] args) =>
        {
            badcmd.Object.Execute();
            return scopeList.Object;
        }).Execute();

        var command = new DeleteGameCommand("1");

        Assert.Throws<Exception>(() => { command.Execute(); });
    }
    [Fact]
    public void DeleteGameCommand_Test_DisabilityToGetObjectFromGameListCauseException()
    {
        var gameList = new Mock<IDictionary<string, IInjectable>>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetGamesList", (object[] args) => gameList.Object).Execute();
        gameList.SetupGet(x => x[It.IsAny<string>()]).Throws(new Exception());
        var scopeList = new Mock<IDictionary<string, object>>();
        scopeList.Setup(x => x.Remove(It.IsAny<string>())).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scope.List", (object[] args) => scopeList.Object).Execute();

        var command = new DeleteGameCommand("1");

        Assert.Throws<Exception>(() => { command.Execute(); });
    }
    [Fact]
    public void DeleteGameCommand_Test_DisabilityToInjectCommandCauseException()
    {
        var gameList = new Mock<IDictionary<string, IInjectable>>();
        var injectCommand = new Mock<IInjectable>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetGamesList", (object[] args) => gameList.Object).Execute();
        gameList.SetupGet(x => x[It.IsAny<string>()]).Returns(injectCommand.Object);
        injectCommand.Setup(x => x.Inject(It.IsAny<ICommand>())).Throws(new Exception());
        var scopeList = new Mock<IDictionary<string, object>>();
        scopeList.Setup(x => x.Remove(It.IsAny<string>())).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scope.List", (object[] args) => scopeList.Object).Execute();

        var command = new DeleteGameCommand("1");

        Assert.Throws<Exception>(() => { command.Execute(); });
    }
    [Fact]
    public void DeleteGameCommand_Test_DisabilityToRemoveScopedCauseException()
    {
        var gameList = new Mock<IDictionary<string, IInjectable>>();
        var injectCommand = new Mock<IInjectable>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetGamesList", (object[] args) => gameList.Object).Execute();
        gameList.SetupGet(x => x[It.IsAny<string>()]).Returns(injectCommand.Object);
        injectCommand.Setup(x => x.Inject(It.IsAny<ICommand>())).Verifiable();
        var scopeList = new Mock<IDictionary<string, object>>();
        scopeList.Setup(x => x.Remove(It.IsAny<string>())).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Scope.List", (object[] args) => scopeList.Object).Execute();

        var command = new DeleteGameCommand("1");

        Assert.Throws<Exception>(() => { command.Execute(); });
    }
}
