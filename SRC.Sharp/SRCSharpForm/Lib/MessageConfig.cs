namespace SRCSharpForm.Lib
{
    public class MessageConfig
    {
        public static readonly float BrThresholdLimit = 0.95f;
        // XXX 用途がMinimumでいいのかは微妙
        public static readonly float BrThresholdMinimum = 0.5f;

        // メッセージ長の超過による改行の位置
        public float BrThresholdOver { get; private set; }
        // "。"," "による改行の位置
        public float BrThresholdKuten { get; private set; }
        // "、"による改行の位置
        public float BrThresholdTouten { get; private set; }

        public MessageConfig()
        {
            BrThresholdOver = 0.8f;
            BrThresholdKuten = 0.6f;
            BrThresholdTouten = 0.75f;
        }

        public void ConfigureShortSpace()
        {
            BrThresholdOver = 0.94f;
            BrThresholdKuten = 0.7f;
            BrThresholdTouten = 0.85f;
        }
    }
}
