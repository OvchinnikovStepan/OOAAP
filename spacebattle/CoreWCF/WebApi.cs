using CoreWCF;
using SpaceBattle.Lib;
using System.Collections;

namespace WebHttp
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class WebApi : IWebApi
    {
        public string GetOrder(ExampleContract param)
        {
            IoC.Resolve<Hwdtech.ICommand>("Server.Commands.SendCommand"
                ,IoC.Resolve<int>("GetServerIdByGameId",param.GameId),
                   IoC.Resolve<Hwdtech.ICommand>("CreateOrderCmd", param)).Execute();
            var respone="Code 202-Accepted";
            return respone;
        }
    }
}
