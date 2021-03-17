using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Units
{
    // === ユニット用特殊能力関連処理 ===
    public partial class Unit
    {
        public IList<Models.FeatureData> Features => colFeature.List;

        //        // 特殊能力の総数
        //        public int CountFeature()
        //        {
        //            int CountFeatureRet = default;
        //            CountFeatureRet = colFeature.Count;
        //            return CountFeatureRet;
        //        }

        //        // 特殊能力
        //        public string Feature(object Index)
        //        {
        //            string FeatureRet = default;
        //            FeatureData fd;
        //            fd = (FeatureData)colFeature[Index];
        //            FeatureRet = fd.Name;
        //            return FeatureRet;
        //        }

        //        // 特殊能力の名称
        //        public string FeatureName(object Index)
        //        {
        //            string FeatureNameRet = default;
        //            FeatureNameRet = FeatureNameInt(Index, colFeature);
        //            return FeatureNameRet;
        //        }

        //        private string FeatureNameInt(object Index, Collection feature_list)
        //        {
        //            string FeatureNameIntRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 30194


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)feature_list[Index];
        //            // 非表示の能力
        //            switch (fd.Name ?? "")
        //            {
        //                case "ノーマルモード":
        //                case "パーツ合体":
        //                case "換装":
        //                case "制限時間":
        //                case "制御不可":
        //                case "主形態":
        //                case "他形態":
        //                case "合体制限":
        //                case "格闘武器":
        //                case "迎撃武器":
        //                case "合体技":
        //                case "変形技":
        //                case "ランクアップ":
        //                case "追加パイロット":
        //                case "暴走時パイロット":
        //                case "追加サポート":
        //                case "装備個所":
        //                case "ハードポイント":
        //                case "武器クラス":
        //                case "防具クラス":
        //                case "ＢＧＭ":
        //                case "武器ＢＧＭ":
        //                case "アビリティＢＧＭ":
        //                case "合体ＢＧＭ":
        //                case "分離ＢＧＭ":
        //                case "変形ＢＧＭ":
        //                case "ハイパーモードＢＧＭ":
        //                case "ユニット画像":
        //                case "パイロット画像":
        //                case "パイロット愛称":
        //                case "パイロット読み仮名":
        //                case "性別":
        //                case "性格変更":
        //                case "吸収":
        //                case "無効化":
        //                case "耐性":
        //                case "弱点":
        //                case "有効":
        //                case "特殊効果無効化":
        //                case "アイテム所有":
        //                case "レアアイテム所有":
        //                case "ラーニング可能技":
        //                case "改造費修正":
        //                case "最大改造数":
        //                case "パイロット能力付加":
        //                case "パイロット能力強化":
        //                case "非表示":
        //                case "攻撃属性":
        //                case "射程延長":
        //                case "武器強化":
        //                case "命中率強化":
        //                case "ＣＴ率強化":
        //                case "特殊効果発動率強化":
        //                case "必要技能":
        //                case "不必要技能":
        //                case "ダミーユニット":
        //                case "地形ユニット":
        //                case "召喚解除コマンド名":
        //                case "変身解除コマンド名":
        //                case "１人乗り可能":
        //                case "特殊効果":
        //                case "戦闘アニメ":
        //                case "パイロット地形適応変更":
        //                case "メッセージクラス":
        //                case "用語名":
        //                case "発光":
        //                    {
        //                        // ユニット用特殊能力
        //                        FeatureNameIntRet = "";
        //                        return FeatureNameIntRet;
        //                    }

        //                case "愛称変更":
        //                case "読み仮名変更":
        //                case "サイズ変更":
        //                case "地形適応変更":
        //                case "地形適応固定変更":
        //                case "空中移動":
        //                case "陸上移動":
        //                case "水中移動":
        //                case "宇宙移動":
        //                case "地中移動":
        //                case "修理費修正":
        //                case "経験値修正":
        //                case "最大弾数増加":
        //                case "ＥＮ消費減少":
        //                case "Ｖ－ＵＰ":
        //                case "大型アイテム":
        //                case "呪い":
        //                    {
        //                        // アイテム用特殊能力
        //                        FeatureNameIntRet = "";
        //                        return FeatureNameIntRet;
        //                    }
        //            }

        //            // ADD START MARGE
        //            // 拡大画像能力は「拡大画像(文字列)」といった指定もあるので他の非表示能力と異なる
        //            // 判定方法を使う
        //            if (Strings.InStr(fd.Name, "拡大画像") == 1)
        //            {
        //                FeatureNameIntRet = "";
        //                return FeatureNameIntRet;
        //            }
        //            // ADD END MARGE

        //            if (Strings.Len(fd.StrData) > 0)
        //            {
        //                // 別名の指定あり
        //                FeatureNameIntRet = GeneralLib.ListIndex(fd.StrData, 1);
        //                if (FeatureNameIntRet == "非表示" | FeatureNameIntRet == "解説")
        //                {
        //                    FeatureNameIntRet = "";
        //                }
        //            }
        //            else if (fd.Level == Constants.DEFAULT_LEVEL)
        //            {
        //                // レベル指定なし
        //                FeatureNameIntRet = fd.Name;
        //            }
        //            else if (fd.Level >= 0d)
        //            {
        //                // レベル指定あり
        //                FeatureNameIntRet = fd.Name + "Lv" + SrcFormatter.Format(fd.Level);
        //                if (fd.Name == "射撃強化")
        //                {
        //                    if (CountPilot() > 0)
        //                    {
        //                        if (MainPilot().HasMana())
        //                        {
        //                            FeatureNameIntRet = "魔力強化Lv" + SrcFormatter.Format(fd.Level);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // マイナスのレベル指定
        //                switch (fd.Name ?? "")
        //                {
        //                    case "格闘強化":
        //                        {
        //                            FeatureNameIntRet = "格闘低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
        //                            break;
        //                        }

        //                    case "射撃強化":
        //                        {
        //                            FeatureNameIntRet = "射撃低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
        //                            if (CountPilot() > 0)
        //                            {
        //                                if (MainPilot().HasMana())
        //                                {
        //                                    FeatureNameIntRet = "魔力低下Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
        //                                }
        //                            }

        //                            break;
        //                        }

        //                    case "命中強化":
        //                        {
        //                            FeatureNameIntRet = "命中低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
        //                            break;
        //                        }

        //                    case "回避強化":
        //                        {
        //                            FeatureNameIntRet = "回避低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
        //                            break;
        //                        }

        //                    case "技量強化":
        //                        {
        //                            FeatureNameIntRet = "技量低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
        //                            break;
        //                        }

        //                    case "反応強化":
        //                        {
        //                            FeatureNameIntRet = "反応低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            FeatureNameIntRet = fd.Name + "Lv" + SrcFormatter.Format(fd.Level);
        //                            break;
        //                        }
        //                }
        //            }

        //            return FeatureNameIntRet;
        //        ErrorHandler:
        //            ;

        //            // 見つからなかった場合
        //            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //            FeatureNameIntRet = Conversions.ToString(Index);
        //        }

        //        public string FeatureName0(object Index)
        //        {
        //            string FeatureName0Ret = default;
        //            FeatureName0Ret = FeatureName(Index);
        //            if (Strings.InStr(FeatureName0Ret, "Lv") > 0)
        //            {
        //                FeatureName0Ret = Strings.Left(FeatureName0Ret, Strings.InStr(FeatureName0Ret, "Lv") - 1);
        //            }

        //            return FeatureName0Ret;
        //        }

        // 特殊能力のレベル
        public double FeatureLevel(string Index)
        {
            var level = colFeature[Index]?.Level ?? 0d;
            return level == Constants.DEFAULT_LEVEL ? 1d : level;
        }

        // 特殊能力のデータ
        public string FeatureData(string Index)
        {
            return colFeature[Index]?.StrData ?? "";
        }

        //        // 特殊能力の必要技能
        //        public string FeatureNecessarySkill(object Index)
        //        {
        //            string FeatureNecessarySkillRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 34947


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)colFeature[Index];
        //            FeatureNecessarySkillRet = fd.NecessarySkill;
        //            return FeatureNecessarySkillRet;
        //        ErrorHandler:
        //            ;
        //            FeatureNecessarySkillRet = "";
        //        }

        // 指定した特殊能力を所有しているか？
        public bool IsFeatureAvailable(string fname)
        {
            return colFeature.ContainsKey(fname);
        }

        // 特殊能力にレベル指定がされている？
        public bool IsFeatureLevelSpecified(string Index)
        {
            return (colFeature[Index]?.Level ?? Constants.DEFAULT_LEVEL) != Constants.DEFAULT_LEVEL;
        }

        //        // 特殊能力の総数(必要条件を満たさないものを含む)
        //        public int CountAllFeature()
        //        {
        //            int CountAllFeatureRet = default;
        //            CountAllFeatureRet = colAllFeature.Count;
        //            return CountAllFeatureRet;
        //        }

        //        // 特殊能力(必要条件を満たさないものを含む)
        //        public string AllFeature(object Index)
        //        {
        //            string AllFeatureRet = default;
        //            FeatureData fd;
        //            fd = (FeatureData)colAllFeature[Index];
        //            AllFeatureRet = fd.Name;
        //            return AllFeatureRet;
        //        }

        //        // 特殊能力の名称(必要条件を満たさないものを含む)
        //        public string AllFeatureName(object Index)
        //        {
        //            string AllFeatureNameRet = default;
        //            AllFeatureNameRet = FeatureNameInt(Index, colAllFeature);
        //            return AllFeatureNameRet;
        //        }

        //        // 特殊能力のレベル(必要条件を満たさないものを含む)
        //        public double AllFeatureLevel(object Index)
        //        {
        //            double AllFeatureLevelRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 36565


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)colAllFeature[Index];
        //            AllFeatureLevelRet = fd.Level;
        //            if (fd.Level == Constants.DEFAULT_LEVEL)
        //            {
        //                AllFeatureLevelRet = 1d;
        //            }

        //            return AllFeatureLevelRet;
        //        ErrorHandler:
        //            ;
        //            AllFeatureLevelRet = 0d;
        //        }

        //        // 特殊能力のレベルが指定されているか(必要条件を満たさないものを含む)
        //        public bool AllFeatureLevelSpecified(object Index)
        //        {
        //            bool AllFeatureLevelSpecifiedRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 37017


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)colAllFeature[Index];
        //            if (fd.Level != Constants.DEFAULT_LEVEL)
        //            {
        //                AllFeatureLevelSpecifiedRet = true;
        //            }

        //            return AllFeatureLevelSpecifiedRet;
        //        ErrorHandler:
        //            ;
        //            AllFeatureLevelSpecifiedRet = false;
        //        }

        //        // 特殊能力のデータ(必要条件を満たさないものを含む)
        //        public string AllFeatureData(object Index)
        //        {
        //            string AllFeatureDataRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 37422


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)colAllFeature[Index];
        //            AllFeatureDataRet = fd.StrData;
        //            return AllFeatureDataRet;
        //        ErrorHandler:
        //            ;
        //            AllFeatureDataRet = "";
        //        }

        //        // 特殊能力にレベル指定がされている？(必要条件を満たさないものを含む)
        //        public bool IsAllFeatureLevelSpecified(object Index)
        //        {
        //            bool IsAllFeatureLevelSpecifiedRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 37760


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)colAllFeature[Index];
        //            if (fd.Level == Constants.DEFAULT_LEVEL)
        //            {
        //                IsAllFeatureLevelSpecifiedRet = false;
        //            }
        //            else
        //            {
        //                IsAllFeatureLevelSpecifiedRet = true;
        //            }

        //            return IsAllFeatureLevelSpecifiedRet;
        //        ErrorHandler:
        //            ;
        //            IsAllFeatureLevelSpecifiedRet = false;
        //        }

        //        // 特殊能力が必要条件を満たしているか
        //        public bool IsFeatureActivated(object Index)
        //        {
        //            bool IsFeatureActivatedRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 38217


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)colAllFeature[Index];
        //            foreach (FeatureData fd2 in colFeature)
        //            {
        //                if (ReferenceEquals(fd, fd2))
        //                {
        //                    IsFeatureActivatedRet = true;
        //                    return IsFeatureActivatedRet;
        //                }
        //            }

        //            IsFeatureActivatedRet = false;
        //            return IsFeatureActivatedRet;
        //        ErrorHandler:
        //            ;
        //            IsFeatureActivatedRet = false;
        //        }

        //        // 特殊能力を登録
        //        private void AddFeatures(Collection fdc, bool is_item = false)
        //        {
        //            if (fdc is null)
        //            {
        //                return;
        //            }

        //            foreach (FeatureData fd in fdc)
        //            {
        //                // アイテムで指定された下記の能力はアイテムそのものの属性なので
        //                // ユニット側には追加しない
        //                if (is_item)
        //                {
        //                    switch (fd.Name ?? "")
        //                    {
        //                        case "必要技能":
        //                        case "不必要技能":
        //                        case "表示":
        //                        case "非表示":
        //                        case "呪い":
        //                            {
        //                                goto NextFeature;
        //                                break;
        //                            }
        //                    }
        //                }

        //                // 封印されている？
        //                bool localIsDisabled() { string argfname = GeneralLib.LIndex(fd.StrData, 1); var ret = IsDisabled(argfname); return ret; }

        //                if (IsDisabled(fd.Name) | localIsDisabled())
        //                {
        //                    goto NextFeature;
        //                }

        //                // 既にその能力が登録されている？
        //                if (!IsFeatureRegistered(fd.Name))
        //                {
        //                    colFeature.Add(fd, fd.Name);
        //                }
        //                else
        //                {
        //                    colFeature.Add(fd, fd.Name + ":" + SrcFormatter.Format(colFeature.Count));
        //                }

        //            NextFeature:
        //                ;
        //            }
        //        }

        //        // 特殊能力を登録済み？
        //        private bool IsFeatureRegistered(string fname)
        //        {
        //            bool IsFeatureRegisteredRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 107792


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)colFeature[fname];
        //            IsFeatureRegisteredRet = true;
        //            return IsFeatureRegisteredRet;
        //        ErrorHandler:
        //            ;
        //            IsFeatureRegisteredRet = false;
        //        }

        //        // 特殊能力を登録済み？(必要条件を満たさない特殊能力を含む)
        //        private bool IsAllFeatureRegistered(string fname)
        //        {
        //            bool IsAllFeatureRegisteredRet = default;
        //            FeatureData fd;
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 108130


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            fd = (FeatureData)colAllFeature[fname];
        //            IsAllFeatureRegisteredRet = true;
        //            return IsAllFeatureRegisteredRet;
        //        ErrorHandler:
        //            ;
        //            IsAllFeatureRegisteredRet = false;
        //        }

        //        // 特殊能力が必要条件を満たしているかどうか判定し、満たしていない能力を削除する
        //        // fnameが指定された場合、指定された特殊能力に対してのみ必要技能を判定
        //        private void UpdateFeatures(string fname = "")
        //        {
        //            FeatureData fd;
        //            FeatureData[] farray;
        //            int i;
        //            bool found;
        //            if (!string.IsNullOrEmpty(fname))
        //            {
        //                // 必要技能＆条件を満たしてない特殊能力を削除。
        //                found = false;
        //                i = 1;
        //                {
        //                    var withBlock = colFeature;
        //                    while (i <= withBlock.Count)
        //                    {
        //                        // 必要技能を満たしている？
        //                        // UPGRADE_WARNING: オブジェクト colFeature.Item(i).Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(fname, withBlock[i].Name, false)))
        //                        {
        //                            // UPGRADE_WARNING: オブジェクト colFeature.Item().NecessaryCondition の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                            // UPGRADE_WARNING: オブジェクト colFeature.Item().NecessarySkill の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                            bool localIsNecessarySkillSatisfied() { string argnabilities = Conversions.ToString(withBlock[i].NecessarySkill); Pilot argp = null; var ret = IsNecessarySkillSatisfied(argnabilities, p: argp); return ret; }

        //                            bool localIsNecessarySkillSatisfied1() { string argnabilities = Conversions.ToString(withBlock[i].NecessaryCondition); Pilot argp = null; var ret = IsNecessarySkillSatisfied(argnabilities, p: argp); return ret; }

        //                            if (!localIsNecessarySkillSatisfied() | !localIsNecessarySkillSatisfied1())
        //                            {
        //                                // 必要技能＆条件を満たしていないので削除
        //                                withBlock.Remove(i);
        //                                found = true;
        //                            }
        //                            else
        //                            {
        //                                i = (i + 1);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            i = (i + 1);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // 必要技能を満たしてない特殊能力を削除。
        //                found = false;
        //                i = 1;
        //                {
        //                    var withBlock1 = colFeature;
        //                    while (i <= withBlock1.Count)
        //                    {
        //                        // 必要技能を満たしている？
        //                        // UPGRADE_WARNING: オブジェクト colFeature.Item().NecessarySkill の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                        bool localIsNecessarySkillSatisfied2() { string argnabilities = Conversions.ToString(withBlock1[i].NecessarySkill); Pilot argp = null; var ret = IsNecessarySkillSatisfied(argnabilities, p: argp); return ret; }

        //                        if (!localIsNecessarySkillSatisfied2())
        //                        {
        //                            // 必要技能を満たしていないので削除
        //                            withBlock1.Remove(i);
        //                            found = true;
        //                        }
        //                        else
        //                        {
        //                            i = (i + 1);
        //                        }
        //                    }
        //                }

        //                // 必要条件を適用する前の特殊能力を保存
        //                {
        //                    var withBlock2 = colAllFeature;
        //                    foreach (FeatureData currentFd in colAllFeature)
        //                    {
        //                        fd = currentFd;
        //                        withBlock2.Remove(1);
        //                    }

        //                    foreach (FeatureData currentFd1 in colFeature)
        //                    {
        //                        fd = currentFd1;
        //                        if (!IsAllFeatureRegistered(fd.Name))
        //                        {
        //                            withBlock2.Add(fd, fd.Name);
        //                        }
        //                        else
        //                        {
        //                            withBlock2.Add(fd, fd.Name + ":" + SrcFormatter.Format(withBlock2.Count + 1));
        //                        }
        //                    }
        //                }

        //                // 必要条件を満たしてない特殊能力を削除。
        //                i = 1;
        //                {
        //                    var withBlock3 = colFeature;
        //                    while (i <= withBlock3.Count)
        //                    {
        //                        // 必要条件を満たしている？
        //                        // UPGRADE_WARNING: オブジェクト colFeature.Item().NecessaryCondition の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                        bool localIsNecessarySkillSatisfied3() { string argnabilities = Conversions.ToString(withBlock3[i].NecessaryCondition); Pilot argp = null; var ret = IsNecessarySkillSatisfied(argnabilities, p: argp); return ret; }

        //                        if (!localIsNecessarySkillSatisfied3())
        //                        {
        //                            // 必要条件を満たしていないので削除
        //                            withBlock3.Remove(i);
        //                            found = true;
        //                        }
        //                        else
        //                        {
        //                            i = (i + 1);
        //                        }
        //                    }
        //                }
        //            }

        //            // 特殊能力が削除された場合、特殊能力の保持判定が正しく行われるように特殊能力を
        //            // 登録しなおす必要がある。
        //            if (found)
        //            {
        //                {
        //                    var withBlock4 = colFeature;
        //                    farray = new FeatureData[withBlock4.Count + 1];
        //                    var loopTo = withBlock4.Count;
        //                    for (i = 1; i <= loopTo; i++)
        //                        farray[i] = (FeatureData)withBlock4[i];
        //                    var loopTo1 = withBlock4.Count;
        //                    for (i = 1; i <= loopTo1; i++)
        //                        withBlock4.Remove(1);
        //                    var loopTo2 = Information.UBound(farray);
        //                    for (i = 1; i <= loopTo2; i++)
        //                    {
        //                        if (!IsFeatureRegistered(farray[i].Name))
        //                        {
        //                            withBlock4.Add(farray[i], farray[i].Name);
        //                        }
        //                        else
        //                        {
        //                            withBlock4.Add(farray[i], farray[i].Name + ":" + SrcFormatter.Format(i));
        //                        }
        //                    }
        //                }
        //            }
        //        }
    }
}
