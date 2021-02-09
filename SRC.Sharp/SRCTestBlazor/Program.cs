using Blazorise;
using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SRCTestBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services
                .AddScoped(sp => new HttpClient
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                })
              .AddBlazorise(options =>
              {
                  // XXXX ñ{ìñÇ…ÅH
                  // https://blazorise.com/docs/usage/bulma/
                  options.ChangeTextOnKeyPress = true;
              })
              .AddBulmaProviders()
              .AddFontAwesomeIcons();

            await builder.Build().RunAsync();
        }
    }
}
