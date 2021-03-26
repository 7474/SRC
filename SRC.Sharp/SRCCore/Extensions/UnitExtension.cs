
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
        public static List<CombineFeature> CombineFeatures(this Unit currentUnit, SRC SRC)
        {
            // TODO ステータス表示の時をガン無視しているはず
            var combines = new List<CombineFeature>();
            foreach (var fd in currentUnit.Features
                .Where(x => x.Name == "合体")
                .Where(x => !string.IsNullOrEmpty(currentUnit.FeatureName(x.Name))))
            {
                // 3体以上からなる合体能力を持っているか？
                var combineData = new CombineFeature(fd);
                if (combineData.PartUnitNames.Count >= 2)
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

                        if (partu.Status != "出撃" & partu.CurrentForm().IsFeatureAvailable("合体制限"))
                        {
                            break;
                        }

                        if (Math.Abs((currentUnit.x - partu.CurrentForm().x)) + Math.Abs((currentUnit.y - partu.CurrentForm().y)) > 2)
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
