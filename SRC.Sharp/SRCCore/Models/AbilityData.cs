// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Models
{
    // アビリティデータのクラス
    public class AbilityData
    {
        // 名称
        public string Name;
        // 使用可能回数
        public int Stock;
        // ＥＮ消費量
        public int ENConsumption;
        // 必要気力
        public int NecessaryMorale;
        // 最小射程
        public int MinRange;
        // 最大射程
        public int MaxRange;
        // 属性
        public string Class = "";
        // 必要技能
        public string NecessarySkill = "";
        // 必要条件
        public string NecessaryCondition = "";

        // 効果
        private SrcCollection<AbilityEffect> colEffects = new SrcCollection<AbilityEffect>();
        public IList<AbilityEffect> Effects => colEffects.List;

        private SRC SRC { get; }
        private Expressions.Expression Expression => SRC.Expression;

        public AbilityData(SRC src)
        {
            SRC = src;
        }

        // アビリティ愛称
        public string Nickname()
        {
            string NicknameRet = Name;
            Expression.ReplaceSubExpression(ref NicknameRet);
            if (Strings.InStr(NicknameRet, "(") > 0)
            {
                NicknameRet = Strings.Left(NicknameRet, Strings.InStr(NicknameRet, "(") - 1);
            }

            return NicknameRet;
        }

        // アビリティに効果を追加
        public void SetEffect(string elist)
        {
            elist = (elist ?? "").Trim();
            foreach (var eelm in GeneralLib.ToList(elist))
            {
                var dat = new AbilityEffect();
                var buf = eelm;
                var j = Strings.InStr(buf, "Lv");
                var k = Strings.InStr(buf, "=");
                string etype;
                string elevel;
                string edata;
                if (j > 0 && (k == 0 || j < k))
                {
                    // レベル指定のある効果(データ指定があるものを含む)
                    dat.Name = Strings.Left(buf, j - 1);
                    if (k > 0)
                    {
                        // データ指定があるもの
                        dat.Level = Conversions.ToDouble(Strings.Mid(buf, j + 2, k - (j + 2)));
                        buf = Strings.Mid(buf, k + 1);
                        if (Strings.Left(buf, 1) == "\"")
                        {
                            buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                        }

                        j = Strings.InStr(buf, "Lv");
                        k = Strings.InStr(buf, "=");
                        if (j > 0 && (k == 0 || j < k))
                        {
                            // データ指定内にレベル指定がある
                            etype = Strings.Left(buf, j - 1);
                            if (k > 0)
                            {
                                elevel = Strings.Mid(buf, j + 2, k - (j + 2));
                                edata = Strings.Mid(buf, k + 1);
                            }
                            else
                            {
                                elevel = Strings.Mid(buf, j + 2);
                                edata = "";
                            }
                        }
                        else if (k > 0)
                        {
                            // データ指定内にデータ指定がある
                            etype = Strings.Left(buf, k - 1);
                            elevel = "";
                            edata = Strings.Mid(buf, k + 1);
                        }
                        else
                        {
                            // 単純なデータ指定
                            etype = buf;
                            elevel = "";
                            edata = "";
                        }

                        if (dat.Name == "付加" && string.IsNullOrEmpty(elevel))
                        {
                            elevel = SrcFormatter.Format(Constants.DEFAULT_LEVEL);
                        }

                        dat.Data = Strings.Trim(etype + " " + elevel + " " + edata);
                    }
                    else
                    {
                        // データ指定がないもの
                        dat.Level = Conversions.ToDouble(Strings.Mid(buf, j + 2));
                    }
                }
                else if (k > 0)
                {
                    // データ指定を含む効果
                    dat.Name = Strings.Left(buf, k - 1);
                    buf = Strings.Mid(buf, k + 1);
                    if (Strings.Asc(buf) == 34) // "
                    {
                        buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                    }

                    j = Strings.InStr(buf, "Lv");
                    k = Strings.InStr(buf, "=");
                    if (dat.Name == "解説")
                    {
                        // 解説の指定
                        etype = buf;
                        elevel = "";
                        edata = "";
                    }
                    else if (j > 0)
                    {
                        // データ指定内にレベル指定がある
                        etype = Strings.Left(buf, j - 1);
                        if (k > 0)
                        {
                            elevel = Strings.Mid(buf, j + 2, k - (j + 2));
                            edata = Strings.Mid(buf, k + 1);
                        }
                        else
                        {
                            elevel = Strings.Mid(buf, j + 2);
                            edata = "";
                        }
                    }
                    else if (k > 0)
                    {
                        // データ指定内にデータ指定がある
                        etype = Strings.Left(buf, k - 1);
                        elevel = "";
                        edata = Strings.Mid(buf, k + 1);
                    }
                    else
                    {
                        // 単純なデータ指定
                        etype = buf;
                        elevel = "";
                        edata = "";
                    }

                    if (dat.Name == "付加" && string.IsNullOrEmpty(elevel))
                    {
                        elevel = SrcFormatter.Format(Constants.DEFAULT_LEVEL);
                    }

                    dat.Data = Strings.Trim(etype + " " + elevel + " " + edata);
                }
                else
                {
                    // 効果名のみ
                    dat.Name = buf;
                }

                j = 1;
                foreach (AbilityEffect dat2 in colEffects)
                {
                    if ((dat.Name ?? "") == (dat2.Name ?? ""))
                    {
                        j = (j + 1);
                    }
                }

                if (j == 1)
                {
                    colEffects.Add(dat, dat.Name);
                }
                else
                {
                    colEffects.Add(dat, dat.Name + SrcFormatter.Format(j));
                }
            }
        }

        // 効果の総数
        public int CountEffect()
        {
            return colEffects.Count;
        }

        // 効果内容の解説
        // AbilityData.cls の EffectName。元の SRC と同じく、選択中のユニット（Commands.SelectedUnit）の
        // メインパイロットの技能で効果量を補正して表示する。
        public string EffectName(AbilityEffect ae)
        {
            var selectedUnit = SRC.Commands.SelectedUnit;
            var name = "";
            string cname, uname;
            var wclass = "";
            double flevel;
            double heal_lv = 0d, supply_lv = 0d;

            // 効果レベルが回復・増加量を意味するアビリティ用
            var elevel = ae.Level;
            // 効果レベルがターン数を意味するアビリティ及び召喚アビリティ用
            var elevel2 = elevel;

            if (selectedUnit != null && selectedUnit.CountPilot() > 0)
            {
                var p = selectedUnit.MainPilot();
                // 得意技
                if (p.IsSkillAvailable("得意技"))
                {
                    var buf = p.SkillData("得意技");
                    for (var i = 1; i <= Strings.Len(buf); i++)
                    {
                        if (GeneralLib.InStrNotNest(Class, GeneralLib.GetClassBundle(buf, ref i)) > 0)
                        {
                            elevel = 1.2d * elevel;
                            elevel2 = 1.4d * elevel2;
                            break;
                        }
                    }
                }

                // 不得手
                if (p.IsSkillAvailable("不得手"))
                {
                    var buf = p.SkillData("不得手");
                    for (var i = 1; i <= Strings.Len(buf); i++)
                    {
                        if (GeneralLib.InStrNotNest(Class, GeneralLib.GetClassBundle(buf, ref i)) > 0)
                        {
                            elevel = 0.8d * elevel;
                            elevel2 = 0.6d * elevel2;
                            break;
                        }
                    }
                }

                // 術アビリティの場合は魔力によって効果レベルが修正を受ける
                if (GeneralLib.InStrNotNest(Class, "術") > 0)
                {
                    elevel = elevel * p.Shooting / 100d;
                }
                else
                {
                    for (var i = 1; i <= GeneralLib.LLength(NecessarySkill); i++)
                    {
                        if (p.SkillType(GeneralLib.LIndex(NecessarySkill, i)) == "術")
                        {
                            elevel = elevel * p.Shooting / 100d;
                            break;
                        }
                    }
                }

                // 修理＆補給技能
                heal_lv = p.SkillLevel("修理");
                supply_lv = p.SkillLevel("補給");
            }

            // アビリティの効果は最低でも１ターン持続
            if (elevel2 != 0d)
            {
                elevel2 = Math.Max(elevel2, 1d);
            }

            switch (ae.Name ?? "")
            {
                case "回復":
                    name = Expression.Term("ＨＰ") + "を";
                    if (elevel > 0d)
                    {
                        name += SrcFormatter.Format(VBMath.IntDiv(500d * elevel * (10d + heal_lv), 10)) + "回復";
                    }
                    else if (elevel < 0d)
                    {
                        name += SrcFormatter.Format(VBMath.CLng(-500d * elevel)) + "減少";
                    }
                    break;

                case "補給":
                    name = Expression.Term("ＥＮ") + "を";
                    if (elevel > 0d)
                    {
                        name += SrcFormatter.Format(VBMath.IntDiv(50d * elevel * (10d + supply_lv), 10)) + "回復";
                    }
                    else if (elevel < 0d)
                    {
                        name += SrcFormatter.Format(VBMath.CLng(-50d * elevel)) + "減少";
                    }
                    break;

                case "霊力回復":
                case "プラーナ回復":
                    if (selectedUnit != null && selectedUnit.CountPilot() > 0)
                    {
                        name = selectedUnit.MainPilot().SkillName0("霊力");
                    }
                    else
                    {
                        name = "霊力";
                    }
                    if (elevel > 0d)
                    {
                        name += "を" + SrcFormatter.Format(VBMath.CLng(10d * elevel)) + "回復";
                    }
                    else if (elevel < 0d)
                    {
                        name += "を" + SrcFormatter.Format(VBMath.CLng(-10d * elevel)) + "減少";
                    }
                    break;

                case "ＳＰ回復":
                    name = Expression.Term("ＳＰ") + "を";
                    if (elevel > 0d)
                    {
                        name += SrcFormatter.Format(VBMath.CLng(10d * elevel)) + "回復";
                    }
                    else if (elevel < 0d)
                    {
                        name += SrcFormatter.Format(VBMath.CLng(-10d * elevel)) + "減少";
                    }
                    break;

                case "気力増加":
                    name = Expression.Term("気力") + "を";
                    if (elevel > 0d)
                    {
                        name += SrcFormatter.Format(VBMath.CLng(10d * elevel)) + "増加";
                    }
                    else if (elevel < 0d)
                    {
                        name += SrcFormatter.Format(VBMath.CLng(-10d * elevel)) + "減少";
                    }
                    break;

                case "装填":
                    if (Strings.Len(ae.Data) == 0)
                    {
                        name = "武器の弾数を回復";
                    }
                    else
                    {
                        if (selectedUnit != null && selectedUnit.Weapons.Any(x => x.WeaponNickname() == ae.Data))
                        {
                            return ae.Data + "の弾数を回復";
                        }
                        name = ae.Data + "属性を持つ武器の弾数を回復";
                    }
                    break;

                case "治癒":
                    for (var i = 1; i <= GeneralLib.LLength(ae.Data); i++)
                    {
                        name = name + " " + ConditionTerm(GeneralLib.LIndex(ae.Data, i));
                    }
                    name = Strings.Trim(name);
                    if (Strings.Len(name) > 0)
                    {
                        name += "を回復";
                    }
                    else
                    {
                        name = "状態回復";
                    }
                    break;

                case "状態":
                    cname = ConditionTerm(GeneralLib.LIndex(ae.Data, 1));
                    if (0d < elevel2 && elevel2 <= 10d)
                    {
                        name = cname + "(" + SrcFormatter.Format(VBMath.CLng(elevel2)) + "ターン)";
                    }
                    else
                    {
                        name = cname;
                    }
                    break;

                case "付加":
                    switch (GeneralLib.LIndex(ae.Data, 1) ?? "")
                    {
                        case "耐性":
                            name = AttributeNameOrAttack(selectedUnit, GeneralLib.LIndex(ae.Data, 3)) + "のダメージを半減";
                            break;
                        case "無効化":
                            name = AttributeNameOrAttack(selectedUnit, GeneralLib.LIndex(ae.Data, 3)) + "を無効化";
                            break;
                        case "特殊効果無効化":
                            name = AttributeNameOrAttack(selectedUnit, GeneralLib.LIndex(ae.Data, 3)) + "の特殊効果を無効化";
                            break;
                        case "吸収":
                            name = AttributeNameOrAttack(selectedUnit, GeneralLib.LIndex(ae.Data, 3)) + "を吸収";
                            break;
                        case "追加パイロット":
                            name = "パイロット変化";
                            break;
                        case "追加サポート":
                            name = "サポート追加";
                            break;
                        case "性格変更":
                            name = "パイロットの性格を" + GeneralLib.LIndex(ae.Data, 3) + "に変更";
                            break;
                        case "ＢＧＭ":
                            name = "ＢＧＭ変更";
                            break;
                        case "攻撃属性":
                            for (var i = 4; i <= GeneralLib.LLength(ae.Data); i++)
                            {
                                if (Strings.InStr(GeneralLib.LIndex(ae.Data, i), "!") == 0)
                                {
                                    wclass += GeneralLib.LIndex(ae.Data, i);
                                }
                            }
                            name = WeaponType(wclass) + "の属性に" + GeneralLib.LIndex(ae.Data, 3) + "を追加";
                            break;
                        case "武器強化":
                            wclass = WeaponClassFrom(ae.Data);
                            flevel = GeneralLib.StrToDbl(GeneralLib.LIndex(ae.Data, 2));
                            name = WeaponType(wclass) + "の攻撃力を" + (flevel >= 0d ? "+" : "") + SrcFormatter.Format(100d * flevel);
                            break;
                        case "命中率強化":
                            wclass = WeaponClassFrom(ae.Data);
                            flevel = GeneralLib.StrToDbl(GeneralLib.LIndex(ae.Data, 2));
                            name = WeaponType(wclass) + "の命中率を" + (flevel >= 0d ? "+" : "") + SrcFormatter.Format(5d * flevel);
                            break;
                        case "ＣＴ率強化":
                        case "特殊効果発動率強化":
                            wclass = WeaponClassFrom(ae.Data);
                            flevel = GeneralLib.StrToDbl(GeneralLib.LIndex(ae.Data, 2));
                            name = WeaponType(wclass) + "のＣＴ率を" + (flevel >= 0d ? "+" : "") + SrcFormatter.Format(5d * flevel);
                            break;
                        case "射程延長":
                            wclass = WeaponClassFrom(ae.Data);
                            flevel = GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 2));
                            name = WeaponType(wclass) + "の射程を" + (flevel >= 0d ? "+" : "") + SrcFormatter.Format(flevel);
                            break;
                        case "サイズ変更":
                            name = "サイズが" + GeneralLib.LIndex(ae.Data, 3) + "に変化";
                            break;
                        case "地形適応変更":
                            if (GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 3)) > 0)
                            {
                                name = "空への適応を強化";
                            }
                            else if (GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 4)) > 0)
                            {
                                name = "陸への適応を強化";
                            }
                            else if (GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 5)) > 0)
                            {
                                name = "水中への適応を強化";
                            }
                            else if (GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 6)) > 0)
                            {
                                name = "宇宙への適応を強化";
                            }
                            break;
                        case "地形適応固定変更":
                            {
                                // 元の SRC と同じく、「強制」かどうかは常に 6 番目の要素で判定する
                                var force = GeneralLib.LIndex(ae.Data, 6) == "強制" ? "強制的に" : "";
                                var areas = new[] { "空", "陸", "水中", "宇宙" };
                                for (var i = 0; i < areas.Length; i++)
                                {
                                    var lv = GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, i + 3));
                                    if (lv <= 5 && lv >= 0)
                                    {
                                        name = areas[i] + "への適応を" + force + "変化";
                                        break;
                                    }
                                }
                                break;
                            }
                        case "Ｖ－ＵＰ":
                            switch (GeneralLib.LIndex(ae.Data, 3) ?? "")
                            {
                                case "武器":
                                    name = "武器攻撃力を強化";
                                    break;
                                case "ユニット":
                                    name = "各パラメータを強化";
                                    break;
                                default:
                                    name = "ユニットを強化";
                                    break;
                            }
                            break;
                        case "格闘武器":
                        case "迎撃武器":
                        case "制限時間":
                            name += "付加";
                            break;
                        case "パイロット愛称":
                        case "パイロット画像":
                        case "愛称変更":
                        case "ユニット画像":
                            name = "";
                            break;
                        default:
                            {
                                name = GeneralLib.ListIndex(ae.Data, 3);
                                if (Strings.Left(name, 1) == "\"")
                                {
                                    name = GeneralLib.ListIndex(Strings.Mid(name, 2, Strings.Len(name) - 2), 1);
                                }
                                var lv = GeneralLib.LIndex(ae.Data, 2) != SrcFormatter.Format(Constants.DEFAULT_LEVEL)
                                    && GeneralLib.LLength(ae.Data) <= 3
                                    ? "Lv" + GeneralLib.LIndex(ae.Data, 2)
                                    : "";
                                if (string.IsNullOrEmpty(name) || name == "非表示")
                                {
                                    name = GeneralLib.LIndex(ae.Data, 1) + lv + "付加";
                                }
                                else
                                {
                                    name = name + lv + "付加";
                                }
                                break;
                            }
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (0d < elevel2 && elevel2 <= 10d)
                        {
                            name += "(" + SrcFormatter.Format(VBMath.CLng(elevel2)) + "ターン)";
                        }
                    }
                    break;

                case "強化":
                    name = GeneralLib.ListIndex(ae.Data, 3);
                    if (string.IsNullOrEmpty(name) || name == "非表示")
                    {
                        if (GeneralLib.StrToLng(GeneralLib.LIndex(ae.Data, 2)) > 0)
                        {
                            name = GeneralLib.LIndex(ae.Data, 1) + "Lv" + GeneralLib.LIndex(ae.Data, 2);
                        }
                        else
                        {
                            name = GeneralLib.LIndex(ae.Data, 1);
                        }
                    }
                    if (0d < elevel2 && elevel2 <= 10d)
                    {
                        name += "強化(" + SrcFormatter.Format(VBMath.CLng(elevel2)) + "ターン)";
                    }
                    break;

                case "召喚":
                    if (!SRC.UDList.IsDefined(ae.Data))
                    {
                        SRC.GUI.ErrorMessage("召喚ユニット「" + ae.Data + "」が定義されていません");
                        return "";
                    }
                    if (elevel2 > 1d)
                    {
                        name = SRC.UDList.Item(ae.Data).Nickname + "を"
                            + Strings.StrConv(SrcFormatter.Format(VBMath.CLng(elevel2)), VbStrConv.Wide) + "体召喚";
                    }
                    else
                    {
                        name = SRC.UDList.Item(ae.Data).Nickname + "を召喚";
                    }
                    break;

                case "変身":
                    uname = GeneralLib.LIndex(ae.Data, 1);
                    if (!SRC.UDList.IsDefined(uname))
                    {
                        SRC.GUI.ErrorMessage("変身先のデータ「" + uname + "」が定義されていません");
                        return "";
                    }
                    name = SRC.UDList.Item(uname).Nickname + "に変身";
                    if (0d < elevel2 && elevel2 <= 10d)
                    {
                        name += "(" + SrcFormatter.Format(VBMath.CLng(elevel2)) + "ターン)";
                    }
                    break;

                case "能力コピー":
                    name = "任意の味方ユニットに変身";
                    if (0d < elevel2 && elevel2 <= 10d)
                    {
                        name += "(" + SrcFormatter.Format(VBMath.CLng(elevel2)) + "ターン)";
                    }
                    break;

                case "再行動":
                    name = MaxRange != 0 ? "行動済みユニットを再行動" : "行動非消費";
                    break;

                case "解説":
                    name = ae.Data;
                    break;
            }

            return name ?? "";
        }

        // 状態名の用語を置き換える（EffectName の「治癒」「状態」）
        private string ConditionTerm(string cname)
        {
            switch (cname ?? "")
            {
                case "装甲劣化":
                    return Expression.Term("装甲") + "劣化";
                case "運動性ＵＰ":
                    return Expression.Term("運動性") + "ＵＰ";
                case "運動性ＤＯＷＮ":
                    return Expression.Term("運動性") + "ＤＯＷＮ";
                case "移動力ＵＰ":
                    return Expression.Term("移動力") + "ＵＰ";
                case "移動力ＤＯＷＮ":
                    return Expression.Term("移動力") + "ＤＯＷＮ";
                default:
                    return cname;
            }
        }

        // 属性名。属性名が無ければ「○○攻撃」とする（EffectName の「付加」の耐性など）
        private string AttributeNameOrAttack(Units.Unit u, string atr)
        {
            var aname = SRC.Help.AttributeName(u, atr);
            if (string.IsNullOrEmpty(aname))
            {
                aname = atr + "攻撃";
            }
            return aname;
        }

        // 武器強化系の付加で対象にする武器の属性（3 番目以降の要素から「!」の付いたものを除く）
        private static string WeaponClassFrom(string data)
        {
            var wclass = "";
            for (var i = 3; i <= GeneralLib.LLength(data); i++)
            {
                if (Strings.InStr(GeneralLib.LIndex(data, i), "!") == 0)
                {
                    wclass += GeneralLib.LIndex(data, i);
                }
            }
            return wclass;
        }

        // 付加する武器強化系能力の対象表示用に武器の種類を判定
        private string WeaponType(string wclass)
        {
            if (wclass == "全" || string.IsNullOrEmpty(wclass))
            {
                return "武器";
            }
            else if (Strings.Len(wclass) == 1)
            {
                return SRC.Help.AttributeName(null, wclass);
            }
            else if (Strings.Right(wclass, 2) == "装備")
            {
                return Strings.Left(wclass, Strings.Len(wclass) - 2);
            }
            else
            {
                return wclass + "属性攻撃";
            }
        }

        // 使い捨てアイテムによるアビリティかどうかを返す
        public bool IsItem()
        {
            bool IsItemRet = default;
            int i;
            var loopTo = GeneralLib.LLength(NecessarySkill);
            for (i = 1; i <= loopTo; i++)
            {
                if (GeneralLib.LIndex(NecessarySkill, i) == "アイテム")
                {
                    IsItemRet = true;
                    return IsItemRet;
                }
            }

            return IsItemRet;
        }
    }
}
