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
    public void Test_Disabbility_To_Get_IUObject_List_Cause_Exception()
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
    public void Test_Disabbility_To_Add_IUObject_To_List_Cause_Exception()
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
}
