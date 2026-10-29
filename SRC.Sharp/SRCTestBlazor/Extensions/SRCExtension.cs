using SRCCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRCTestBlazor.Extensions
{
    public static class SRCExtension
    {
        /// <summary>データのファイルを名前で種類を決めて読む。読まない種類なら false</summary>
        public static bool LoadDataFile(this SRC src, string fileName, Stream stream)
        {
            switch (Path.GetFileName(fileName).ToLower())
            {
                case "unit.txt":
                case "robot.txt":
                    src.UDList.Load(fileName, stream);
                    return true;
                case "pilot.txt":
                    src.PDList.Load(fileName, stream);
                    return true;
                case "non_pilot.txt":
                    src.NPDList.Load(fileName, stream);
                    return true;
                case "pilot_message.txt":
                    src.MDList.Load(fileName, false, stream);
                    return true;
                case "pilot_dialog.txt":
                    src.DDList.Load(fileName, stream);
                    return true;
                case "item.txt":
                    src.IDList.Load(fileName, stream);
                    return true;
                default:
                    return false;
            }
        }

        public static void InitSPDList(this SRC src)
        {
            var names = new string[]
            {
                "愛",
                "足かせ",
                "威圧",
                "祈り",
                "癒し",
                "応援",
                "覚醒",
                "かく乱",
                "隠れ身",
                "加速",
                "気合",
                "奇襲",
                "奇跡",
                "激怒",
                "激闘",
                "激励",
                "幸運",
                "鼓舞",
                "根性",
                "再動",
                "自爆",
                "集中",
                "祝福",
                "神速",
                "信頼",
                "捨て身",
                "戦慄",
                "狙撃",
                "脱力",
                "魂",
                "挑発",
                "直撃",
                "偵察",
                "てかげん",
                "鉄壁",
                "ド根性",
                "突撃",
                "努力",
                "熱血",
                "必中",
                "ひらめき",
                "復活",
                "補給",
                "みがわり",
                "見極め",
                "魅惑",
                "瞑想",
                "友情",
                "夢",
                "介抱",
                "我慢",
                "感応",
                "看破",
                "絆",
                "機先",
                "気迫",
                "決意",
                "献身",
                "堅牢",
                "号令",
                "策謀",
                "助言",
                "心眼",
                "痛撃",
                "闘志",
                "忍耐",
                "報復",
                "目眩し",
                "勇気",
                "守護",
            };
            if (src.SPDList.Items.Any()) { return; }
            foreach (var spname in names)
            {
                src.SPDList.Add(spname);
            }
        }
    }
}
