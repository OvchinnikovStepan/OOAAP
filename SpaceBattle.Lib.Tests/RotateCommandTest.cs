namespace SpaceBattle.Lib.Tests;
using Moq;

public class MoveCommandTest
{
    [Fact]
    public void CommandPositive()
    {
        //pre

        var rotatable = new Mock<IRotatable>();
        rotatable.SetupGet(m => m.Position).Returns(new Rotate_Vector(360, 45)).Verifiable();
        rotatable.SetupGet(m => m.Velocity).Returns(new Rotate_Vector(360, 45)).Verifiable();
        var mc = new RotateCommand(rotatable.Object);

        //act
        mc.Execute();

        rotatable.VerifySet(m => m.Position = new Rotate_Vector(360, 90), Times.Once);
        rotatable.VerifyAll();
    }
    [Fact]
    public void Attemt_to_move_without_position_failed()
    {
        var movable = new Mock<IRotatable>();

        movable.SetupGet(m => m.Position).Throws(new Exception()).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Rotate_Vector(360, 45)).Verifiable();

        ICommand moveCommand = new RotateCommand(movable.Object);

        Assert.Throws<Exception>(moveCommand.Execute);
    }
    [Fact]
    public void Attemt_to_move_without_velocity_failed()
    {
        var movable = new Mock<IRotatable>();

        movable.SetupGet(m => m.Position).Returns(new Rotate_Vector(360, 45)).Verifiable();
        movable.SetupGet(m => m.Velocity).Throws(new Exception()).Verifiable();

        ICommand moveCommand = new RotateCommand(movable.Object);

        Assert.Throws<Exception>(moveCommand.Execute);
    }
    [Fact]
    public void Attemt_to_move_immoveble_object_failed()
    {
        var movable = new Mock<IRotatable>();

        movable.SetupGet(m => m.Position).Returns(new Rotate_Vector(360, 45)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Rotate_Vector(360, 45)).Verifiable();

        movable.SetupSet(m => m.Position = It.IsAny<Rotate_Vector>()).Throws(new Exception()).Verifiable();

        ICommand moveCommand = new RotateCommand(movable.Object);

        Assert.Throws<Exception>(moveCommand.Execute);
    }
}
