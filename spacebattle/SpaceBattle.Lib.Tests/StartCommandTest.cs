using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class StartMoveCommand_Tests
{
    public StartMoveCommand_Tests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.IUObject.SetProperty",
            (object[] args) =>
            {
                var order = (IUObject)args[0];
                var key = (string)args[1];
                var value = args[2];

                order.setProperty(key, value);
                return new object();
            }
        ).Execute();

        var LongMoveCommand = new Mock<ICommand>().Object;
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Commands.LongMove",
            (object[] args) =>
            {
                return LongMoveCommand;
            }
        ).Execute();
    }

    [Fact]
    public void StartCommand_Positive()
    {
        var queue = new Mock<IQueue>();
        var realQueue = new Queue<ICommand>();
        queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(realQueue.Enqueue);
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Queue",
            (object[] args) =>
            {
                return queue.Object;
            }
        ).Execute();

        var startable = new Mock<IStartCommand>();
        var order = new Mock<IUObject>();
        var orderDict = new Dictionary<string, object>();
        var properties = new Dictionary<string, object> {
            { "id", 1 },
        };

        startable.SetupGet(s => s.Properties).Returns(properties);
        startable.SetupGet(s => s.Target).Returns(order.Object);
        order.Setup(o => o.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>(orderDict.Add);
        queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(realQueue.Enqueue);

        var startCommand = new StartCommand(startable.Object);
        startCommand.Execute();

        Assert.Contains("id", orderDict.Keys);
        Assert.Contains("Game.Commands.LongMove", orderDict.Keys);
        Assert.NotEmpty(realQueue);
    }

    [Fact]
    public void StartCommand_CantSetProperties()
    {
        var startable = new Mock<IStartCommand>();
        var order = new Mock<IUObject>();
        var orderDict = new Dictionary<string, object>();

        var properties = new Dictionary<string, object> {
            { "position", new Vector( new int[] { 2, 1 }) },
        };

        startable.SetupGet(s => s.Properties).Returns(properties);
        startable.SetupGet(s => s.Target).Returns(order.Object);
        order.Setup(o => o.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback(() => throw new NotImplementedException());

        var startCommand = new StartCommand(startable.Object);

        Assert.Throws<NotImplementedException>(startCommand.Execute);
    }

    [Fact]
    public void StartCommand_CantPutCommandInQueue()
    {
        var queue = new Mock<IQueue>();
        var realQueue = new Queue<ICommand>();
        queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(() => {});
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Queue",
            (object[] args) =>
            {
                return queue.Object;
            }
        ).Execute();

        var startable = new Mock<IStartCommand>();
        var order = new Mock<IUObject>();
        var orderDict = new Dictionary<string, object>();
        var properties = new Dictionary<string, object> {
            { "action", new Mock<ICommand>() }
        };

        startable.SetupGet(s => s.Properties).Returns(properties);
        startable.SetupGet(s => s.Target).Returns(order.Object);
        order.Setup(o => o.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>(orderDict.Add);

        var startCommand = new StartCommand(startable.Object);
        startCommand.Execute();

        Assert.Empty(realQueue);
    }

    [Fact]
    public void StartCommand_CantReadPropertiesOfOrder_FromStartable()
    {
        var startable = new Mock<IStartCommand>();
        startable.SetupGet(s => s.Properties).Throws(new NotImplementedException());
        var startCommand = new StartCommand(startable.Object);

        Assert.Throws<NotImplementedException>(startCommand.Execute);
    }

    [Fact]
    public void StartCommand_CantReadOrder_FromStartable()
    {
        var startable = new Mock<IStartCommand>();
        startable.SetupGet(s => s.Target).Throws(new NotImplementedException());
        var properties = new Dictionary<string, object> {
            { "position", new Vector( new int[] { 2, 1 }) },
        };

        startable.SetupGet(s => s.Properties).Returns(properties);
        var startCommand = new StartCommand(startable.Object);

        Assert.Throws<NotImplementedException>(startCommand.Execute);
    }
}