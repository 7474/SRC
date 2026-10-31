using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Units
{
    // === 特殊状態関連処理 ===
    public partial class Unit
    {
        // 特殊状態を付加
        public void AddCondition(string cname, int ltime, double clevel = Constants.DEFAULT_LEVEL, string cdata = "")
        {
            var new_condition = new Condition();

            // 同じ特殊状態が既に付加されている？
            foreach (Condition cnd in colCondition.List)
            {
                if ((cnd.Name ?? "") == (cname ?? ""))
                {
                    if (cnd.Lifetime < 0 || ltime < 0)
                    {
                        cnd.Lifetime = -1;
                    }
                    else
                    {
                        cnd.Lifetime = GeneralLib.MaxLng(cnd.Lifetime, ltime);
                    }

                    cnd.Name = cname;
                    cnd.Level = clevel;
                    cnd.StrData = cdata;
                    return;
                }
            }

            // 特殊状態を付加
            new_condition.Name = cname;
            new_condition.Lifetime = ltime;
            new_condition.Level = clevel;
            new_condition.StrData = cdata;
            colCondition.Add(new_condition, cname);
        }

        // 特殊状態を削除
        public void DeleteCondition(string Index)
        {
            {
                var withBlock = colCondition[Index];
                colCondition.Remove(Index);

                // 特殊能力付加の場合はユニットのステータスをアップデート
                if (Strings.Right(Conversions.ToString(withBlock.Name), 2) == "付加"
                    && Strings.InStr(Conversions.ToString(withBlock.StrData), "パイロット能力付加") == 0)
                {
                    Update();
                }
            }
        }

        public void DeleteCondition0(string Index)
        {
            colCondition.Remove(Index);
        }

        public void ClearCondition()
        {
            colCondition.Clear();
        }

        // 付加された特殊状態の総数
        public int CountCondition()
        {
            return colCondition.Count;
        }

        // 特殊状態
        public Condition Condition(string Index)
        {
            //return colCondition[Index].Name;
            return colCondition[Index];
        }

        // 特殊状態の残りターン数
        public int ConditionLifetime(string Index)
        {
            return colCondition[Index]?.Lifetime ?? -1;
        }

        // 指定した特殊能力が付加されているか？
        public bool IsConditionSatisfied(string Index)
        {
            return colCondition[Index] != null;
        }

        // 特殊状態のレベル
        public double ConditionLevel(string Index)
        {
            return Condition(Index)?.Level ?? 0d;
        }

        //// 特殊状態のレベルの変更
        //public void SetConditionLevel(string Index, double lv)
        //{
        //    ;
        //    // UPGRADE_WARNING: オブジェクト colCondition.Item().Level の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //    colCondition[Index].Level = lv;
        //ErrorHandler:
        //    ;
        //}

        // 特殊状態のデータ
        public string ConditionData(string Index)
        {
            return colCondition[Index]?.StrData ?? "";
        }

        // 特殊能力の残りターン数を更新
        public void UpdateCondition(bool decrement_lifetime = false)
        {
            var update_is_necessary = default(bool);
            var charge_complete = default(bool);
            foreach (Condition cnd in colCondition.List.CloneList())
            {
                if (decrement_lifetime)
                {
                    // 残りターン数を1減らす
                    if (cnd.Lifetime > 0)
                    {
                        cnd.Lifetime = (cnd.Lifetime - 1);
                    }
                }

                if (cnd.Lifetime == 0)
                {
                    // 残りターン数が0なら削除
                    colCondition.Remove(cnd.Name);
                    switch (cnd.Name ?? "")
                    {
                        case "魅了":
                            {
                                // 魅了を解除
                                if (Master is object)
                                {
                                    Master.CurrentForm().DeleteSlave(ID);
                                    // UPGRADE_NOTE: オブジェクト Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                    Master = null;
                                }

                                Mode = "通常";
                                break;
                            }

                        case "チャージ":
                            {
                                // チャージ完了
                                charge_complete = true;
                                break;
                            }

                        case "活動限界":
                            {
                                // 活動限界時間切れ
                                GUI.Center(x, y);
                                Escape();
                                GUI.OpenMessageForm(u1: null, u2: null);
                                GUI.DisplaySysMessage(Nickname + "は強制的に退却させられた。");
                                GUI.CloseMessageForm();
                                Event.HandleEvent("破壊", MainPilot().ID);
                                break;
                            }

                        default:
                            {
                                // 特殊能力付加を解除
                                if (Strings.Right(cnd.Name, 2) == "付加" || Strings.Right(cnd.Name, 2) == "強化")
                                {
                                    update_is_necessary = true;
                                }

                                break;
                            }
                    }
                }
            }

            // チャージ状態が終了したらチャージ完了状態にする
            if (charge_complete)
            {
                AddCondition("チャージ完了", 1, cdata: "");
            }

            // ユニットのステータス変化あり？
            if (update_is_necessary)
            {
                Update();
            }
        }

        // ステータス画面に表示する特殊ステータスの行を返す
        // VB 版 Status.bas の DisplayUnitStatus にある「ユニットにかかっている特殊ステータス」に合わせる。
        public IList<string> ConditionStatusLines()
        {
            var lines = new List<string>();
            var nameList = new List<string>();
            // 残りターン数の表記
            string Rest(int lifetime, int limit)
            {
                return (0 < lifetime && lifetime < limit) ? " 残り" + SrcFormatter.Format(lifetime) + "ターン" : "";
            }
            string Opt(string data)
            {
                return !string.IsNullOrEmpty(data) ? "(" + data + ")" : "";
            }

            foreach (var cnd in Conditions)
            {
                var cname = cnd.Name ?? "";
                var cdata = cnd.StrData ?? "";
                var level = SrcFormatter.Format(cnd.Level);

                // 時間切れ？
                if (cnd.Lifetime == 0)
                {
                    continue;
                }
                // 非表示？
                if (Strings.InStr(cdata, "非表示") > 0)
                {
                    continue;
                }
                // 解説？
                if (GeneralLib.LIndex(cdata, 1) == "解説")
                {
                    continue;
                }

                switch (cname)
                {
                    case "データ不明":
                    case "形態固定":
                    case "機体固定":
                    case "不死身":
                    case "無敵":
                    case "識別済み":
                    case "非操作":
                    case "破壊キャンセル":
                    case "盾ダメージ":
                    case "能力コピー":
                    case "メッセージ付加":
                    case "ノーマルモード付加":
                    case "追加パイロット付加":
                    case "追加サポート付加":
                    case "パイロット愛称付加":
                    case "パイロット画像付加":
                    case "性格変更付加":
                    case "性別付加":
                    case "ＢＧＭ付加":
                    case "愛称変更付加":
                    case "スペシャルパワー無効化":
                    case "精神コマンド無効化":
                    case "ユニット画像付加":
                        // 非表示
                        break;
                    case "残り時間":
                        if (0 < cnd.Lifetime && cnd.Lifetime < 100)
                        {
                            lines.Add("残り時間" + SrcFormatter.Format(cnd.Lifetime) + "ターン");
                        }
                        break;
                    case "無効化付加":
                    case "耐性付加":
                    case "吸収付加":
                    case "弱点付加":
                        // VB 版でもここだけ「 NT」の表記になっている
                        lines.Add(cdata + cname
                            + ((0 < cnd.Lifetime && cnd.Lifetime < 100) ? " " + SrcFormatter.Format(cnd.Lifetime) + "T" : ""));
                        break;
                    case "特殊効果無効化付加":
                        lines.Add(cdata + "無効化付加" + Rest(cnd.Lifetime, 100));
                        break;
                    case "攻撃属性付加":
                        lines.Add(GeneralLib.LIndex(cdata, 1) + "属性付加" + Rest(cnd.Lifetime, 100));
                        break;
                    case "武器強化付加":
                    case "命中率強化付加":
                    case "ＣＴ率強化付加":
                    case "特殊効果発動率強化付加":
                        lines.Add(Strings.Left(cname, Strings.Len(cname) - 2) + "Lv" + level + "付加"
                            + Opt(cdata) + Rest(cnd.Lifetime, 100));
                        break;
                    case "地形適応変更付加":
                        lines.Add("地形適応変更付加" + Rest(cnd.Lifetime, 100));
                        break;
                    case "盾付加":
                        lines.Add(GeneralLib.LIndex(cdata, 1) + "付加" + "(" + level + ")" + Rest(cnd.Lifetime, 100));
                        break;
                    case "ダミー破壊":
                        {
                            var buf = FeatureName("ダミー");
                            if (Strings.InStr(buf, "Lv") > 0)
                            {
                                buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
                            }
                            lines.Add(buf + Strings.StrConv(level, VbStrConv.Wide) + "体破壊");
                            break;
                        }
                    case "ダミー付加":
                        lines.Add(FeatureName("ダミー") + "残り" + Strings.StrConv(level, VbStrConv.Wide) + "体"
                            + Rest(cnd.Lifetime, 100));
                        break;
                    case "バリア発動":
                    case "フィールド発動":
                        // 残りターン数は常に表示する
                        lines.Add((!string.IsNullOrEmpty(cdata) ? cdata : cname)
                            + " 残り" + SrcFormatter.Format(cnd.Lifetime) + "ターン");
                        break;
                    case "装甲劣化":
                        lines.Add(Expression.Term("装甲", this) + "劣化" + Rest(cnd.Lifetime, 20));
                        break;
                    case "運動性ＵＰ":
                    case "運動性ＤＯＷＮ":
                    case "移動力ＵＰ":
                    case "移動力ＤＯＷＮ":
                        lines.Add(Expression.Term(Strings.Left(cname, 3), this) + Strings.Mid(cname, 4) + Rest(cnd.Lifetime, 20));
                        break;
                    default:
                        {
                            // 充填中？
                            if (Strings.Right(cname, 3) == "充填中")
                            {
                                // 残りターン数は常に表示する
                                lines.Add((IsHero() ? Strings.Left(cname, Strings.Len(cname) - 3) + "準備中" : cname)
                                    + " 残り" + SrcFormatter.Format(cnd.Lifetime) + "ターン");
                                break;
                            }

                            // パイロット特殊能力付加＆強化による状態は表示しない
                            if (Strings.Right(cname, 3) == "付加２" || Strings.Right(cname, 3) == "強化２")
                            {
                                break;
                            }

                            string buf;
                            if (Strings.Right(cname, 2) == "付加" && !string.IsNullOrEmpty(cdata))
                            {
                                buf = GeneralLib.LIndex(cdata, 1) + "付加";
                            }
                            else if (Strings.Right(cname, 2) == "強化" && !string.IsNullOrEmpty(cdata))
                            {
                                // 強化アビリティ
                                buf = GeneralLib.LIndex(cdata, 1) + "強化Lv" + level;
                            }
                            else if (cnd.Level > 0d)
                            {
                                // 付加アビリティ(レベル指定あり)
                                buf = Strings.Left(cname, Strings.Len(cname) - 2) + "Lv" + level + "付加";
                            }
                            else
                            {
                                // 付加アビリティ(レベル指定なし)
                                buf = cname;
                            }

                            // エリアスされた特殊能力の付加表示がだぶらないように
                            if (nameList.Contains(buf))
                            {
                                break;
                            }
                            nameList.Add(buf);

                            lines.Add(buf + Rest(cnd.Lifetime, 20));
                            break;
                        }
                }
            }

            return lines;
        }
    }
}
