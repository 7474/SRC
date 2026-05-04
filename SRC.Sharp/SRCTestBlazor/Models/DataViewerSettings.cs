namespace SRCTestBlazor.Models
{
    public enum ThemeMode
    {
        System,
        Light,
        Dark,
    }

    public enum DataDisplayOrder
    {
        UnitFirst,
        PilotFirst,
    }

    public class DataViewerSettings
    {
        public ThemeMode ThemeMode { get; set; } = ThemeMode.System;
        public bool ShowComment { get; set; } = true;
        public bool ShowRawButton { get; set; } = true;
        public bool ShowPilotMessage { get; set; } = true;
        public DataDisplayOrder DataDisplayOrder { get; set; } = DataDisplayOrder.UnitFirst;
    }
}
