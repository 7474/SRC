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
            var source = "using System; class C { double d = 1.5; float f = 1.5f; decimal m = 1.5m; int i = 1; long l = 1; bool a, b, c; void M() { " + body + " } }";
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
    }
}
