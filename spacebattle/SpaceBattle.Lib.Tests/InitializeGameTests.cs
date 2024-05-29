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
    public void QueuePopStrategy_Test_DissabilityToGetQueueCauseException()
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
    public void QueuePopStrategy_Test_DissabilityToTakeCommandCauseException()
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
}
