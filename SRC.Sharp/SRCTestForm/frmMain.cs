using SRC.Core;
using SRC.Core.Units;
using SRCTestForm.FormLib;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SRCTestForm
{
    public partial class frmMain : Form, IGUI
    {
        public SRC.Core.SRC SRC;

        public frmMain()
        {
            InitializeComponent();

            SRC = new SRC.Core.SRC();
            SRC.GUI = this;
        }

        private void menuLoadData_Click(object sender, EventArgs e)
        {
            LoadData();
            UpdateDataTree();
        }
        private void treeViewData_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var srcNode = e.Node as SrcTreeNode;
            if (srcNode != null)
            {
                SetMainText(srcNode.DataJson());
            }
        }

        private void LoadData()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var res = fbd.ShowDialog();
                if (res == DialogResult.OK)
                {
                    //try
                    //{
                    SetStatusText($"Load data [{fbd.SelectedPath}].");
                    var sw = new Stopwatch();
                    sw.Start();
                    SRC.LoadDirectory(fbd.SelectedPath);
                    sw.Stop();
                    SetStatusText($"Loaded. {sw.ElapsedMilliseconds}ms");
                    //}
                    //catch (Exception ex)
                    //{
                    //    SetStatusText(ex.Message);
                    //    SetMainText(ex.ToString());
                    //}
                }
            }
        }

        private void UpdateDataTree()
        {
            treeViewData.Nodes.Clear();
            var unitNodes = SRC.UDList.Items.Select(ud => new SrcTreeNode(ud.Name, ud)).ToArray();
            var unitListNode = new TreeNode("Unit", unitNodes);

            var pilotNodes = SRC.PDList.Items.Select(pd => new SrcTreeNode(pd.Name, pd)).ToArray();
            var pilotListNode = new TreeNode("Pilot", pilotNodes);

            treeViewData.Nodes.Add(unitListNode);
            treeViewData.Nodes.Add(pilotListNode);
        }

        private void SetMainText(string text)
        {
            textMain.Text = text;
        }

        private void SetStatusText(string text)
        {
            toolStripStatusLabel.Text = text;
            Update();
        }

        private void SetProgress(int value, int max)
        {
            toolStripProgressBar.Value = value;
            toolStripProgressBar.Maximum = max;
        }

        void IGUI.LoadMainFormAndRegisterFlash()
        {
            throw new NotImplementedException();
        }

        void IGUI.LoadForms()
        {
            throw new NotImplementedException();
        }

        void IGUI.SetNewGUIMode()
        {
            throw new NotImplementedException();
        }

        void IGUI.OpenMessageForm(Unit u1, Unit u2)
        {
            throw new NotImplementedException();
        }

        void IGUI.CloseMessageForm()
        {
            throw new NotImplementedException();
        }

        void IGUI.ClearMessageForm()
        {
            throw new NotImplementedException();
        }

        void IGUI.UpdateMessageForm(Unit u1, Unit u2)
        {
            throw new NotImplementedException();
        }

        void IGUI.SaveMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        void IGUI.KeepMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplayMessage(string pname, string msg, string msg_mode)
        {
            throw new NotImplementedException();
        }

        void IGUI.PrintMessage(string msg, bool is_sys_msg)
        {
            throw new NotImplementedException();
        }

        int IGUI.MessageLen(string msg)
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplayBattleMessage(string pname, string msg, string msg_mode)
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplaySysMessage(string msg, bool int_wait)
        {
            throw new NotImplementedException();
        }

        void IGUI.SetupBackground(string draw_mode, string draw_option, int filter_color, double filter_trans_par)
        {
            throw new NotImplementedException();
        }

        void IGUI.RedrawScreen(bool late_refresh)
        {
            throw new NotImplementedException();
        }

        void IGUI.MaskScreen()
        {
            throw new NotImplementedException();
        }

        void IGUI.RefreshScreen(bool without_refresh, bool delay_refresh)
        {
            throw new NotImplementedException();
        }

        void IGUI.Center(int new_x, int new_y)
        {
            throw new NotImplementedException();
        }

        int IGUI.MapToPixelX(int X)
        {
            throw new NotImplementedException();
        }

        int IGUI.MapToPixelY(int Y)
        {
            throw new NotImplementedException();
        }

        int IGUI.PixelToMapX(int X)
        {
            throw new NotImplementedException();
        }

        int IGUI.PixelToMapY(int Y)
        {
            throw new NotImplementedException();
        }

        int IGUI.MakeUnitBitmap(Unit u)
        {
            throw new NotImplementedException();
        }

        void IGUI.PaintUnitBitmap(Unit u, string smode)
        {
            throw new NotImplementedException();
        }

        void IGUI.EraseUnitBitmap(int X, int Y, bool do_refresh)
        {
            throw new NotImplementedException();
        }

        void IGUI.MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division)
        {
            throw new NotImplementedException();
        }

        void IGUI.MoveUnitBitmap2(Unit u, int wait_time0, int division)
        {
            throw new NotImplementedException();
        }

        int IGUI.ListBox(string lb_caption, string[] list, string lb_info, string lb_mode)
        {
            throw new NotImplementedException();
        }

        void IGUI.EnlargeListBoxHeight()
        {
            throw new NotImplementedException();
        }

        void IGUI.ReduceListBoxHeight()
        {
            throw new NotImplementedException();
        }

        void IGUI.EnlargeListBoxWidth()
        {
            throw new NotImplementedException();
        }

        void IGUI.ReduceListBoxWidth()
        {
            throw new NotImplementedException();
        }

        void IGUI.AddPartsToListBox()
        {
            throw new NotImplementedException();
        }

        void IGUI.RemovePartsOnListBox()
        {
            throw new NotImplementedException();
        }

        int IGUI.WeaponListBox(Unit u, string caption_msg, string lb_mode, string BGM)
        {
            throw new NotImplementedException();
        }

        int IGUI.AbilityListBox(Unit u, string caption_msg, string lb_mode, bool is_item)
        {
            throw new NotImplementedException();
        }

        int IGUI.LIPS(string lb_caption, string[] list, string lb_info, int time_limit)
        {
            throw new NotImplementedException();
        }

        int IGUI.MultiColumnListBox(string lb_caption, string[] list, bool is_center)
        {
            throw new NotImplementedException();
        }

        int IGUI.MultiSelectListBox(string lb_caption, string[] list, string lb_info, int max_num)
        {
            throw new NotImplementedException();
        }

        bool IGUI.DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            throw new NotImplementedException();
        }

        void IGUI.MakePicBuf()
        {
            throw new NotImplementedException();
        }

        void IGUI.DrawString(string msg, int X, int Y, bool without_cr)
        {
            throw new NotImplementedException();
        }

        void IGUI.DrawSysString(int X, int Y, string msg, bool without_refresh)
        {
            throw new NotImplementedException();
        }

        void IGUI.SaveScreen()
        {
            throw new NotImplementedException();
        }

        void IGUI.ClearPicture()
        {
            throw new NotImplementedException();
        }

        void IGUI.ClearPicture2(int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
        }

        void IGUI.LockGUI()
        {
            throw new NotImplementedException();
        }

        void IGUI.UnlockGUI()
        {
            throw new NotImplementedException();
        }

        void IGUI.SaveCursorPos()
        {
            throw new NotImplementedException();
        }

        void IGUI.MoveCursorPos(string cursor_mode, Unit t)
        {
            throw new NotImplementedException();
        }

        void IGUI.RestoreCursorPos()
        {
            throw new NotImplementedException();
        }

        void IGUI.OpenTitleForm()
        {
            throw new NotImplementedException();
        }

        void IGUI.CloseTitleForm()
        {
            throw new NotImplementedException();
        }

        void IGUI.OpenNowLoadingForm()
        {
            throw new NotImplementedException();
        }

        void IGUI.CloseNowLoadingForm()
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplayLoadingProgress()
        {
            throw new NotImplementedException();
        }

        void IGUI.SetLoadImageSize(int new_size)
        {
            throw new NotImplementedException();
        }

        void IGUI.ChangeDisplaySize(int w, int h)
        {
            throw new NotImplementedException();
        }

        void IGUI.ErrorMessage(string msg)
        {
            SetStatusText(msg);
        }

        void IGUI.DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            throw new NotImplementedException();
        }

        bool IGUI.IsRButtonPressed(bool ignore_message_wait)
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplayTelop(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
