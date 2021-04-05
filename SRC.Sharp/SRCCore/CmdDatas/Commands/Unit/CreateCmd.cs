using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class CreateCmd : CmdData
    {
        public CreateCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CreateCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string opt;
            var num = ArgNum;
            switch (GetArgAsString(num) ?? "")
            {
                case "非同期":
                    {
                        opt = "非同期";
                        num = (num - 1);
                        break;
                    }

                case "アニメ非表示":
                    {
                        opt = "";
                        num = (num - 1);
                        break;
                    }

                default:
                    {
                        opt = "出撃";
                        break;
                    }
            }

            if (num < 0)
            {
                throw new EventErrorException(this, "Createコマンドのパラメータの括弧の対応が取れていません");
            }
            else if (num != 8 & num != 9)
            {
                throw new EventErrorException(this, "Createコマンドの引数の数が違います");
            }

            var uparty = GetArgAsString(2);
            if (!(uparty == "味方" | uparty == "ＮＰＣ" | uparty == "敵" | uparty == "中立"))
            {
                throw new EventErrorException(this, "所属の指定「" + uparty + "」が間違っています");
            }

            var uname = GetArgAsString(3);
            if (!SRC.UDList.IsDefined(uname))
            {
                throw new EventErrorException(this, "指定したユニット「" + uname + "」のデータが見つかりません");
            }

            var buf = GetArgAsString(4);
            if (!Information.IsNumeric(buf))
            {
                throw new EventErrorException(this, "ユニットのランクが不正です");

            }
            var urank = Conversions.ToInteger(buf);

            var pname = GetArgAsString(5);
            if (!SRC.PDList.IsDefined(pname))
            {
                throw new EventErrorException(this, "指定したパイロット「" + pname + "」のデータが見つかりません");
            }

            buf = GetArgAsString(6);
            if (!Information.IsNumeric(buf))
            {
                throw new EventErrorException(this, "パイロットのレベルが不正です");
            }
            var plevel = Conversions.ToInteger(buf);
            if (Expression.IsOptionDefined("レベル限界突破"))
            {
                if (plevel > 999)
                {
                    plevel = 999;
                }
            }
            else if (plevel > 99)
            {
                plevel = 99;
            }

            if (plevel < 1)
            {
                plevel = 1;
            }

            buf = GetArgAsString(7);
            if (!Information.IsNumeric(buf))
            {
                throw new EventErrorException(this, "Ｘ座標の値が不正です");
            }

            var ux = Conversions.ToInteger(buf);
            if (ux < 1)
            {
                ux = 1;
            }
            else if (ux > Map.MapWidth)
            {
                ux = Map.MapWidth;
            }

            buf = GetArgAsString(8);
            if (!Information.IsNumeric(buf))
            {
                throw new EventErrorException(this, "Ｙ座標の値が不正です");
            }

            var uy = Conversions.ToInteger(buf);
            if (uy < 1)
            {
                uy = 1;
            }
            else if (uy > Map.MapHeight)
            {
                uy = Map.MapHeight;
            }

            Unit u = SRC.UList.Add(uname, urank, uparty);
            if (u is null)
            {
                throw new EventErrorException(this, uname + "のデータが不正です");
            }

            Pilot p;
            if (num == 9)
            {
                p = SRC.PList.Add(pname, plevel, uparty, GetArgAsString(9));
            }
            else
            {
                p = SRC.PList.Add(pname, plevel, uparty, gid: "");
            }

            p.Ride(u);
            if (opt != "非同期" & GUI.MainFormVisible & !GUI.IsPictureVisible)
            {
                GUI.Center(ux, uy);
                GUI.RefreshScreen();
            }

            u.FullRecover();
            foreach (var of in u.OtherForms)
            {
                of.FullSupply();
            }

            u.UsedAction = 0;
            u.StandBy(ux, uy, opt);
            u.CheckAutoHyperMode();
            Event.SelectedUnitForEvent = u.CurrentForm();

            return EventData.ID + 1;
        }
    }
}
