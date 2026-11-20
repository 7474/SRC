// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;

namespace SRCCore.Units
{
    // 元の SRC（VB5）のセーブデータのユニットを読む（Units.cls の Load・LoadLinkInfo・Restore・RestoreLinkInfo・RestoreParameter。#1577）
    public partial class Units
    {
        // ユニットリストにユニットの情報をロード (リンクは後で行う)（VB: Load）
        public void Load(VbSaveDataReader reader)
        {
            IDCount = reader.InputInt();
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                // Name
                var name = reader.Input();
                if (!SRC.UDList.IsDefined(name))
                {
                    throw SRC.TerminateLoading(name + "のデータが定義されていません");
                }

                // ID, Rank, Status
                var id = reader.Input();
                var rank = reader.InputInt();
                var status = reader.Input();

                // 旧形式のユニットＩＤを新形式に変換
                if (reader.SaveDataVersion < 10700)
                {
                    id = SRC.ConvertUnitID(id);
                }

                var new_unit = new Unit(SRC)
                {
                    Name = name,
                    ID = id,
                    Rank = rank,
                    Party = "味方",
                    Status = status,
                };
                new_unit.FullRecover();
                colUnits.Add(new_unit, new_unit.ID);

                // OtherForm・Pilot・Support・Item
                for (var k = 1; k <= 4; k++)
                {
                    var num2 = reader.InputInt();
                    for (var j = 1; j <= num2; j++)
                    {
                        reader.LineInput();
                    }
                }
            }
        }

        // ユニットリストにユニットの情報をロードし、リンクを行う（VB: LoadLinkInfo）
        public void LoadLinkInfo(VbSaveDataReader reader)
        {
            IDCount = reader.InputInt();
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                // Name
                reader.LineInput();
                // ID, Rank, Status
                var id = reader.Input();
                reader.InputInt();
                reader.Input();

                // 旧形式のユニットＩＤを新形式に変換
                if (reader.SaveDataVersion < 10700)
                {
                    id = SRC.ConvertUnitID(id);
                }

                var u = Item(id);

                // OtherForm
                var num2 = reader.InputInt();
                for (var j = 1; j <= num2; j++)
                {
                    var id2 = SRC.ConvertUnitID(reader.Input());
                    if (IsDefined(id2))
                    {
                        u.AddOtherForm(Item(id2));
                    }
                }

                // Pilot
                num2 = reader.InputInt();
                for (var j = 1; j <= num2; j++)
                {
                    var id2 = reader.Input();
                    if (!SRC.PList.IsDefined(id2))
                    {
                        id2 = Pilots.Pilots.LeftOfParenthesis(id2);
                        if (!SRC.PList.IsDefined(id2))
                        {
                            continue;
                        }
                    }
                    u.AddPilot(SRC.PList.Item(id2));
                    if (u.Status == "離脱")
                    {
                        SRC.PList.Item(id2).Away = true;
                    }
                }

                // Support
                num2 = reader.InputInt();
                for (var j = 1; j <= num2; j++)
                {
                    var id2 = reader.Input();
                    if (SRC.PList.IsDefined(id2))
                    {
                        u.AddSupport(SRC.PList.Item(id2));
                        if (u.Status == "離脱")
                        {
                            SRC.PList.Item(id2).Away = true;
                        }
                    }
                }

                // Item
                num2 = reader.InputInt();
                for (var j = 1; j <= num2; j++)
                {
                    var id2 = reader.Input();
                    if (SRC.IList.IsDefined(id2))
                    {
                        if (SRC.IList.Item(id2).Unit is null)
                        {
                            u.CurrentForm().AddItem0(SRC.IList.Item(id2));
                        }
                    }
                    else if (SRC.IDList.IsDefined(id2))
                    {
                        u.CurrentForm().AddItem0(SRC.IList.Add(id2));
                    }
                }
            }

            foreach (var u in colUnits.List)
            {
                u.Update(true);
            }
        }

        // 一時中断用データをファイルからロードする（VB: Restore）
        public void Restore(VbSaveDataReader reader)
        {
            colUnits.Clear();

            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                var u = new Unit(SRC);
                u.Restore(reader);
                colUnits.Add(u, u.ID);
            }
        }

        // 一時中断用データのリンク情報をファイルからロードする（VB: RestoreLinkInfo）
        public void RestoreLinkInfo(VbSaveDataReader reader)
        {
            reader.InputInt();
            foreach (var u in colUnits.List)
            {
                u.RestoreLinkInfo(reader);
            }
        }

        // 一時中断用データのパラメータ情報をファイルからロードする（VB: RestoreParameter）
        public void RestoreParameter(VbSaveDataReader reader)
        {
            reader.InputInt();
            foreach (var u in colUnits.List)
            {
                u.RestoreParameter(reader);
            }
        }
    }
}
