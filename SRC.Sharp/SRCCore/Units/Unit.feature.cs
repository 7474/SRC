using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

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
            return colFeature[Index]?.FeatureName(this) ?? "";
        }
        public string FeatureName(int Index)
        {
            return colFeature[Index]?.FeatureName(this) ?? "";
        }
        public string FeatureName0(string Index)
        {
            return colFeature[Index]?.FeatureName0(this) ?? "";
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

        // 特殊能力(必要条件を満たさないものを含む)
        public FeatureData AllFeature(int Index)
        {
            return colAllFeature[Index];
        }
        public FeatureData AllFeature(string Index)
        {
            return colAllFeature[Index];
        }

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

        // 特殊能力が必要条件を満たしているかどうか判定し、満たしていない能力を削除する
        // fnameが指定された場合、指定された特殊能力に対してのみ必要技能を判定
        private void UpdateFeatures(string fname = "")
        {
            var found = false;
            if (!string.IsNullOrEmpty(fname))
            {
                // XXX colAllFeature の更新要らんの？
                // 必要技能＆条件を満たしてない特殊能力を削除。
                foreach (var fd in colFeature.List.Where(fd => fd.Name == fname)
                    .Where(fd => !IsNecessarySkillSatisfied(fd.NecessarySkill) 
                        || !IsNecessarySkillSatisfied(fd.NecessaryCondition)))
                {
                    // 必要技能を満たしていないので削除
                    colFeature.Remove(fd);
                    found = true;
                }
            }
            else
            {
                // 必要技能を満たしてない特殊能力を削除。
                foreach (var fd in colFeature.List.Where(fd => !IsNecessarySkillSatisfied(fd.NecessarySkill)))
                {
                    // 必要技能を満たしていないので削除
                    colFeature.Remove(fd);
                    found = true;
                }

                // 必要条件を適用する前の特殊能力を保存
                {
                    colAllFeature.Clear();
                    foreach (FeatureData fd in colFeature)
                    {
                        if (!colAllFeature.ContainsKey(fd.Name))
                        {
                            colAllFeature.Add(fd, fd.Name);
                        }
                        else
                        {
                            colAllFeature.Add(fd, fd.Name + ":" + SrcFormatter.Format(colAllFeature.Count + 1));
                        }
                    }
                }

                // 必要条件を満たしてない特殊能力を削除。
                foreach (var fd in colFeature.List.Where(fd => !IsNecessarySkillSatisfied(fd.NecessaryCondition)))
                {
                    // 必要条件を満たしていないので削除
                    colFeature.Remove(fd);
                    found = true;
                }
            }

            // 特殊能力が削除された場合、特殊能力の保持判定が正しく行われるように特殊能力を
            // 登録しなおす必要がある。
            if (found)
            {
                var cloneList = colFeature.List.CloneList();
                colFeature.Clear();
                foreach (FeatureData fd in cloneList)
                {
                    if (!colFeature.ContainsKey(fd.Name))
                    {
                        colFeature.Add(fd, fd.Name);
                    }
                    else
                    {
                        colFeature.Add(fd, fd.Name + ":" + SrcFormatter.Format(colAllFeature.Count + 1));
                    }
                }
            }
        }
    }
}
