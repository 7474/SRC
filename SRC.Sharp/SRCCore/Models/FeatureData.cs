// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.Models
{
    // 特殊能力のクラス
    public class FeatureData : ILevelElement
    {
        // 名称
        public string Name { get; set; }
        // レベル (レベル指定のない能力の場合はDEFAULT_LEVEL)
        public double Level { get; set; }
        // データ
        public string StrData { get; set; }
        // 必要技能
        public string NecessarySkill;
        // 必要条件
        public string NecessaryCondition;

        public bool HasLevel => Level != Constants.DEFAULT_LEVEL;

        public double FeatureLevel => HasLevel ? Level : 0d;
        public string Data => StrData ?? "";
        public IList<string> DataL => GeneralLib.ToL(Data);

        // 特殊能力の名称
        public string FeatureName(Unit unit)
        {
            FeatureData fd = this;
            // 非表示の能力
            switch (fd.Name ?? "")
            {
                case "ノーマルモード":
                case "パーツ合体":
                case "換装":
                case "制限時間":
                case "制御不可":
                case "主形態":
                case "他形態":
                case "合体制限":
                case "格闘武器":
                case "迎撃武器":
                case "合体技":
                case "変形技":
                case "ランクアップ":
                case "追加パイロット":
                case "暴走時パイロット":
                case "追加サポート":
                case "装備個所":
                case "ハードポイント":
                case "武器クラス":
                case "防具クラス":
                case "ＢＧＭ":
                case "武器ＢＧＭ":
                case "アビリティＢＧＭ":
                case "合体ＢＧＭ":
                case "分離ＢＧＭ":
                case "変形ＢＧＭ":
                case "ハイパーモードＢＧＭ":
                case "ユニット画像":
                case "パイロット画像":
                case "パイロット愛称":
                case "パイロット読み仮名":
                case "性別":
                case "性格変更":
                case "吸収":
                case "無効化":
                case "耐性":
                case "弱点":
                case "有効":
                case "特殊効果無効化":
                case "アイテム所有":
                case "レアアイテム所有":
                case "ラーニング可能技":
                case "改造費修正":
                case "最大改造数":
                case "パイロット能力付加":
                case "パイロット能力強化":
                case "非表示":
                case "攻撃属性":
                case "射程延長":
                case "武器強化":
                case "命中率強化":
                case "ＣＴ率強化":
                case "特殊効果発動率強化":
                case "必要技能":
                case "不必要技能":
                case "ダミーユニット":
                case "地形ユニット":
                case "召喚解除コマンド名":
                case "変身解除コマンド名":
                case "１人乗り可能":
                case "特殊効果":
                case "戦闘アニメ":
                case "パイロット地形適応変更":
                case "メッセージクラス":
                case "用語名":
                case "発光":
                    // ユニット用特殊能力
                    return "";

                case "愛称変更":
                case "読み仮名変更":
                case "サイズ変更":
                case "地形適応変更":
                case "地形適応固定変更":
                case "空中移動":
                case "陸上移動":
                case "水中移動":
                case "宇宙移動":
                case "地中移動":
                case "修理費修正":
                case "経験値修正":
                case "最大弾数増加":
                case "ＥＮ消費減少":
                case "Ｖ－ＵＰ":
                case "大型アイテム":
                case "呪い":
                    // アイテム用特殊能力
                    return "";
            }

            // 拡大画像能力は「拡大画像(文字列)」といった指定もあるので他の非表示能力と異なる
            // 判定方法を使う
            if (Strings.InStr(fd.Name, "拡大画像") == 1)
            {
                return "";
            }

            string resultName;
            if (Strings.Len(fd.StrData) > 0)
            {
                // 別名の指定あり
                resultName = GeneralLib.ListIndex(fd.StrData, 1);
                if (resultName == "非表示" | resultName == "解説")
                {
                    resultName = "";
                }
            }
            else if (fd.Level == Constants.DEFAULT_LEVEL)
            {
                // レベル指定なし
                resultName = fd.Name;
            }
            else if (fd.Level >= 0d)
            {
                // レベル指定あり
                resultName = fd.Name + "Lv" + SrcFormatter.Format(fd.Level);
                if (fd.Name == "射撃強化")
                {
                    if (unit.CountPilot() > 0)
                    {
                        if (unit.MainPilot().HasMana())
                        {
                            resultName = "魔力強化Lv" + SrcFormatter.Format(fd.Level);
                        }
                    }
                }
            }
            else
            {
                // マイナスのレベル指定
                switch (fd.Name ?? "")
                {
                    case "格闘強化":
                        resultName = "格闘低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
                        break;

                    case "射撃強化":
                        resultName = "射撃低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
                        if (unit.CountPilot() > 0)
                        {
                            if (unit.MainPilot().HasMana())
                            {
                                resultName = "魔力低下Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
                            }
                        }

                        break;

                    case "命中強化":
                        resultName = "命中低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
                        break;

                    case "回避強化":
                        resultName = "回避低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
                        break;

                    case "技量強化":
                        resultName = "技量低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
                        break;

                    case "反応強化":
                        resultName = "反応低下" + "Lv" + SrcFormatter.Format(Math.Abs(fd.Level));
                        break;

                    default:
                        resultName = fd.Name + "Lv" + SrcFormatter.Format(fd.Level);
                        break;
                }
            }

            return resultName;
        }
    }
}
