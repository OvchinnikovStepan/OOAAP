using Hwdtech;
using Hwdtech.Ioc;
using Moq;
namespace SpaceBattle.Lib.Test;

public class MessageTests
{
    public MessageTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void QueuePushCommandPush()
    {
        var mockCommand = new Mock<ICommand>();
        var mockMessage = new Mock<IMessage>();
        mockCommand.Setup(a => a.Execute()).Verifiable();
        mockMessage.SetupGet(m => m.GameID).Returns("GameABC").Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Push", (object[] args) => args[1]).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.CreateCommand", (object[] args) => mockCommand.Object).Execute();

        var command = new InterpreterCommand(mockMessage.Object);
        command.Execute();

        mockCommand.Verify(x => x.Execute(), Times.Once);
    }
    [Fact]
    public void Queue_CantReadId()
    {
        var mockCommand = new Mock<ICommand>();
        var mockMessage = new Mock<IMessage>();
        mockCommand.Setup(a => a.Execute()).Verifiable();
        mockMessage.SetupGet(m => m.GameID).Throws(new Exception()).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Push", (object[] args) => args[1]).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.CreateCommand", (object[] args) => mockCommand.Object).Execute();

        var command = new InterpreterCommand(mockMessage.Object);

        Assert.Throws<Exception>(command.Execute);
    }
}
