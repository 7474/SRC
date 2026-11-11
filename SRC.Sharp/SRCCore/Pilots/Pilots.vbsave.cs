// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.Pilots
{
    // 元の SRC（VB5）のセーブデータのパイロットを読む（Pilots.cls の Load・LoadLinkInfo・Restore・RestoreLinkInfo・RestoreParameter。#1577）
    public partial class Pilots
    {
        // ファイルからデータをロード (リンクは行わない)（VB: Load）
        public void Load(VbSaveDataReader reader)
        {
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                // Name
                var pname = reader.Input();
                // Level, Exp
                var plevel = reader.InputInt();
                var pexp = reader.InputInt();
                // Unit
                reader.Input();

                if (GeneralLib.LLength(pname) == 1)
                {
                    if (!SRC.PDList.IsDefined(pname))
                    {
                        if (Strings.InStr(pname, "(") > 0)
                        {
                            pname = Strings.Left(pname, Strings.InStr(pname, "(") - 1);
                        }

                        if (!SRC.PDList.IsDefined(pname))
                        {
                            throw SRC.TerminateLoading(pname + "のデータが定義されていません");
                        }
                    }

                    Add(pname, plevel, "味方").Exp = pexp;
                }
                else
                {
                    if (!SRC.PDList.IsDefined(GeneralLib.LIndex(pname, 1)))
                    {
                        throw SRC.TerminateLoading(GeneralLib.LIndex(pname, 1) + "のデータが定義されていません");
                    }

                    Add(GeneralLib.LIndex(pname, 1), plevel, "味方", GeneralLib.LIndex(pname, 2)).Exp = pexp;
                }
            }
        }

        // ファイルからデータを読み込みリンク（VB: LoadLinkInfo）
        public void LoadLinkInfo(VbSaveDataReader reader)
        {
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                // Name
                var pname = reader.Input();
                // Level, Exp
                reader.LineInput();
                // Unit
                var uid = reader.Input();

                if (GeneralLib.LLength(pname) == 1)
                {
                    if (!IsDefined(pname))
                    {
                        pname = LeftOfParenthesis(pname);
                    }
                }

                var p = GeneralLib.LLength(pname) == 1 ? Item(pname) : Item(GeneralLib.LIndex(pname, 2));
                switch (uid)
                {
                    case "離脱":
                        // Leaveされたパイロット
                        p.Away = true;
                        continue;

                    case "-":
                    case "Dummy":
                        // ユニットに乗っていないパイロット
                        continue;
                }

                // 旧形式のユニットＩＤを新形式に変換
                if (reader.SaveDataVersion < 10700)
                {
                    uid = SRC.ConvertUnitID(uid);
                }

                if (SRC.UList.IsDefined(uid))
                {
                    // パイロットをユニットに乗せる
                    p.Unit = SRC.UList.Item(uid);
                }
                else
                {
                    // 乗せるべきユニットが見つからなかった場合は強制的にユニットを
                    // 作って乗せる (バグ対策だったけど……不要？)
                    var ret = Strings.InStr(uid, ":");
                    if (ret == 0)
                    {
                        // VB: Left$ の長さが負になり、実行時エラー 5
                        throw new System.ArgumentException("ユニットのＩＤ「" + uid + "」に「:」がありません");
                    }
                    uid = Strings.Left(uid, ret - 1);
                    if (SRC.UDList.IsDefined(uid))
                    {
                        var u = SRC.UList.Add(uid, 0, "味方");
                        p.Ride(u);
                    }
                }
            }
        }

        // 「(」より前（VB: Left$(pname, InStr(pname, "(") - 1)。「(」が無ければ実行時エラー 5）
        internal static string LeftOfParenthesis(string name)
        {
            var ret = Strings.InStr(name, "(");
            if (ret == 0)
            {
                throw new System.ArgumentException("「" + name + "」が見つかりません");
            }
            return Strings.Left(name, ret - 1);
        }

        // 一時中断用データをファイルからロードする（VB: Restore）
        public void Restore(VbSaveDataReader reader)
        {
            colPilots.Clear();

            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                var p = Pilot.Restore(SRC, reader);
                colPilots.Add(p, p.ID);
            }
        }

        // 一時中断用データのリンク情報をファイルからロードする（VB: RestoreLinkInfo）
        public void RestoreLinkInfo(VbSaveDataReader reader)
        {
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                colPilots.List[i - 1].RestoreLinkInfo(reader);
            }
        }

        // 一時中断用データのパラメータ情報をファイルからロードする（VB: RestoreParameter）
        public void RestoreParameter(VbSaveDataReader reader)
        {
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                colPilots.List[i - 1].RestoreParameter(reader);
            }
        }
    }
}
