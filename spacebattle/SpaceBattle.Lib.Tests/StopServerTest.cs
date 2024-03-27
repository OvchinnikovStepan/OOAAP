using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;
using Moq;

public class StopServerTest
{
    public StopServerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var logfilepath = Path.GetTempFileName();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetLogFilePath", (object[] args) =>
      {
          return logfilepath;
      }).Execute();
        var a = new InitCommand();
        a.Execute();
    }

    [Fact]

    public void StopServer_Test()
    {
        var threadList = new List<int> { 1, 2, 3 };
        var barrier = new Barrier(4);
        var stopCommand = new Mock<Hwdtech.ICommand>();
        stopCommand.Setup(x => x.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.GetThreadIDs", (object[] args) => { return threadList; }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.StopServerBarrierRemove", (object[] args) =>
        {
            return () => { barrier.RemoveParticipant(); };
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.StopServerBarrierWait", (object[] args) =>
        {
            return new ActionCommand(() => { barrier.SignalAndWait(); });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.SoftStopThread", (object[] args) =>
        {
            return new ActionCommand((Action)args[1]);
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.SendCommand", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                ((Hwdtech.ICommand)args[1]).Execute();
                stopCommand.Object.Execute();
            });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("Game.Commands.StopServerCommand").Execute();

        Assert.Equal(1, barrier.CurrentPhaseNumber);
        stopCommand.Verify(cmd => cmd.Execute(), Times.Exactly(3));
    }

    [Fact]
    public void StopServer_TestwithExeption()
    {
        IoC.Resolve<Hwdtech.ICommand>("Game.Commands.ExeptionHandler", new ActionCommand(() => { }), new Exception()).Execute();

        var testString = "Error occurred in command: SpaceBattle.Lib.ActionCommand\nException: System.Exception: Exception of type 'System.Exception' was thrown.\n";
        Assert.Equal(File.ReadAllText(IoC.Resolve<string>("GetLogFilePath")), testString);
    }
}
