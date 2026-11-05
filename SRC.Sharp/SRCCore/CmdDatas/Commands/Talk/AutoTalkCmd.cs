using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class AutoTalkCmd : ATalkCmd
    {
        public AutoTalkCmd(SRC src, EventDataLine eventData) : base(src, CmdType.AutoTalkCmd, eventData)
        {
        }

        protected override void DisplayMessage(string pname, string msg, string msg_mode = "")
        {
            GUI.DisplayBattleMessage(pname, msg, msg_mode);
        }

        protected override int ExecInternal()
        {
            // メッセージ表示速度を「普通」の値に設定
            var prev_msg_wait = GUI.MessageWait;
            // 元の SRC と同じく 700 ミリ秒（メッセージ速度「普通」）
            GUI.MessageWait = 700;

            try
            {
                return ProcessTalk();
            }
            finally
            {
                // メッセージ表示速度を元に戻す
                GUI.MessageWait = prev_msg_wait;
            }
        }
    }
}
