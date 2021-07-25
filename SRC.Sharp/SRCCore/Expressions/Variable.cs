// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.Expressions
{
    // === 変数に関する処理 ===
    public partial class Expression
    {
        private static bool isArray(string vname)
        {
            return !string.IsNullOrEmpty(vname)
                && Strings.InStr(vname, "[") > 0
                && Strings.Right(vname, 1) == "]";
        }

        // 変数の値を評価
        public ValueType GetVariable(string var_name, ValueType etype, out string str_result, out double num_result)
        {
            string vname = var_name;

            // 未定義値の設定
            str_result = var_name;
            num_result = 0d;

            try
            {
                if (isArray(vname))
                {
                    // 定義されていない要素を使って配列を読み出した場合は空文字列を返す
                    str_result = "";
                }

                var varObj = GetVariableObject(vname);
                if (varObj != null)
                {
                    return varObj.ReferenceValue(etype, out str_result, out num_result);
                }

                return ValueType.UndefinedType;
            }
            finally
            {
                SRC.LogTrace("<", etype.ToString().Substring(0, 1), var_name, str_result, num_result.ToString());
            }
        }

        private string ResolveArrayVarName(string vname)
        {
            // XXX 正規表現のほうが楽に書けそうではある
            // インデックス部分の切りだし
            var bracketPos = Strings.InStr(vname, "[");
            var idx = Strings.Mid(vname, bracketPos + 1, Strings.Len(vname) - bracketPos - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                int start_idx = 1;
                int depth = 0;
                bool is_term = true;
                bool in_single_quote = false;
                bool in_double_quote = false;
                // XXX StringBuilder
                string buf = "";
                string ipara;
                var loopTo = Strings.Len(idx);
                int i;
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                                        buf = buf + GetValueAsString(ipara, is_term);
                                        start_idx = (i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ipara, is_term);
                }
            }
            else
            {
                idx = GetValueAsString(idx);
            }

            // 変数名を配列のインデックス部を計算して再構築
            return Strings.Left(vname, bracketPos) + idx + "]";
        }

        // 指定した変数が定義されているか？
        public bool IsVariableDefined(string var_name)
        {
            bool IsVariableDefinedRet = default;
            string vname;
            switch (Strings.Asc(var_name))
            {
                case 36: // $
                    {
                        vname = Strings.Mid(var_name, 2);
                        break;
                    }

                default:
                    {
                        vname = var_name;
                        break;
                    }
            }

            // 変数が配列？
            if (isArray(vname))
            {
                vname = ResolveArrayVarName(vname);
            }

            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (IsSubLocalVariableDefined(vname))
            {
                return true;
            }

            // ローカル変数
            if (IsLocalVariableDefined(vname))
            {
                IsVariableDefinedRet = true;
                return IsVariableDefinedRet;
            }

            // グローバル変数
            if (IsGlobalVariableDefined(vname))
            {
                IsVariableDefinedRet = true;
                return IsVariableDefinedRet;
            }

            return IsVariableDefinedRet;
        }

        // 指定した名前のサブルーチンローカル変数が定義されているか？
        public bool IsSubLocalVariableDefined(string vname)
        {
            return Event.SubLocalVar(vname) != null;
        }

        // 指定した名前のローカル変数が定義されているか？
        public bool IsLocalVariableDefined(string vname)
        {
            return Event.LocalVariableList.ContainsKey(vname);
        }

        // 指定した名前のグローバル変数が定義されているか？
        public bool IsGlobalVariableDefined(string vname)
        {
            return Event.GlobalVariableList.ContainsKey(vname);
        }

        // 変数の値を設定
        public void SetVariable(string var_name, ValueType etype, string str_value, double num_value)
        {
            SRC.LogTrace(">", etype.ToString().Substring(0, 1), var_name, str_value, num_value.ToString());

            VarData new_var;
            string vname = var_name;
            int i, ret;
            var vname0 = default(string);
            var is_subroutine_local_array = default(bool);

            // 左辺値を伴う関数
            ret = Strings.InStr(vname, "(");
            if (ret > 1 && Strings.Right(vname, 1) == ")")
            {
                switch (Strings.LCase(Strings.Left(vname, ret - 1)) ?? "")
                {
                    case "hp":
                        {
                            Unit u;
                            var idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(idx);

                            if (SRC.UList.IsDefined2(idx))
                            {
                                u = SRC.UList.Item2(idx);
                            }
                            else if (SRC.PList.IsDefined(idx))
                            {
                                u = SRC.PList.Item(idx).Unit;
                            }
                            else
                            {
                                u = Event.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.HP = (int)num_value;
                                }
                                else
                                {
                                    u.HP = GeneralLib.StrToLng(str_value);
                                }

                                if (u.HP <= 0)
                                {
                                    u.HP = 1;
                                }
                            }

                            return;
                        }

                    case "en":
                        {
                            Unit u;
                            var idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(idx);

                            if (SRC.UList.IsDefined2(idx))
                            {
                                u = SRC.UList.Item2(idx);
                            }
                            else if (SRC.PList.IsDefined(idx))
                            {
                                u = SRC.PList.Item(idx).Unit;
                            }
                            else
                            {
                                u = Event.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.EN = (int)num_value;
                                }
                                else
                                {
                                    u.EN = GeneralLib.StrToLng(str_value);
                                }

                                if (u.EN == 0 && u.Status == "出撃")
                                {
                                    GUI.PaintUnitBitmap(u);
                                }
                            }

                            return;
                        }

                    case "sp":
                        {
                            Pilot p;
                            var idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(idx);

                            if (SRC.UList.IsDefined2(idx))
                            {
                                p = SRC.UList.Item2(idx).MainPilot();
                            }
                            else if (SRC.PList.IsDefined(idx))
                            {
                                p = SRC.PList.Item(idx);
                            }
                            else
                            {
                                p = Event.SelectedUnitForEvent.MainPilot();
                            }

                            if (p is object)
                            {
                                {
                                    var withBlock = p;
                                    if (withBlock.MaxSP > 0)
                                    {
                                        if (etype == ValueType.NumericType)
                                        {
                                            withBlock.SP = (int)num_value;
                                        }
                                        else
                                        {
                                            withBlock.SP = GeneralLib.StrToLng(str_value);
                                        }
                                    }
                                }
                            }

                            return;
                        }

                    case "plana":
                        {
                            Pilot p;
                            var idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(idx);

                            if (SRC.UList.IsDefined2(idx))
                            {
                                p = SRC.UList.Item2(idx).MainPilot();
                            }
                            else if (SRC.PList.IsDefined(idx))
                            {
                                p = SRC.PList.Item(idx);
                            }
                            else
                            {
                                p = Event.SelectedUnitForEvent.MainPilot();
                            }

                            if (p is object)
                            {
                                if (p.MaxPlana() > 0)
                                {
                                    if (etype == ValueType.NumericType)
                                    {
                                        p.Plana = (int)num_value;
                                    }
                                    else
                                    {
                                        p.Plana = GeneralLib.StrToLng(str_value);
                                    }
                                }
                            }

                            return;
                        }

                    case "action":
                        {
                            Unit u;
                            var idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(idx);

                            if (SRC.UList.IsDefined2(idx))
                            {
                                u = SRC.UList.Item2(idx);
                            }
                            else if (SRC.PList.IsDefined(idx))
                            {
                                u = SRC.PList.Item(idx).Unit;
                            }
                            else
                            {
                                u = Event.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.UsedAction = (int)(u.MaxAction() - num_value);
                                }
                                else
                                {
                                    u.UsedAction = (u.MaxAction() - GeneralLib.StrToLng(str_value));
                                }
                            }

                            return;
                        }

                    case "eval":
                        {
                            vname = Strings.Trim(Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1));
                            vname = GetValueAsString(vname);
                            break;
                        }
                }
            }

            // 変数が配列？
            if (isArray(vname))
            {
                vname = ResolveArrayVarName(vname);

                // 配列名
                vname0 = Strings.Left(vname, Strings.InStr(vname, "[") - 1);

                // サブルーチンローカルな配列として定義済みかどうかチェック
                if (IsSubLocalVariableDefined(vname0))
                {
                    is_subroutine_local_array = true;
                }
            }

            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数として定義済み？
            if (IsSubLocalVariableDefined(vname))
            {
                var v = Event.SubLocalVar(vname);
                if (v != null)
                {
                    v.SetValue(vname, etype, str_value, num_value);
                    return;
                }
            }

            if (is_subroutine_local_array)
            {
                // サブルーチンローカル変数の配列の要素として定義
                if (Event.VarIndex >= Events.Event.MaxVarIndex)
                {
                    Event.VarIndex = Events.Event.MaxVarIndex;
                    Event.DisplayEventErrorMessage(Event.CurrentLineNum, "作成したサブルーチンローカル変数の総数が" + SrcFormatter.Format(Events.Event.MaxVarIndex) + "個を超えています");
                    return;
                }

                Event.NewSubLocalVar().SetValue(vname, etype, str_value, num_value);
                return;
            }

            // ローカル変数として定義済み？
            if (IsLocalVariableDefined(vname))
            {
                var v = Event.LocalVariableList[vname];
                v.SetValue(vname, etype, str_value, num_value);
                return;
            }

            // グローバル変数として定義済み？
            if (IsGlobalVariableDefined(vname))
            {
                var v = Event.GlobalVariableList[vname];
                v.SetValue(vname, etype, str_value, num_value);
                return;
            }

            // システム変数？
            switch (Strings.LCase(vname) ?? "")
            {
                case "basex":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            Event.BaseX = (int)num_value;
                        }
                        else
                        {
                            Event.BaseX = GeneralLib.StrToLng(str_value);
                        }
                        GUI.UpdateBaseX(Event.BaseX);
                        return;
                    }

                case "basey":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            Event.BaseY = (int)num_value;
                        }
                        else
                        {
                            Event.BaseY = GeneralLib.StrToLng(str_value);
                        }
                        GUI.UpdateBaseY(Event.BaseY);
                        return;
                    }

                case "ターン数":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            SRC.Turn = (int)num_value;
                        }
                        else
                        {
                            SRC.Turn = GeneralLib.StrToLng(str_value);
                        }

                        return;
                    }

                case "総ターン数":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            SRC.TotalTurn = (int)num_value;
                        }
                        else
                        {
                            SRC.TotalTurn = GeneralLib.StrToLng(str_value);
                        }

                        return;
                    }

                case "資金":
                    {
                        SRC.Money = 0;
                        if (etype == ValueType.NumericType)
                        {
                            SRC.IncrMoney((int)num_value);
                        }
                        else
                        {
                            SRC.IncrMoney(GeneralLib.StrToLng(str_value));
                        }

                        return;
                    }
            }

            // 未定義だった場合

            // 配列の要素として作成
            VarData new_var2;
            if (Strings.Len(vname0) != 0)
            {
                // ローカル変数の配列の要素として定義
                if (IsLocalVariableDefined(vname0))
                {
                    // Nop
                }
                // グローバル変数の配列の要素として定義
                else if (IsGlobalVariableDefined(vname0))
                {
                    DefineGlobalVariable(vname);
                    var v = Event.GlobalVariableList[vname];
                    v.SetValue(vname, etype, str_value, num_value);

                    return;
                }
                // 未定義の配列なのでローカル変数の配列を作成
                else
                {
                    // ローカル変数の配列のメインＩＤを作成
                    new_var2 = new VarData();
                    new_var2.Init(vname0);
                    if (Strings.InStr(new_var2.Name, "\"") > 0)
                    {
                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "不正な変数「" + new_var2.Name + "」が作成されました");
                    }

                    Event.LocalVariableList.Add(vname0, new_var2);
                }
            }

            // ローカル変数として作成
            new_var = new VarData();
            new_var.SetValue(vname, etype, str_value, num_value);
            if (Strings.InStr(new_var.Name, "\"") > 0)
            {
                Event.DisplayEventErrorMessage(Event.CurrentLineNum, "不正な変数「" + new_var.Name + "」が作成されました");
            }

            Event.LocalVariableList.Add(vname, new_var);
        }

        public void SetVariableAsString(string vname, string new_value)
        {
            SetVariable(vname, ValueType.StringType, new_value, 0d);
        }

        public void SetVariableAsDouble(string vname, double new_value)
        {
            SetVariable(vname, ValueType.NumericType, "", new_value);
        }

        public void SetVariableAsLong(string vname, int new_value)
        {
            SetVariable(vname, ValueType.NumericType, "", new_value);
        }

        // グローバル変数を定義
        public void DefineGlobalVariable(string vname)
        {
            var new_var = new VarData();
            new_var.Init(vname);
            Event.GlobalVariableList.Add(vname, new_var);
        }

        // ローカル変数を定義
        public void DefineLocalVariable(string vname)
        {
            var new_var = new VarData();
            new_var.Init(vname);
            Event.LocalVariableList.Add(vname, new_var);
        }

        // 変数を消去
        public void UndefineVariable(string var_name)
        {
            string vname, vname2;
            int i, ret;
            string idx, buf = default;
            int start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            if (Strings.Asc(var_name) == 36) // $
            {
                vname = Strings.Mid(var_name, 2);
            }
            else
            {
                vname = var_name;
            }

            // Eval関数
            if (Strings.LCase(Strings.Left(vname, 5)) == "eval(")
            {
                if (Strings.Right(vname, 1) == ")")
                {
                    vname = Strings.Mid(vname, 6, Strings.Len(vname) - 6);
                    vname = GetValueAsString(vname);
                }
            }

            // 配列の要素？
            ret = Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // 配列の要素を指定された場合

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        buf = buf + GetValueAsString(Strings.Mid(idx, start_idx, i - start_idx), is_term);
                                        start_idx = (i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(Strings.Mid(idx, start_idx, i - start_idx), is_term);
                }
                else
                {
                    idx = GetValueAsString(Strings.Mid(idx, start_idx, i - start_idx), is_term);
                }
            }
            else
            {
                idx = GetValueAsString(idx);
            }

            // インデックス部分を評価して変数名を置き換え
            vname = Strings.Left(vname, ret) + idx + "]";

            // サブルーチンローカル変数？
            if (IsSubLocalVariableDefined(vname))
            {
                var v = Event.SubLocalVar(vname);
                if (v != null)
                {
                    v.Clear();
                    return;
                }
            }

            // ローカル変数？
            if (IsLocalVariableDefined(vname))
            {
                Event.LocalVariableList.Remove(vname);
                return;
            }

            // グローバル変数？
            if (IsGlobalVariableDefined(vname))
            {
                Event.GlobalVariableList.Remove(vname);
            }

            // 配列の場合はここで終了
            return;
        SkipArrayHandling:
            ;


            // 通常の変数名を指定された場合

            // 配列要素の判定用
            vname2 = vname + "[";

            // サブルーチンローカル変数？
            if (IsSubLocalVariableDefined(vname))
            {
                // XXX 配列の取得
                foreach (var v in Event.SubLocalVars())
                {
                    if ((vname ?? "") == (v.Name ?? "") || Strings.InStr(v.Name, vname2) == 1)
                    {
                        v.Clear();
                    }
                }
                return;
            }

            // ローカル変数？
            if (IsLocalVariableDefined(vname))
            {
                Event.LocalVariableList.Remove(vname);
                foreach (VarData currentVar in Event.LocalVariableList.Values)
                {
                    if (Strings.InStr(currentVar.Name, vname2) == 1)
                    {
                        Event.LocalVariableList.Remove(currentVar.Name);
                    }
                }

                return;
            }

            // グローバル変数？
            if (IsGlobalVariableDefined(vname))
            {
                Event.GlobalVariableList.Remove(vname);
                foreach (VarData currentVar in Event.GlobalVariableList.Values)
                {
                    if (Strings.InStr(currentVar.Name, vname2) == 1)
                    {
                        Event.GlobalVariableList.Remove(currentVar.Name);
                    }
                }

                return;
            }
        }

        // 引数1で指定した変数のオブジェクトを取得
        public VarData GetVariableObject(string var_name)
        {
            string vname = var_name;
            // 変数が配列？
            if (isArray(vname))
            {
                vname = ResolveArrayVarName(vname);
            }

            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (Event.CallDepth > 0)
            {
                foreach (var v in Event.SubLocalVars())
                {
                    if ((vname ?? "") == (v.Name ?? ""))
                    {
                        return v;
                    }
                }
            }

            // ローカル変数
            if (IsLocalVariableDefined(vname))
            {
                return Event.LocalVariableList[vname];
            }

            // グローバル変数
            if (IsGlobalVariableDefined(vname))
            {
                return Event.GlobalVariableList[vname];
            }

            ValueType etype = ValueType.UndefinedType;
            string str_result = null;
            double num_result = 0d;
            // システム変数？
            switch (vname ?? "")
            {
                case "対象ユニット":
                case "対象パイロット":
                    {
                        if (Event.SelectedUnitForEvent is object)
                        {
                            {
                                var withBlock = Event.SelectedUnitForEvent;
                                if (withBlock.CountPilot() > 0)
                                {
                                    str_result = withBlock.MainPilot().ID;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "相手ユニット":
                case "相手パイロット":
                    {
                        if (Event.SelectedTargetForEvent is object)
                        {
                            {
                                var withBlock1 = Event.SelectedTargetForEvent;
                                if (withBlock1.CountPilot() > 0)
                                {
                                    str_result = withBlock1.MainPilot().ID;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "対象ユニットＩＤ":
                    {
                        if (Event.SelectedUnitForEvent is object)
                        {
                            str_result = Event.SelectedUnitForEvent.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "相手ユニットＩＤ":
                    {
                        if (Event.SelectedTargetForEvent is object)
                        {
                            str_result = Event.SelectedTargetForEvent.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "対象ユニット使用武器":
                    {
                        if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            if (Commands.SelectedWeapon > 0)
                            {
                                str_result = Commands.SelectedWeaponName;
                            }
                            else
                            {
                                str_result = "";
                            }
                        }
                        else if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedTarget))
                        {
                            if (Commands.SelectedTWeapon > 0)
                            {
                                str_result = Commands.SelectedTWeaponName;
                            }
                            else
                            {
                                str_result = Commands.SelectedDefenseOption;
                            }
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "相手ユニット使用武器":
                    {
                        if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedTarget))
                        {
                            if (Commands.SelectedTWeapon > 0)
                            {
                                str_result = Commands.SelectedTWeaponName;
                            }
                            else
                            {
                                str_result = Commands.SelectedDefenseOption;
                            }
                        }
                        else if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedUnit))
                        {
                            if (Commands.SelectedWeapon > 0)
                            {
                                str_result = Commands.SelectedWeaponName;
                            }
                            else
                            {
                                str_result = "";
                            }
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "対象ユニット使用武器番号":
                    {
                        if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            num_result = Commands.SelectedWeapon;
                        }
                        else if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedTarget))
                        {
                            num_result = Commands.SelectedTWeapon;
                        }

                        etype = ValueType.NumericType;
                        break;
                    }

                case "相手ユニット使用武器番号":
                    {
                        if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedTarget))
                        {
                            num_result = Commands.SelectedTWeapon;
                        }
                        else if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedUnit))
                        {
                            num_result = Commands.SelectedWeapon;
                        }

                        etype = ValueType.NumericType;
                        break;
                    }

                case "対象ユニット使用アビリティ":
                    {
                        if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            if (Commands.SelectedAbility > 0)
                            {
                                str_result = Commands.SelectedAbilityName;
                            }
                            else
                            {
                                str_result = "";
                            }
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "対象ユニット使用アビリティ番号":
                    {
                        if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            num_result = Commands.SelectedAbility;
                        }

                        etype = ValueType.NumericType;
                        break;
                    }

                case "対象ユニット使用スペシャルパワー":
                    {
                        if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            str_result = Commands.SelectedSpecialPower;
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "サポートアタックユニットＩＤ":
                    {
                        if (Commands.SupportAttackUnit is object)
                        {
                            str_result = Commands.SupportAttackUnit.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "サポートガードユニットＩＤ":
                    {
                        if (Commands.SupportGuardUnit is object)
                        {
                            str_result = Commands.SupportGuardUnit.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "選択":
                    if (Information.IsNumeric(Event.SelectedAlternative))
                    {
                        num_result = GeneralLib.StrToDbl(Event.SelectedAlternative);
                        etype = ValueType.NumericType;
                    }
                    else
                    {
                        str_result = Event.SelectedAlternative;
                        etype = ValueType.StringType;
                    }

                    break;

                case "ターン数":
                    {
                        num_result = SRC.Turn;
                        etype = ValueType.NumericType;
                        break;
                    }

                case "総ターン数":
                    {
                        num_result = SRC.TotalTurn;
                        etype = ValueType.NumericType;
                        break;
                    }

                case "フェイズ":
                    {
                        str_result = SRC.Stage;
                        etype = ValueType.StringType;
                        break;
                    }

                case "味方数":
                case "ＮＰＣ数":
                case "敵数":
                case "中立数":
                    {
                        var party = Strings.Left(vname, Strings.Len(vname) - 1);
                        var num = 0;
                        foreach (var u in SRC.UList.Items)
                        {
                            if (u.Party0 == party && (u.Status == "出撃" || u.Status == "格納"))
                            {
                                num = num + 1;
                            }
                        }
                        num_result = num;
                        etype = ValueType.NumericType;
                        break;
                    }

                case "資金":
                    {
                        num_result = SRC.Money;
                        etype = ValueType.NumericType;
                        break;
                    }

                default:
                    {
                        // アルファベットの変数名はlow caseで判別
                        switch (Strings.LCase(vname))
                        {
                            case "apppath":
                                {
                                    str_result = SRC.AppPath;
                                    etype = ValueType.StringType;
                                    break;
                                }

                            case "appversion":
                                {
                                    // revision は捨てた
                                    // Ref. System.Version
                                    var v = SRC.Version;
                                    num_result = v.Major * 100000 + v.Minor * 100 + v.Build;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "argnum":
                                {
                                    // XXX GetVariableObjectと実装が違う、そもそもこの辺はバグフィクスで修正したい
                                    //num = (short)(Event.ArgIndex - Event.ArgIndexStack[Event.CallDepth - 1 - Event.UpVarLevel]);
                                    //num_result = num;
                                    //etype = ValueType.NumericType;
                                    //break;
                                    // UpVarの呼び出し回数を累計
                                    var num = Event.UpVarLevel;
                                    var i = Event.CallDepth;
                                    while (num > 0)
                                    {
                                        i = (i - num);
                                        if (i < 1)
                                        {
                                            i = 1;
                                            break;
                                        }

                                        num = Event.UpVarLevelStack[i];
                                    }

                                    num = (Event.ArgIndex - Event.ArgIndexStack[i - 1]);
                                    num_result = num;
                                    etype = ValueType.NumericType;

                                    break;
                                }

                            case "basex":
                                {
                                    num_result = Event.BaseX;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "basey":
                                {
                                    num_result = Event.BaseY;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "extdatapath":
                                {
                                    str_result = SRC.ExtDataPath;
                                    etype = ValueType.StringType;
                                    break;
                                }

                            case "extdatapath2":
                                {
                                    str_result = SRC.ExtDataPath2;
                                    etype = ValueType.StringType;
                                    break;
                                }

                            case "mousex":
                                {
                                    num_result = GUI.MouseX;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "mousey":
                                {
                                    num_result = GUI.MouseY;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "now":
                                {
                                    // XXX Format DateTime
                                    str_result = Conversions.ToString(Conversions.GetNow());
                                    etype = ValueType.StringType;
                                    break;
                                }

                            case "scenariopath":
                                {
                                    str_result = SRC.ScenarioPath;
                                    etype = ValueType.StringType;
                                    break;
                                }
                        }

                        break;
                    }
            }

            // コンフィグ変数？
            if (BCVariable.IsConfig)
            {
                switch (vname ?? "")
                {
                    case "攻撃値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.AttackExp);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.AttackExp;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "攻撃側ユニットＩＤ":
                        {
                            str_result = BCVariable.AtkUnit.ID;
                            etype = ValueType.StringType;
                            break;
                        }

                    case "防御側ユニットＩＤ":
                        {
                            if (BCVariable.DefUnit is object)
                            {
                                str_result = BCVariable.DefUnit.ID;
                                etype = ValueType.StringType;
                                break;
                            }

                            break;
                        }

                    case "武器番号":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.WeaponNumber);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.WeaponNumber;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "地形適応":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.TerrainAdaption);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.TerrainAdaption;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "武器威力":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.WeaponPower);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.WeaponPower;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "サイズ補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.SizeMod);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.SizeMod;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "装甲値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.Armor);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.Armor;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "最終値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.LastVariable);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.LastVariable;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "攻撃側補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.AttackVariable);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.AttackVariable;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "防御側補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.DffenceVariable);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.DffenceVariable;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }

                    case "ザコ補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = SrcFormatter.Format(BCVariable.CommonEnemy);
                                etype = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.CommonEnemy;
                                etype = ValueType.NumericType;
                            }

                            break;
                        }
                }

                // パイロットに関する変数
                {
                    var withBlock16 = BCVariable.MeUnit.MainPilot();
                    switch (vname ?? "")
                    {
                        case "気力":
                            {
                                var num = 0;
                                if (IsOptionDefined("気力効果小"))
                                {
                                    num = (50 + (withBlock16.Morale + withBlock16.MoraleMod) / 2); // 気力の補正込み値を代入
                                }
                                else
                                {
                                    num = (withBlock16.Morale + withBlock16.MoraleMod);
                                } // 気力の補正込み値を代入

                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(num);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = num;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "耐久":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Defense);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Defense;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "ＬＶ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Level);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Level;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "経験":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Exp);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Exp;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "ＳＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.SP);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.SP;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "霊力":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Plana);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Plana;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "格闘":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Infight);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Infight;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "射撃":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Shooting);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Shooting;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "命中":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Hit);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Hit;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "回避":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Dodge);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Dodge;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "技量":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Technique);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Technique;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "反応":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock16.Intuition);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Intuition;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }
                    }
                }

                // ユニットに関する変数
                {
                    var withBlock17 = BCVariable.MeUnit;
                    switch (vname ?? "")
                    {
                        case "最大ＨＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock17.MaxHP);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.MaxHP;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "現在ＨＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock17.HP);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.HP;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "最大ＥＮ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock17.MaxEN);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.MaxEN;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "現在ＥＮ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock17.EN);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.EN;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "移動力":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock17.Speed);
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.Speed;
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "装甲":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock17.get_Armor(""));
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.get_Armor("");
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }

                        case "運動性":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format(withBlock17.get_Mobility(""));
                                    etype = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.get_Mobility("");
                                    etype = ValueType.NumericType;
                                }

                                break;
                            }
                    }
                }
            }

            if (etype != ValueType.UndefinedType)
            {
                // XXX 0 と 未定義の区別ついていない
                if (etype == ValueType.NumericType
                    && str_result == null
                    && num_result != 0d)
                {
                    str_result = SrcFormatter.Format(num_result);
                }
                return new VarData(vname, etype, str_result ?? "", num_result);
            }

            return null;
        }
    }
}
