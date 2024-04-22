using System.Collections.Generic;
using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

namespace SpaceBattle.WebHttp;
[DataContract(Name = "OrderContract", Namespace = "http://example.com")]
public class OrderContract
{
    [DataMember(Name = "OrderType", Order = 1)]
    [OpenApiProperty(Description = "OrderType")]
    public string OrderType { get; set; }

    [DataMember(Name = "GameId", Order = 2)]
    [OpenApiProperty(Description = "GameId")]
    public string GameId { get; set; }

    [DataMember(Name = "ObjectId", Order = 3)]
    [OpenApiProperty(Description = "ObjectId")]
    public string ObjectId { get; set; }

    [DataMember(Name = "Properties", Order = 4)]
    [OpenApiProperty(Description = "Properties")]
    public IDictionary<string, object> Properties { get; set; }
}
