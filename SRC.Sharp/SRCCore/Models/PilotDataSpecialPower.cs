using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Models
{
    public class PilotDataSpecialPower
    {
        public string Name { get; }
        public int NecessaryLevel { get; }
        public int SPConsumption { get; }

        public PilotDataSpecialPower(string name, int necessaryLevel, int sPConsumption)
        {
            Name = name;
            NecessaryLevel = necessaryLevel;
            SPConsumption = sPConsumption;
        }
    }
}
