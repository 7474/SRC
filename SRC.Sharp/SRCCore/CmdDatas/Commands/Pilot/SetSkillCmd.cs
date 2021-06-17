using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetSkillCmd : CmdData
    {
        public SetSkillCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetSkillCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname;
            //            string vname;
            //            string slist;
            //            string sname;
            //            string[] sname_array;
            //            double slevel;
            //            double[] slevel_array;
            //            var sdata = default(string);
            //            string[] sdata_array;
            //            short i, j;
            //            if (ArgNum != 4 && ArgNum != 5)
            //            {
            //                Event.EventErrorMessage = "SetSkillコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 456672


            //                Input:
            //                            Error(0)

            //                 */
            //            }

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
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 457050


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            sname = GetArgAsString(3);
            //            slevel = GetArgAsDouble(4);
            //            if (ArgNum == 5)
            //            {
            //                sdata = GetArgAsString(5);
            //            }

            //            // エリアスが定義されている？
            //            if (SRC.ALDList.IsDefined(sname))
            //            {
            //                {
            //                    var withBlock = SRC.ALDList.Item(sname);
            //                    sname_array = new string[(withBlock.Count + 1)];
            //                    slevel_array = new double[(withBlock.Count + 1)];
            //                    sdata_array = new string[(withBlock.Count + 1)];
            //                    var loopTo = withBlock.Count;
            //                    for (i = 1; i <= loopTo; i++)
            //                    {
            //                        string localLIndex() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock.get_AliasData(i) = arglist; return ret; }

            //                        if (localLIndex() == "解説")
            //                        {
            //                            if (string.IsNullOrEmpty(sdata))
            //                            {
            //                                sname_array[i] = withBlock.get_AliasType(i);
            //                            }
            //                            else
            //                            {
            //                                sname_array[i] = GeneralLib.LIndex(sdata, 1);
            //                            }

            //                            if (slevel == 0d)
            //                            {
            //                                slevel_array[i] = 0d;
            //                            }
            //                            else
            //                            {
            //                                slevel_array[i] = Constants.DEFAULT_LEVEL;
            //                            }

            //                            sdata_array[i] = withBlock.get_AliasData(i);
            //                        }
            //                        else
            //                        {
            //                            sname_array[i] = withBlock.get_AliasType(i);
            //                            if (slevel == -1)
            //                            {
            //                                slevel_array[i] = withBlock.get_AliasLevel(i);
            //                            }
            //                            else if (withBlock.get_AliasLevelIsPlusMod(i))
            //                            {
            //                                slevel_array[i] = slevel + withBlock.get_AliasLevel(i);
            //                            }
            //                            else if (withBlock.get_AliasLevelIsMultMod(i))
            //                            {
            //                                slevel_array[i] = slevel * withBlock.get_AliasLevel(i);
            //                            }
            //                            else
            //                            {
            //                                slevel_array[i] = slevel;
            //                            }

            //                            if (string.IsNullOrEmpty(sdata))
            //                            {
            //                                sdata_array[i] = withBlock.get_AliasData(i);
            //                            }
            //                            else
            //                            {
            //                                string localListTail() { string arglist = withBlock.get_AliasData(i); var ret = GeneralLib.ListTail(arglist, 2); withBlock.get_AliasData(i) = arglist; return ret; }

            //                                sdata_array[i] = Strings.Trim(sdata + " " + localListTail());
            //                            }

            //                            if (withBlock.get_AliasLevelIsPlusMod(i) || withBlock.get_AliasLevelIsMultMod(i))
            //                            {
            //                                sdata_array[i] = GeneralLib.LIndex(sdata_array[i], 1) + "Lv" + SrcFormatter.Format(slevel) + " " + GeneralLib.ListTail(sdata_array[i], 2);
            //                                sdata_array[i] = Strings.Trim(sdata_array[i]);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                sname_array = new string[2];
            //                slevel_array = new double[2];
            //                sdata_array = new string[2];
            //                sname_array[1] = sname;
            //                slevel_array[1] = slevel;
            //                sdata_array[1] = sdata;
            //            }

            //            var loopTo1 = Information.UBound(sname_array);
            //            for (i = 1; i <= loopTo1; i++)
            //            {
            //                sname = sname_array[i];
            //                slevel = slevel_array[i];
            //                sdata = sdata_array[i];
            //                if (string.IsNullOrEmpty(sname))
            //                {
            //                    goto NextSkill;
            //                }

            //                // アビリティ一覧表示用にSetSkillが適用された能力の一覧用変数を作成
            //                bool localIsGlobalVariableDefined() { string argvname = "Ability(" + pname + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

            //                if (!localIsGlobalVariableDefined())
            //                {
            //                    Expression.DefineGlobalVariable("Ability(" + pname + ")");
            //                    slist = sname;
            //                }
            //                else
            //                {
            //                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                    slist = Conversions.ToString(Event.GlobalVariableList["Ability(" + pname + ")"].StringValue);
            //                    var loopTo2 = GeneralLib.LLength(slist);
            //                    for (j = 1; j <= loopTo2; j++)
            //                    {
            //                        if ((sname ?? "") == (GeneralLib.LIndex(slist, j) ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    if (j > GeneralLib.LLength(slist))
            //                    {
            //                        slist = slist + " " + sname;
            //                    }
            //                }

            //                Expression.SetVariableAsString("Ability(" + pname + ")", slist);

            //                // 今回SetSkillが適用された能力sname用変数を作成
            //                vname = "Ability(" + pname + "," + sname + ")";
            //                if (!Expression.IsGlobalVariableDefined(vname))
            //                {
            //                    Expression.DefineGlobalVariable(vname);
            //                }

            //                if (!string.IsNullOrEmpty(sdata))
            //                {
            //                    // 別名指定があった場合
            //                    Expression.SetVariableAsString(vname, SrcFormatter.Format(slevel) + " " + sdata);

            //                    // 必要技能用
            //                    if (sdata != "非表示" && GeneralLib.LIndex(sdata, 1) != "解説")
            //                    {
            //                        vname = "Ability(" + pname + "," + GeneralLib.LIndex(sdata, 1) + ")";
            //                        if (!Expression.IsGlobalVariableDefined(vname))
            //                        {
            //                            Expression.DefineGlobalVariable(vname);
            //                        }

            //                        Expression.SetVariableAsString(vname, SrcFormatter.Format(slevel));
            //                    }
            //                }
            //                else
            //                {
            //                    Expression.SetVariableAsString(vname, SrcFormatter.Format(slevel));
            //                }

            //                NextSkill:
            //                ;
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
