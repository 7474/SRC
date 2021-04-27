using SRCCore.Events;

namespace SRCCore.Commands
{
    public class UiCommand
    {
        public UiCommand(int id, string label, bool isChecked = false)
        {
            Id = id;
            Label = label;
            IsChecked = isChecked;
        }
        public UiCommand(int id, string label, LabelData labelData, bool isChecked = false)
        {
            Id = id;
            Label = label;
            LabelData = labelData;
            IsChecked = isChecked;
        }

        public int Id { get; }
        public string Label { get; }
        public LabelData LabelData { get; }
        public bool IsChecked { get; }
    }
}
