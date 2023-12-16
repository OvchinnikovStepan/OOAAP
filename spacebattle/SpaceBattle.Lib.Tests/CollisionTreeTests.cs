using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests
{
    public class CheckCollisionTests
    {
        public CheckCollisionTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.GetProperty", (object[] args) => new List<int> { 1, 1, 1, 1, 1 }).Execute();

            new InitFindVariationsCommand().Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.CheckCollision", (object[] args) => new CollisionCheckCommand((IUObject)args[0], (IUObject)args[1])).Execute();
        }

        [Fact]
        public void CheckCollision_Command_Should_Not_Execute_Collision_Event_If_No_Collision_Occurs()
        {
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.Execute()).Verifiable();

            var mockDictionary = new Mock<IDictionary<int, object>>();
            mockDictionary.SetupGet(d => d[It.IsAny<int>()]).Throws(new System.Collections.Generic.KeyNotFoundException()).Verifiable();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.BuildCollisionTree", (object[] args) => mockDictionary.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Event.Collision", (object[] args) => mockCommand.Object).Execute();

            var mockUObject = new Mock<IUObject>();

            var checkCollisionCommand = IoC.Resolve<ICommand>("Game.Command.CheckCollision", mockUObject.Object, mockUObject.Object);

            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => checkCollisionCommand.Execute());
            mockDictionary.Verify(d => d[It.IsAny<int>()], Times.Once());
            mockCommand.Verify(command => command.Execute(), Times.Never());
        }


        [Fact]
        public void CheckCollision_Command_Should_Execute_Collision_Event_If_Collision_Occurs()
        {

            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.Execute()).Verifiable();

            var mockDictionary = new Mock<IDictionary<int, object>>();
            mockDictionary.SetupGet(d => d[It.IsAny<int>()]).Returns(mockDictionary.Object);

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.BuildCollisionTree", (object[] args) => mockDictionary.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Event.Collision", (object[] args) => mockCommand.Object).Execute();

            var mockUObject = new Mock<IUObject>();

            var checkCollisionCommand = IoC.Resolve<ICommand>("Game.Command.CheckCollision", mockUObject.Object, mockUObject.Object);

            Assert.NotNull(checkCollisionCommand);
            Assert.IsType<CollisionCheckCommand>(checkCollisionCommand);
            checkCollisionCommand.Execute();
        }

        [Fact]
        public void CheckCollision_Command_Should_Return_Valid_Collision_Tree_If_Collision_Occurs()
        {
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.Execute()).Verifiable();

            var mockDictionary = new Mock<IDictionary<int, object>>();
            mockDictionary.SetupGet(d => d[It.IsAny<int>()]).Returns(new Dictionary<int, object>()).Verifiable();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Command.BuildCollisionTree", (object[] args) => mockDictionary.Object).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Event.Collision", (object[] args) => mockCommand.Object).Execute();

            var mockUObject = new Mock<IUObject>();

            var checkCollisionCommand = IoC.Resolve<ICommand>("Game.Command.CheckCollision", mockUObject.Object, mockUObject.Object);

            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => { checkCollisionCommand.Execute(); });

            mockDictionary.Verify(d => d[It.IsAny<int>()], Times.Once());
            mockCommand.Verify(command => command.Execute(), Times.Never());
        }

        [Fact]
        public void InitFindVariationsCommand_Should_Initialize_FindVariationsCommand()
        {
            var findVariationsCommand = new InitFindVariationsCommand();
            Assert.NotNull(findVariationsCommand);
        }
    }
}
