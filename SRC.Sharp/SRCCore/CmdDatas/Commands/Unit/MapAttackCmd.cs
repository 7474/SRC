using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class MapAttackCmd : CmdData
    {
        public MapAttackCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MapAttackCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var num = ArgNum;
            var is_event = true;
            string wname;
            Unit u;
            UnitWeapon uw;
            int tx, ty;
            if (num <= 6)
            {
                if (GetArgAsString(num) == "通常戦闘")
                {
                    is_event = false;
                    num = (num - 1);
                }
            }

            switch (num)
            {
                case 5:
                    {
                        u = GetArgAsUnit(2);
                        wname = GetArgAsString(3);
                        uw = u.Weapons.FirstOrDefault(x => x.Name == wname && x.IsWeaponClassifiedAs("Ｍ"));
                        if (uw == null)
                        {
                            throw new EventErrorException(this, "マップ攻撃名が間違っています");
                        }

                        tx = GetArgAsLong(4);
                        if (tx < 1)
                        {
                            tx = 1;
                        }
                        else if (tx > Map.MapWidth)
                        {
                            tx = Map.MapWidth;
                        }

                        ty = GetArgAsLong(5);
                        if (ty < 1)
                        {
                            ty = 1;
                        }
                        else if (ty > Map.MapHeight)
                        {
                            ty = Map.MapHeight;
                        }

                        break;
                    }

                case 4:
                    {
                        u = Event.SelectedUnitForEvent;
                        wname = GetArgAsString(2);
                        uw = u.Weapons.FirstOrDefault(x => x.Name == wname && x.IsWeaponClassifiedAs("Ｍ"));
                        if (uw == null)
                        {
                            throw new EventErrorException(this, "マップ攻撃名が間違っています");
                        }

                        tx = GetArgAsLong(3);
                        if (tx < 1)
                        {
                            tx = 1;
                        }
                        else if (tx > Map.MapWidth)
                        {
                            tx = Map.MapWidth;
                        }

                        ty = GetArgAsLong(4);
                        if (ty < 1)
                        {
                            ty = 1;
                        }
                        else if (ty > Map.MapHeight)
                        {
                            ty = Map.MapHeight;
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "MapAttackコマンドの引数の数が違います");
            }

            if (u.Status != "出撃")
            {
                throw new EventErrorException(this, u.Nickname + "は出撃していません");
            }

            // ステージを仮想的に変更しておく
            var cur_stage = SRC.Stage;
            SRC.Stage = u.Party;
            var prev_w = Commands.SelectedWeapon;
            var prev_tw = Commands.SelectedTWeapon;
            Commands.SelectedWeapon = uw.WeaponNo();
            Commands.SelectedTWeapon = 0;
            Commands.SelectedX = tx;
            Commands.SelectedY = ty;
            u.MapAttack(uw, tx, ty, is_event);
            Commands.SelectedWeapon = prev_w;
            Commands.SelectedTWeapon = prev_tw;
            SRC.Stage = cur_stage;
            GUI.RedrawScreen();
            return EventData.NextID;
        }
    }
}
