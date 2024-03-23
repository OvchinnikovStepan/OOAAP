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
        var stopCommand = new Mock<ICommand>();
        var i = 0;
        stopCommand.Setup(x => x.Execute()).Callback(() => { i += 1; });
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.GetThreadIDs", (object[] args) => { return threadList; }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.SoftStopThread", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                stopCommand.Object.Execute();
            });
        }).Execute();

        IoC.Resolve<ICommand>("Game.Commands.StopServerCommand").Execute();

        Assert.Equal(3, i);
    }

    [Fact]
    public void StopServer_TestwithExeption()
    {
        var threadList = new List<int> { 1 };
        var stopCommand = new Mock<ICommand>();
        stopCommand.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.GetThreadIDs", (object[] args) => { return threadList; }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.SoftStopThread", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                stopCommand.Object.Execute();
            });
        }).Execute();

        IoC.Resolve<ICommand>("Game.Commands.StopServerCommand").Execute();

        var testString = "Error occured\r\n";
        Assert.Equal(File.ReadAllText(IoC.Resolve<string>("GetLogFilePath")), testString);
    }
}
