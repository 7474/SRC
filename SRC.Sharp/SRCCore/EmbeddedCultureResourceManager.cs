using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace SRCCore
{
    /// <summary>
    /// 訳をサテライトアセンブリのほかに、登録したアセンブリに埋め込んだリソース（"{BaseName}.{カルチャ名}.resources"）からも引く ResourceManager。
    /// マルチスレッドの WebAssembly（SRC#Web）ではサテライトアセンブリを読み込めないため、訳をアプリのアセンブリに埋め込んで使う。
    /// 登録が無ければ通常の ResourceManager と同じ（サテライトアセンブリから引く）。
    /// </summary>
    public class EmbeddedCultureResourceManager : ResourceManager
    {
        private static readonly List<Assembly> embeddedAssemblies = new List<Assembly>();
        private static readonly ConcurrentDictionary<string, ResourceSet> sets = new ConcurrentDictionary<string, ResourceSet>();

        public EmbeddedCultureResourceManager(string baseName, Assembly assembly) : base(baseName, assembly)
        {
        }

        /// <summary>訳を埋め込んだアセンブリを登録する。リソースを引く前に呼ぶ</summary>
        public static void RegisterEmbeddedAssembly(Assembly assembly)
        {
            lock (embeddedAssemblies)
            {
                if (!embeddedAssemblies.Contains(assembly))
                {
                    embeddedAssemblies.Add(assembly);
                }
            }
        }

        public override string GetString(string name, CultureInfo culture)
        {
            return FindEmbedded(name, culture ?? CultureInfo.CurrentUICulture) ?? base.GetString(name, culture);
        }

        private string FindEmbedded(string name, CultureInfo culture)
        {
            Assembly[] assemblies;
            lock (embeddedAssemblies)
            {
                if (embeddedAssemblies.Count == 0)
                {
                    return null;
                }
                assemblies = embeddedAssemblies.ToArray();
            }
            // 地域付きのカルチャから親へたどる（zh-TW → zh-Hant → zh）。ニュートラル（既定の言語）は通常どおり本体のアセンブリから引く
            for (var c = culture; c != null && !c.Equals(CultureInfo.InvariantCulture); c = c.Parent)
            {
                foreach (var assembly in assemblies)
                {
                    var value = GetSet(assembly, BaseName + "." + c.Name + ".resources")?.GetString(name);
                    if (value != null)
                    {
                        return value;
                    }
                }
                if (c.Equals(c.Parent))
                {
                    break;
                }
            }
            return null;
        }

        private static ResourceSet GetSet(Assembly assembly, string resourceName)
        {
            return sets.GetOrAdd(assembly.FullName + "/" + resourceName, _ =>
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                return stream == null ? null : new ResourceSet(stream);
            });
        }
    }
}
