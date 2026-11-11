using System.Collections.Immutable;
using System.Linq;
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
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/7474/SRC/blob/master/SRC.Sharp/SRCPortingAnalyzers/README.md");

        public static readonly DiagnosticDescriptor MixedLogicalOperators = new DiagnosticDescriptor(
            "SRCP002",
            "&& と || を括弧なしで混ぜない",
            "&& と || を括弧なしで混ぜている。VB の条件と同じ組み合わせかを確かめて、括弧で書く",
            "Porting",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/7474/SRC/blob/master/SRC.Sharp/SRCPortingAnalyzers/README.md");

        public static readonly DiagnosticDescriptor UninitializedString = new DiagnosticDescriptor(
            "SRCP003",
            "string のフィールド・プロパティに初期値を書く",
            "string の{0} {1} に初期値が無く、null で始まる。VB の String は \"\" で始まるので、= \"\" を付ける",
            "Porting",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/7474/SRC/blob/master/SRC.Sharp/SRCPortingAnalyzers/README.md");

        public static readonly DiagnosticDescriptor GeneralLibToList = new DiagnosticDescriptor(
            "SRCP004",
            "GeneralLib.ToList を使わない",
            "GeneralLib.ToList は VB の ListSplit・ListIndex・ListLength と結果が違う（閉じていない括弧を区別しない）。GeneralLib.ListSplit・ListIndex・ListLength などを使う",
            "Porting",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: "https://github.com/7474/SRC/blob/master/SRC.Sharp/SRCPortingAnalyzers/README.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(FloatToIntegralCast, MixedLogicalOperators, UninitializedString, GeneralLibToList);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeCast, SyntaxKind.CastExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLogicalOr, SyntaxKind.LogicalOrExpression);
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
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

        private static bool IsString(SemanticModel model, TypeSyntax type, System.Threading.CancellationToken ct)
        {
            return model.GetTypeInfo(type, ct).Type?.SpecialType == SpecialType.System_String;
        }

        // VB のモジュール・クラスを移した型のある名前空間。SRCCore.Config・Filesystem・Logging など、SRC# で足した仕組みの名前空間は対象にしない
        private static readonly string[] VbPortNamespaces =
        {
            "SRCCore.Models", "SRCCore.Units", "SRCCore.Pilots", "SRCCore.Items", "SRCCore.Events",
            "SRCCore.Commands", "SRCCore.CmdDatas", "SRCCore.Maps", "SRCCore.Expressions",
        };

        private static bool IsVbPortType(SyntaxNodeAnalysisContext context, SyntaxNode typeDeclaration)
        {
            var type = context.SemanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken) as INamedTypeSymbol;
            var ns = type?.ContainingNamespace?.ToDisplayString();
            if (ns == null)
            {
                return false;
            }
            // SRCCore の直下（SRC・Effect・Sound など VB の標準モジュールを移した型）と、VbPortNamespaces とその下
            if (ns == "SRCCore")
            {
                return true;
            }
            foreach (var x in VbPortNamespaces)
            {
                if (ns == x || ns.StartsWith(x + ".", System.StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        // 型のすべてのコンストラクター（: this(...) で他へ渡すものを除く）が、その名前に代入しているか。
        // 代入していれば null で始まることは無いので、初期値を求めない
        private static bool IsAssignedInAllConstructors(SyntaxNodeAnalysisContext context, SyntaxNode typeDeclaration, string name, bool isStatic)
        {
            var type = context.SemanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken) as INamedTypeSymbol;
            if (type == null)
            {
                return false;
            }
            var constructors = isStatic ? type.StaticConstructors : type.InstanceConstructors;
            var checkedAny = false;
            foreach (var constructor in constructors)
            {
                if (constructor.DeclaringSyntaxReferences.Length == 0)
                {
                    // 書いていないコンストラクター（既定のコンストラクター）は何も代入しない
                    return false;
                }
                foreach (var reference in constructor.DeclaringSyntaxReferences)
                {
                    var declaration = reference.GetSyntax(context.CancellationToken) as ConstructorDeclarationSyntax;
                    if (declaration == null)
                    {
                        return false;
                    }
                    if (declaration.Initializer != null && declaration.Initializer.IsKind(SyntaxKind.ThisConstructorInitializer))
                    {
                        continue;
                    }
                    checkedAny = true;
                    var assigned = false;
                    foreach (var assignment in declaration.DescendantNodes().OfType<AssignmentExpressionSyntax>())
                    {
                        var left = assignment.Left;
                        if (left is MemberAccessExpressionSyntax member && member.Expression is ThisExpressionSyntax)
                        {
                            left = member.Name;
                        }
                        if (left is IdentifierNameSyntax identifier && identifier.Identifier.Text == name)
                        {
                            assigned = true;
                            break;
                        }
                    }
                    if (!assigned)
                    {
                        return false;
                    }
                }
            }
            return checkedAny;
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var field = (FieldDeclarationSyntax)context.Node;
            if (field.Modifiers.Any(SyntaxKind.ConstKeyword) || !(field.Parent is ClassDeclarationSyntax)
                || !IsVbPortType(context, field.Parent))
            {
                return;
            }
            if (!IsString(context.SemanticModel, field.Declaration.Type, context.CancellationToken))
            {
                return;
            }
            foreach (var variable in field.Declaration.Variables)
            {
                if (variable.Initializer == null
                    && !IsAssignedInAllConstructors(context, field.Parent, variable.Identifier.Text, field.Modifiers.Any(SyntaxKind.StaticKeyword)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(UninitializedString, variable.Identifier.GetLocation(), "フィールド", variable.Identifier.Text));
                }
            }
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            if (property.Initializer != null || property.AccessorList == null || !(property.Parent is ClassDeclarationSyntax)
                || property.Modifiers.Any(SyntaxKind.AbstractKeyword) || !IsVbPortType(context, property.Parent))
            {
                return;
            }
            // 自動実装のプロパティ（get・set に本体が無い）だけを見る
            foreach (var accessor in property.AccessorList.Accessors)
            {
                if (accessor.Body != null || accessor.ExpressionBody != null)
                {
                    return;
                }
            }
            if (!IsString(context.SemanticModel, property.Type, context.CancellationToken)
                || IsAssignedInAllConstructors(context, property.Parent, property.Identifier.Text, property.Modifiers.Any(SyntaxKind.StaticKeyword)))
            {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(UninitializedString, property.Identifier.GetLocation(), "プロパティ", property.Identifier.Text));
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol method
                && method.Name == "ToList"
                && method.ContainingType?.Name == "GeneralLib")
            {
                context.ReportDiagnostic(Diagnostic.Create(GeneralLibToList, invocation.GetLocation()));
            }
        }
    }
}
