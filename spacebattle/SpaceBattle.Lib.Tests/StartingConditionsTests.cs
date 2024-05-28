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
    public void Test_Creating_IUObjcet_List_Succesfuly()
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
    public void Test_Arranging_One_SpaceShip_Succecfully()
    {
        var obj = new Mock<IUObject>();
        var somecommand = new Mock<Hwdtech.ICommand>();
        var iterator = new Mock<IEnumerator<object>>();
        somecommand.Setup(x => x.Execute()).Verifiable();
        iterator.Setup(x => x.MoveNext()).Verifiable();
        iterator.SetupGet(x => x.Current).Returns(new Vector(1, 1)).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.SetProperty", (object[] args) =>
        {
            return new ActionCommand(() => { somecommand.Object.Execute(); });
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
            return new ActionCommand(() => { somecommand.Object.Execute(); });
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
            exeptionCmd.Object.Execute();
            return new ActionCommand(() => { somecommand.Object.Execute(); });
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
            return new ActionCommand(() => { somecommand.Object.Execute(); });
        }).Execute();
        Assert.Throws<Exception>(() => { new ArrangeOneSpaceShipCommand(obj.Object, iterator.Object).Execute(); });
    }
}
