using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class StartCommand_Tests
{
    public StartCommand_Tests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Game.IUObject.SetProperty",(object[] args) =>
            {
                var order = (IUObject)args[0];
                var key = (string)args[1];
                var value = args[2];
                order.setProperty(key, value);
                return new object();
            }
            ).Execute();

        var LongMoveCommand = new Mock<ICommand>().Object;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Game.Commands.LongOperation",
        (object[] args) => {return LongMoveCommand;}).Execute();
    }

    [Fact]
    public void StartCommand_Positive()
    {
        var queue = new Mock<IQueue>();
        var realQueue = new Queue<ICommand>();
        queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(realQueue.Enqueue).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Game.Queue",(object[] args) =>{return queue.Object;}).Execute();

        var startcmd = new Mock<IStartCommand>();
        var target = new Mock<IUObject>();
        var targetProperties = new Dictionary<string, object>();
        var properties = new Dictionary<string, object> {
            { "SomeProperty", "SomeValue" },
        };

        startcmd.SetupGet(s => s.Properties).Returns(properties).Verifiable();
        startcmd.SetupGet(s => s.Target).Returns(target.Object).Verifiable();
        target.Setup(t => t.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>(targetProperties.Add).Verifiable();
        queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(realQueue.Enqueue).Verifiable();

        var startCommand = new StartCommand(startcmd.Object);
        startCommand.Execute();

        Assert.Contains("SomeProperty", targetProperties.Keys);
        Assert.Contains("SomeValue", targetProperties.Values);
        Assert.Contains("Game.Commands.LongOperation", targetProperties.Keys);
        Assert.NotEmpty(realQueue);
    }

    [Fact]
    public void StartCommand_CantSetProperties()
    {
        var startcmd = new Mock<IStartCommand>();
        var target = new Mock<IUObject>();
        var targetProperties = new Dictionary<string, object>();

        var properties = new Dictionary<string, object> {{ "Position", new Vector(0,1,2,3,4,5) },};

        startcmd.SetupGet(s => s.Properties).Returns(properties).Verifiable();
        startcmd.SetupGet(s => s.Target).Returns(target.Object).Verifiable();
        target.Setup(o => o.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback(() => throw new Exception()).Verifiable();

        var startCommand = new StartCommand(startcmd.Object);

        Assert.Throws<Exception>(startCommand.Execute);
    }

    [Fact]
    public void StartCommand_CantPutCommandInQueue()
    {
        var queue = new Mock<IQueue>();
        var realQueue = new Queue<ICommand>();
        queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(()=>{}).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Game.Queue",(object[] args) =>{return queue.Object;} ).Execute();

        var startcmd = new Mock<IStartCommand>();
        var target = new Mock<IUObject>();
        var targetProperties = new Dictionary<string, object>();
        var properties = new Dictionary<string, object> {{ "SomeProperty", new Mock<ICommand>() }};

        startcmd.SetupGet(s => s.Properties).Returns(properties).Verifiable();
        startcmd.SetupGet(s => s.Target).Returns(target.Object).Verifiable();
        target.Setup(o => o.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>(targetProperties.Add).Verifiable();

        var startCommand = new StartCommand(startcmd.Object);
        startCommand.Execute();

        Assert.Empty(realQueue);
    }

    [Fact]
    public void StartCommand_CantReadPropertiesOfOrder_FromStartable()
    {
        var startcmd = new Mock<IStartCommand>();
        startcmd.SetupGet(s => s.Properties).Throws(new Exception()).Verifiable();
        var startCommand = new StartCommand(startcmd.Object);

        Assert.Throws<Exception>(startCommand.Execute);
    }

    [Fact]
    public void StartCommand_CantReadOrder_FromStartable()
    {
        var startcmd = new Mock<IStartCommand>();
        startcmd.SetupGet(s => s.Target).Throws(new Exception()).Verifiable();
        var properties = new Dictionary<string, object> {{ "Position", new Vector(5,4,3,2,1,0) },};

        startcmd.SetupGet(s => s.Properties).Returns(properties).Verifiable();
        var startCommand = new StartCommand(startcmd.Object);

        Assert.Throws<Exception>(startCommand.Execute);
    }
}