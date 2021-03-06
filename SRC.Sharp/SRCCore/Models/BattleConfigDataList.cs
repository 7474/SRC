// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Models
{
    // バトルコンフィグデータを管理するクラス
    // --- ダメージ計算、命中率算出など、バトルに関連するエリアスの定義を設定します。
    public class BattleConfigDataList
    {
        // バトルコンフィグデータのコレクション
        private SrcCollection<BattleConfigData> colBattleConfigData = new SrcCollection<BattleConfigData>();

        private SRC SRC { get; }
        public BattleConfigDataList(SRC src)
        {
            SRC = src;
        }

        // バトルコンフィグデータリストにデータを追加
        public BattleConfigData Add(string cname)
        {
            var cd = new BattleConfigData(SRC);
            cd.Name = cname;
            colBattleConfigData.Add(cd, cname);
            return cd;
        }

        // バトルコンフィグデータリストに登録されているデータの総数
        public int Count()
        {
            return colBattleConfigData.Count;
        }

        // バトルコンフィグデータリストからデータを削除
        public void Delete(string Index)
        {
            colBattleConfigData.Remove(Index);
        }

        // バトルコンフィグデータリストからデータを取り出す
        public BattleConfigData Item(string Index)
        {
            return colBattleConfigData[Index];
        }

        // バトルコンフィグデータリストに指定したデータが定義されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // データファイル fname からデータをロード
        public void Load(string fname)
        {
            using (var stream = new FileStream(fname, FileMode.Open))
            {
                Load(fname, stream);
            }
        }
        public void Load(string fname, Stream stream)
        {
            using (var reader = new SrcDataReader(fname, stream))
            {
                while (reader.HasMore)
                {
                    string buf, buf2;
                    string data_name = "";
                    string line_buf = "";
                    // 空行をスキップ
                    while (reader.HasMore && string.IsNullOrEmpty(line_buf))
                    {
                        line_buf = reader.GetLine();
                    }
                    if (string.IsNullOrEmpty(line_buf))
                    {
                        break;
                    }
                    data_name = line_buf;
                    if (IsDefined(data_name))
                    {
                        // すでに定義されているエリアスのデータであれば置き換える
                        Delete(data_name);
                    }

                    // XXX これでいいんか？　nullになったり上書きされそう。
                    var cd = Add(data_name);
                    while (reader.HasMore)
                    {
                        line_buf = reader.GetLine();
                        if (string.IsNullOrEmpty(line_buf))
                        {
                            break;
                        }

                        cd.ConfigCalc = line_buf;
                    }
                }
            }
        }
    }
}