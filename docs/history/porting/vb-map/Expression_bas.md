# Expression.bas

VB 版の `SRC/SRC_20121125/Expression.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Function EvalExpr | 46 | 777 | `SRCCore/Expressions/Eval.cs` Expression.EvalExpr | 未確認 |  |
| Function EvalTerm | 825 | 86 | `SRCCore/Expressions/Eval.cs` Expression.EvalTerm | 未確認 |  |
| Function CallFunction | 916 | 2971 | `SRCCore/Expressions/Expression.function.cs` Expression.CallFunction | 未確認 |  |
| Function EvalInfoFunc | 3889 | 2272 | （見つからない） | 未確認 |  |
| Function GetVariable | 6166 | 926 | `SRCCore/Expressions/Variable.cs` Expression.GetVariable | 未確認 |  |
| Function IsVariableDefined | 7094 | 113 | `SRCCore/Expressions/Variable.cs` Expression.IsVariableDefined | 未確認 |  |
| Function IsSubLocalVariableDefined | 7209 | 12 | `SRCCore/Expressions/Variable.cs` Expression.IsSubLocalVariableDefined | 未確認 |  |
| Function IsLocalVariableDefined | 7223 | 11 | `SRCCore/Expressions/Variable.cs` Expression.IsLocalVariableDefined | 未確認 |  |
| Function IsGlobalVariableDefined | 7236 | 11 | `SRCCore/Expressions/Variable.cs` Expression.IsGlobalVariableDefined | 未確認 |  |
| Sub SetVariable | 7249 | 376 | `SRCCore/Expressions/Variable.cs` Expression.SetVariable | 未確認 |  |
| Sub SetVariableAsString | 7626 | 3 | `SRCCore/Expressions/Variable.cs` Expression.SetVariableAsString | 未確認 |  |
| Sub SetVariableAsDouble | 7630 | 3 | `SRCCore/Expressions/Variable.cs` Expression.SetVariableAsDouble | 未確認 |  |
| Sub SetVariableAsLong | 7634 | 3 | `SRCCore/Expressions/Variable.cs` Expression.SetVariableAsLong | 未確認 |  |
| Sub DefineGlobalVariable | 7639 | 10 | `SRCCore/Expressions/Variable.cs` Expression.DefineGlobalVariable | 未確認 |  |
| Sub DefineLocalVariable | 7651 | 10 | `SRCCore/Expressions/Variable.cs` Expression.DefineLocalVariable | 未確認 |  |
| Sub UndefineVariable | 7663 | 165 | `SRCCore/Expressions/Variable.cs` Expression.UndefineVariable | 未確認 |  |
| Function GetValueAsString | 7834 | 10 | `SRCCore/Expressions/GetValue.cs` Expression.GetValueAsString | 未確認 |  |
| Function GetValueAsDouble | 7846 | 10 | `SRCCore/Expressions/GetValue.cs` Expression.GetValueAsDouble | 未確認 |  |
| Function GetValueAsLong | 7858 | 11 | `SRCCore/Expressions/GetValue.cs` Expression.GetValueAsLong | 未確認 |  |
| Function IsExpr | 7873 | 8 | （見つからない） | 未確認 |  |
| Function IsOptionDefined | 7884 | 11 | `SRCCore/Expressions/Expression.option.cs` Expression.IsOptionDefined | 未確認 |  |
| Sub ReplaceSubExpression | 7898 | 37 | `SRCCore/Expressions/Expression.replace.cs` Expression.ReplaceSubExpression | 未確認 |  |
| Sub FormatMessage | 7937 | 11 | `SRCCore/Expressions/Expression.replace.cs` Expression.FormatMessage | 未確認 |  |
| Function Term | 7952 | 42 | `SRCCore/Expressions/Expression.term.cs` Expression.Term | 未確認 |  |
| Function GetVariableObject | 7997 | 306 | `SRCCore/Expressions/Variable.cs` Expression.GetVariableObject | 未確認 |  |
