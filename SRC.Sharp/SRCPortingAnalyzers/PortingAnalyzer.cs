using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SRCPortingAnalyzers
{
    /// <summary>
    /// VB 版からの移植で起きやすい誤りの規則（README.md）。
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PortingAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor FloatToIntegralCast = new DiagnosticDescriptor(
            "SRCP001",
            "実数を整数へ (int)・(long) で変換しない",
            "{0} から {1} への変換は切り捨てになる。VB の動作に合わせて VBMath.CLng（代入・CInt・CLng の偶数丸め）・VBMath.Int（Int 関数）・VBMath.Fix（Fix 関数）のどれかを使う",
            "Porting",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/7474/SRC/blob/master/SRC.Sharp/SRCPortingAnalyzers/README.md");

        public static readonly DiagnosticDescriptor MixedLogicalOperators = new DiagnosticDescriptor(
            "SRCP002",
            "&& と || を括弧なしで混ぜない",
            "&& と || を括弧なしで混ぜている。VB の条件と同じ組み合わせかを確かめて、括弧で書く",
            "Porting",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/7474/SRC/blob/master/SRC.Sharp/SRCPortingAnalyzers/README.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(FloatToIntegralCast, MixedLogicalOperators);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeCast, SyntaxKind.CastExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLogicalOr, SyntaxKind.LogicalOrExpression);
        }

        private static bool IsFloating(ITypeSymbol type)
        {
            switch (type?.SpecialType)
            {
                case SpecialType.System_Double:
                case SpecialType.System_Single:
                case SpecialType.System_Decimal:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsIntegral(ITypeSymbol type)
        {
            switch (type?.SpecialType)
            {
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Byte:
                case SpecialType.System_SByte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                    return true;
                default:
                    return false;
            }
        }

        private static void AnalyzeCast(SyntaxNodeAnalysisContext context)
        {
            var cast = (CastExpressionSyntax)context.Node;
            var target = context.SemanticModel.GetTypeInfo(cast.Type, context.CancellationToken).Type;
            if (!IsIntegral(target))
            {
                return;
            }
            var source = context.SemanticModel.GetTypeInfo(cast.Expression, context.CancellationToken).Type;
            if (!IsFloating(source))
            {
                return;
            }
            // Math.Round・Floor・Ceiling・Truncate の結果は、丸め方を書いてあるので対象にしない
            var inner = cast.Expression;
            while (inner is ParenthesizedExpressionSyntax paren)
            {
                inner = paren.Expression;
            }
            if (inner is InvocationExpressionSyntax invocation
                && context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol method
                && method.ContainingType?.ToDisplayString() == "System.Math"
                && (method.Name == "Round" || method.Name == "Floor" || method.Name == "Ceiling" || method.Name == "Truncate"))
            {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(FloatToIntegralCast, cast.GetLocation(),
                source.ToDisplayString(), target.ToDisplayString()));
        }

        private static void AnalyzeLogicalOr(SyntaxNodeAnalysisContext context)
        {
            var or = (BinaryExpressionSyntax)context.Node;
            if (or.Left.IsKind(SyntaxKind.LogicalAndExpression) || or.Right.IsKind(SyntaxKind.LogicalAndExpression))
            {
                context.ReportDiagnostic(Diagnostic.Create(MixedLogicalOperators, or.OperatorToken.GetLocation()));
            }
        }
    }
}
