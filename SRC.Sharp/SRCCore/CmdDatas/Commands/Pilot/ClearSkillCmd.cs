using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearSkillCmd : CmdData
    {
        public ClearSkillCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearSkillCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
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

            var sname0 = GetArgAsString(3);

            // エリアスが定義されている？
            var sarray = new List<string>();
            if (SRC.ALDList.IsDefined(sname0))
            {
                var alias = SRC.ALDList.Item(sname0);
                sarray = alias.Elements.Select(x => x.strAliasType).ToList();
            }
            else
            {
                sarray.Add(sname0);
            }

            foreach (var sname in sarray)
            {
                var sname2 = "";
                var vname = "Ability(" + pname + "," + sname + ")";
                if (GeneralLib.LLength(Expression.GetValueAsString(vname)) >= 2)
                {
                    // 必要技能用変数を削除
                    sname2 = GeneralLib.LIndex(Expression.GetValueAsString(vname), 2);
                    var vname2 = "Ability(" + pname + "," + sname2 + ")";
                    Expression.UndefineVariable(vname2);
                }

                // レベル設定用変数を削除
                Expression.UndefineVariable(vname);

                // 特殊能力一覧作成用変数を削除
                vname = "Ability(" + pname + ")";
                if (Expression.IsGlobalVariableDefined(vname))
                {
                    var buf = Expression.GetValueAsString(vname);
                    var slist = "";
                    var loopTo2 = GeneralLib.LLength(buf);
                    for (var j = 1; j <= loopTo2; j++)
                    {
                        if ((GeneralLib.LIndex(buf, j) ?? "") != (sname ?? "") && (GeneralLib.LIndex(buf, j) ?? "") != (sname2 ?? ""))
                        {
                            slist = slist + " " + GeneralLib.LIndex(buf, j);
                        }
                    }

                    if (GeneralLib.LLength(slist) > 0)
                    {
                        slist = Strings.Trim(slist);
                        Expression.SetVariableAsString(vname, slist);
                    }
                    else
                    {
                        Expression.UndefineVariable(vname);
                    }
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
