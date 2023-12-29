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
            IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.IUObject.SetProperty",
            (object[] args) =>
            {
                var target = (IUObject)args[0];
                var key = (string)args[1];
                var value = args[2];

                target.setProperty(key, value);
                return new object();
            }
        ).Execute();

            var longOperationCommand = new Mock<ICommand>().Object;
            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "Game.Commands.LongOperation",
                (object[] args) =>
                {
                    return longOperationCommand;
                }
            ).Execute();

            var init = new InitLongOperationStrategy();
            init.Execute();

        }

        [Fact]
        public void LongOperationTestPositive()
        {
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(x => x.Execute()).Verifiable();

            var name = "Movement";
            var mockUObject = new Mock<IUObject>();

            var queue = new Mock<IQueue>();
            var realQueue = new Queue<ICommand>();
            queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(realQueue.Enqueue);
            queue.Setup(q => q.Take()).Returns(() => realQueue.Dequeue());

            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "Game.Queue",
                (object[] args) =>
                {
                    return queue.Object;
                }
            ).Execute();

            var startable = new Mock<IStartCommand>();
            var target = new Mock<IUObject>();
            var targetDict = new Dictionary<string, object>();
            var properties = new Dictionary<string, object> {
                { "id", 1 },
            };

            startable.SetupGet(s => s.Properties).Returns(properties);
            startable.SetupGet(s => s.Target).Returns(target.Object);
            target.Setup(o => o.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>(targetDict.Add);
            queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(realQueue.Enqueue);

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command." + name, (object[] args) => mockCommand.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.ConvertToStartable", (object[] args) =>
            {
                var emptyStartableObject = new Mock<IStartCommand>();
                emptyStartableObject.Setup(x => x.Target).Returns((IUObject)args[0]);
                emptyStartableObject.Setup(x => x.Properties).Returns(new Dictionary<string, object>());
                return emptyStartableObject.Object;
            }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.NewStartCommand", (object[] args) => new StartCommand((IStartCommand)args[0])).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Macro", (object[] args) => mockCommand.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.Injectable", (object[] args) => new InjectCommand(mockCommand.Object)).Execute();
            var LOcmd = IoC.Resolve<ICommand>("Game.Commands.InitLO", name, mockUObject.Object);
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Operation." + name, (object[] args) => { return LOcmd; }).Execute();

            IoC.Resolve<ICommand>("Game.Operation." + name).Execute();

            queue.Object.Take().Execute();
            mockCommand.Verify();
        }
        [Fact]
        public void LongOperationTestNegative()
        {
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(x => x.Execute());

            var unactiveMockCommand = new Mock<ICommand>();
            unactiveMockCommand.Setup(u => u.Execute()).Verifiable();

            var name = "Movement";
            var mockUObject = new Mock<IUObject>();

            var queue = new Mock<IQueue>();
            var realQueue = new Queue<ICommand>();

            queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(realQueue.Enqueue);
            queue.Setup(q => q.Take()).Returns(() => realQueue.Dequeue());

            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "Game.Queue",
                (object[] args) =>
                {
                    return queue.Object;
                }
            ).Execute();

            var startable = new Mock<IStartCommand>();
            var target = new Mock<IUObject>();
            var targetDict = new Dictionary<string, object>();
            var properties = new Dictionary<string, object> {
                { "id", 1 },
            };

            startable.SetupGet(s => s.Properties).Returns(properties);
            startable.SetupGet(s => s.Target).Returns(target.Object);
            target.Setup(o => o.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>(targetDict.Add);
            queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(realQueue.Enqueue);

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command." + name, (object[] args) => mockCommand.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.ConvertToStartable", (object[] args) =>
            {
                var emptyStartableObject = new Mock<IStartCommand>();
                emptyStartableObject.Setup(x => x.Target).Returns((IUObject)args[0]);
                emptyStartableObject.Setup(x => x.Properties).Returns(new Dictionary<string, object>());
                return emptyStartableObject.Object;
            }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.NewStartCommand", (object[] args) => new StartCommand((IStartCommand)args[0])).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Macro", (object[] args) => mockCommand.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.Injectable", (object[] args) => new InjectCommand(mockCommand.Object)).Execute();
            var LOcmd = IoC.Resolve<ICommand>("Game.Commands.InitLO", name, mockUObject.Object);
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Operation." + name, (object[] args) => { return LOcmd; }).Execute();

            IoC.Resolve<ICommand>("Game.Operation." + name, mockUObject.Object).Execute();

            queue.Object.Take().Execute();
            unactiveMockCommand.Verify(x => x.Execute(), Times.Never);
        }
    }
}
