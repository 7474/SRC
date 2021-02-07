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
    public partial class frmTeatMain : Form, IGUI
    {
        public SRC.Core.SRC SRC;
        public SRC.Core.Expressions.Expression Expression => SRC.Expression;

        public frmTeatMain()
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
        private void menuLoadEve_Click(object sender, EventArgs e)
        {
            LoadEve();
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

        private void LoadEve()
        {
            using (var fbd = new OpenFileDialog())
            {
                var res = fbd.ShowDialog();
                if (res == DialogResult.OK)
                {
                    //try
                    //{
                    SetStatusText($"Load file [{fbd.FileName}].");
                    SRC.Execute(fbd.FileName);
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
    }
}
