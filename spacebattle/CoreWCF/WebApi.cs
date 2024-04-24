using CoreWCF;
using Hwdtech;

namespace SpaceBattle.WebHttp;
[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class WebApi : IWebApi
{
    public string SendOrder(OrderContract param)
    {
        var ServerThreadId = (int)IoC.Resolve<object>("TryGetServerIdByGameId", param.GameId);
        IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand"
            , ServerThreadId,
               IoC.Resolve<Hwdtech.ICommand>("CreateOrderCmd", param)).Execute();
        return "OK";
    }
}
