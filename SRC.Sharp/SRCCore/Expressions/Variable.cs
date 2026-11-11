// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Logging;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Globalization;

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
        // VB の GetVariable と同じく、変数・システム変数・コンフィグ変数の順に探す。
        // 見つからないときは、数値として求めると 0（NumericType）、それ以外は変数名（配列の要素なら空文字列。StringType）を返す
        public ValueType GetVariable(string var_name, ValueType etype, out string str_result, out double num_result)
        {
            string vname = var_name;

            // 未定義値の設定
            str_result = var_name;
            num_result = 0d;
            var result = ValueType.UndefinedType;

            try
            {
                // 変数が配列？
                if (isArray(vname))
                {
                    vname = ResolveArrayVarName(vname);

                    // 定義されていない要素を使って配列を読み出した場合は空文字列を返す
                    str_result = "";
                }

                // ここから配列と通常変数の共通処理

                // サブルーチンローカル変数
                if (Event.CallDepth > 0)
                {
                    foreach (var v in Event.SubLocalVars())
                    {
                        if (vname == v.Name)
                        {
                            result = ReadVariableValue(v, etype, ref str_result, ref num_result);
                            return result;
                        }
                    }
                }

                // ローカル変数
                if (IsLocalVariableDefined(vname))
                {
                    result = ReadVariableValue(Event.LocalVariableList[vname], etype, ref str_result, ref num_result);
                    return result;
                }

                // グローバル変数
                if (IsGlobalVariableDefined(vname))
                {
                    result = ReadVariableValue(Event.GlobalVariableList[vname], etype, ref str_result, ref num_result);
                    return result;
                }

                // システム変数？
                if (GetSystemVariable(vname, etype, ref str_result, ref num_result, out result))
                {
                    return result;
                }

                // コンフィグ変数？
                if (BCVariable.IsConfig && GetConfigVariable(vname, etype, ref str_result, ref num_result, out result))
                {
                    return result;
                }

                if (etype == ValueType.NumericType)
                {
                    num_result = 0d;
                    result = ValueType.NumericType;
                }
                else
                {
                    result = ValueType.StringType;
                }
                return result;
            }
            finally
            {
                SRC.ExpressionLog.TraceWithCaller("<", etype.ToString().Substring(0, 1), var_name, str_result, num_result.ToString(CultureInfo.InvariantCulture));
            }
        }

        // VB の GetVariable で、定義済みの変数（VarStack・LocalVariableList・GlobalVariableList の要素）の値を読む部分。
        // 求めた型と違う型の値は、数値なら StrToDbl、文字列なら FormatNum で変換する。
        // 求めたほうの結果だけを書き換え、もう一方は呼び出し元の値のまま残す
        private static ValueType ReadVariableValue(VarData v, ValueType etype, ref string str_result, ref double num_result)
        {
            switch (etype)
            {
                case ValueType.NumericType:
                    if (v.VariableType == ValueType.NumericType)
                    {
                        num_result = v.NumericValue;
                    }
                    else
                    {
                        num_result = GeneralLib.StrToDbl(v.StringValue);
                    }
                    return ValueType.NumericType;

                case ValueType.StringType:
                    if (v.VariableType == ValueType.StringType)
                    {
                        str_result = v.StringValue;
                    }
                    else
                    {
                        str_result = GeneralLib.FormatNum(v.NumericValue);
                    }
                    return ValueType.StringType;

                default:
                    if (v.VariableType == ValueType.StringType)
                    {
                        str_result = v.StringValue;
                        return ValueType.StringType;
                    }
                    else
                    {
                        num_result = v.NumericValue;
                        return ValueType.NumericType;
                    }
            }
        }

        // 数値のシステム変数を、求めた型で返す（VB の If etype = StringType Then Format$(…) Else …）
        private static ValueType ReturnNumber(double value, ValueType etype, ref string str_result, ref double num_result)
        {
            if (etype == ValueType.StringType)
            {
                str_result = SrcFormatter.Format(value);
                return ValueType.StringType;
            }
            else
            {
                num_result = value;
                return ValueType.NumericType;
            }
        }

        // VB の CStr(Now)。日本語版の Windows の既定の書式（短い日付 yyyy/MM/dd、長い時刻 H:mm:ss）で、
        // 時刻が 0:00:00 のときは日付だけになる
        private static string FormatNow()
        {
            var now = Conversions.GetNow();
            if (now.Hour == 0 && now.Minute == 0 && now.Second == 0)
            {
                return now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            }
            return now.ToString("yyyy/MM/dd H:mm:ss", CultureInfo.InvariantCulture);
        }

        // VB の GetVariable の「システム変数？」の Select Case。当たれば true
        private bool GetSystemVariable(string vname, ValueType etype, ref string str_result, ref double num_result, out ValueType result)
        {
            result = ValueType.UndefinedType;
            switch (vname ?? "")
            {
                case "対象ユニット":
                case "対象パイロット":
                    {
                        var u = Event.SelectedUnitForEvent;
                        if (u is object && u.CountPilot() > 0)
                        {
                            str_result = u.MainPilot().ID;
                        }
                        else
                        {
                            str_result = "";
                        }
                        result = ValueType.StringType;
                        return true;
                    }

                case "相手ユニット":
                case "相手パイロット":
                    {
                        var u = Event.SelectedTargetForEvent;
                        if (u is object && u.CountPilot() > 0)
                        {
                            str_result = u.MainPilot().ID;
                        }
                        else
                        {
                            str_result = "";
                        }
                        result = ValueType.StringType;
                        return true;
                    }

                case "対象ユニットＩＤ":
                    str_result = Event.SelectedUnitForEvent?.ID ?? "";
                    result = ValueType.StringType;
                    return true;

                case "相手ユニットＩＤ":
                    str_result = Event.SelectedTargetForEvent?.ID ?? "";
                    result = ValueType.StringType;
                    return true;

                case "対象ユニット使用武器":
                    str_result = "";
                    if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                    {
                        str_result = Commands.SelectedWeapon > 0 ? Commands.SelectedWeaponName : "";
                    }
                    else if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedTarget))
                    {
                        str_result = Commands.SelectedTWeapon > 0 ? Commands.SelectedTWeaponName : Commands.SelectedDefenseOption;
                    }
                    result = ValueType.StringType;
                    return true;

                case "相手ユニット使用武器":
                    str_result = "";
                    if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedTarget))
                    {
                        str_result = Commands.SelectedTWeapon > 0 ? Commands.SelectedTWeaponName : Commands.SelectedDefenseOption;
                    }
                    else if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedUnit))
                    {
                        str_result = Commands.SelectedWeapon > 0 ? Commands.SelectedWeaponName : "";
                    }
                    result = ValueType.StringType;
                    return true;

                // 武器・アビリティの番号は、どちらのユニットにも当たらないときは型を決めない（VB では UndefinedType で空文字列）
                case "対象ユニット使用武器番号":
                    str_result = "";
                    if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                    {
                        result = ReturnNumber(Commands.SelectedWeapon, etype, ref str_result, ref num_result);
                    }
                    else if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedTarget))
                    {
                        result = ReturnNumber(Commands.SelectedTWeapon, etype, ref str_result, ref num_result);
                    }
                    return true;

                case "相手ユニット使用武器番号":
                    str_result = "";
                    if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedTarget))
                    {
                        result = ReturnNumber(Commands.SelectedTWeapon, etype, ref str_result, ref num_result);
                    }
                    else if (ReferenceEquals(Event.SelectedTargetForEvent, Commands.SelectedUnit))
                    {
                        result = ReturnNumber(Commands.SelectedWeapon, etype, ref str_result, ref num_result);
                    }
                    return true;

                case "対象ユニット使用アビリティ":
                    str_result = "";
                    if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                    {
                        str_result = Commands.SelectedAbility > 0 ? Commands.SelectedAbilityName : "";
                    }
                    result = ValueType.StringType;
                    return true;

                case "対象ユニット使用アビリティ番号":
                    str_result = "";
                    if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                    {
                        result = ReturnNumber(Commands.SelectedAbility, etype, ref str_result, ref num_result);
                    }
                    return true;

                case "対象ユニット使用スペシャルパワー":
                    str_result = "";
                    if (ReferenceEquals(Event.SelectedUnitForEvent, Commands.SelectedUnit))
                    {
                        str_result = Commands.SelectedSpecialPower;
                    }
                    result = ValueType.StringType;
                    return true;

                case "サポートアタックユニットＩＤ":
                    str_result = Commands.SupportAttackUnit?.ID ?? "";
                    result = ValueType.StringType;
                    return true;

                case "サポートガードユニットＩＤ":
                    str_result = Commands.SupportGuardUnit?.ID ?? "";
                    result = ValueType.StringType;
                    return true;

                // 文字列として求めると、選んだ文字列をそのまま返す（「007」は「007」のまま）
                case "選択":
                    if (etype == ValueType.NumericType)
                    {
                        num_result = GeneralLib.StrToDbl(Event.SelectedAlternative);
                        result = ValueType.NumericType;
                    }
                    else
                    {
                        str_result = Event.SelectedAlternative ?? "";
                        result = ValueType.StringType;
                    }
                    return true;

                case "ターン数":
                    result = ReturnNumber(SRC.Turn, etype, ref str_result, ref num_result);
                    return true;

                case "総ターン数":
                    result = ReturnNumber(SRC.TotalTurn, etype, ref str_result, ref num_result);
                    return true;

                case "フェイズ":
                    str_result = SRC.Stage;
                    result = ValueType.StringType;
                    return true;

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
                        result = ReturnNumber(num, etype, ref str_result, ref num_result);
                        return true;
                    }

                case "資金":
                    if (etype == ValueType.StringType)
                    {
                        str_result = GeneralLib.FormatNum(SRC.Money);
                        result = ValueType.StringType;
                    }
                    else
                    {
                        num_result = SRC.Money;
                        result = ValueType.NumericType;
                    }
                    return true;
            }

            // アルファベットの変数名はlow caseで判別
            switch (Strings.LCase(vname))
            {
                case "apppath":
                    str_result = SRC.AppPath;
                    result = ValueType.StringType;
                    return true;

                case "appversion":
                    {
                        // 元の SRC と同じく 10000 * Major + 100 * Minor + Revision（例: Ver.2.2.33 → 20233）
                        // System.Version では 3 番目の数が Build
                        var v = SRC.CompatibleVersion;
                        result = ReturnNumber(10000 * v.Major + 100 * v.Minor + Math.Max(0, v.Build), etype, ref str_result, ref num_result);
                        return true;
                    }

                case "argnum":
                    result = ReturnNumber(ArgNum(), etype, ref str_result, ref num_result);
                    return true;

                case "basex":
                    result = ReturnNumber(Event.BaseX, etype, ref str_result, ref num_result);
                    return true;

                case "basey":
                    result = ReturnNumber(Event.BaseY, etype, ref str_result, ref num_result);
                    return true;

                case "extdatapath":
                    str_result = SRC.ExtDataPath;
                    result = ValueType.StringType;
                    return true;

                case "extdatapath2":
                    str_result = SRC.ExtDataPath2;
                    result = ValueType.StringType;
                    return true;

                case "mousex":
                    result = ReturnNumber(GUI.MouseX, etype, ref str_result, ref num_result);
                    return true;

                case "mousey":
                    result = ReturnNumber(GUI.MouseY, etype, ref str_result, ref num_result);
                    return true;

                case "now":
                    str_result = FormatNow();
                    result = ValueType.StringType;
                    return true;

                case "scenariopath":
                    str_result = SRC.ScenarioPath;
                    result = ValueType.StringType;
                    return true;
            }

            return false;
        }

        // VB の GetVariable の argnum。UpVar の呼び出し回数を累計して、引数を数える呼び出しの階層を決める
        private int ArgNum()
        {
            var num = Event.UpVarLevel;
            var i = Event.CallDepth;
            while (num > 0)
            {
                i = i - num;
                if (i < 1)
                {
                    i = 1;
                    break;
                }

                num = Event.UpVarLevelStack[i];
            }

            return Event.ArgIndex - Event.ArgIndexStack[i - 1];
        }

        // VB の GetVariable の「コンフィグ変数？」（BCVariable.IsConfig のとき）。当たれば true
        private bool GetConfigVariable(string vname, ValueType etype, ref string str_result, ref double num_result, out ValueType result)
        {
            result = ValueType.UndefinedType;
            switch (vname ?? "")
            {
                case "攻撃値":
                    result = ReturnNumber(BCVariable.AttackExp, etype, ref str_result, ref num_result);
                    return true;

                case "攻撃側ユニットＩＤ":
                    str_result = BCVariable.AtkUnit.ID;
                    result = ValueType.StringType;
                    return true;

                case "防御側ユニットＩＤ":
                    // 防御側が無いときは、ほかのコンフィグ変数と同じく次を探す
                    if (BCVariable.DefUnit is object)
                    {
                        str_result = BCVariable.DefUnit.ID;
                        result = ValueType.StringType;
                        return true;
                    }
                    break;

                case "武器番号":
                    result = ReturnNumber(BCVariable.WeaponNumber, etype, ref str_result, ref num_result);
                    return true;

                case "地形適応":
                    result = ReturnNumber(BCVariable.TerrainAdaption, etype, ref str_result, ref num_result);
                    return true;

                case "武器威力":
                    result = ReturnNumber(BCVariable.WeaponPower, etype, ref str_result, ref num_result);
                    return true;

                case "サイズ補正":
                    result = ReturnNumber(BCVariable.SizeMod, etype, ref str_result, ref num_result);
                    return true;

                case "装甲値":
                    result = ReturnNumber(BCVariable.Armor, etype, ref str_result, ref num_result);
                    return true;

                case "最終値":
                    result = ReturnNumber(BCVariable.LastVariable, etype, ref str_result, ref num_result);
                    return true;

                case "攻撃側補正":
                    result = ReturnNumber(BCVariable.AttackVariable, etype, ref str_result, ref num_result);
                    return true;

                case "防御側補正":
                    result = ReturnNumber(BCVariable.DffenceVariable, etype, ref str_result, ref num_result);
                    return true;

                case "ザコ補正":
                    result = ReturnNumber(BCVariable.CommonEnemy, etype, ref str_result, ref num_result);
                    return true;
            }

            // パイロットに関する変数
            var p = BCVariable.MeUnit.MainPilot();
            switch (vname ?? "")
            {
                case "気力":
                    {
                        int num;
                        if (IsOptionDefined("気力効果小"))
                        {
                            // 気力の補正込み値を代入（VB の Integer の \ は 0 に近いほうへ切り捨て）
                            num = 50 + (p.Morale + p.MoraleMod) / 2;
                        }
                        else
                        {
                            num = p.Morale + p.MoraleMod;
                        }
                        result = ReturnNumber(num, etype, ref str_result, ref num_result);
                        return true;
                    }

                case "耐久":
                    result = ReturnNumber(p.Defense, etype, ref str_result, ref num_result);
                    return true;

                case "ＬＶ":
                    result = ReturnNumber(p.Level, etype, ref str_result, ref num_result);
                    return true;

                case "経験":
                    result = ReturnNumber(p.Exp, etype, ref str_result, ref num_result);
                    return true;

                case "ＳＰ":
                    result = ReturnNumber(p.SP, etype, ref str_result, ref num_result);
                    return true;

                case "霊力":
                    result = ReturnNumber(p.Plana, etype, ref str_result, ref num_result);
                    return true;

                case "格闘":
                    result = ReturnNumber(p.Infight, etype, ref str_result, ref num_result);
                    return true;

                case "射撃":
                    result = ReturnNumber(p.Shooting, etype, ref str_result, ref num_result);
                    return true;

                case "命中":
                    result = ReturnNumber(p.Hit, etype, ref str_result, ref num_result);
                    return true;

                case "回避":
                    result = ReturnNumber(p.Dodge, etype, ref str_result, ref num_result);
                    return true;

                case "技量":
                    result = ReturnNumber(p.Technique, etype, ref str_result, ref num_result);
                    return true;

                case "反応":
                    result = ReturnNumber(p.Intuition, etype, ref str_result, ref num_result);
                    return true;
            }

            // ユニットに関する変数
            var u = BCVariable.MeUnit;
            switch (vname ?? "")
            {
                case "最大ＨＰ":
                    result = ReturnNumber(u.MaxHP, etype, ref str_result, ref num_result);
                    return true;

                case "現在ＨＰ":
                    result = ReturnNumber(u.HP, etype, ref str_result, ref num_result);
                    return true;

                case "最大ＥＮ":
                    result = ReturnNumber(u.MaxEN, etype, ref str_result, ref num_result);
                    return true;

                case "現在ＥＮ":
                    result = ReturnNumber(u.EN, etype, ref str_result, ref num_result);
                    return true;

                case "移動力":
                    result = ReturnNumber(u.Speed, etype, ref str_result, ref num_result);
                    return true;

                case "装甲":
                    result = ReturnNumber(u.get_Armor(""), etype, ref str_result, ref num_result);
                    return true;

                case "運動性":
                    result = ReturnNumber(u.get_Mobility(""), etype, ref str_result, ref num_result);
                    return true;
            }

            return false;
        }

        private string ResolveArrayVarName(string vname)
        {
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
        // VB と同じく、サブルーチンの中（CallDepth > 0）の、いまの呼び出しの変数だけを見る
        public bool IsSubLocalVariableDefined(string vname)
        {
            return Event.CallDepth > 0 && Event.SubLocalVar(vname) != null;
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
            SRC.ExpressionLog.TraceWithCaller(">", etype.ToString().Substring(0, 1), var_name, str_value, num_value.ToString(CultureInfo.InvariantCulture));

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
                                    u.HP = VBMath.CLng(num_value);
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
                                    u.EN = VBMath.CLng(num_value);
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
                                            withBlock.SP = VBMath.CLng(num_value);
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
                                        p.Plana = VBMath.CLng(num_value);
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
                                    u.UsedAction = VBMath.CLng(u.MaxAction() - num_value);
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
                            Event.BaseX = VBMath.CLng(num_value);
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
                            Event.BaseY = VBMath.CLng(num_value);
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
                            SRC.Turn = VBMath.CLng(num_value);
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
                            SRC.TotalTurn = VBMath.CLng(num_value);
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
                            SRC.IncrMoney(VBMath.CLng(num_value));
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
                // VB と同じく、いまの呼び出しの変数のうち、名前が同じものと配列の要素を消す
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
            string str_result = "";
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
                                    // 元の SRC と同じく 10000 * Major + 100 * Minor + Revision（例: Ver.2.2.33 → 20233）
                                    // System.Version では 3 番目の数が Build
                                    var v = SRC.CompatibleVersion;
                                    num_result = 10000 * v.Major + 100 * v.Minor + Math.Max(0, v.Build);
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "argnum":
                                {
                                    // VB の GetVariableObject は ArgIndex - ArgIndexStack(CallDepth - 1 - UpVarLevel) で、
                                    // UpVar を重ねたときの数え方が GetVariable と違い、添字が負になることもある（VB の不具合）。
                                    // Swap・CopyArray に argnum を渡したときだけ表に出るので、GetVariable と同じ求め方にそろえる
                                    num_result = ArgNum();
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
                                    str_result = FormatNow();
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

            // コンフィグ変数は VB の GetVariableObject でも探さない
            if (etype != ValueType.UndefinedType)
            {
                return new VarData(vname, etype, str_result ?? "", num_result);
            }

            return null;
        }
    }
}
