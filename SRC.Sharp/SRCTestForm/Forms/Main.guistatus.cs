using SRCCore;
using SRCCore.Maps;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCTestForm.Resoruces;
using System.Drawing;
using System.Windows.Forms;

namespace SRCTestForm
{
    // TODO インタフェースの切り方見直す
    internal partial class frmMain : IGUIStatus
    {
        public void ClearUnitStatus()
        {
        }

        public void DisplayGlobalStatus()
        {
        }

        public void DisplayPilotStatus(Pilot p)
        {
        }

        public void DisplayUnitStatus(Unit u, short pindex = 0)
        {
        }

        public void InstantUnitStatusDisplay(short X, short Y)
        {
        }
    }
}
