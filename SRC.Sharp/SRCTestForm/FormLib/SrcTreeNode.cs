using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SRCTestForm.FormLib
{
    public class SrcTreeNode : TreeNode
    {
        public object Item { get; }

        public SrcTreeNode(string text, object item)
            : base(text)
        {
            Item = item;
        }

        public string DataJson()
        {
            return JsonConvert.SerializeObject(Item, Formatting.Indented);
        }
    }
}
