using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // VB の WithEvents から変換した、イベントを付け外しするプロパティ。
    // Designer のファイルには生成したものだけを置く（.NET 11 の WinForms のアナライザー WFO3001）
    internal partial class frmConfiguration
    {
        public TextBox txtMP3Volume
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txtMP3Volume;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_txtMP3Volume != null)
                {
                    _txtMP3Volume.TextChanged -= txtMP3Volume_TextChanged;
                }

                _txtMP3Volume = value;
                if (_txtMP3Volume != null)
                {
                    _txtMP3Volume.TextChanged += txtMP3Volume_TextChanged;
                }
            }
        }

        public HScrollBar hscMP3Volume
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _hscMP3Volume;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_hscMP3Volume != null)
                {
                    _hscMP3Volume.Scroll -= hscMP3Volume_Scroll;
                }

                _hscMP3Volume = value;
                if (_hscMP3Volume != null)
                {
                    _hscMP3Volume.Scroll += hscMP3Volume_Scroll;
                }
            }
        }

        public Button cmdCancel
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdCancel;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdCancel != null)
                {
                    _cmdCancel.Click -= cmdCancel_Click;
                }

                _cmdCancel = value;
                if (_cmdCancel != null)
                {
                    _cmdCancel.Click += cmdCancel_Click;
                }
            }
        }

        public Button cmdOK
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdOK;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdOK != null)
                {
                    _cmdOK.Click -= cmdOK_Click;
                }

                _cmdOK = value;
                if (_cmdOK != null)
                {
                    _cmdOK.Click += cmdOK_Click;
                }
            }
        }

        public CheckBox chkBattleAnimation
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _chkBattleAnimation;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_chkBattleAnimation != null)
                {
                    _chkBattleAnimation.CheckStateChanged -= chkBattleAnimation_CheckStateChanged;
                }

                _chkBattleAnimation = value;
                if (_chkBattleAnimation != null)
                {
                    _chkBattleAnimation.CheckStateChanged += chkBattleAnimation_CheckStateChanged;
                }
            }
        }
    }
}
