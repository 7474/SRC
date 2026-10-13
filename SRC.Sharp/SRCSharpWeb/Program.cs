using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SRCSharpWeb;
using System;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLocalization();
await JSHost.ImportAsync(WebInterop.ModuleName, "../js/srcweb.js");
// 表示言語（localStorage の srcweb.language）。RunAsync がこの UI カルチャのサテライトアセンブリを読み込む
//WebLocalization.Apply();
await builder.Build().RunAsync();
