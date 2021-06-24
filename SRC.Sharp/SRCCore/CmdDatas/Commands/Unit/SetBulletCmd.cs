using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class SetBulletCmd : CmdData
    {
        public SetBulletCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetBulletCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string wname;
            int wid, num;
            Unit u;
            switch (ArgNum)
            {
                case 4:
                    {
                        u = GetArgAsUnit(2);
                        wname = GetArgAsString(3);
                        num = GetArgAsLong(4);
                        break;
                    }

                case 3:
                    {
                        u = Event.SelectedUnitForEvent;
                        wname = GetArgAsString(2);
                        num = GetArgAsLong(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "SetBulletコマンドの引数の数が違います");
            }

            if (Information.IsNumeric(wname))
            {
                wid = GeneralLib.StrToLng(wname);
                if (wid < 1 || u.CountWeapon() < wid)
                {
                    throw new EventErrorException(this, "武器の番号「" + wname + "」が間違っています");
                }
            }
            else
            {
                var loopTo = u.CountWeapon();
                for (wid = 1; wid <= loopTo; wid++)
                {
                    if ((u.Weapon(wid).Name ?? "") == (wname ?? ""))
                    {
                        break;
                    }
                }

                if (wid < 1 || u.CountWeapon() < wid)
                {
                    throw new EventErrorException(this, u.Name + "は武器「" + wname + "」を持っていません");
                }
            }

            var uw = u.Weapon(wid);
            uw.SetBullet(GeneralLib.MinLng(num, uw.MaxBullet()));
            return EventData.NextID;
        }
    }
}
