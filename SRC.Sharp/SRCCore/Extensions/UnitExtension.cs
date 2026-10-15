
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions
{
    public class CombineFeature
    {
        public FeatureData FeatureData { get; private set; }

        public string CombineName { get; private set; }
        public string ConbineUnitName { get; private set; }
        public IList<string> PartUnitNames { get; private set; }

        public CombineFeature(FeatureData fd)
        {
            FeatureData = fd;

            var opts = GeneralLib.ToL(fd.StrData);
            CombineName = opts.First();
            var unitnames = opts.Skip(1).ToList();
            ConbineUnitName = unitnames.First();
            PartUnitNames = unitnames.Skip(1).ToList();
        }
    }

    public static class UnitExtension
    {
        public static List<CombineFeature> TwoUnitCombineFeatures(this Unit currentUnit, SRC SRC)
        {
            return currentUnit.Features
                .Where(x => x.Name == "合体")
                .Where(x => !string.IsNullOrEmpty(currentUnit.FeatureName(x.Name)))
                .Select(x => new CombineFeature(x))
                .Where(x => x.PartUnitNames.Count == 1)
                .Where(x =>
                {
                    var cu = SRC.UList.Item(x.ConbineUnitName);
                    return cu != null
                        && !cu.IsConditionSatisfied("行動不能")
                        && cu.Status != "破棄";
                })
                .Where(x =>
                {
                    var pu = SRC.UList.Item(x.PartUnitNames.First());
                    return pu != null
                        && !pu.CurrentForm().IsFeatureAvailable("合体制限");
                }).ToList();
        }

        /// <summary>
        /// 3 体以上（ユニットステータスコマンドの時は 2 体も）からなる、今すぐ合体できる合体能力。
        /// </summary>
        /// <param name="forMenu">
        /// ユニットのコマンドメニューに出すかの判定（VB Command.bas の CommandSelect）。
        /// 元の SRC はメニューではパートナーとの距離を縦横の和（2 以内）で測り、2 体合体もステータス表示の例外も無い。
        /// 偽なら合体コマンドの実行時の判定（CombineCommand）で、縦・横それぞれ 2 以内、ステータス表示では距離を見ない。
        /// </param>
        public static List<CombineFeature> CombineFeatures(this Unit currentUnit, SRC SRC, bool forMenu = false)
        {
            var combines = new List<CombineFeature>();
            foreach (var fd in currentUnit.Features
                .Where(x => x.Name == "合体")
                .Where(x => !string.IsNullOrEmpty(currentUnit.FeatureName(x.Name))))
            {
                // 3体以上からなる合体能力を持っているか？
                // ユニットステータスコマンドの時は2体合体も対象にする
                var combineData = new CombineFeature(fd);
                if (combineData.PartUnitNames.Count >= 2 || (!forMenu && SRC.Map.IsStatusView))
                {
                    var n = 0;
                    // パートナーは隣接しているか？
                    foreach (var uname in combineData.PartUnitNames)
                    {
                        var partu = SRC.UList.Item(uname);
                        if (partu is null)
                        {
                            break;
                        }

                        if (!partu.IsOperational())
                        {
                            break;
                        }

                        if (partu.Status != "出撃" && partu.CurrentForm().IsFeatureAvailable("合体制限"))
                        {
                            break;
                        }

                        var dx = Math.Abs(currentUnit.x - partu.CurrentForm().x);
                        var dy = Math.Abs(currentUnit.y - partu.CurrentForm().y);
                        if (forMenu)
                        {
                            if (dx + dy > 2)
                            {
                                break;
                            }
                        }
                        // ユニットステータスコマンドの時は距離を判定しない
                        else if (!SRC.Map.IsStatusView && (dx > 2 || dy > 2))
                        {
                            break;
                        }

                        n = (n + 1);
                    }

                    // 合体先のユニットが作成され、かつ合体可能な状態にあるか？
                    var u = SRC.UList.Item(combineData.ConbineUnitName);
                    if (u is null)
                    {
                        n = 0;
                    }
                    else if (u.IsConditionSatisfied("行動不能"))
                    {
                        n = 0;
                    }

                    // すべての条件を満たしている場合
                    if (n == combineData.PartUnitNames.Count)
                    {
                        combines.Add(combineData);
                    }
                }
            }
            return combines;
        }
    }
}
