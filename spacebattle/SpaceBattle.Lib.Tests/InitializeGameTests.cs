using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;
public class InitializeGameTests
{
    public InitializeGameTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void Start_Server_Test()
    {

    }
}
