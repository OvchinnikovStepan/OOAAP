using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;
using Moq;

public class StartConditionTest
{
    public StartConditionTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void StartCondition_Test_RegistrationAndIncommingCommandTest()
    {
        var testCommand = new Mock<Hwdtech.ICommand>();
        testCommand.Setup(x => x.Execute()).Verifiable();
        var dependencies = new Dictionary<string, IStrategy>();
        dependencies.Add("Shoot", new CreateShootCommandStrategy());
        dependencies.Add("StartMovement", new CreateStartMovementCommandStrategy());
        dependencies.Add("EndMovement", new CreateEndMovementCommandStrategy());
        dependencies.Add("Rotate", new CreateRotationCommandStrategy());
        var obj = new Mock<IUObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.Get", (object[] args) => obj.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Dependencies.Get.Commands", (object[] args) => dependencies).Execute();
        new RegisterCommandsCommand().Execute();
        var message = new Mock<IMessage>();
        message.SetupGet(m => m.GameID).Returns("1").Verifiable();
        message.SetupGet(m => m.Properties).Returns(new Dictionary<string, object>() { { "1", "1" } });
        message.SetupGet(m => m.OrderType).Returns("Rotate");
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.SetProprty", (object[] args) => testCommand.Object).Execute();

        var iRotatable = new Mock<IRotatable>();
        iRotatable.SetupGet(r => r.Position).Returns(new Rotate_Vector(1, 0)).Verifiable();
        iRotatable.SetupGet(r => r.Velocity).Returns(new Rotate_Vector(1, 0));
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.Adapter.IRotatable", (object[] args) => iRotatable.Object).Execute();
        new IncomingCommand(message.Object).Execute();

        testCommand.Verify(m => m.Execute(), Times.Once());
        iRotatable.Verify(r => r.Position, Times.Once());
    }
    [Fact]
    public void StartCondition_Test_ShootingTest()
    {
        var strategy = new CreateShootCommandStrategy();
        var iShootable = new Mock<IShootable>();
        var testCommand = new Mock<Hwdtech.ICommand>();
        testCommand.Setup(m => m.Execute()).Verifiable();
        var pushCmd = new Mock<Hwdtech.ICommand>();
        var obj = new Mock<IUObject>();
        pushCmd.Setup(x => x.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Create.Bullet", (object[] args) =>
        {
            testCommand.Object.Execute();
            return (object)1;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Bullet.Act", (object[] args) =>
        {
            testCommand.Object.Execute();
            return pushCmd.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.Adapter.IShootable", (object[] args) =>
        {
            testCommand.Object.Execute();
            return iShootable.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Push", (object[] args) =>
        {
            testCommand.Object.Execute();
            return pushCmd.Object;
        }).Execute();
        ((ICommand)new CreateShootCommandStrategy().Run(new object[] { obj })).Execute();
        pushCmd.Verify(x => x.Execute(), Times.Once());
        testCommand.Verify(x => x.Execute(), Times.Exactly(4));
    }
}
