// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Expressions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Events
{
    // 元の SRC（VB5）のセーブデータのイベントのデータ（変数・ラベル・Require のファイル）を読む（Event.bas。#1577）
    public partial class Event
    {
        // 一時中断用データをファイルからロードする（VB: RestoreEventData）
        public void RestoreEventData(VbSaveDataReader reader)
        {
            // グローバル変数
            LoadGlobalVariables(reader);
            // ローカル変数
            LoadLocalVariables(reader);

            // イベント用ラベル
            var num = reader.InputInt();
            var label_enabled = new List<bool>();
            for (var i = 1; i <= num; i++)
            {
                label_enabled.Add(reader.InputBool());
            }

            // Requireコマンドで追加されたイベントファイル。
            // 元の SRC は数が 0 のとき（と、書いていない古い版のとき）はいまのファイル名の一覧をそのままにする
            var files = AdditionalEventFileNames?.ToList() ?? new List<string>();
            if (reader.SaveDataVersion > 20003)
            {
                // 追加するイベントファイル数
                num = reader.InputInt();
                if (num > 0)
                {
                    files = new List<string>();
                    for (var i = 1; i <= num; i++)
                    {
                        files.Add(reader.Input());
                    }
                }
            }

            // イベントファイルを読み込み、イベント用ラベルを設定する（JSON から読むときと同じ）
            Restore(new SRCSuspendData
            {
                AdditionalEventFileNames = files,
                EventLabelEnabled = label_enabled,
            });
        }

        // 一時中断用データのイベントデータ部分を読み飛ばす（VB: SkipEventData）
        public static void SkipEventData(VbSaveDataReader reader)
        {
            // グローバル変数
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                reader.LineInput();
            }
            // ローカル変数
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                reader.LineInput();
            }

            // ラベル情報
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                reader.LineInput();
            }

            // Requireコマンドで読み込んだイベントデータ
            if (reader.SaveDataVersion > 20003)
            {
                num = reader.InputInt();
                for (var i = 1; i <= num; i++)
                {
                    reader.LineInput();
                }
            }
        }

        // 変数の値を読む。「"」で始まれば文字列（「""」は「"」にする）、そうでなければ数値（VB: LoadGlobalVariables・LoadLocalVariables）。
        // 元の SRC は数値の変数も Format$ の文字列で書くので、元の SRC が書いた変数は文字列の変数として読まれる
        private static string ReadVariableValue(VbSaveDataReader reader, out bool is_number)
        {
            var buf = reader.LineInput();
            if (Strings.Left(buf, 1) == "\"")
            {
                is_number = false;
                return Strings.Mid(buf, 2, Strings.Len(buf) - 2).Replace("\"\"", "\"");
            }
            is_number = true;
            return buf;
        }

        // 変数に値を設定する（VB: With GlobalVariableList.Item(vname) の StringValue・VariableType・NumericValue）
        private static void SetLoadedValue(VarData v, string vvalue, bool is_number)
        {
            if (is_number)
            {
                v.SetValue(v.Name, ValueType.NumericType, vvalue, Conversions.ToDouble(vvalue));
            }
            else
            {
                v.SetValue(v.Name, ValueType.StringType, vvalue, v.NumericValue);
            }
        }

        // グローバル変数をファイルからロード（VB: LoadGlobalVariables）
        public void LoadGlobalVariables(VbSaveDataReader reader)
        {
            // グローバル変数を全削除
            GlobalVariableList.Clear();

            // グローバル変数の総数を読み出し
            var num = reader.InputInt();

            // 各変数の値を読み出し
            for (var i = 1; i <= num; i++)
            {
                var vname = reader.Input();
                var vvalue = ReadVariableValue(reader, out var is_number);
                string aname, buf;
                int idx;

                if (reader.SaveDataVersion < 10724)
                {
                    // SetSkillコマンドのセーブデータをエリアスに対応させる
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        idx = Strings.InStr(vname, ",");
                        if (idx > 0)
                        {
                            // 個々の能力定義
                            aname = Strings.Mid(vname, idx + 1, Strings.Len(vname) - idx - 1);
                            if (SRC.ALDList.IsDefined(aname))
                            {
                                vname = Strings.Left(vname, idx) + SRC.ALDList.Item(aname).Elements.First().strAliasType + ")";
                                if (GeneralLib.LLength(vvalue) == 1)
                                {
                                    vvalue = vvalue + " " + aname;
                                }
                            }
                        }
                        else
                        {
                            // 必要技能用の能力一覧
                            buf = "";
                            for (var j = 1; j <= GeneralLib.LLength(vvalue); j++)
                            {
                                aname = GeneralLib.LIndex(vvalue, j);
                                if (SRC.ALDList.IsDefined(aname))
                                {
                                    aname = SRC.ALDList.Item(aname).Elements.First().strAliasType;
                                }
                                buf = buf + " " + aname;
                            }
                            vvalue = Strings.Trim(buf);
                        }
                    }
                }

                if (reader.SaveDataVersion < 10730)
                {
                    // ラーニングした特殊能力が使えないバグに対応
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        idx = Strings.InStr(vname, ",");
                        if (idx > 0)
                        {
                            var vname2 = Strings.Left(vname, idx - 1) + ")";
                            aname = Strings.Mid(vname, idx + 1, Strings.Len(vname) - idx - 1);
                            if (!Expression.IsGlobalVariableDefined(vname2))
                            {
                                Expression.DefineGlobalVariable(vname2);
                                var v2 = GlobalVariableList[vname2];
                                v2.SetValue(v2.Name, v2.VariableType, aname, v2.NumericValue);
                            }
                        }
                    }
                }

                if (reader.SaveDataVersion < 10731)
                {
                    // 不必要な非表示能力に対するSetSkillを削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (Strings.Right(vname, 5) == ",非表示)")
                        {
                            continue;
                        }
                    }
                }

                if (reader.SaveDataVersion < 10732)
                {
                    // 不必要な非表示能力に対するSetSkillと能力名のダブりを削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (Strings.InStr(vname, ",") == 0)
                        {
                            buf = "";
                            for (var j = 1; j <= GeneralLib.LLength(vvalue); j++)
                            {
                                aname = GeneralLib.LIndex(vvalue, j);
                                if (aname != "非表示")
                                {
                                    int k;
                                    for (k = 1; k <= GeneralLib.LLength(buf); k++)
                                    {
                                        if (GeneralLib.LIndex(buf, k) == aname)
                                        {
                                            break;
                                        }
                                    }
                                    if (k > GeneralLib.LLength(buf))
                                    {
                                        buf = buf + " " + aname;
                                    }
                                }
                            }
                            vvalue = Strings.Trim(buf);
                        }
                    }
                }

                if (reader.SaveDataVersion < 20027)
                {
                    // エリアスされた能力をSetSkillした際にエリアスに含まれる解説が無効になるバグへの対処
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (GeneralLib.LIndex(vvalue, 1) == "0")
                        {
                            if (GeneralLib.LIndex(vvalue, 2) == "解説")
                            {
                                vvalue = SrcFormatter.Format(Constants.DEFAULT_LEVEL) + " 解説 " + GeneralLib.ListTail(vvalue, 3);
                            }
                        }
                    }
                }

                if (!Expression.IsGlobalVariableDefined(vname))
                {
                    Expression.DefineGlobalVariable(vname);
                }
                SetLoadedValue(GlobalVariableList[vname], vvalue, is_number);
            }
            // 元の SRC はここで新ＧＵＩのモード（SetNewGUIMode）を確かめる。SRC# は新ＧＵＩに対応していないので見ない
        }

        // ローカル変数をファイルからロード（VB: LoadLocalVariables）
        public void LoadLocalVariables(VbSaveDataReader reader)
        {
            // ローカル変数を全削除
            LocalVariableList.Clear();

            // ローカル変数の総数を読み出し
            var num = reader.InputInt();

            for (var i = 1; i <= num; i++)
            {
                // 変数の値を読み出し
                var vname = reader.Input();
                var vvalue = ReadVariableValue(reader, out var is_number);

                if (reader.SaveDataVersion < 10731)
                {
                    // ClearSkillのバグで設定された変数を削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (vname == vvalue)
                        {
                            continue;
                        }
                    }
                }

                // 変数の値を設定
                if (!Expression.IsLocalVariableDefined(vname))
                {
                    Expression.DefineLocalVariable(vname);
                }
                SetLoadedValue(LocalVariableList[vname], vvalue, is_number);
            }
        }
    }
}
