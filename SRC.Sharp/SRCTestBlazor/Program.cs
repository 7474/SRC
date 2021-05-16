using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using SRCTestBlazor.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRCTestBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // https://github.com/dotnet/aspnetcore/issues/24461#issuecomment-667068936
            var js = (IJSInProcessRuntime)builder.Services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
            var startupParams = js.Invoke<string[]>("getStartupParams");

            builder.Services
                .AddScoped(sp => new HttpClient
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                })
                .AddSingleton<SrcDataContainer>()
                .AddSingleton<Random>(sp =>
                {
                    var random = new Random();
                    return isE2ETest(startupParams) ? new Random(0) : new Random();
                });

            builder.RootComponents.Add<App>("#app");

            var host = builder.Build();
            await host.RunAsync();
        }

        private static bool isE2ETest(string[] startupParams)
        {
            return startupParams?.Any(arg => arg == "--cypress") ?? false;
        }
    }
}
