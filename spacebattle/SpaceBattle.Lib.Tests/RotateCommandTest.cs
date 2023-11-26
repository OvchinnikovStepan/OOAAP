namespace SpaceBattle.Lib.Tests;
using Moq;

public class RotateCommandTest
{
    [Fact]
    public void PositiveRotationCommand()
    {
        var rotatable = new Mock<IRotatable>();
        rotatable.SetupGet(m => m.Position).Returns(new Rotate_Vector(360, 45)).Verifiable();
        rotatable.SetupGet(m => m.Velocity).Returns(new Rotate_Vector(360, 45)).Verifiable();
        var rotationCommand = new RotateCommand(rotatable.Object);

        rotationCommand.Execute();

        rotatable.VerifySet(m => m.Position = new Rotate_Vector(360, 90), Times.Once);
        rotatable.VerifyAll();
    }
    [Fact]
    public void CannotDetermineAngle()
    {
        var rotatable = new Mock<IRotatable>();

        rotatable.SetupGet(m => m.Position).Throws(new Exception()).Verifiable();
        rotatable.SetupGet(m => m.Velocity).Returns(new Rotate_Vector(360, 45)).Verifiable();

        ICommand rotateCommand = new RotateCommand(rotatable.Object);

        Assert.Throws<Exception>(rotateCommand.Execute);
    }
    [Fact]
    public void CannotDetermineAngularVelocity()
    {
        var rotatable = new Mock<IRotatable>();

        rotatable.SetupGet(m => m.Position).Returns(new Rotate_Vector(360, 45)).Verifiable();
        rotatable.SetupGet(m => m.Velocity).Throws(new Exception()).Verifiable();

        ICommand rotateCommand = new RotateCommand(rotatable.Object);

        Assert.Throws<Exception>(rotateCommand.Execute);
    }
    [Fact]
    public void CannotChangeAngle()
    {
        var rotatable = new Mock<IRotatable>();

        rotatable.SetupGet(m => m.Position).Returns(new Rotate_Vector(360, 45)).Verifiable();
        rotatable.SetupGet(m => m.Velocity).Returns(new Rotate_Vector(360, 45)).Verifiable();

        rotatable.SetupSet(m => m.Position = It.IsAny<Rotate_Vector>()).Throws(new Exception()).Verifiable();

        ICommand rotateCommand = new RotateCommand(rotatable.Object);

        Assert.Throws<Exception>(rotateCommand.Execute);
    }
    [Fact]
    public void HashCodeTest()
    {
        Rotate_Vector rotate_Vector = new Rotate_Vector(360, 0);

        int hashCode = rotate_Vector.GetHashCode();

        Assert.True(true);
    }
}
