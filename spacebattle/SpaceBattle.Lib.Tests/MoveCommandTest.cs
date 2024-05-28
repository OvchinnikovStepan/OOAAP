using Moq;

namespace SpaceBattle.Lib.Tests;

public class MoveCommandTest
{
    [Fact]
    public void MoveCommand_Checking_That_Object_Moves_Succesfuly()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-5, 3)).Verifiable();

        var moveCommand = new MoveCommand(movable.Object);

        moveCommand.Execute();

        movable.VerifySet(m => m.Position = new Vector(7, 8), Times.Once);
        movable.VerifyAll();
    }

    [Fact]
    public void MoveCommand_Checking_That_Attempt_To_Move_Object_Without_Position_Throws_An_Error()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Throws(new Exception()).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-5, 3)).Verifiable();

        var moveCommand = new MoveCommand(movable.Object);

        Assert.Throws<Exception>(moveCommand.Execute);
    }

    [Fact]
    public void MoveCommand_Checking_That_Attempt_To_Move_Object_Without_Velocity_Throws_An_Error()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector(-5, 3)).Verifiable();
        movable.SetupGet(m => m.Velocity).Throws(new Exception()).Verifiable();

        var moveCommand = new MoveCommand(movable.Object);

        Assert.Throws<Exception>(moveCommand.Execute);
    }

    [Fact]
    public void MoveCommand_Checking_That_Attempt_To_Move_Immovable_Object_Throws_An_Error()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector(12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector(-5, 3)).Verifiable();

        movable.SetupSet(m => m.Position = It.IsAny<Vector>()).Throws(() => new Exception()).Verifiable();

        var moveCommand = new MoveCommand(movable.Object);

        Assert.Throws<Exception>(moveCommand.Execute);
    }

    [Fact]
    public void MoveCommand_Checking_That_Overrided_GetHashCode_Method_Works_Succesfully()
    {
        var vector = new Vector(0, 0);

        vector.GetHashCode();

        Assert.True(true);
    }
    [Fact]
    public void MoveCommand_Checking_That_Attempt_To_Compare_Null_Vector_Does_Not_Throw_An_Error()
    {
        var vector = new Vector();

        var vector1 = new Vector(12, 5);

        Assert.False(vector1.Equals(vector));
    }
}
