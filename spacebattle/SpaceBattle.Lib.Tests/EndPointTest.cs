namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
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

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateServerResponse", (object[] args) =>
        {
            if (args[0] != null && args[0] is int id && id == 10)
            {
                return "Code 202 - Accepted";
            }
            else
            {
                return "Code 400 - Entered id does not exist";
            }
        }).Execute();
    }

    [Fact]
    public void EndPoint_works_correctly()
    {
        var IdGetter = new Mock<IStrategy>();
        IdGetter.Setup(x => x.Run(It.IsAny<string>())).Returns(10).Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TryGetServerIdByGameId", (object[] args) => { return IdGetter.Object; }).Execute();

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

        var q = new BlockingCollection<Hwdtech.ICommand>(10);

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateOrderCmd", (object[] args) =>
        {
            return CreateOrderCmd.Object;
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Server.Commands.SendCommand", (object[] args) =>
        {
            return new ActionCommand(() => { q.Add((Hwdtech.ICommand)args[1]); });
        }).Execute();

        var webApi = new WebApi();

        var respone = webApi.SendOrder(orders[0]);
        orders.ForEach(order => webApi.SendOrder(order));

        Assert.Equal(5, q.Count());
        Assert.Equal("Code 202 - Accepted", respone);

        q.Take().Execute();
        q.Take().Execute();
        CreateOrderCmd.Verify(cmd => cmd.Execute(), Times.Exactly(2));
    }
    [Fact]
    public void EndPoint_NoIdFoundTest()
    {
        var IdGetter = new Mock<IStrategy>();
        IdGetter.Setup(x => x.Run(It.IsAny<string>())).Returns("Whatever").Verifiable();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TryGetServerIdByGameId", (object[] args) => { return IdGetter.Object; }).Execute();
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
            OrderType = "start movement",
            GameId = "1",
            ObjectId = "1",
            Properties = new Dictionary<string, object>() { { "Velocity", 1 } }
        };
        var respone = webApi.SendOrder(order);
        Assert.Equal("Code 400 - Entered id does not exist", respone);
    }
}
