using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Models
{
    public interface ILevelElement
    {
        // 名称
        string Name { get; }
        // レベル (レベル指定のない能力の場合はDEFAULT_LEVEL)
        double Level { get; }
        // データ
        string StrData { get; }

        bool HasLevel { get; }
    }
}
