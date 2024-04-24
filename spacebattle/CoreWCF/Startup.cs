// using System.Diagnostics.CodeAnalysis;
// using CoreWCF;
// using CoreWCF.Configuration;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using SpaceBattle.WebHttp;
// using Swashbuckle.AspNetCore.Swagger;

// [ExcludeFromCodeCoverage]
// internal sealed class Startup
// {
//     [ExcludeFromCodeCoverage]
//     public static void ConfigureServices(IServiceCollection services)
//     {
//         services.AddServiceModelWebServices(o =>
//         {
//             o.Title = "Space Battle API";
//             o.Version = "1";
//             o.Description = "API Description";
//             o.TermsOfService = new("http://spacebattle.com/terms");
//             o.ContactName = "Contact";
//             o.ContactEmail = "support@spacebattle.com";
//             o.ContactUrl = new("http://spacebattle.com/contact");
//             o.ExternalDocumentUrl = new("http://spacebattle.com/doc.pdf");
//             o.ExternalDocumentDescription = "Documentation";
//         });

//         services.AddSingleton(new SwaggerOptions());
//     }
//     [ExcludeFromCodeCoverage]
//     public static void Configure(IApplicationBuilder app)
//     {
//         app.UseMiddleware<SwaggerMiddleware>();
//         app.UseSwaggerUI();

//         app.UseServiceModel(builder =>
//         {
//             builder.AddService<WebApi>();
//             builder.AddServiceWebEndpoint<WebApi, IWebApi>(new WebHttpBinding
//             {
//                 MaxReceivedMessageSize = 5242880,
//                 MaxBufferSize = 65536,
//             }, "api", behavior =>
//             {
//                 behavior.HelpEnabled = true;
//                 behavior.AutomaticFormatSelectionEnabled = true;
//             });
//         });
//     }
// }
