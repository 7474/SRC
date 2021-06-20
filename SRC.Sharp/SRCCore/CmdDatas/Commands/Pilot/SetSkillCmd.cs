using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Commands
{
    public class SetSkillCmd : CmdData
    {
        public SetSkillCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetSkillCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 4 && ArgNum != 5)
            {
                throw new EventErrorException(this, "SetSkillコマンドの引数の数が違います");
            }

            var pname = GetArgAsString(2);
            if (SRC.PList.IsDefined(pname))
            {
                pname = SRC.PList.Item(pname).ID;
            }
            else if (SRC.PDList.IsDefined(pname))
            {
                pname = SRC.PDList.Item(pname).Name;
            }
            else
            {
                throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
            }

            var sname = GetArgAsString(3);
            var slevel = GetArgAsDouble(4);
            var sdata = "";
            if (ArgNum == 5)
            {
                sdata = GetArgAsString(5);
            }

            // エリアスが定義されている？
            var sList = new List<SkillData>();
            if (SRC.ALDList.IsDefined(sname))
            {
                {
                    var alias = SRC.ALDList.Item(sname);
                    foreach (var elm in alias.Elements)
                    {
                        var sd = new SkillData();
                        sList.Add(sd);

                        if (GeneralLib.LIndex(elm.strAliasData, 1) == "解説")
                        {
                            if (string.IsNullOrEmpty(sdata))
                            {
                                sd.Name = elm.strAliasType;
                            }
                            else
                            {
                                sd.Name = GeneralLib.LIndex(sdata, 1);
                            }

                            if (slevel == 0d)
                            {
                                sd.Level = 0d;
                            }
                            else
                            {
                                sd.Level = Constants.DEFAULT_LEVEL;
                            }

                            sd.StrData = elm.strAliasData;
                        }
                        else
                        {
                            sd.Name = elm.strAliasType;
                            if (slevel == -1)
                            {
                                sd.Level = elm.dblAliasLevel;
                            }
                            else if (elm.blnAliasLevelIsPlusMod)
                            {
                                sd.Level = slevel + elm.dblAliasLevel;
                            }
                            else if (elm.blnAliasLevelIsMultMod)
                            {
                                sd.Level = slevel * elm.dblAliasLevel;
                            }
                            else
                            {
                                sd.Level = slevel;
                            }

                            if (string.IsNullOrEmpty(sdata))
                            {
                                sd.StrData = elm.strAliasData;
                            }
                            else
                            {
                                sd.StrData = Strings.Trim(sdata + " " + GeneralLib.ListTail(elm.strAliasData, 2));
                            }

                            if (elm.blnAliasLevelIsPlusMod || elm.blnAliasLevelIsPlusMod)
                            {
                                sd.StrData = Strings.Trim(GeneralLib.LIndex(sd.StrData, 1) + "Lv" + SrcFormatter.Format(slevel) + " " + GeneralLib.ListTail(sd.StrData, 2));
                            }
                        }
                    }
                }
            }
            else
            {
                sList.Add(new SkillData
                {
                    Name = sname,
                    Level = slevel,
                    StrData = sdata,
                });
            }

            foreach (var sd in sList)
            {
                sname = sd.Name;
                slevel = sd.Level;
                sdata = sd.StrData;
                if (string.IsNullOrEmpty(sname))
                {
                    continue;
                }

                string slist;
                // アビリティ一覧表示用にSetSkillが適用された能力の一覧用変数を作成
                if (!Expression.IsGlobalVariableDefined("Ability(" + pname + ")"))
                {
                    Expression.DefineGlobalVariable("Ability(" + pname + ")");
                    slist = sname;
                }
                else
                {
                    slist = Conversions.ToString(Event.GlobalVariableList["Ability(" + pname + ")"].StringValue);
                    if (!GeneralLib.ToL(slist).Contains(sname))
                    {
                        slist = slist + " " + sname;
                    }
                }

                Expression.SetVariableAsString("Ability(" + pname + ")", slist);

                // 今回SetSkillが適用された能力sname用変数を作成
                var vname = "Ability(" + pname + "," + sname + ")";
                if (!Expression.IsGlobalVariableDefined(vname))
                {
                    Expression.DefineGlobalVariable(vname);
                }

                if (!string.IsNullOrEmpty(sdata))
                {
                    // 別名指定があった場合
                    Expression.SetVariableAsString(vname, SrcFormatter.Format(slevel) + " " + sdata);

                    // 必要技能用
                    if (sdata != "非表示" && GeneralLib.LIndex(sdata, 1) != "解説")
                    {
                        vname = "Ability(" + pname + "," + GeneralLib.LIndex(sdata, 1) + ")";
                        if (!Expression.IsGlobalVariableDefined(vname))
                        {
                            Expression.DefineGlobalVariable(vname);
                        }

                        Expression.SetVariableAsString(vname, SrcFormatter.Format(slevel));
                    }
                }
                else
                {
                    Expression.SetVariableAsString(vname, SrcFormatter.Format(slevel));
                }
            }

            // パイロットやユニットのステータスをアップデート
            if (SRC.PList.IsDefined(pname))
            {
                {
                    var withBlock1 = SRC.PList.Item(pname);
                    withBlock1.Update();
                    if (withBlock1.Unit is object)
                    {
                        withBlock1.Unit.Update();
                        if (withBlock1.Unit.Status == "出撃")
                        {
                            SRC.PList.UpdateSupportMod(withBlock1.Unit);
                        }
                    }
                }
            }
            return EventData.NextID;
        }
    }
}
