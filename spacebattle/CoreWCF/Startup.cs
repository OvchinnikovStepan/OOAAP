using System.Diagnostics.CodeAnalysis;
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpaceBattle.WebHttp;
using Swashbuckle.AspNetCore.Swagger;

[ExcludeFromCodeCoverage]
internal sealed class Startup
{
    public static void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<SwaggerMiddleware>();
        app.UseSwaggerUI();

        app.UseServiceModel(builder =>
        {
            builder.AddService<WebApi>();
            builder.AddServiceWebEndpoint<WebApi, IWebApi>(new WebHttpBinding
            {
                MaxReceivedMessageSize = 5242880,
                MaxBufferSize = 65536,
            }, "api", behavior =>
            {
                behavior.HelpEnabled = true;
                behavior.AutomaticFormatSelectionEnabled = true;
            });
        });
    }
}
