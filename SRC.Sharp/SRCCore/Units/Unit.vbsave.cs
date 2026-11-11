// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.Units
{
    // 元の SRC（VB5）の中断データのユニットを読む（Unit.cls の Restore・RestoreLinkInfo・RestoreParameter。#1577）
    public partial class Unit
    {
        // 一時中断用データをファイルからロードする（VB: Restore）
        public void Restore(VbSaveDataReader reader)
        {
            // Name, ID, Party
            var sbuf = reader.Input();
            strName = sbuf;
            Data = SRC.UDList.Item(sbuf);
            ID = reader.Input();
            strParty = reader.Input();

            // Rank, BossRank, X, Y
            intRank = reader.InputInt();
            intBossRank = reader.InputInt();
            x = reader.InputInt();
            y = reader.InputInt();

            // Area, Action, Mode, Status
            Area = reader.Input();
            UsedAction = reader.InputInt();
            strMode = reader.Input();
            Status = reader.Input();

            // Master, Summoner
            reader.Input();
            reader.Input();

            // UsedSupportAttack, UsedSupportGuard
            UsedSupportAttack = reader.InputInt();
            UsedSupportGuard = reader.InputInt();

            // UsedSyncAttack, UsedCounterAttack
            UsedSyncAttack = reader.InputInt();
            UsedCounterAttack = reader.InputInt();

            // OtherForm
            SkipLines(reader);
            // UnitOnBoard
            SkipLines(reader);

            // Pilot
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                AddPilot(SRC.PList.Item(reader.Input()));
            }

            // Support
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                AddSupport(SRC.PList.Item(reader.Input()));
            }

            // AdditionalSupport
            if (reader.SaveDataVersion >= 20210)
            {
                sbuf = reader.Input();
                if (!string.IsNullOrEmpty(sbuf))
                {
                    pltAdditionalSupport = SRC.PList.Item(sbuf);
                    pltAdditionalSupport.Unit = this;
                    pltAdditionalSupport.IsAdditionalSupport = true;
                }
            }

            // Servant
            SkipLines(reader);
            // Item
            SkipLines(reader);

            // SpecialPowerInEffect
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                var ctype = reader.Input();
                var cdata = reader.Input();
                MakeSpecialPowerInEffect(ctype, cdata);
            }

            // Condition
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                sbuf = reader.LineInput();

                // 付加した特殊能力のデータに「"」や「,」が含まれているとデータの処理が
                // 正しく行えないので手動でパーシング
                var ret = Strings.InStr(sbuf, ",");
                var ctype = Strings.Left(sbuf, ret - 1);
                if (Strings.Left(ctype, 1) == "\"")
                {
                    ctype = Strings.Mid(ctype, 2, Strings.Len(ctype) - 2);
                }
                sbuf = Strings.Mid(sbuf, ret + 1);

                ret = Strings.InStr(sbuf, ",");
                var cltime = GeneralLib.StrToLng(Strings.Left(sbuf, ret - 1));
                sbuf = Strings.Mid(sbuf, ret + 1);

                ret = Strings.InStr(sbuf, ",");
                double clevel = GeneralLib.StrToLng(Strings.Left(sbuf, ret - 1));
                sbuf = Strings.Mid(sbuf, ret + 1);

                var cdata = sbuf;
                if (Strings.Left(cdata, 1) == "\"")
                {
                    cdata = Strings.Mid(cdata, 2, Strings.Len(cdata) - 2);
                }

                if (reader.SaveDataVersion < 10741)
                {
                    if (Strings.InStr(cdata, " パイロット能力付加 ") > 0)
                    {
                        continue;
                    }
                    if (Strings.InStr(cdata, " パイロット能力強化 ") > 0)
                    {
                        continue;
                    }
                }

                AddCondition(ctype, cltime, clevel, cdata);
            }

            // Weapon
            SkipLines(reader);
            // Ability
            SkipLines(reader);

            // HP, EN
            var lbuf = reader.InputInt();
            var ibuf = reader.InputInt();
            HP = lbuf;
            EN = ibuf;
        }

        // 一時中断用データのリンク情報をファイルからロードする（VB: RestoreLinkInfo）
        public void RestoreLinkInfo(VbSaveDataReader reader)
        {
            // Name, ID, Party
            reader.LineInput();
            // Rank, BossRank, X, Y
            reader.LineInput();
            // Area, Action, Mode, Status
            reader.LineInput();

            // Master, Summoner
            Master = SRC.UList.Item(reader.Input());
            Summoner = SRC.UList.Item(reader.Input());

            // SupportAttackStock, SupportGuardStock
            reader.LineInput();
            // UsedSyncAttack, UsedCounterAttack
            reader.LineInput();

            // OtherForm
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                AddOtherForm(SRC.UList.Item(reader.Input()));
            }

            // UnitOnBoard
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                LoadUnit(SRC.UList.Item(reader.Input()));
            }

            // Pilot
            SkipLines(reader);
            // Support
            SkipLines(reader);

            // AdditionalSupport
            if (reader.SaveDataVersion >= 20210)
            {
                reader.LineInput();
            }

            // Servant
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                AddServant(SRC.UList.Item(reader.Input()));
            }

            // Item
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                AddItem0(SRC.IList.Item(reader.Input()));
            }

            // SpecialPowerInEffect
            SkipLines(reader);
            // Condition
            SkipLines(reader);

            Update(true);

            // Weapon
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                var ibuf = reader.InputInt();
                if (i <= CountWeapon())
                {
                    Weapon(i).SetBullet(ibuf);
                }
            }

            // Ability
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                var ibuf = reader.InputInt();
                if (i <= CountAbility())
                {
                    Ability(i).SetStock(ibuf);
                }
            }

            // HP, EN
            var lbuf = reader.InputInt();
            var ebuf = reader.InputInt();
            HP = lbuf;
            EN = ebuf;
        }

        // 一時中断用データのパラメータ情報をファイルからロードする（VB: RestoreParameter）
        public void RestoreParameter(VbSaveDataReader reader)
        {
            // Name, ID, Party
            reader.LineInput();
            // Rank, BossRank, X, Y
            reader.LineInput();
            // Area, Action, Mode, Status
            reader.LineInput();
            // Master
            reader.LineInput();
            // Summoner
            reader.LineInput();
            // SupportAttackStock, SupportGuardStock
            reader.LineInput();
            // UsedSyncAttack, UsedCounterAttack
            reader.LineInput();

            // OtherForm
            SkipLines(reader);
            // UnitOnBoard
            SkipLines(reader);
            // Pilot
            SkipLines(reader);
            // Support
            SkipLines(reader);

            // AdditionalSupport
            if (reader.SaveDataVersion >= 20210)
            {
                reader.LineInput();
            }

            // Servant
            SkipLines(reader);
            // Item
            SkipLines(reader);
            // SpecialPowerInEffect
            SkipLines(reader);
            // Condition
            SkipLines(reader);
            // Weapon
            SkipLines(reader);
            // Ability
            SkipLines(reader);

            // HP, EN
            var lbuf = reader.InputInt();
            var ibuf = reader.InputInt();
            HP = lbuf;
            EN = ibuf;

            Update();
        }

        // 数を読み、その数の行を読み飛ばす
        private static void SkipLines(VbSaveDataReader reader)
        {
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                reader.LineInput();
            }
        }
    }
}
