using SRCCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRCTestBlazor.Extensions
{
    public static class SRCExtension
    {
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
            };
            if (src.SPDList.Items.Any()) { return; }
            foreach (var spname in names)
            {
                src.SPDList.Add(spname);
            }
        }
    }
}
