using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Xml;
using SpaceBattle.WebHttp;

internal sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServiceModelWebServices(o =>
        {
            o.Title = "Space Battle API";
            o.Version = "1";
            o.Description = "API Description";
            o.TermsOfService = new("http://spacebattle.com/terms");
            o.ContactName = "Contact";
            o.ContactEmail = "support@spacebattle.com";
            o.ContactUrl = new("http://spacebattle.com/contact");
            o.ExternalDocumentUrl = new("http://spacebattle.com/doc.pdf");
            o.ExternalDocumentDescription = "Documentation";
        });

        services.AddSingleton(new SwaggerOptions());
    }

    public void Configure(IApplicationBuilder app)
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
