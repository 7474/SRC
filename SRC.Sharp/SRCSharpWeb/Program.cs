using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SRCCore;
using SRCSharpWeb;
using SRCSharpWeb.Resources;
using System;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// 訳はサテライトアセンブリではなく、このアセンブリに埋め込む（Resources/WebStringLocalizerFactory.cs）
EmbeddedCultureResourceManager.RegisterEmbeddedAssembly(typeof(App).Assembly);
builder.Services.AddSingleton<IStringLocalizerFactory, WebStringLocalizerFactory>();
builder.Services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
await JSHost.ImportAsync(WebInterop.ModuleName, "../js/srcweb.js");
// 表示言語（index.html が決めて環境変数で渡す）。RunAsync がこの UI カルチャのサテライトアセンブリを読み込む
WebLocalization.Apply();
// ログのレベル（index.html が環境変数で渡す）
WebLogging.Configure(builder.Logging);
// ブラウザのファイルシステムを書き換えるのはこのアプリだけ（書き換えたらキャッシュを捨てる）。存在の確認をフォルダの一覧のキャッシュで答える
SRCCore.Filesystem.PathResolver.TrustDirectoryCache = true;
var host = builder.Build();
WebLogging.Attach(host.Services.GetRequiredService<ILoggerFactory>());
await host.RunAsync();
