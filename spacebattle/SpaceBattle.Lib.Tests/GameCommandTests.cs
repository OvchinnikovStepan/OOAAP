namespace SpaceBattle.Lib.Test;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using SpaceBattle.Lib;
using Xunit;

public class GameCommandTests
{

    public GameCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }

    [Fact]
    public void GameCommand_Works_Correctly()
    {
        var realQueue = new Queue<Hwdtech.ICommand>();
        var someCmd = new Mock<Hwdtech.ICommand>();

        someCmd.Setup(command => command.Execute()).Verifiable();

        realQueue.Enqueue(someCmd.Object);

        realQueue.Enqueue(someCmd.Object);
        var queue = new Mock<CmdSource>();
        queue.Setup(q => q.Take()).Returns(() =>
         {
             return realQueue.Dequeue();
         }).Verifiable();
        queue.Setup(q => q.IsEmpty()).Returns(() => realQueue.Count == 0).Verifiable();

        double quant = 100;

        var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetGameQuant", (object[] par) => (object)quant).Execute();
        var game = new GameCommand(queue.Object, gameScope);

        game.Execute();
        queue.Verify(q => q.IsEmpty(), Times.Exactly(3));
        queue.Verify(q => q.Take(), Times.Exactly(2));
        someCmd.Verify(cmd => cmd.Execute(), Times.Exactly(2));
    }

    [Fact]
    public void GameCommand_Cmds_does_not_execute_after_quand_ends()
    {
        var realQueue = new Queue<Hwdtech.ICommand>();
        var delayCmd = new Mock<Hwdtech.ICommand>();
        delayCmd.Setup(command => command.Execute()).Callback(() =>
        {
            Thread.Sleep(50);
        }).Verifiable();
        var someCmd = new Mock<Hwdtech.ICommand>();
        someCmd.Setup(command => command.Execute()).Verifiable();
        realQueue.Enqueue(delayCmd.Object);
        realQueue.Enqueue(someCmd.Object);
        var queue = new Mock<CmdSource>();
        queue.Setup(q => q.Take()).Returns(() =>
        {
            return realQueue.Dequeue();
        });
        queue.Setup(q => q.IsEmpty()).Returns(() => realQueue.Count == 0);
        double quant = 20;
        var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetGameQuant", (object[] par) => (object)quant).Execute();
        var game = new GameCommand(queue.Object, gameScope);
        game.Execute();
        delayCmd.Verify(cmd => cmd.Execute(), Times.Once());
        someCmd.Verify(cmd => cmd.Execute(), Times.Never());
    }
    [Fact]
    public void GameCommand_Disability_to_get_quant_cause_exeption()
    {
        var excCmd = new Mock<Hwdtech.ICommand>();
        excCmd.Setup(c => c.Execute()).Throws(new Exception());
        var realQueue = new Queue<Hwdtech.ICommand>();
        var queue = new Mock<CmdSource>();
        queue.Setup(q => q.Take()).Returns(() =>
        {
            return realQueue.Dequeue();
        });
        queue.Setup(q => q.IsEmpty()).Returns(() => realQueue.Count == 0);
        double quant = 20;
        var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetGameQuant", (object[] par) =>
        {
            excCmd.Object.Execute();
            return (object)quant;
        }).Execute();
        var game = new GameCommand(queue.Object, gameScope);
        Assert.Throws<Exception>(() => { game.Execute(); });
    }
    [Fact]
    public void GameCommand_Exeption_handler()
    {

        var cmd1 = new Mock<Hwdtech.ICommand>();
        var cmd2 = new Mock<Hwdtech.ICommand>();
        cmd1.Setup(q => q.Execute()).Throws(new Exception());
        cmd2.Setup(q => q.Execute()).Throws(new Exception());

        var exhandleCmd = new Mock<Hwdtech.ICommand>();
        exhandleCmd.Setup(c => c.Execute()).Verifiable();
        var exhandledefaultCmd = new Mock<Hwdtech.ICommand>();
        exhandleCmd.Setup(c => c.Execute()).Verifiable();

        var realQueue = new Queue<Hwdtech.ICommand>();
        var queue = new Mock<CmdSource>();
        queue.Setup(q => q.Take()).Returns(() =>
        {
            return realQueue.Dequeue();
        });
        queue.Setup(q => q.IsEmpty()).Returns(() => realQueue.Count == 0);

        var regcmd = new Mock<Hwdtech.ICommand>();
        regcmd.Setup(c => c.Execute()).Callback(() =>
        {
            new InitDefaultExceptionHandler().Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TryFindStrategy", (object[] args) =>
            {
                return (object)((Hwdtech.ICommand)args[0] == cmd1.Object);
            }).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExeptionStrategy", (object[] args) =>
            {
                return exhandleCmd.Object;
            }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "DefaultStrategy", (object[] args) =>
            {
                return exhandledefaultCmd.Object;
            }).Execute();
        });
        realQueue.Enqueue(regcmd.Object);
        realQueue.Enqueue(cmd1.Object);
        realQueue.Enqueue(cmd2.Object);
        double quant = 1000;
        var gameScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetGameQuant", (object[] par) => (object)quant).Execute();
        var game = new GameCommand(queue.Object, gameScope);
        game.Execute();

        exhandleCmd.Verify(cmd => cmd.Execute(), Times.Once());
        exhandledefaultCmd.Verify(cmd => cmd.Execute(), Times.Once());
    }
}
