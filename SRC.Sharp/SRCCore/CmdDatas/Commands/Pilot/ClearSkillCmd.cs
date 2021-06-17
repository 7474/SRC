using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearSkillCmd : CmdData
    {
        public ClearSkillCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearSkillCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname;
            //            string slist, sname, sname2, buf;
            //            string[] sarray;
            //            string vname, vname2;
            //            short i, j;
            //            pname = GetArgAsString(2);
            //            bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //            if (SRC.PList.IsDefined(pname))
            //            {
            //                Pilot localItem() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                pname = localItem().ID;
            //            }
            //            else if (localIsDefined())
            //            {
            //                PilotData localItem1() { object argIndex1 = (object)pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                pname = localItem1().Name;
            //            }
            //            else
            //            {
            //                Event.EventErrorMessage = "「" + pname + "」というパイロットが見つかりません";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 185363


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            sname = GetArgAsString(3);

            //            // エリアスが定義されている？
            //            if (SRC.ALDList.IsDefined(sname))
            //            {
            //                {
            //                    var withBlock = SRC.ALDList.Item(sname);
            //                    sarray = new string[(withBlock.Count + 1)];
            //                    var loopTo = withBlock.Count;
            //                    for (i = 1; i <= loopTo; i++)
            //                        sarray[i] = withBlock.get_AliasType(i);
            //                }
            //            }
            //            else
            //            {
            //                sarray = new string[2];
            //                sarray[1] = sname;
            //            }

            //            var loopTo1 = Information.UBound(sarray);
            //            for (i = 1; i <= loopTo1; i++)
            //            {
            //                sname = sarray[i];
            //                sname2 = "";
            //                vname = "Ability(" + pname + "," + sname + ")";
            //                if (GeneralLib.LLength(Expression.GetValueAsString(vname)) >= 2)
            //                {
            //                    // 必要技能用変数を削除
            //                    sname2 = GeneralLib.LIndex(Expression.GetValueAsString(vname), 2);
            //                    vname2 = "Ability(" + pname + "," + sname2 + ")";
            //                    Expression.UndefineVariable(vname2);
            //                }

            //                // レベル設定用変数を削除
            //                Expression.UndefineVariable(vname);

            //                // 特殊能力一覧作成用変数を削除
            //                vname = "Ability(" + pname + ")";
            //                if (Expression.IsGlobalVariableDefined(vname))
            //                {
            //                    buf = Expression.GetValueAsString(vname);
            //                    slist = "";
            //                    var loopTo2 = GeneralLib.LLength(buf);
            //                    for (j = 1; j <= loopTo2; j++)
            //                    {
            //                        if ((GeneralLib.LIndex(buf, j) ?? "") != (sname ?? "") && (GeneralLib.LIndex(buf, j) ?? "") != (sname2 ?? ""))
            //                        {
            //                            slist = slist + " " + GeneralLib.LIndex(buf, j);
            //                        }
            //                    }

            //                    if (GeneralLib.LLength(slist) > 0)
            //                    {
            //                        slist = Strings.Trim(slist);
            //                        Expression.SetVariableAsString(vname, slist);
            //                    }
            //                    else
            //                    {
            //                        Expression.UndefineVariable(vname);
            //                    }
            //                }
            //            }

            //            // パイロットやユニットのステータスをアップデート
            //            if (SRC.PList.IsDefined(pname))
            //            {
            //                {
            //                    var withBlock1 = SRC.PList.Item(pname);
            //                    withBlock1.Update();
            //                    if (withBlock1.Unit is object)
            //                    {
            //                        withBlock1.Unit.Update();
            //                        if (withBlock1.Unit.Status == "出撃")
            //                        {
            //                            SRC.PList.UpdateSupportMod(withBlock1.Unit);
            //                        }
            //                    }
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
