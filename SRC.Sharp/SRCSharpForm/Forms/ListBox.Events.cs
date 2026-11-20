using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // VB の WithEvents から変換した、イベントを付け外しするプロパティ。
    // Designer のファイルには生成したものだけを置く（.NET 11 の WinForms のアナライザー WFO3001）
    internal partial class frmListBox
    {
        public Timer Timer2
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Timer2;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Timer2 != null)
                {
                    _Timer2.Tick -= Timer2_Tick;
                }

                _Timer2 = value;
                if (_Timer2 != null)
                {
                    _Timer2.Tick += Timer2_Tick;
                }
            }
        }

        public Timer Timer1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Timer1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Timer1 != null)
                {
                    _Timer1.Tick -= Timer1_Tick;
                }

                _Timer1 = value;
                if (_Timer1 != null)
                {
                    _Timer1.Tick += Timer1_Tick;
                }
            }
        }
    }
}
