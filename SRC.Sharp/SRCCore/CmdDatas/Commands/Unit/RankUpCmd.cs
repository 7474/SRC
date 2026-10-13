using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Units;
using System;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class RankUpCmd : CmdData
    {
        public RankUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RankUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            int rk;
            switch (ArgNum)
            {
                case 3:
                    {
                        var uname = GetArgAsString(2);
                        u = SRC.UList.Item(uname);
                        if (u is null)
                        {
                            throw new EventErrorException(this, uname + "というユニットは存在しません");
                        }

                        rk = GetArgAsLong(3);
                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        rk = GetArgAsLong(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "RankUpコマンドの引数の数が違います");
            }

            // VB版 ExecRankUpCmd と同様に各形態・分離ユニットのランクを相対的に変化させる
            AddRank(u, rk);
            foreach (var of in u.OtherForms)
            {
                AddRank(of, rk);
            }

            // 合体できる場合は他の分離ユニットのユニットランクを上げる
            if (u.IsFeatureAvailable("合体"))
            {
                // 合体後の形態を検索
                var cfd = u.Features
                    .Where(fd => fd.Name == "合体")
                    .FirstOrDefault(fd =>
                    {
                        var buf = GeneralLib.LIndex(fd.Data, 2);
                        if (!SRC.UDList.IsDefined(buf))
                        {
                            return false;
                        }
                        if (GeneralLib.LLength(fd.Data) == 3)
                        {
                            return SRC.UDList.Item(buf).IsFeatureAvailable("主形態");
                        }
                        else
                        {
                            return !SRC.UDList.Item(buf).IsFeatureAvailable("制限時間");
                        }
                    });
                if (cfd != null)
                {
                    var buf = SRC.UDList.Item(GeneralLib.LIndex(cfd.Data, 2)).FeatureData("分離");
                    for (var i = 2; i <= GeneralLib.LLength(buf); i++)
                    {
                        var pu = SRC.UList.Item(GeneralLib.LIndex(buf, i));
                        if (pu == null || u.IsEqual(pu.Name))
                        {
                            continue;
                        }

                        // 他の分離形態のユニットランクを上げる
                        AddRank(pu, rk);
                        foreach (var of in pu.OtherForms)
                        {
                            AddRank(of, rk);
                        }
                    }
                }
            }

            // 分離できる場合は分離ユニットのユニットランクを上げる
            if (u.IsFeatureAvailable("分離"))
            {
                var buf = u.FeatureData("分離");
                for (var i = 2; i <= GeneralLib.LLength(buf); i++)
                {
                    var pu = SRC.UList.Item(GeneralLib.LIndex(buf, i));
                    if (pu == null)
                    {
                        continue;
                    }

                    pu.Rank = GeneralLib.MaxLng(pu.Rank, u.Rank);
                    pu.HP = pu.MaxHP;
                    pu.EN = pu.MaxEN;
                    foreach (var of in pu.OtherForms)
                    {
                        AddRank(of, rk);
                    }
                }
            }

            return EventData.NextID;
        }

        // ＨＰ・ＥＮの割合を保ったままランクを変化させる
        private static void AddRank(Unit u, int rk)
        {
            var hp_ratio = u.MaxHP != 0 ? 100d * u.HP / u.MaxHP : 100d;
            var en_ratio = u.MaxEN != 0 ? 100d * u.EN / u.MaxEN : 100d;
            u.Rank = u.Rank + rk;
            u.HP = (int)Math.Round(u.MaxHP * hp_ratio / 100d, MidpointRounding.ToEven);
            u.EN = (int)Math.Round(u.MaxEN * en_ratio / 100d, MidpointRounding.ToEven);
        }
    }
}
