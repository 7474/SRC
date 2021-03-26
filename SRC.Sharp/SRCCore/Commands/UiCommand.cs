using SRCCore.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Commands
{
    public class UiCommand
    {
        public UiCommand(int id, string label)
        {
            Id = id;
            Label = label;
        }
        public UiCommand(int id, string label, LabelData labelData)
        {
            Id = id;
            Label = label;
            LabelData = labelData;
        }

        public int Id { get; }
        public string Label { get; }
        public LabelData LabelData { get; }
    }
}
