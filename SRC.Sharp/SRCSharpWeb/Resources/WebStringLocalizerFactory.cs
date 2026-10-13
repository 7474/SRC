using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SRCCore;
using System;
using System.Collections.Concurrent;

namespace SRCSharpWeb.Resources
{
    /// <summary>
    /// IStringLocalizer の訳を、サテライトアセンブリではなくアプリのアセンブリに埋め込んだリソースから引く。
    /// マルチスレッドの WebAssembly では、Blazor がサテライトアセンブリを読み込むと
    /// 「Cannot call synchronous C# methods」で起動に失敗するため（csproj の EmbeddedResource）。
    /// </summary>
    public class WebStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ConcurrentDictionary<string, ResourceManagerStringLocalizer> localizers = new ConcurrentDictionary<string, ResourceManagerStringLocalizer>();
        private readonly ResourceNamesCache cache = new ResourceNamesCache();

        public WebStringLocalizerFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return localizers.GetOrAdd(resourceSource.FullName, baseName => new ResourceManagerStringLocalizer(
                new EmbeddedCultureResourceManager(baseName, resourceSource.Assembly),
                resourceSource.Assembly,
                baseName,
                cache,
                loggerFactory.CreateLogger<ResourceManagerStringLocalizer>()));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            throw new NotSupportedException();
        }
    }
}
