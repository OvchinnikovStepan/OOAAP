using System.Net;
using CoreWCF;
using CoreWCF.OpenApi.Attributes;
using CoreWCF.Web;

namespace SpaceBattle.WebHttp;
[ServiceContract]
[OpenApiBasePath("/api")]
public interface IWebApi
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "/body")]
    [OpenApiTag("Tag")]
    [OpenApiResponse(ContentTypes = new[] { "application/json", "text/xml" }, Description = "Success", StatusCode = HttpStatusCode.OK, Type = typeof(OrderContract))]
    string SendOrder(
        [OpenApiParameter(ContentTypes = new[] { "application/json", "text/xml" }, Description = "param description.")] OrderContract param);
}
