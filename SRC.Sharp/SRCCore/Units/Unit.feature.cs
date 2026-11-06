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

        // ステータス画面の特殊能力一覧に並べる特殊能力と表示名
        // (VB: Status.bas DisplayUnitStatus の
        //  For i = .AdditionalFeaturesNum + 1 To .CountAllFeature / fname = .AllFeatureName(i))
        // 付加された特殊能力は特殊ステータスとして別に表示されるので含めない。
        // 表示名が空のものは非表示の能力(別名が「非表示」「解説」の能力や常に非表示の能力)。
        public IEnumerable<(FeatureData Feature, string Name)> StatusFeatures()
        {
            return AllFeatures.Skip(AdditionalFeaturesNum)
                .Select(fd => (fd, fd.FeatureName(this)));
        }

        // 特殊能力の総数
        public int CountFeature()
        {
            return colFeature.Count;
        }

        // 特殊能力
        // 元の SRC は名称を返すが、SRC# は FeatureData を返す（名称は .Name で参照する）
        public FeatureData Feature(string Index)
        {
            return colFeature[Index];
        }
        public FeatureData Feature(int Index)
        {
            return colFeature[Index];
        }

        // 番号で特殊能力を参照する。元の SRC の On Error と同じく、範囲外なら null
        private static FeatureData FeatureAt(SrcCollection<FeatureData> list, int index)
        {
            return index >= 1 && index <= list.Count ? list[index] : null;
        }

        // 特殊能力の名称
        // 見つからなかった場合は、元の SRC と同じく Index をそのまま返す
        public string FeatureName(string Index)
        {
            var fd = colFeature[Index];
            return fd is null ? Index ?? "" : fd.FeatureName(this);
        }
        public string FeatureName(int Index)
        {
            var fd = FeatureAt(colFeature, Index);
            return fd is null ? SrcFormatter.Format(Index) : fd.FeatureName(this);
        }
        public string FeatureName0(string Index)
        {
            var fname = FeatureName(Index);
            if (Strings.InStr(fname, "Lv") > 0)
            {
                fname = Strings.Left(fname, Strings.InStr(fname, "Lv") - 1);
            }
            return fname;
        }

        // 特殊能力のレベル
        public double FeatureLevel(string Index)
        {
            return colFeature[Index]?.FeatureLevel ?? 0d;
        }
        public double FeatureLevel(int Index)
        {
            return FeatureAt(colFeature, Index)?.FeatureLevel ?? 0d;
        }

        // 特殊能力のデータ
        public string FeatureData(string Index)
        {
            return colFeature[Index]?.Data ?? "";
        }
        public string FeatureData(int Index)
        {
            return FeatureAt(colFeature, Index)?.Data ?? "";
        }

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

        // 特殊能力(必要条件を満たさないものを含む)
        // 元の SRC の CountAllFeature・AllFeatureName・AllFeatureLevel・AllFeatureLevelSpecified・
        // AllFeatureData・IsAllFeatureLevelSpecified は、AllFeatures と FeatureData のメンバー
        // （FeatureName(unit)・FeatureLevel・HasLevel・Data）で参照する
        public FeatureData AllFeature(int Index)
        {
            return colAllFeature[Index];
        }
        public FeatureData AllFeature(string Index)
        {
            return colAllFeature[Index];
        }

        // 特殊能力が必要条件を満たしているか
        // （必要条件を満たさないものを含む一覧の fd が、有効な特殊能力の一覧に同じインスタンスとしてあるか）
        public bool IsFeatureActivated(FeatureData fd)
        {
            if (fd is null)
            {
                return false;
            }
            return colFeature.List.Any(x => ReferenceEquals(x, fd));
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
                if (IsDisabled(fd.Name) || IsDisabled(GeneralLib.LIndex(fd.StrData, 1)))
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
                // 元の SRC と同じく、同じ名前の 2 つ目以降は一覧での位置（1 始まり）を付けて登録する
                for (var i = 1; i <= cloneList.Count; i++)
                {
                    var fd = cloneList[i - 1];
                    if (!colFeature.ContainsKey(fd.Name))
                    {
                        colFeature.Add(fd, fd.Name);
                    }
                    else
                    {
                        colFeature.Add(fd, fd.Name + ":" + SrcFormatter.Format(i));
                    }
                }
            }
        }
    }
}
