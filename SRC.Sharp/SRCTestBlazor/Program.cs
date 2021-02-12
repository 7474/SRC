using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SRCTestBlazor.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRCTestBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services
                .AddScoped(sp => new HttpClient
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                })
                .AddSingleton<SrcDataContainer>()
                .AddSingleton<Random>();

            builder.RootComponents.Add<App>("#app");

            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
