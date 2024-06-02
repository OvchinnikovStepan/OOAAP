using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

[ExcludeFromCodeCoverage]
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options =>
            {
                options.ListenAnyIP(8080);
            })
            .UseStartup<Startup>();

        var app = builder.Build();
        app.Run();
    }
}
