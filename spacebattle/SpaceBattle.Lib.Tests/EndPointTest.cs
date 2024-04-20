namespace SpaceBattle.Lib.Tests;

using WebHttp;
using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

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
        var orders = new List<ExampleContract>()
        {
            new(){
                OrderType="start movement",
                GameId="1",
                ObjectId="1",
                Properties=new(){{"Velocity",1}}
            },
            new(){
                OrderType="start rotatement",
                GameId="1",
                ObjectId="1",
                Properties=new(){{"A_Velocity",1}}
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
                Properties=new(){{"Weapon_Type","EnergyBlast"}}
            }
        };
        var CreateOrderCmd=new Mock<ICommand>();
        
        CreateOrderCmd.Setup(cmd=>cmd.Execute()).Verifiable();

        var id =1;
        var q = new BlockingCollection<Hwdtech.ICommand>(10);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","GetServerIdByGameId",(object[] args)=>{return id;}).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","CreateOrderCmd", (object[] args)=>
        {
            return CreateOrderCmd.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Server.Commands.SendCommand", (object[] args)=>
        {
            return new ActionCommand(()=>{q.Add((Hwdtech.ICommand)args[1]);});
        }).Execute();

        var webApi=new WebApi();

        var respone=webApi(orders[0]);
        orders.ForEach(order=>webApi(order));

        Assert.Equal(5,q.Count());
        Assert.Equal("Code 202-Accepted",respone);

        q.Take().Execute();
        q.Take().Execute();
        CreateOrderCmd.Verify(cmd=>cmd.Execute(),Times.Exactly(2));
    }
}
