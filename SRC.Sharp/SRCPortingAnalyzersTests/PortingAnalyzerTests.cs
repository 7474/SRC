using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPortingAnalyzers;

namespace SRCPortingAnalyzersTests
{
    [TestClass]
    public class PortingAnalyzerTests
    {
        private static string[] Analyze(string body)
        {
            return AnalyzeSource("using System; class C { double d = 1.5; float f = 1.5f; decimal m = 1.5m; int i = 1; long l = 1; bool a, b, c; void M() { " + body + " } }");
        }

        private static string[] AnalyzeSource(string source)
        {
            var tree = CSharpSyntaxTree.ParseText(source);
            var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(System.IO.Path.PathSeparator)
                .Select(x => MetadataReference.CreateFromFile(x));
            var compilation = CSharpCompilation.Create("t", new[] { tree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var errors = compilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).ToList();
            Assert.AreEqual(0, errors.Count, string.Join("\n", errors));
            return compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new PortingAnalyzer()))
                .GetAnalyzerDiagnosticsAsync().Result
                .Select(x => x.Id)
                .ToArray();
        }

        [TestMethod]
        [DataRow("var x = (int)d;")]
        [DataRow("var x = (int)(d * 1.5);")]
        [DataRow("var x = (long)f;")]
        [DataRow("var x = (short)m;")]
        [DataRow("var x = (int)(i * 1.5d);")]
        public void SRCP001_FloatingToIntegralCast(string body)
        {
            CollectionAssert.AreEqual(new[] { "SRCP001" }, Analyze(body));
        }

        [TestMethod]
        [DataRow("var x = (int)Math.Round(d);")]
        [DataRow("var x = (int)Math.Floor(d * 2);")]
        [DataRow("var x = (long)(Math.Ceiling(d));")]
        [DataRow("var x = (int)Math.Truncate(d);")]
        [DataRow("var x = (int)l;")]
        [DataRow("var x = (double)i;")]
        [DataRow("var x = (int)'a';")]
        public void SRCP001_NotReported(string body)
        {
            CollectionAssert.AreEqual(Array.Empty<string>(), Analyze(body));
        }

        [TestMethod]
        [DataRow("var x = a && b || c;")]
        [DataRow("var x = a || b && c;")]
        public void SRCP002_MixedLogicalOperators(string body)
        {
            CollectionAssert.AreEqual(new[] { "SRCP002" }, Analyze(body));
        }

        [TestMethod]
        [DataRow("var x = (a && b) || c;")]
        [DataRow("var x = a || (b && c);")]
        [DataRow("var x = a || b || c;")]
        [DataRow("var x = a && b && c;")]
        public void SRCP002_NotReported(string body)
        {
            CollectionAssert.AreEqual(Array.Empty<string>(), Analyze(body));
        }

        // SRCCore.Models などの名前空間にある型の、string のフィールド・自動実装のプロパティ
        private static string[] AnalyzeMember(string member, string ns = "SRCCore.Models")
        {
            return AnalyzeSource("namespace " + ns + " { class C { " + member + " } }");
        }

        [TestMethod]
        [DataRow("public string Name;")]
        [DataRow("private string strName;")]
        [DataRow("public static string Stage;")]
        [DataRow("public string Name { get; set; }")]
        [DataRow("public string Name { get; private set; }")]
        // 一部のコンストラクターだけが代入しているもの
        [DataRow("public string Name; public C(int x) { Name = \"a\"; } public C() { }")]
        public void SRCP003_UninitializedString(string member)
        {
            CollectionAssert.AreEqual(new[] { "SRCP003" }, AnalyzeMember(member));
        }

        [TestMethod]
        [DataRow("public string Name, Class;", 2)]
        [DataRow("public string Name = \"\", Class;", 1)]
        public void SRCP003_EachVariable(string member, int count)
        {
            Assert.AreEqual(count, AnalyzeMember(member).Count(x => x == "SRCP003"));
        }

        [TestMethod]
        [DataRow("SRCCore")]
        [DataRow("SRCCore.Units")]
        [DataRow("SRCCore.Pilots")]
        [DataRow("SRCCore.Items")]
        [DataRow("SRCCore.Events")]
        [DataRow("SRCCore.Commands")]
        [DataRow("SRCCore.CmdDatas.Commands")]
        [DataRow("SRCCore.Maps")]
        [DataRow("SRCCore.Expressions")]
        public void SRCP003_VbPortNamespaces(string ns)
        {
            CollectionAssert.AreEqual(new[] { "SRCP003" }, AnalyzeMember("public string Name;", ns));
        }

        [TestMethod]
        [DataRow("public string Name = \"\";")]
        [DataRow("public string Name { get; set; } = \"\";")]
        [DataRow("public const string Name = \"a\";")]
        [DataRow("public string Name => \"a\";")]
        [DataRow("public string Name { get { return \"a\"; } }")]
        [DataRow("public int Count;")]
        [DataRow("public object Data;")]
        // すべてのコンストラクターが代入しているもの
        [DataRow("public string Name; public C() { Name = \"a\"; }")]
        [DataRow("public string Name { get; } public C(string x) { this.Name = x; } public C() : this(\"a\") { }")]
        [DataRow("public static string Stage; static C() { Stage = \"\"; }")]
        public void SRCP003_NotReported(string member)
        {
            CollectionAssert.AreEqual(Array.Empty<string>(), AnalyzeMember(member));
        }

        [TestMethod]
        [DataRow("SRCCore.Config")]
        [DataRow("SRCCore.Filesystem")]
        [DataRow("SRCCoreX")]
        [DataRow("Other")]
        public void SRCP003_OtherNamespaces_NotReported(string ns)
        {
            CollectionAssert.AreEqual(Array.Empty<string>(), AnalyzeMember("public string Name;", ns));
        }

        [TestMethod]
        public void SRCP003_StructInterfaceAbstract_NotReported()
        {
            CollectionAssert.AreEqual(Array.Empty<string>(),
                AnalyzeSource("namespace SRCCore.Models { struct S { public string Name; } interface I { string Name { get; set; } } abstract class A { public abstract string Name { get; set; } } }"));
        }

        [TestMethod]
        public void SRCP003_SuppressMessage()
        {
            // 理由を書いて抑止できる（README の「規則に当たらない書き方にできないとき」）
            CollectionAssert.AreEqual(Array.Empty<string>(),
                AnalyzeMember("[System.Diagnostics.CodeAnalysis.SuppressMessage(\"Porting\", \"SRCP003\", Justification = \"理由\")] public string UnitId;"));
        }

        private const string GeneralLib = "namespace SRCCore.Lib { static class GeneralLib { public static System.Collections.Generic.IList<string> ToList(string list, bool removeKakko = false) => null; public static int ListLength(string list) => 0; } } ";

        [TestMethod]
        [DataRow("var x = SRCCore.Lib.GeneralLib.ToList(\"a b\");")]
        [DataRow("var x = SRCCore.Lib.GeneralLib.ToList(\"(a b)\", true).Count;")]
        public void SRCP004_GeneralLibToList(string body)
        {
            CollectionAssert.AreEqual(new[] { "SRCP004" }, AnalyzeSource(GeneralLib + "class C { void M() { " + body + " } }"));
        }

        [TestMethod]
        [DataRow("var x = SRCCore.Lib.GeneralLib.ListLength(\"a b\");")]
        [DataRow("var x = System.Linq.Enumerable.ToList(new System.Collections.Generic.List<int>());")]
        [DataRow("var x = ToList(); System.Collections.Generic.IList<string> ToList() => null;")]
        public void SRCP004_NotReported(string body)
        {
            CollectionAssert.AreEqual(Array.Empty<string>(), AnalyzeSource(GeneralLib + "class C { void M() { " + body + " } }"));
        }
    }
}
