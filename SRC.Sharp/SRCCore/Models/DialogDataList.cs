// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Models
{
    // 全ダイアログデータを管理するリストのクラス
    public class DialogDataList
    {
        private SRC SRC;
        public DialogDataList(SRC src)
        {
            SRC = src;
        }

        // ダイアログデータのコレクション
        private SrcCollection<DialogData> colDialogDataList = new SrcCollection<DialogData>();

        public IList<DialogData> Items => colDialogDataList.List;

        // ダイアログデータを追加
        public DialogData Add(string dname)
        {
            var new_dd = new DialogData(SRC);
            new_dd.Name = dname;
            colDialogDataList.Add(new_dd, dname);
            return new_dd;
        }

        // ダイアログデータの総数
        public int Count()
        {
            return colDialogDataList.Count;
        }

        public void Delete(string Index)
        {
            colDialogDataList.Remove(Index);
        }

        // ダイアログデータを返す
        public DialogData Item(string Index)
        {
            return colDialogDataList[Index];
        }

        // 指定したダイアログデータが定義されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // データファイル fname からデータをロード
        public void Load(string fname)
        {
            using (var stream = SRC.FileSystem.OpenText(SRC.SystemConfig.SRCCompatibilityMode, fname))
            {
                Load(fname, stream);
            }
        }

        public void Load(string fname, Stream stream)
        {
            using (var reader = new SrcDataReader(fname, stream))
            {
                string line_buf = "";
                while (reader.HasMore)
                {
                    line_buf = reader.GetLine();
                    while (reader.HasMore && string.IsNullOrEmpty(line_buf))
                    {
                        line_buf = reader.GetLine();
                    }

                    // パイロット名一覧
                    var pilot_list = GeneralLib.LNormalize(line_buf);
                    if (IsDefined(pilot_list))
                    {
                        Delete(pilot_list);
                    }


                    DialogData dd = Add(pilot_list);
                    line_buf = reader.GetLine();
                    while (reader.HasMore && Strings.Len(line_buf) > 0)
                    {
                        // シチューション
                        var d = dd.AddDialog(line_buf);
                        while (reader.HasMore)
                        {
                            line_buf = reader.GetLine();

                            // 話者
                            var ret = Strings.InStr(line_buf, ",");
                            if (ret == 0)
                            {
                                // 次のシチュエーションないしダイアログ
                                break;
                            }

                            var pname = Strings.Left(line_buf, ret - 1);

                            // 指定した話者のデータが存在するかチェック。
                            // ただし合体技のパートナーは場合は他の作品のパイロットであることも
                            // あるので話者チェックを行わない。
                            if (Strings.Left(pname, 1) != "@")
                            {
                                if (!SRC.PDList.IsDefined(pname) && !SRC.NPDList.IsDefined(pname) && pname != "システム")
                                {
                                    throw reader.InvalidDataException("パイロット「" + pname + "」が定義されていません。", pilot_list);
                                }
                            }

                            // メッセージ
                            if (Strings.Len(line_buf) == ret)
                            {
                                throw reader.InvalidDataException("メッセージが定義されていません。", pilot_list);
                            }

                            var msg = Strings.Trim(Strings.Mid(line_buf, ret + 1));
                            d.AddMessage(pname, msg);
                        }
                    }
                }
            }
        }
    }
}
