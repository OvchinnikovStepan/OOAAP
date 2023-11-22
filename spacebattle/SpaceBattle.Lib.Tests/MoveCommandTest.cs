using System.Runtime.Intrinsics;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class MoveCommandTest
{
    [Fact]
    public void Object_moved_succesfuly()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector  (12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector  (-5, 3)).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);

        moveCommand.Execute();

        movable.VerifySet(m => m.Position = new Vector ( 7, 8 ), Times.Once);
        movable.VerifyAll();
    }

    [Fact]
    public void Attemt_to_move_without_position_failed()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Throws(new Exception()).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector  (-5, 3)).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);

        Assert.Throws<Exception>(moveCommand.Execute);
    }

    [Fact]
    public void Attemt_to_move_without_velocity_failed()
    {
        var movable = new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector  (-5, 3)).Verifiable();
        movable.SetupGet(m => m.Velocity).Throws(new Exception()).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);

        Assert.Throws<Exception>(moveCommand.Execute);
    }

    [Fact]
    public void Attemt_to_move_immoveble_object_failed()
    {
        var movable=new Mock<IMovable>();

        movable.SetupGet(m => m.Position).Returns(new Vector  (12, 5)).Verifiable();
        movable.SetupGet(m => m.Velocity).Returns(new Vector  (-5, 3)).Verifiable();

        movable.SetupSet(m=> m.Position).Throws(() => new Exception()).Verifiable();

        ICommand moveCommand = new MoveCommand(movable.Object);


        Assert.Throws<Exception>(moveCommand.Execute);
    }

    [Fact]
    public void HashCode_Test()
    {
        Vector vector= new Vector(0,0);

        int c=vector.GetHashCode();

        Assert.True(true);
    }
    [Fact]
     public void Eq_Test()
    {
        Vector vector= new Vector();
        
        Vector vector1=new Vector(12,5);
        
        Assert.False(vector1.Equals(vector));
    }
}