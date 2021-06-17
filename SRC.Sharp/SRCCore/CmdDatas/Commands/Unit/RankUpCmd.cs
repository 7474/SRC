using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RankUpCmd : CmdData
    {
        public RankUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RankUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string uname;
            //            Unit u;
            //            short rk;
            //            double hp_ratio, en_ratio;
            //            short i, j;
            //            string buf;
            //            switch (ArgNum)
            //            {
            //                case 3:
            //                    {
            //                        uname = GetArgAsString(2);
            //                        u = SRC.UList.Item((object)uname);
            //                        if (u is null)
            //                        {
            //                            Event.EventErrorMessage = uname + "というユニットは存在しません";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 401470


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        rk = GetArgAsLong(3);
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        rk = GetArgAsLong(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "RankUpコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 401717


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            hp_ratio = 100 * u.HP / (double)u.MaxHP;
            //            en_ratio = 100 * u.EN / (double)u.MaxEN;
            //            u.Rank = (u.Rank + rk);
            //            u.HP = (u.MaxHP * hp_ratio / 100d);
            //            u.EN = (u.MaxEN * en_ratio / 100d);
            //            var loopTo = u.CountOtherForm();
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                {
            //                    var withBlock = u.OtherForm(i);
            //                    hp_ratio = 100 * withBlock.HP / (double)withBlock.MaxHP;
            //                    en_ratio = 100 * withBlock.EN / (double)withBlock.MaxEN;
            //                    withBlock.Rank = (withBlock.Rank + rk);
            //                    withBlock.HP = (withBlock.MaxHP * hp_ratio / 100d);
            //                    withBlock.EN = (withBlock.MaxEN * en_ratio / 100d);
            //                }
            //            }

            //            // 合体できる場合は他の分離ユニットのユニットランクを上げる
            //            if (u.IsFeatureAvailable("合体"))
            //            {
            //                // 合体後の形態を検索
            //                var loopTo1 = u.CountFeature();
            //                for (i = 1; i <= loopTo1; i++)
            //                {
            //                    if (u.Feature(i) == "合体")
            //                    {
            //                        string localFeatureData() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //                        buf = GeneralLib.LIndex(localFeatureData(), 2);
            //                        string localFeatureData1() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //                        if (GeneralLib.LLength(localFeatureData1()) == 3)
            //                        {
            //                            if (SRC.UDList.IsDefined(buf))
            //                            {
            //                                UnitData localItem() { object argIndex1 = buf; var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                                if (localItem().IsFeatureAvailable("主形態"))
            //                                {
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (SRC.UDList.IsDefined(buf))
            //                            {
            //                                UnitData localItem1() { object argIndex1 = buf; var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                                if (!localItem1().IsFeatureAvailable("制限時間"))
            //                                {
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }

            //                if (i <= u.CountFeature())
            //                {
            //                    string localFeatureData2() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

            //                    string localLIndex() { string arglist = hs2a2f208e052147729d34e3cbaff37f7f(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                    UnitData localItem2() { object argIndex1 = (object)hse505f2f9d761474e89617a2bf6851fa7(); var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                    buf = localItem2().FeatureData("分離");
            //                    var loopTo2 = GeneralLib.LLength(buf);
            //                    for (i = 2; i <= loopTo2; i++)
            //                    {
            //                        if (SRC.UList.IsDefined(GeneralLib.LIndex(buf, i)))
            //                        {
            //                            {
            //                                var withBlock1 = SRC.UList.Item(GeneralLib.LIndex(buf, i));
            //                                if (!u.IsEqual(withBlock1.Name))
            //                                {
            //                                    // 他の分離形態のユニットランクを上げる
            //                                    hp_ratio = 100 * withBlock1.HP / (double)withBlock1.MaxHP;
            //                                    en_ratio = 100 * withBlock1.EN / (double)withBlock1.MaxEN;
            //                                    withBlock1.Rank = (withBlock1.Rank + rk);
            //                                    withBlock1.HP = (withBlock1.MaxHP * hp_ratio / 100d);
            //                                    withBlock1.EN = (withBlock1.MaxEN * en_ratio / 100d);
            //                                    var loopTo3 = withBlock1.CountOtherForm();
            //                                    for (j = 1; j <= loopTo3; j++)
            //                                    {
            //                                        {
            //                                            var withBlock2 = withBlock1.OtherForm(j);
            //                                            hp_ratio = 100 * withBlock2.HP / (double)withBlock2.MaxHP;
            //                                            en_ratio = 100 * withBlock2.EN / (double)withBlock2.MaxEN;
            //                                            withBlock2.Rank = (withBlock2.Rank + rk);
            //                                            withBlock2.HP = (withBlock2.MaxHP * hp_ratio / 100d);
            //                                            withBlock2.EN = (withBlock2.MaxEN * en_ratio / 100d);
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            // 分離できる場合は分離ユニットのユニットランクを上げる
            //            if (u.IsFeatureAvailable("分離"))
            //            {
            //                buf = u.FeatureData("分離");
            //                var loopTo4 = GeneralLib.LLength(buf);
            //                for (i = 2; i <= loopTo4; i++)
            //                {
            //                    if (SRC.UList.IsDefined(GeneralLib.LIndex(buf, i)))
            //                    {
            //                        {
            //                            var withBlock3 = SRC.UList.Item(GeneralLib.LIndex(buf, i));
            //                            withBlock3.Rank = GeneralLib.MaxLng(withBlock3.Rank, u.Rank);
            //                            withBlock3.HP = withBlock3.MaxHP;
            //                            withBlock3.EN = withBlock3.MaxEN;
            //                            var loopTo5 = withBlock3.CountOtherForm();
            //                            for (j = 1; j <= loopTo5; j++)
            //                            {
            //                                {
            //                                    var withBlock4 = withBlock3.OtherForm(j);
            //                                    hp_ratio = 100 * withBlock4.HP / (double)withBlock4.MaxHP;
            //                                    en_ratio = 100 * withBlock4.EN / (double)withBlock4.MaxEN;
            //                                    withBlock4.Rank = (withBlock4.Rank + rk);
            //                                    withBlock4.HP = (withBlock4.MaxHP * hp_ratio / 100d);
            //                                    withBlock4.EN = (withBlock4.MaxEN * en_ratio / 100d);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //return EventData.NextID;
        }
    }
}
