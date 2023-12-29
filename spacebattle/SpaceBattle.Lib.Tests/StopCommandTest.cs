using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests
{
    public class EndCommandTests
    {
        private static void SetUpEndCommandTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.Inject", (object[] args) =>
            {
                var target = (IInjectable)args[0];
                var injectedCommand = (ICommand)args[1];
                target.Inject(injectedCommand);
                return target;
            }).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.DeleteProperty", (object[] args) =>
            {
                var target = (IUObject)args[0];
                var properties = (List<string>)args[1];
                properties.ForEach(prop => target.DeleteProperty(prop));
                return "";
            }).Execute();
        }

        [Fact]
        public void TestEndMovementCommand()
        {
            SetUpEndCommandTest();
            var mockEndable = new Mock<IEndable>();
            var mockCommand = new Mock<ICommand>();
            var injectCommand = new InjectCommand(mockCommand.Object);
            var target = new Mock<IUObject>();
            var keys = new List<string>() { "Movement" };
            var characteristics = new Dictionary<string, object>();

            var queue = new Mock<IQueue>();
            var realQueue = new Queue<ICommand>();

            queue.Setup(q => q.Add(It.IsAny<ICommand>())).Callback<ICommand>(() => { }).Verifiable();
            queue.Setup(q => q.Take()).Returns(() => realQueue.Dequeue());
            // IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Add", (object[] args) => { queue.Add(args[0]); }).Execute();
            // IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.Take", (object[] args) => { queue.Take(); }).Execute();

            target.Setup(t => t.setProperty(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>((key, value) => characteristics.Add(key, value));
            target.Setup(t => t.DeleteProperty(It.IsAny<string>())).Callback<string>((string key) => characteristics.Remove(key));
            target.Setup(t => t.getProperty(It.IsAny<string>())).Returns((string key) => characteristics[key]);
            target.Object.setProperty("Movement", 1);

            mockEndable.SetupGet(e => e.command).Returns(injectCommand);
            mockEndable.SetupGet(e => e.target).Returns(target.Object);
            mockEndable.SetupGet(e => e.property).Returns(keys);
            //mockCommand.Setup(x => x.Execute()).Callback().Verifiable();

            var endmovementcommand = new EndMovementCommand(mockEndable.Object);
            // endmovementcommand.Execute();
            // injectCommand.Execute();
            var queueEx = queue.Object;
            // IoC.Resolve("Game.Queue.Add", endmovementcommand);
            // IoC.Resolve("Game.Queue.Add", injectCommand);
            // IoC.Resolve("Game.Queue.Take", endmovementcommand);
            // IoC.Resolve("Game.Queue.Take", injectCommand);
            mockCommand.Setup(x => x.Execute()).Callback<ICommand>(mockCommand => queue.Object.Add(mockCommand));

            realQueue.Enqueue(injectCommand);
            realQueue.Enqueue(endmovementcommand);
            realQueue.Enqueue(injectCommand);

            // queueEx.Add(endmovementcommand);
            // queueEx.Add(injectCommand);
            Assert.NotEmpty(realQueue);
            queueEx.Take().Execute();
            queueEx.Take().Execute();
            queueEx.Take().Execute();
            queue.Verify(q => q.Add(mockCommand.Object), Times.Once());
            // mockCommand.Verify(m => m.Execute(), Times.Never());
            Assert.Empty(realQueue);
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => target.Object.getProperty("Movement"));
        }

        [Fact]
        public void TestInjectCommand()
        {
            SetUpEndCommandTest();

            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(x => x.Execute()).Verifiable();

            var injectCommand = new InjectCommand(mockCommand.Object);
            injectCommand.Inject(new EmptyCommand());
            injectCommand.Execute();

            mockCommand.Verify(m => m.Execute(), Times.Never());
        }
        [Fact]
        public void EndMovementCommandDisabilityToDeletePropertiesCausesExeption()
        {
            SetUpEndCommandTest();

            var mockEndable = new Mock<IEndable>();
            var mockCommand = new Mock<ICommand>();
            var injectCommand = new InjectCommand(mockCommand.Object);
            var target = new Mock<IUObject>();
            var keys = new List<string>() { "NonExistentProperty" };

            target.Setup(t => t.DeleteProperty(It.IsAny<string>())).Callback(() => throw new Exception());

            mockEndable.SetupGet(e => e.command).Returns(injectCommand);
            mockEndable.SetupGet(e => e.target).Returns(target.Object);
            mockEndable.SetupGet(e => e.property).Returns(keys);

            var endmovementcomman = new EndMovementCommand(mockEndable.Object);

            Assert.Throws<Exception>(() => endmovementcomman.Execute());
        }
    }
}
