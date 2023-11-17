using Moq;

namespace SpaceBattle.Lib.Tests;

public class MoveCommandTest
{

    [Fact]
    public void MoveCommandPositive()
    {
        // pre
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new int[] { 12, 5 }).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new int[] { -7, 3 }).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);

        // action
        moveCommand.Execute();

        //post
        // movable.Position is correct
        movable.VerifySet(m => m.Position = new int[] { 5, 8 }, Times.Once);
        movable.VerifyAll(); // !!!
    }
}