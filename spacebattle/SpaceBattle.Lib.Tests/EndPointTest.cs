namespace SpaceBattle.Lib.Tests;

using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using SpaceBattle.Lib;
using SpaceBattle.WebHttp;
public class EndPointTest
{
    public EndPointTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }

    [Fact]
    public void EndPoint_works_correctly()
    {
        var id = 1;
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TryGetServerIdByGameId", (object[] args) => { return (object)id; }).Execute();

        var orders = new List<OrderContract>()
        {
            new(){
                OrderType="start movement",
                GameId="1",
                ObjectId="1",
                Properties=new Dictionary<string,object>(){{"Velocity",1}}
            },
            new(){
                OrderType="start rotatement",
                GameId="1",
                ObjectId="1",
                Properties=new Dictionary<string,object>(){{"A_Velocity",1}}
            },
            new(){
                OrderType="stop all movement",
                GameId="1",
                ObjectId="1",
            },
             new(){
                OrderType="fire",
                GameId="1",
                ObjectId="1",
                Properties=new Dictionary<string,object>(){{"Weapon_Type","EnergyBlast"}}
            }
        };
        var CreateOrderCmd = new Mock<Hwdtech.ICommand>();

        CreateOrderCmd.Setup(cmd => cmd.Execute()).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return CreateOrderCmd.Object;
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
        {
            return new ActionCommand(() => { ((Hwdtech.ICommand)args[1]).Execute(); });
        }).Execute();

        var webApi = new WebApi();

        var respone = webApi.SendOrder(orders[0]);
        orders.ForEach(order => webApi.SendOrder(order));
        Assert.Equal("OK", respone);
        CreateOrderCmd.Verify(cmd => cmd.Execute(), Times.Exactly(5));
    }
    [Fact]
    public void EndPoint_AttemtToGetServerIdCauseExeption()
    {
        var exeptioncmd = new Mock<Hwdtech.ICommand>();
        exeptioncmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TryGetServerIdByGameId", (object[] args) =>
                {
                    exeptioncmd.Object.Execute();
                    return (object)1;
                }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return new ActionCommand(() => { });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
        {
            return new ActionCommand(() => { });
        }).Execute();
        var webApi = new WebApi();
        var order = new OrderContract()
        {
            OrderType = "fire",
            GameId = "1",
            ObjectId = "1",
            Properties = new Dictionary<string, object>() { { "Weapon_Type", "EnergyBlast" } }
        };
        Assert.Throws<Exception>(new ActionCommand(() => { webApi.SendOrder(order); }).Execute);
    }
    [Fact]
    public void EndPoint_AttemtToCreateCmdCauseExeption()
    {
        var exeptioncmd = new Mock<Hwdtech.ICommand>();
        exeptioncmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TryGetServerIdByGameId", (object[] args) =>
                {
                    return (object)1;
                }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            exeptioncmd.Object.Execute();
            return new ActionCommand(() => { });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
        {
            return new ActionCommand(() => { });
        }).Execute();
        var webApi = new WebApi();
        var order = new OrderContract()
        {
            OrderType = "fire",
            GameId = "1",
            ObjectId = "1",
            Properties = new Dictionary<string, object>() { { "Weapon_Type", "EnergyBlast" } }
        };
        Assert.Throws<Exception>(new ActionCommand(() => { webApi.SendOrder(order); }).Execute);
    }
    [Fact]
    public void EndPoint_AttemtToCreateSendCommandCauseExeption()
    {
        var exeptioncmd = new Mock<Hwdtech.ICommand>();
        exeptioncmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TryGetServerIdByGameId", (object[] args) =>
                {
                    return (object)1;
                }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return new ActionCommand(() => { });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
        {
            exeptioncmd.Object.Execute();
            return new ActionCommand(() => { });
        }).Execute();
        var webApi = new WebApi();
        var order = new OrderContract()
        {
            OrderType = "fire",
            GameId = "1",
            ObjectId = "1",
            Properties = new Dictionary<string, object>() { { "Weapon_Type", "EnergyBlast" } }
        };
        Assert.Throws<Exception>(new ActionCommand(() => { webApi.SendOrder(order); }).Execute);
    }
    [Fact]
    public void EndPoint_AttemtToCreateSendCommandReturnBrokenCommand()
    {
        var exeptioncmd = new Mock<Hwdtech.ICommand>();
        exeptioncmd.Setup(x => x.Execute()).Throws(new Exception());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TryGetServerIdByGameId", (object[] args) =>
                {
                    return (object)1;
                }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return new ActionCommand(() => { });
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
        {
            return exeptioncmd.Object;
        }).Execute();
        var webApi = new WebApi();
        var order = new OrderContract()
        {
            OrderType = "fire",
            GameId = "1",
            ObjectId = "1",
            Properties = new Dictionary<string, object>() { { "Weapon_Type", "EnergyBlast" } }
        };
        Assert.Throws<Exception>(new ActionCommand(() => { webApi.SendOrder(order); }).Execute);
    }
}
