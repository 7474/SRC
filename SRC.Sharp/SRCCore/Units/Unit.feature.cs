using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Units
{
    // === ユニット用特殊能力関連処理 ===
    public partial class Unit
    {
        public IList<Models.FeatureData> Features => colFeature.List;
        public IList<Models.FeatureData> AllFeatures => colAllFeature.List;

        // 特殊能力の総数
        public int CountFeature()
        {
            return colFeature.Count;
        }

        // 特殊能力
        public FeatureData Feature(string Index)
        {
            // XXX 参照先はNameだった
            //FeatureRet = fd.Name;
            return colFeature[Index];
        }
        public FeatureData Feature(int Index)
        {
            // XXX 参照先はNameだった
            //FeatureRet = fd.Name;
            return colFeature[Index];
        }

        // 特殊能力の名称
        public string FeatureName(string Index)
        {
            return colFeature[Index].FeatureName(this);
        }
        public string FeatureName(int Index)
        {
            return colFeature[Index].FeatureName(this);
        }
        public string FeatureName0(string Index)
        {
            return colFeature[Index].FeatureName0(this);
        }

        // 特殊能力のレベル
        public double FeatureLevel(string Index)
        {
            return colFeature[Index]?.FeatureLevel ?? 0d;
        }
        public double FeatureLevel(int Index)
        {
            return colFeature[Index]?.FeatureLevel ?? 0d;
        }

        // 特殊能力のデータ
        public string FeatureData(string Index)
        {
            return colFeature[Index]?.Data ?? "";
        }
        public string FeatureData(int Index)
        {
            return colFeature[Index]?.Data ?? "";
        }

        //        // 特殊能力の必要技能
        //        public string FeatureNecessarySkill(string Index)
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
        //        public string AllFeature(string Index)
        //        {
        //            string AllFeatureRet = default;
        //            FeatureData fd;
        //            fd = (FeatureData)colAllFeature[Index];
        //            AllFeatureRet = fd.Name;
        //            return AllFeatureRet;
        //        }

        //        // 特殊能力の名称(必要条件を満たさないものを含む)
        //        public string AllFeatureName(string Index)
        //        {
        //            string AllFeatureNameRet = default;
        //            AllFeatureNameRet = FeatureNameInt(Index, colAllFeature);
        //            return AllFeatureNameRet;
        //        }

        //        // 特殊能力のレベル(必要条件を満たさないものを含む)
        //        public double AllFeatureLevel(string Index)
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
        //        public bool AllFeatureLevelSpecified(string Index)
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
        //        public string AllFeatureData(string Index)
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
        //        public bool IsAllFeatureLevelSpecified(string Index)
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

        // 特殊能力が必要条件を満たしているか
        public bool IsFeatureActivated(FeatureData fd)
        {
            // XXX 同一インスタンスじゃないかも
            return AllFeatures.Contains(fd);
        }

        // 特殊能力を登録
        private void AddFeatures(IList<FeatureData> fdc, bool is_item = false)
        {
            if (fdc is null)
            {
                return;
            }

            foreach (FeatureData fd in fdc)
            {
                // アイテムで指定された下記の能力はアイテムそのものの属性なので
                // ユニット側には追加しない
                if (is_item)
                {
                    switch (fd.Name ?? "")
                    {
                        case "必要技能":
                        case "不必要技能":
                        case "表示":
                        case "非表示":
                        case "呪い":
                            continue;
                    }
                }

                // 封印されている？
                if (IsDisabled(fd.Name) | IsDisabled(GeneralLib.LIndex(fd.StrData, 1)))
                {
                    continue;
                }

                // 既にその能力が登録されている？
                if (!IsFeatureRegistered(fd.Name))
                {
                    colFeature.Add(fd, fd.Name);
                }
                else
                {
                    colFeature.Add(fd, fd.Name + ":" + SrcFormatter.Format(colFeature.Count));
                }

            NextFeature:
                ;
            }
        }

        // 特殊能力を登録済み？
        private bool IsFeatureRegistered(string fname)
        {
            return colFeature[fname] != null;
        }

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
