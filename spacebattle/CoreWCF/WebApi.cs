using CoreWCF;
using Hwdtech;
using SpaceBattle.Lib;

namespace SpaceBattle.WebHttp;
[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class WebApi : IWebApi
{
    public string SendOrder(OrderContract param)
    {
        var ServerThreadId = IoC.Resolve<IStrategy>("TryGetServerIdByGameId").Run(param.GameId);
        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand"
            , ServerThreadId,
               IoC.Resolve<Hwdtech.ICommand>("CreateOrderCmd", param)).Execute();
        return IoC.Resolve<string>("CreateServerResponse", ServerThreadId);
    }
}
