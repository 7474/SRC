using Microsoft.Extensions.Logging;
using SRCCore;
using SRCCore.Filesystem;
using SRCTestForm.FormLib;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SRCTestForm
{
    public partial class frmTeatMain : Form
    {
        public SRCCore.SRC SRC;

        public frmTeatMain()
        {
            InitializeComponent();

            SRC = new SRCCore.SRC();
            SRC.FileSystem = new LocalFileSystem();
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
                    SRC.LoadDataDirectory(fbd.SelectedPath);
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

            treeViewData.Nodes.Add(new TreeNode("Alias", SRC.ALDList.Items.Select(x => new SrcTreeNode(x.Name, x)).ToArray()));
            treeViewData.Nodes.Add(new TreeNode("Unit", SRC.UDList.Items.Select(x => new SrcTreeNode(x.Name, x)).ToArray()));
            treeViewData.Nodes.Add(new TreeNode("Pilot", SRC.PDList.Items.Select(x => new SrcTreeNode(x.Name, x)).ToArray()));
            treeViewData.Nodes.Add(new TreeNode("NonPilot", SRC.NPDList.Items.Select(x => new SrcTreeNode(x.Name, x)).ToArray()));
            treeViewData.Nodes.Add(new TreeNode("Message", SRC.MDList.Items.Select(x => new SrcTreeNode(x.Name, x)).ToArray()));
            treeViewData.Nodes.Add(new TreeNode("Dialog", SRC.DDList.Items.Select(x => new SrcTreeNode(x.Name, x)).ToArray()));
            treeViewData.Nodes.Add(new TreeNode("Item", SRC.IDList.Items.Select(x => new SrcTreeNode(x.Name, x)).ToArray()));
            treeViewData.Nodes.Add(new TreeNode("SP", SRC.SPDList.Items.Select(x => new SrcTreeNode(x.Name, x)).ToArray()));
        }

        private void SetMainText(string text)
        {
            textMain.Text = text;
        }

        private void SetStatusText(string text)
        {
            Program.Log.LogDebug(text);
            toolStripStatusLabel.Text = text;
            Update();
        }

        private void SetProgress(int value, int max)
        {
            toolStripProgressBar.Value = value;
            toolStripProgressBar.Maximum = max;
        }
    }
}
