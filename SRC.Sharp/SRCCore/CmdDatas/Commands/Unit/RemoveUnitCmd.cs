using SRCCore.Events;
using SRCCore.Exceptions;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveUnitCmd : CmdData
    {
        public RemoveUnitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveUnitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var num = ArgNum;
            var opt = "";
            if (num > 1)
            {
                if (GetArgAsString(num) == "非同期")
                {
                    opt = "非同期";
                    num = (num - 1);
                }
            }

            switch (num)
            {
                case 1:
                    {
                        {
                            var u = Event.SelectedUnitForEvent.CurrentForm();
                            u.Escape(opt);
                            if (u.CountPilot() > 0)
                            {
                                u.Pilots.First().GetOff();
                            }

                            u.Status = "破棄";
                            foreach (var cf in u.OtherForms)
                            {
                                if (cf.Status == "他形態")
                                {
                                    cf.Status = "破棄";
                                }
                            }
                        }

                        break;
                    }

                case 2:
                    {
                        var uname = GetArgAsString(2);
                        var u = SRC.UList.Item(uname);

                        // ユニットが存在しなければそのまま終了
                        if (u is null)
                        {
                            return EventData.NextID;
                        }

                        // ユニットＩＤで指定された場合
                        if ((u.ID ?? "") == (uname ?? ""))
                        {
                            u.Escape(opt);
                            if (u.CountPilot() > 0)
                            {
                                u.Pilots.First().GetOff();
                            }

                            u.Status = "破棄";
                            foreach (var cf in u.OtherForms)
                            {
                                if (cf.Status == "他形態")
                                {
                                    cf.Status = "破棄";
                                }
                            }

                            return EventData.NextID;
                        }

                        // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
                        // 名前をデータのそれとあわせる
                        if (SRC.UDList.IsDefined(uname))
                        {
                            uname = SRC.UDList.Item(uname).Name;
                        }

                        // パイロットが乗ってないユニットを優先
                        u = SRC.UList.Items.Select(u => u.CurrentForm())
                            .Where(u => u.Status == "破棄")
                            .FirstOrDefault(u => u.CountPilot() == 0);
                        // 見つからなければパイロットが乗っているユニットを削除
                        if (u == null)
                        {
                            u = SRC.UList.Items.Select(u => u.CurrentForm())
                                .FirstOrDefault(u => u.Status == "破棄");
                        }
                        if (u != null)
                        {
                            u.Escape(opt);
                            if (u.CountPilot() > 0)
                            {
                                u.Pilots.First().GetOff();
                            }

                            u.Status = "破棄";
                            foreach (var cf in u.OtherForms)
                            {
                                if (cf.Status == "他形態")
                                {
                                    cf.Status = "破棄";
                                }
                            }
                        }
                        break;
                    }

                default:
                    throw new EventErrorException(this, "RemoveUnitの引数の数が違います");
            }

            return EventData.NextID;
        }
    }
}
