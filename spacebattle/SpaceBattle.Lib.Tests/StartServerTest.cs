using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;
using Moq;

public class StartServerTest
{
    public StartServerTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        var a = new InitCommand();
        a.Execute();
    }
    [Fact]
    public void Start_Server_Test()
    {
        var startCommand = new Mock<Hwdtech.ICommand>();
        startCommand.Setup(cmd => cmd.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.CreateAndStartThread", (object[] args) =>
        {
            return startCommand.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Game.Commands.StartServerCommand", 5).Execute();
        startCommand.Verify(cmd => cmd.Execute(), Times.Exactly(5));
    }
}
