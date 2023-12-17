using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests
{
    public class LongOperationTests
    {
        public LongOperationTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();

            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.NewStartCommand", (object[] args) => new NewStartCommand((ICommand)args[0])).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Repeat", (object[] args) => (ICommand)args[0]).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Inject", (object[] args) => (ICommand)args[0]).Execute();
        }

        [Fact]
        public void TestPositive()
        {
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(x => x.Execute()).Verifiable();

            var name = "Movement";
            var mockUObject = new Mock<IUObject>();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command." + name, (object[] args) => mockCommand.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Operation." + name, (object[] args) => { return new LongOperationStrategy(name, (IUObject)args[0]).Run(); }).Execute();

            IoC.Resolve<ICommand>("Game.Operation." + name, mockUObject.Object).Execute();
            mockCommand.Verify();
        }
        [Fact]
        public void TestNegative()
        {
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(m => m.Execute());

            var unactiveMockCommand = new Mock<ICommand>();
            unactiveMockCommand.Setup(u => u.Execute()).Verifiable();

            var name = "Fire";
            var mockUObject = new Mock<IUObject>();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command." + name, (object[] args) => mockCommand.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Operation." + name, (object[] args) => { return new LongOperationStrategy(name, (IUObject)args[0]).Run(); }).Execute();

            IoC.Resolve<ICommand>("Game.Operation." + name, mockUObject.Object).Execute();
            unactiveMockCommand.Verify(x => x.Execute(), Times.Never);
        }
    }
}
