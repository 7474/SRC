using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SRCSharpWeb.Tests
{
    /// <summary>カタログ・サイトの設定（srcweb.json）の読み取り</summary>
    [TestClass]
    public class CatalogTests
    {
        private static readonly Uri Site = new Uri("https://example.com/SRC/web/");

        /// <summary>決まった応答を返す HTTP の送り先</summary>
        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> respond;

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
            {
                this.respond = respond;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(respond(request));
            }
        }

        private static HttpClient Client(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            return new HttpClient(new StubHandler(respond)) { BaseAddress = Site };
        }

        private static HttpResponseMessage Json(string json, HttpStatusCode status = HttpStatusCode.OK)
        {
            return new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        }

        [TestMethod]
        public void Parse_ArrayIsRelativeToSite()
        {
            // 以前のサイト組み込みの形式（配列だけ）
            var catalog = Catalog.Parse(@"[
                { ""id"": ""a"", ""archive"": ""scenarios/a.zip"", ""eve"": ""test.eve"" },
                { ""id"": ""basic"", ""kind"": ""asset"", ""title"": ""素材"", ""archive"": ""https://cdn.example.net/basic.zip"" },
                { ""title"": ""id が無い"" },
                null,
            ]", new Uri(Site, "scenarios/catalog.json"), Site);

            Assert.AreEqual("https://example.com/SRC/web/scenarios/catalog.json", catalog.Url);
            Assert.AreEqual(2, catalog.Entries.Count);
            var a = catalog.Entries[0];
            Assert.AreEqual(CatalogEntry.KindScenario, a.Kind);
            Assert.AreEqual("a", a.Title);
            Assert.AreEqual("https://example.com/SRC/web/scenarios/a.zip", a.Archive);
            Assert.AreEqual("test.eve", a.Eve);
            Assert.AreEqual(0, a.Requires.Count);
            Assert.AreEqual("https://cdn.example.net/basic.zip", catalog.Entries[1].Archive);
            CollectionAssert.AreEqual(new[] { "a" }, catalog.Scenarios.Select(x => x.Id).ToArray());
        }

        [TestMethod]
        public void Parse_ObjectIsRelativeToCatalog()
        {
            var catalog = Catalog.Parse(@"{
                // コメントと末尾のカンマを許す
                ""format"": 1,
                ""name"": ""外のカタログ"",
                ""entries"": [
                    { ""id"": ""x"", ""archive"": ""x.zip"", ""version"": ""v2"", ""root"": ""game"", ""requires"": [ ""basic"" ], ""rating"": ""R15"" },
                ],
            }", new Uri("https://other.example.org/catalogs/list.json"), Site);

            Assert.AreEqual("外のカタログ", catalog.Name);
            Assert.AreEqual(1, catalog.Format);
            var x = catalog.Entries.Single();
            Assert.AreEqual("https://other.example.org/catalogs/x.zip", x.Archive);
            Assert.AreEqual("v2", x.Version);
            Assert.AreEqual("game", x.Root);
            CollectionAssert.AreEqual(new[] { "basic" }, x.Requires);
            Assert.AreEqual(15, x.RatingAge);
        }

        [TestMethod]
        public void Parse_NullOrEmptyEntries()
        {
            Assert.AreEqual(0, Catalog.Parse("null", new Uri(Site, "c.json"), Site).Entries.Count);
            Assert.AreEqual(0, Catalog.Parse(@"{ ""name"": ""空"", ""entries"": null }", new Uri(Site, "c.json"), Site).Entries.Count);
        }

        [TestMethod]
        public void Parse_RejectsBrokenJson()
        {
            Assert.Throws<JsonException>(() => Catalog.Parse("{ entries: ", new Uri(Site, "c.json"), Site), "JSON として読めないものは呼び出し側で扱う");
        }

        [TestMethod]
        [DataRow(null, 0)]
        [DataRow("general", 0)]
        [DataRow("r15", 15)]
        [DataRow("R18", 18)]
        [DataRow("unknown", 0)]
        public void RatingAgeOf(string rating, int age)
        {
            Assert.AreEqual(age, CatalogEntry.RatingAgeOf(rating));
        }

        [TestMethod]
        public async Task LoadAsync_ResolvesUrlAgainstSite()
        {
            Uri requested = null;
            using (var http = Client(request =>
            {
                requested = request.RequestUri;
                return Json(@"{ ""entries"": [ { ""id"": ""a"", ""archive"": ""a.zip"" } ] }");
            }))
            {
                var catalog = await Catalog.LoadAsync(http, "scenarios/catalog.json");
                Assert.AreEqual("https://example.com/SRC/web/scenarios/catalog.json", requested.ToString());
                Assert.AreEqual("https://example.com/SRC/web/scenarios/a.zip", catalog.Entries.Single().Archive);
            }
        }

        [TestMethod]
        public void SiteConfig_Defaults()
        {
            var site = new SiteConfig();
            Assert.AreEqual("SRC#Web", site.Title);
            CollectionAssert.AreEqual(new[] { "scenarios/catalog.json" }, site.Catalogs);
            Assert.IsTrue(site.UserScenariosEnabled);
            Assert.IsTrue(site.UserAssetsEnabled);
            Assert.IsTrue(site.UserArchivesEnabled);
            Assert.IsTrue(site.AllowUserCatalogs);
            // 既定では年齢を確かめない
            Assert.AreEqual(0, site.RatingAgeOf(new CatalogEntry { Rating = "r18" }));
        }

        [TestMethod]
        [DataRow(true, true, true, true, true, true)]
        [DataRow(false, true, true, false, false, false)]
        [DataRow(true, false, true, false, true, true)]
        [DataRow(true, true, false, true, false, true)]
        [DataRow(true, false, false, false, false, false)]
        public void SiteConfig_UserArchives(bool archives, bool scenarios, bool assets, bool scenariosEnabled, bool assetsEnabled, bool anyEnabled)
        {
            var site = new SiteConfig { AllowUserArchives = archives, AllowUserScenarios = scenarios, AllowUserAssets = assets };
            Assert.AreEqual(scenariosEnabled, site.UserScenariosEnabled);
            Assert.AreEqual(assetsEnabled, site.UserAssetsEnabled);
            Assert.AreEqual(anyEnabled, site.UserArchivesEnabled);
        }

        [TestMethod]
        public async Task SiteConfig_LoadAsync_ReadsFile()
        {
            Uri requested = null;
            using (var http = Client(request =>
            {
                requested = request.RequestUri;
                return Json(@"{
                    ""Title"": ""私のサイト"",
                    ""catalogs"": [ ""a.json"", ""https://other.example.org/b.json"" ],
                    ""playCatalog"": ""scenarios/{id}.json"",
                    ""allowUserArchives"": false,
                    ""ratingConfirm"": true,
                    ""start"": ""lantern"",
                    ""language"": ""en"",
                }");
            }))
            {
                var site = await SiteConfig.LoadAsync(http);
                Assert.AreEqual("https://example.com/SRC/web/srcweb.json", requested.ToString());
                Assert.AreEqual("私のサイト", site.Title);
                CollectionAssert.AreEqual(new[] { "a.json", "https://other.example.org/b.json" }, site.Catalogs);
                Assert.AreEqual("scenarios/{id}.json", site.PlayCatalog);
                Assert.IsFalse(site.UserArchivesEnabled);
                Assert.AreEqual("lantern", site.Start);
                Assert.AreEqual("en", site.Language);
                Assert.AreEqual(18, site.RatingAgeOf(new CatalogEntry { Rating = "r18" }));
            }
        }

        [TestMethod]
        [DataRow(HttpStatusCode.NotFound, "{}")]
        [DataRow(HttpStatusCode.OK, "{ broken")]
        [DataRow(HttpStatusCode.OK, "null")]
        public async Task SiteConfig_LoadAsync_DefaultsWhenMissingOrBroken(HttpStatusCode status, string body)
        {
            using (var http = Client(_ => Json(body, status)))
            {
                var site = await SiteConfig.LoadAsync(http);
                Assert.AreEqual("SRC#Web", site.Title);
                Assert.IsTrue(site.UserArchivesEnabled);
            }
        }

        [TestMethod]
        public async Task SiteConfig_LoadAsync_DefaultsWhenUnreachable()
        {
            using (var http = Client(_ => throw new HttpRequestException("network")))
            {
                Assert.AreEqual("SRC#Web", (await SiteConfig.LoadAsync(http)).Title);
            }
        }
    }
}
