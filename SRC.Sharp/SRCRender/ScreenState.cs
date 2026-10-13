using SRCCore;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>設定変更ダイアログ</summary>
        public Compat.ConfigWindow Config { get; set; }

        /// <summary>ホットポイントのツールチップ</summary>
        public TooltipState Tooltip { get; set; }

        public MessageState Message { get; } = new MessageState();

        /// <summary>選択肢（null で非表示）</summary>
        public ListState List { get; set; }

        /// <summary>ポップアップメニュー（null で非表示）</summary>
        public MenuState Menu { get; set; }

        /// <summary>ダイアログ（null で非表示）</summary>
        public DialogState Dialog { get; set; }
    }

    /// <summary>
    /// メッセージの状態（表示はメッセージウィンドウが行う。テスト・自動プレイで参照する）。
    /// </summary>
    public class MessageState
    {
        public bool Visible { get; set; }
        public string Speaker { get; set; }
        public IList<string> Lines { get; } = new List<string>();
        /// <summary>クリック待ちの表示</summary>
        public bool Waiting { get; set; }

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

    /// <summary>
    /// 表示中のリストボックス。描画と位置はリストボックスの部品（ListWindow）による。
    /// </summary>
    public class ListState
    {
        public ListState(Compat.Form window, IEnumerable<ListBoxItem> items, bool hasFlag)
        {
            Window = window;
            Items = items.Select(x => new ListItemState(x.Text, hasFlag && x.ListItemFlag)).ToList();
        }

        public Compat.Form Window { get; }
        public string Caption => Window.Text;
        public string Info => (Window as Compat.ListWindow)?.labCaption.Text;
        public IList<ListItemState> Items { get; }
        /// <summary>カーソル位置（0 始まり、-1 でなし）</summary>
        public int Highlight
        {
            get => Window.lstItems.SelectedIndex;
            set
            {
                Window.lstItems.SelectedIndex = value;
                Window.lstItems.EnsureVisible(value);
            }
        }
    }

    public class TooltipState
    {
        public string Text { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
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
