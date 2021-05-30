// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Commands
{
    public partial class Command
    {
        // ユニットコマンドのメニュー番号
        public const int MoveCmdID = 0;
        public const int TeleportCmdID = 1;
        public const int JumpCmdID = 2;
        public const int TalkCmdID = 3;
        public const int AttackCmdID = 4;
        public const int FixCmdID = 5;
        public const int SupplyCmdID = 6;
        public const int AbilityCmdID = 7;
        public const int ChargeCmdID = 8;
        public const int SpecialPowerCmdID = 9;
        public const int TransformCmdID = 10;
        public const int SplitCmdID = 11;
        public const int CombineCmdID = 12;
        public const int HyperModeCmdID = 13;
        public const int GroundCmdID = 14;
        public const int SkyCmdID = 15;
        public const int UndergroundCmdID = 16;
        public const int WaterCmdID = 17;
        public const int LaunchCmdID = 18;
        public const int ItemCmdID = 19;
        public const int DismissCmdID = 20;
        public const int OrderCmdID = 21;
        public const int ExchangeFormCmdID = 22;
        public const int FeatureListCmdID = 23;
        public const int WeaponListCmdID = 24;
        public const int AbilityListCmdID = 25;
        public const int UnitCommandCmdID = 127;
        public const int WaitCmdID = 255;

        // マップコマンドのメニュー番号
        public const int EndTurnCmdID = 0;
        public const int DumpCmdID = 1;
        public const int UnitListCmdID = 2;
        public const int SearchSpecialPowerCmdID = 3;
        public const int GlobalMapCmdID = 4;
        public const int OperationObjectCmdID = 5;
        public const int MapCommandCmdID = 6;
        public const int AutoDefenseCmdID = 16;
        public const int ConfigurationCmdID = 17;
        public const int RestartCmdID = 18;
        public const int QuickLoadCmdID = 19;
        public const int QuickSaveCmdID = 20;
    }
}
