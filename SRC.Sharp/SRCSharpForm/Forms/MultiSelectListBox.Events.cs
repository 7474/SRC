using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // VB の WithEvents から変換した、イベントを付け外しするプロパティ。
    // Designer のファイルには生成したものだけを置く（.NET 11 の WinForms のアナライザー WFO3001）
    internal partial class frmMultiSelectListBox
    {
        public Button cmdResume
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdResume;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdResume != null)
                {
                    _cmdResume.Click -= cmdResume_Click;
                }

                _cmdResume = value;
                if (_cmdResume != null)
                {
                    _cmdResume.Click += cmdResume_Click;
                }
            }
        }

        public Button cmdSort
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdSort;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdSort != null)
                {
                    _cmdSort.Click -= cmdSort_Click;
                }

                _cmdSort = value;
                if (_cmdSort != null)
                {
                    _cmdSort.Click += cmdSort_Click;
                }
            }
        }

        public Button cmdSelectAll2
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdSelectAll2;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdSelectAll2 != null)
                {
                    _cmdSelectAll2.Click -= cmdSelectAll2_Click;
                }

                _cmdSelectAll2 = value;
                if (_cmdSelectAll2 != null)
                {
                    _cmdSelectAll2.Click += cmdSelectAll2_Click;
                }
            }
        }

        public Button cmdSelectAll
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdSelectAll;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdSelectAll != null)
                {
                    _cmdSelectAll.Click -= cmdSelectAll_Click;
                }

                _cmdSelectAll = value;
                if (_cmdSelectAll != null)
                {
                    _cmdSelectAll.Click += cmdSelectAll_Click;
                }
            }
        }

        public Button cmdFinish
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdFinish;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdFinish != null)
                {
                    _cmdFinish.Click -= cmdFinish_Click;
                }

                _cmdFinish = value;
                if (_cmdFinish != null)
                {
                    _cmdFinish.Click += cmdFinish_Click;
                }
            }
        }
    }
}
