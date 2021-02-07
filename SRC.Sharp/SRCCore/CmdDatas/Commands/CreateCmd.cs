using SRC.Core.Events;
using SRC.Core.Exceptions;
using SRC.Core.Pilots;
using SRC.Core.Units;

namespace SRC.Core.CmdDatas.Commands
{
    public class CreateCmd : CmdData
    {
        public CreateCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CreateCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecCreateCmdRet = default;
            //string uname, uparty;
            //short urank;
            //string pname;
            //short plevel;
            //short ux, uy;
            //Unit u;
            //Pilot p;
            //string buf;
            //short i, num;
            //string opt;
            //num = ArgNum;
            //switch (GetArgAsString(num) ?? "")
            //{
            //    case "非同期":
            //        {
            //            opt = "非同期";
            //            num = (short)(num - 1);
            //            break;
            //        }

            //    case "アニメ非表示":
            //        {
            //            opt = "";
            //            num = (short)(num - 1);
            //            break;
            //        }

            //    default:
            //        {
            //            opt = "出撃";
            //            break;
            //        }
            //}

            //if ((int)num < 0)
            //{
            //    throw new EventErrorException("Createコマンドのパラメータの括弧の対応が取れていません");
            //}
            //else if ((int)num != 8 & (int)num != 9)
            //{
            //    throw new EventErrorException("Createコマンドの引数の数が違います");
            //}

            //uparty = GetArgAsString(2);
            //if (!(uparty == "味方" | uparty == "ＮＰＣ" | uparty == "敵" | uparty == "中立"))
            //{
            //    throw new EventErrorException("所属の指定「" + uparty + "」が間違っています");
            //}

            //uname = GetArgAsString(3);
            //bool localIsDefined() { object argIndex1 = uname; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

            //if (!localIsDefined())
            //{
            //    throw new EventErrorException("指定したユニット「" + uname + "」のデータが見つかりません");
            //}

            //buf = GetArgAsString(4);
            //if (!Information.IsNumeric(buf))
            //{
            //    throw new EventErrorException("ユニットのランクが不正です");

            //}

            //urank = Conversions.ToShort(buf);
            //pname = GetArgAsString(5);
            //bool localIsDefined1() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

            //if (!localIsDefined1())
            //{
            //    throw new EventErrorException("指定したパイロット「" + pname + "」のデータが見つかりません");
            //}

            //buf = GetArgAsString(6);
            //if (!Information.IsNumeric(buf))
            //{
            //    throw new EventErrorException("パイロットのレベルが不正です");
            //}

            //plevel = Conversions.ToShort(buf);
            //string argoname = "レベル限界突破";
            //if (Expression.IsOptionDefined(ref argoname))
            //{
            //    if (plevel > 999)
            //    {
            //        plevel = 999;
            //    }
            //}
            //else if (plevel > 99)
            //{
            //    plevel = 99;
            //}

            //if (plevel < 1)
            //{
            //    plevel = 1;
            //}

            //buf = GetArgAsString(7);
            //if (!Information.IsNumeric(buf))
            //{
            //    throw new EventErrorException("Ｘ座標の値が不正です");
            //}

            //ux = Conversions.ToShort(buf);
            //if (ux < 1)
            //{
            //    ux = 1;
            //}
            //else if (ux > Map.MapWidth)
            //{
            //    ux = Map.MapWidth;
            //}

            //buf = GetArgAsString(8);
            //if (!Information.IsNumeric(buf))
            //{
            //    throw new EventErrorException("Ｙ座標の値が不正です");
            //}

            //uy = Conversions.ToShort(buf);
            //if (uy < 1)
            //{
            //    uy = 1;
            //}
            //else if (uy > Map.MapHeight)
            //{
            //    uy = Map.MapHeight;
            //}

            //u = SRC.UList.Add(ref uname, urank, ref uparty);
            //if (u is null)
            //{
            //    throw new EventErrorException(uname + "のデータが不正です");
            //}

            //if (num == 9)
            //{
            //    string arggid = GetArgAsString(9);
            //    p = SRC.PList.Add(ref pname, plevel, ref uparty, ref arggid);
            //}
            //else
            //{
            //    string arggid1 = "";
            //    p = SRC.PList.Add(ref pname, plevel, ref uparty, gid: ref arggid1);
            //}

            //p.Ride(ref u);
            //if (opt != "非同期" & GUI.MainForm.Visible & !GUI.IsPictureVisible)
            //{
            //    GUI.Center(ux, uy);
            //    GUI.RefreshScreen();
            //}

            //u.FullRecover();
            //var loopTo = u.CountOtherForm();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    Unit localOtherForm() { object argIndex1 = i; var ret = u.OtherForm(ref argIndex1); return ret; }

            //    localOtherForm().FullSupply();
            //}

            //u.UsedAction = 0;
            //u.StandBy(ux, uy, opt);
            //u.CheckAutoHyperMode();
            //Event_Renamed.SelectedUnitForEvent = u.CurrentForm();
            //ExecCreateCmdRet = LineNum + 1;
            return ExecCreateCmdRet;
        }
    }
}
