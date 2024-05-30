using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;
using Moq;

public class StartConditionsTests
{
    public StartConditionsTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void Test_Creating_IUObjcet_List_Successfully()
    {
        var IUobjectlist = new Dictionary<int, IUObject>();
        var obj = new Mock<IUObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) =>
        {
            return IUobjectlist;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.Create", (object[] args) =>
        {
            return obj.Object;
        }).Execute();
        new CreateStartingIUObjectListCommand(3).Execute();
        Assert.Equal(3, IUobjectlist.Count);
        Assert.Equal(obj.Object, IUobjectlist[0]);
    }
    [Fact]
    public void Test_Creating_IUObject_List_Disabbility_To_Get_IUObject_List_Cause_Exception()
    {
        var IUobjectlist = new Dictionary<int, IUObject>();
        var obj = new Mock<IUObject>();
        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) =>
        {
            exeptionCmd.Object.Execute();
            return IUobjectlist;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.Create", (object[] args) =>
        {
            return obj.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new CreateStartingIUObjectListCommand(3).Execute(); });
    }
    [Fact]
    public void Test_Creating_IUObject_List_Disabbility_To_Add_IUObject_To_List_Cause_Exception()
    {
        var IUobjectlist = new Dictionary<int, IUObject>();
        var obj = new Mock<IUObject>();
        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.List", (object[] args) =>
        {
            return IUobjectlist;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.IUObject.Create", (object[] args) =>
        {
            exeptionCmd.Object.Execute();
            return obj.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new CreateStartingIUObjectListCommand(3).Execute(); });
    }
    [Fact]
    public void Test_Arranging_One_SpaceShip_Successfully()
    {
        var obj = new Mock<IUObject>();
        var somecommand = new Mock<Hwdtech.ICommand>();
        var iterator = new Mock<IEnumerator<object>>();
        somecommand.Setup(x => x.Execute()).Verifiable();
        iterator.Setup(x => x.MoveNext()).Verifiable();
        iterator.SetupGet(x => x.Current).Returns(new Vector(1, 1)).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            return somecommand.Object;
        }).Execute();
        new ArrangeOneSpaceShipCommand(obj.Object, iterator.Object).Execute();
        somecommand.Verify(somecommand => somecommand.Execute(), Times.Once());
        iterator.Verify(iterator => iterator.MoveNext(), Times.Once());
        iterator.Verify(iterator => iterator.Current, Times.Once());
    }
    [Fact]
    public void Test_Arranging_One_SpaceShip_Disabbility_To_Read_Iterators_Current_Cause_Exception()
    {
        var obj = new Mock<IUObject>();
        var somecommand = new Mock<Hwdtech.ICommand>();
        var iterator = new Mock<IEnumerator<object>>();

        somecommand.Setup(x => x.Execute()).Verifiable();
        iterator.Setup(x => x.MoveNext()).Verifiable();
        iterator.SetupGet(x => x.Current).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            return somecommand.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeOneSpaceShipCommand(obj.Object, iterator.Object).Execute(); });
    }
    [Fact]
    public void Test_Arranging_One_SpaceShip_Disabbility_To_Create_Set_Property_Command_Cause_Exception()
    {
        var obj = new Mock<IUObject>();
        var somecommand = new Mock<Hwdtech.ICommand>();
        var iterator = new Mock<IEnumerator<object>>();

        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());

        somecommand.Setup(x => x.Execute()).Verifiable();
        iterator.Setup(x => x.MoveNext()).Verifiable();
        iterator.SetupGet(x => x.Current).Returns(new Vector(1, 1));
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            exeptionCmd.Object.Execute();
            return somecommand.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeOneSpaceShipCommand(obj.Object, iterator.Object).Execute(); });
    }
    [Fact]
    public void Test_Arranging_One_SpaceShip_Disabbility_To_Set_Property_Cause_Exception()
    {
        var obj = new Mock<IUObject>();
        var somecommand = new Mock<Hwdtech.ICommand>();
        var iterator = new Mock<IEnumerator<object>>();

        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());

        somecommand.Setup(x => x.Execute()).Verifiable();
        iterator.Setup(x => x.MoveNext()).Verifiable();
        iterator.SetupGet(x => x.Current).Returns(new Vector(1, 1));
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            return exeptionCmd.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeOneSpaceShipCommand(obj.Object, iterator.Object).Execute(); });
    }
    [Fact]
    public void Test_Arranging_One_SpaceShip_Disabbility_To_Move_Next_Cause_Exception()
    {
        var obj = new Mock<IUObject>();
        var somecommand = new Mock<Hwdtech.ICommand>();
        var iterator = new Mock<IEnumerator<object>>();

        somecommand.Setup(x => x.Execute()).Verifiable();
        iterator.Setup(x => x.MoveNext()).Throws(new Exception()).Verifiable();
        iterator.SetupGet(x => x.Current).Returns(new Vector(1, 1));
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            return somecommand.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeOneSpaceShipCommand(obj.Object, iterator.Object).Execute(); });
    }
    [Fact]
    public void Test_Arranging_SpaceShips_Successfully()
    {
        var spaceships = new List<IUObject>() { new Mock<IUObject>().Object, new Mock<IUObject>().Object };
        var iterator = new Mock<IEnumerator<object>>();
        iterator.Setup(x => x.Reset()).Verifiable();
        var somecommand = new Mock<Hwdtech.ICommand>();
        somecommand.Setup(x => x.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Iterators.Position", (object[] args) =>
        {
            return iterator.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Arrange.SpaceShip", (object[] args) =>
        {
            return somecommand.Object;
        }).Execute();
        new ArrangeSpaceShipsCommand(spaceships).Execute();
        iterator.Verify(x => x.Reset(), Times.Once());
        somecommand.Verify(x => x.Execute(), Times.Exactly(2));
    }

    [Fact]
    public void Test_Arranging_SpaceShips_Disability_To_Get_Iterator_Cause_Exception()
    {
        var spaceships = new List<IUObject>() { new Mock<IUObject>().Object, new Mock<IUObject>().Object };
        var iterator = new Mock<IEnumerator<object>>();
        iterator.Setup(x => x.Reset()).Verifiable();
        var somecommand = new Mock<Hwdtech.ICommand>();
        somecommand.Setup(x => x.Execute()).Verifiable();

        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Iterators.Position", (object[] args) =>
        {
            exeptionCmd.Object.Execute();
            return iterator.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Arrange.SpaceShip", (object[] args) =>
        {
            return somecommand.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeSpaceShipsCommand(spaceships).Execute(); });
    }
    [Fact]
    public void Test_Arranging_SpaceShips_Disability_To_Arrange_Spaceship_Cause_Exception()
    {
        var spaceships = new List<IUObject>() { new Mock<IUObject>().Object, new Mock<IUObject>().Object };
        var iterator = new Mock<IEnumerator<object>>();
        iterator.Setup(x => x.Reset()).Verifiable();
        var somecommand = new Mock<Hwdtech.ICommand>();
        somecommand.Setup(x => x.Execute()).Verifiable();

        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Iterators.Position", (object[] args) =>
        {
            return iterator.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Arrange.SpaceShip", (object[] args) =>
        {
            exeptionCmd.Object.Execute();
            return somecommand.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeSpaceShipsCommand(spaceships).Execute(); });
    }
    [Fact]
    public void Test_Arranging_SpaceShips_Disability_To_Reset_Iterator_Cause_Exception()
    {
        var spaceships = new List<IUObject>() { new Mock<IUObject>().Object, new Mock<IUObject>().Object };
        var iterator = new Mock<IEnumerator<object>>();
        iterator.Setup(x => x.Reset()).Throws(new Exception());
        var somecommand = new Mock<Hwdtech.ICommand>();
        somecommand.Setup(x => x.Execute()).Verifiable();

        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Iterators.Position", (object[] args) =>
        {
            return iterator.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Arrange.SpaceShip", (object[] args) =>
        {
            return somecommand.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeSpaceShipsCommand(spaceships).Execute(); });
    }
    [Fact]
    public void Test_Arranging_SpaceShips_Disability_To_Execute_Arrange_Command_Cause_Exception()
    {
        var spaceships = new List<IUObject>() { new Mock<IUObject>().Object, new Mock<IUObject>().Object };
        var iterator = new Mock<IEnumerator<object>>();
        iterator.Setup(x => x.Reset()).Throws(new Exception());

        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Iterators.Position", (object[] args) =>
        {
            return iterator.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Arrange.SpaceShip", (object[] args) =>
        {
            return exeptionCmd.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeSpaceShipsCommand(spaceships).Execute(); });
    }
    [Fact]
    public void Test_Setting_Fuel_Successfully()
    {
        var spaceships = new List<IUObject>() { new Mock<IUObject>().Object, new Mock<IUObject>().Object };
        var somecommand = new Mock<Hwdtech.ICommand>();

        somecommand.Setup(x => x.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            return somecommand.Object;
        }).Execute();
        new SetFuelCommand(spaceships, 10).Execute();
        somecommand.Verify(x => x.Execute(), Times.Exactly(2));
    }
    [Fact]
    public void Test_Setting_Fuel_Disabbility_To_Set_Property_Cause_Exception()
    {
        var spaceships = new List<IUObject>() { new Mock<IUObject>().Object, new Mock<IUObject>().Object };

        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            return exeptionCmd.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new SetFuelCommand(spaceships, 10).Execute(); });
    }
    [Fact]
    public void Test_Setting_Fuel_Disabbility_To_Create_Set_Property_Command_Cause_Exception()
    {
        var spaceships = new List<IUObject>() { new Mock<IUObject>().Object, new Mock<IUObject>().Object };
        var somecommand = new Mock<Hwdtech.ICommand>();

        somecommand.Setup(x => x.Execute()).Verifiable();
        var exeptionCmd = new Mock<Hwdtech.ICommand>();
        exeptionCmd.Setup(x => x.Execute()).Throws(new Exception());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            exeptionCmd.Object.Execute();
            return somecommand.Object;
        }).Execute();
        Assert.Throws<Exception>(() => { new SetFuelCommand(spaceships, 10).Execute(); });
    }
    [Fact]
    public void Test_Position_Iterator_Works_Successfully()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.StartShipPositions", (object[] args) =>
        {
            return new List<Vector>() { new Vector(0, 1), new Vector(0, 2) };
        }).Execute();
        var iterator = new PositionIterator();
        var position1 = iterator.Current;
        iterator.MoveNext();
        var position2 = iterator.Current;
        iterator.Reset();
        var position3 = iterator.Current;
        Assert.Equal(position1, new Vector(0, 1));
        Assert.Equal(position2, new Vector(0, 2));
        Assert.Equal(position1, position3);
        Assert.Throws<NotImplementedException>(() => { iterator.Dispose(); });
    }
}
