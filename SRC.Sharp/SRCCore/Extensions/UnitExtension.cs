
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions
{
    public static class UnitExtension
    {
        public static List<FeatureData> CombineFeatures(this Unit currentUnit, SRC SRC)
        {
            var combines = new List<FeatureData>();
            foreach (var fd in currentUnit.Features)
            {
                // 3体以上からなる合体能力を持っているか？
                var opts = GeneralLib.ToL(fd.StrData);
                var combinename = opts.First();
                var unitnames = opts.Skip(1).ToList();
                var conbineunitname = unitnames.First();
                var partunitnames = unitnames.Skip(1).ToList();
                if (fd.Name == "合体"
                    && !string.IsNullOrEmpty(currentUnit.FeatureName(fd.Name))
                    && partunitnames.Count >= 2)
                {
                    var n = 0;
                    // パートナーは隣接しているか？
                    foreach (var uname in partunitnames)
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
                    var u = SRC.UList.Item(conbineunitname);
                    if (u is null)
                    {
                        n = 0;
                    }
                    else if (u.IsConditionSatisfied("行動不能"))
                    {
                        n = 0;
                    }

                    // すべての条件を満たしている場合
                    if (n == partunitnames.Count)
                    {
                        combines.Add(fd);
                    }
                }
            }
            return combines;
        }
    }
}
