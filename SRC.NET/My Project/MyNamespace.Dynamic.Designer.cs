using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Project1.My
{
    internal static partial class MyProject
    {
        internal partial class MyForms
        {
            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmConfiguration m_frmConfiguration;

            public frmConfiguration frmConfiguration
            {
                [DebuggerHidden]
                get
                {
                    m_frmConfiguration = Create__Instance__(m_frmConfiguration);
                    return m_frmConfiguration;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmConfiguration))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmConfiguration);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmErrorMessage m_frmErrorMessage;

            public frmErrorMessage frmErrorMessage
            {
                [DebuggerHidden]
                get
                {
                    m_frmErrorMessage = Create__Instance__(m_frmErrorMessage);
                    return m_frmErrorMessage;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmErrorMessage))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmErrorMessage);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmListBox m_frmListBox;

            public frmListBox frmListBox
            {
                [DebuggerHidden]
                get
                {
                    m_frmListBox = Create__Instance__(m_frmListBox);
                    return m_frmListBox;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmListBox))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmListBox);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmMain m_frmMain;

            public frmMain frmMain
            {
                [DebuggerHidden]
                get
                {
                    m_frmMain = Create__Instance__(m_frmMain);
                    return m_frmMain;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmMain))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmMain);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmMessage m_frmMessage;

            public frmMessage frmMessage
            {
                [DebuggerHidden]
                get
                {
                    m_frmMessage = Create__Instance__(m_frmMessage);
                    return m_frmMessage;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmMessage))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmMessage);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmMultiColumnListBox m_frmMultiColumnListBox;

            public frmMultiColumnListBox frmMultiColumnListBox
            {
                [DebuggerHidden]
                get
                {
                    m_frmMultiColumnListBox = Create__Instance__(m_frmMultiColumnListBox);
                    return m_frmMultiColumnListBox;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmMultiColumnListBox))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmMultiColumnListBox);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmMultiSelectListBox m_frmMultiSelectListBox;

            public frmMultiSelectListBox frmMultiSelectListBox
            {
                [DebuggerHidden]
                get
                {
                    m_frmMultiSelectListBox = Create__Instance__(m_frmMultiSelectListBox);
                    return m_frmMultiSelectListBox;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmMultiSelectListBox))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmMultiSelectListBox);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmNowLoading m_frmNowLoading;

            public frmNowLoading frmNowLoading
            {
                [DebuggerHidden]
                get
                {
                    m_frmNowLoading = Create__Instance__(m_frmNowLoading);
                    return m_frmNowLoading;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmNowLoading))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmNowLoading);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmSafeMain m_frmSafeMain;

            public frmSafeMain frmSafeMain
            {
                [DebuggerHidden]
                get
                {
                    m_frmSafeMain = Create__Instance__(m_frmSafeMain);
                    return m_frmSafeMain;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmSafeMain))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmSafeMain);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmTelop m_frmTelop;

            public frmTelop frmTelop
            {
                [DebuggerHidden]
                get
                {
                    m_frmTelop = Create__Instance__(m_frmTelop);
                    return m_frmTelop;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmTelop))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmTelop);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmTitle m_frmTitle;

            public frmTitle frmTitle
            {
                [DebuggerHidden]
                get
                {
                    m_frmTitle = Create__Instance__(m_frmTitle);
                    return m_frmTitle;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmTitle))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmTitle);
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public frmToolTip m_frmToolTip;

            public frmToolTip frmToolTip
            {
                [DebuggerHidden]
                get
                {
                    m_frmToolTip = Create__Instance__(m_frmToolTip);
                    return m_frmToolTip;
                }

                [DebuggerHidden]
                set
                {
                    if (ReferenceEquals(value, m_frmToolTip))
                        return;
                    if (value is object)
                        throw new ArgumentException("Property can only be set to Nothing");
                    Dispose__Instance__(ref m_frmToolTip);
                }
            }
        }
    }
}