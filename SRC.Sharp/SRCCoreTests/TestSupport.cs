using SRCCore.Pilots;
using SRCCore.Units;

namespace SRCCore.Tests
{
    /// <summary>
    /// テスト用 IGUIMap モック: SetMapSize / InitMapSize を no-op で実装
    /// </summary>
    internal class MockGUIMap : IGUIMap
    {
        public void InitMapSize(int w, int h) { }
        public void SetMapSize(int w, int h) { }
    }

    /// <summary>
    /// テスト用 IGUIStatus モック: 全メソッドを no-op で実装
    /// </summary>
    internal class MockGUIStatus : IGUIStatus
    {
        public Unit DisplayedUnit { get; set; }
        public Pilot DisplayedPilot { get; set; }
        public void DisplayGlobalStatus() { }
        public void DisplayUnitStatus(Unit u, Pilot p = null) { }
        public void DisplayPilotStatus(Pilot p) { }
        public void InstantUnitStatusDisplay(int X, int Y) { }
        public void ClearUnitStatus() { }
    }
}
