using SRC.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCoreTests.TestLib
{
    class MockGUI : IGUI
    {
        public void DisplayLoadingProgress()
        {
        }

        public void ErrorMessage(string str)
        {
            Debug.WriteLine(str);
        }
    }
}
