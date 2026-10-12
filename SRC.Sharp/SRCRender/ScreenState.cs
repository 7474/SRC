using System.Collections.Generic;

namespace SRCRender
{
    /// <summary>
    /// 画面に表示する内容（マップ以外）。GUI 実装が更新し、ScreenRenderer が描画する。
    /// </summary>
    public class ScreenState
    {
        /// <summary>マップを表示するか（ClearScrean で消去、RefreshScreen で表示）</summary>
        public bool ShowMap { get; set; }

        /// <summary>マップ表示の中心座標（1 始まり）</summary>
        public int MapX { get; set; } = 1;
        public int MapY { get; set; } = 1;

        /// <summary>タイトルやロード中の表示（null で非表示）</summary>
        public string Splash { get; set; }

        /// <summary>テロップ（null で非表示）</summary>
        public string Telop { get; set; }

        public MessageState Message { get; } = new MessageState();

        /// <summary>選択肢（null で非表示）</summary>
        public ListState List { get; set; }

        /// <summary>ポップアップメニュー（null で非表示）</summary>
        public MenuState Menu { get; set; }

        /// <summary>ダイアログ（null で非表示）</summary>
        public DialogState Dialog { get; set; }
    }

    public class MessageState
    {
        public bool Visible { get; set; }
        public string Speaker { get; set; }
        public IList<string> Lines { get; } = new List<string>();
        /// <summary>クリック待ちの表示</summary>
        public bool Waiting { get; set; }
        /// <summary>顔グラフィックを表示するか</summary>
        public bool ShowFace { get; set; }
        public bool AutoMode { get; set; }
    }

    public class ListItemState
    {
        public ListItemState(string text, bool disabled = false)
        {
            Text = text;
            Disabled = disabled;
        }

        public string Text { get; }
        public bool Disabled { get; }
    }

    public class ListState
    {
        public string Caption { get; set; }
        public string Info { get; set; }
        public IList<ListItemState> Items { get; set; } = new List<ListItemState>();
        /// <summary>カーソル位置（0 始まり、-1 でなし）</summary>
        public int Highlight { get; set; } = -1;
    }

    public class MenuState
    {
        public IList<string> Items { get; set; } = new List<string>();
        public IList<bool> Checked { get; set; } = new List<bool>();
        public int X { get; set; }
        public int Y { get; set; }
        public int Highlight { get; set; } = -1;
    }

    public class DialogState
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public IList<string> Buttons { get; set; } = new List<string> { "OK" };
    }
}
