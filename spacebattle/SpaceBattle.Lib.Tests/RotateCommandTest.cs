namespace SpaceBattle.Lib.Tests;
using Moq;

public class MoveCommandTest
{
    [Fact]
    public void MoveCommandPositive()
    {
        //pre

        var rotatable = new Mock<IRotatable>();
        rotatable.SetupGet(m => m.Position).Returns(new Rotate_Vector(360, 45)).Verifiable();
        rotatable.SetupGet(m => m.Velocity).Returns(new Rotate_Vector(360, 90)).Verifiable();
        var mc = new RotateCommand(rotatable.Object);

        //act
        mc.Execute();

        //post
        //movable // pos == (7, 8)

        rotatable.VerifySet(m => m.Position = new Rotate_Vector(360, 135), Times.Once);
        rotatable.VerifyAll();
    }
}
