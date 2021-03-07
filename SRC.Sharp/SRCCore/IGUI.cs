// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Commands;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore
{
    // ユーザーインターフェースと画面描画の処理を行うためのインタフェース
    public interface IGUI
    {
        void Sleep(int dwMilliseconds, bool withEvents = true);
        //// XXX 要プロパティ更新、戻り値で返したほうがいい気はする。そもそも実行をロックしたくない。
        //void StartWaitClick();

        // マップ画面に表示できるマップのサイズ
        int MainWidth { get; set; }
        int MainHeight { get; set; }

        // マップ画面のサイズ（ピクセル）
        int MainPWidth { get; set; }
        int MainPHeight { get; set; }

        // マップのサイズ（ピクセル）
        int MapPWidth { get; set; }
        int MapPHeight { get; set; }


        // 現在マップウィンドウがマスク表示されているか
        bool ScreenIsMasked { get; set; }
        // 現在マップウィンドウが保存されているか
        bool ScreenIsSaved { get; set; }

        // 現在表示されているマップの座標
        int MapX { get; set; }
        int MapY { get; set; }

        // ドラッグ前のマップの座標
        int PrevMapX { get; set; }
        int PrevMapY { get; set; }

        // 最後に押されたマウスボタン
        GuiButton MouseButton { get; set; }

        // 現在のマウスの座標
        double MouseX { get; set; }
        double MouseY { get; set; }

        // ドラッグ前のマウスの座標
        double PrevMouseX { get; set; }
        double PrevMouseY { get; set; }

        // 移動前のユニットの情報
        int PrevUnitX { get; set; }
        int PrevUnitY { get; set; }
        string PrevUnitArea { get; set; }
        string PrevCommand { get; set; }

        // PaintPictureで画像が描き込まれたか
        bool IsPictureDrawn { get; set; }
        // PaintPictureで画像が描かれているか
        bool IsPictureVisible { get; set; }
        // PaintPictureで描画した画像領域
        int PaintedAreaX1 { get; set; }
        int PaintedAreaY1 { get; set; }
        int PaintedAreaX2 { get; set; }
        int PaintedAreaY2 { get; set; }
        // カーソル画像が表示されているか
        bool IsCursorVisible { get; set; }
        // 背景色
        int BGColor { get; set; }

        // GUIから入力可能かどうか
        bool IsGUILocked { get; set; }

        // リストボックス内で表示位置
        int TopItem { get; set; }

        // メッセージウインドウにに関する情報
        bool MessageWindowIsOut { get; set; }

        // フォームがクリックされたか
        bool IsFormClicked { get; set; }
        // フォームがモーダルか
        bool IsMordal { get; set; }

        // メッセージ表示のウェイト
        int MessageWait { get; set; }

        // メッセージが自働送りかどうか
        bool AutoMessageMode { get; set; }

        // PaintStringの中央表示の設定
        bool HCentering { get; set; }
        bool VCentering { get; set; }
        // PaintStringの書きこみが背景に行われるかどうか
        bool PermanentStringMode { get; set; }
        // PaintStringの書きこみが持続性かどうか
        bool KeepStringMode { get; set; }

        // メインウィンドウのロードとFlashの登録を行う
        void LoadMainFormAndRegisterFlash();
        // 各ウィンドウをロード
        // ただしメインウィンドウはあらかじめLoadMainFormAndRegisterFlashでロードしておくこと
        void LoadForms();
        // Optionによる新ＧＵＩが有効かどうかを再設定する
        void SetNewGUIMode();


        // === メッセージウィンドウに関する処理 ===

        bool MessageFormVisible { get; }
        // メッセージウィンドウを開く
        // 戦闘メッセージ画面など、ユニット表示を行う場合は u1, u2 に指定
        void OpenMessageForm(Unit u1 = null, Unit u2 = null);
        // メッセージウィンドウを閉じる
        void CloseMessageForm();
        // メッセージウィンドウをクリア
        void ClearMessageForm();
        // メッセージウィンドウに表示しているユニット情報を更新
        void UpdateMessageForm(Unit u1, Unit u2 = null);
        // メッセージウィンドウの状態を記録する
        void SaveMessageFormStatus();
        // メッセージウィンドウの状態を記録した状態に保つ
        void KeepMessageFormStatus();

        // === メッセージ表示に関する処理 ===

        // メッセージウィンドウにメッセージを表示
        void DisplayMessage(string pname, string msg, string msg_mode = "");
        // メッセージウィンドウに文字列を書き込む
        void PrintMessage(string msg, bool is_sys_msg = false);
        // メッセージ幅を計算(タグを無視して)
        int MessageLen(string msg);
        // メッセージウィンドウに戦闘メッセージを表示
        void DisplayBattleMessage(string pname, string msg, string msg_mode = "");
        // システムによるメッセージを表示
        void DisplaySysMessage(string msg, bool int_wait = false);

        // === マップウィンドウに関する処理 ===

        // マップ画面背景の設定
        void SetupBackground(string draw_mode = "", string draw_option = "", int filter_color = 0, double filter_trans_par = 0d);
        // 画面の書き換え (ユニット表示からやり直し)
        void RedrawScreen(bool late_refresh = false);
        // 画面をマスクがけして再表示
        void MaskScreen();
        // 画面の書き換え
        void RefreshScreen(bool without_refresh = false, bool delay_refresh = false);
        // 指定されたマップ座標を画面の中央に表示
        void Center(int new_x, int new_y);

        // === 座標変換 ===

        // マップ上での座標がマップ画面のどの位置にくるかを返す
        int MapToPixelX(int X);
        int MapToPixelY(int Y);
        // マップ画面でのピクセルがマップの座標のどの位置にくるかを返す
        int PixelToMapX(int X);
        int PixelToMapY(int Y);

        // === ユニット画像表示に関する処理 ===

        // ユニットのビットマップを作成
        int MakeUnitBitmap(Unit u);
        // ユニットのビットマップをロード
        // XXX
        //void LoadUnitBitmap(Unit u, PictureBox pic, int dx, int dy, bool use_orig_color = false, string fname = "");
        // ユニット画像の描画
        void PaintUnitBitmap(Unit u, string smode = "");
        // ユニット画像の表示を消す
        void EraseUnitBitmap(int X, int Y, bool do_refresh = true);
        // ユニット画像の表示位置を移動 (アニメーション)
        void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division = 2);
        // ユニット画像の表示位置を移動 (アニメーション)
        // 画像の経路を実際の移動経路にあわせる
        void MoveUnitBitmap2(Unit u, int wait_time0, int division = 2);

        // === 各種リストボックスに関する処理 ===

        // リストボックスを表示
        int ListBox(string lb_caption, IList<ListBoxItem> list, string lb_info, string lb_mode = "");
        // リストボックスの高さを大きくする
        void EnlargeListBoxHeight();
        // リストボックスの高さを小さくする
        void ReduceListBoxHeight();
        // リストボックスの幅を大きくする
        void EnlargeListBoxWidth();
        // リストボックスの幅を小さくする
        void ReduceListBoxWidth();
        // 武器選択用にリストボックスを切り替え
        void AddPartsToListBox();
        // 武器選択用リストボックスを通常のものに切り替え
        void RemovePartsOnListBox();
        // 武器選択用リストボックス
        int WeaponListBox(Unit u, IList<UnitWeapon> weapons, string caption_msg, string lb_mode, string BGM = "");
        // アビリティ選択用リストボックス
        int AbilityListBox(Unit u, string caption_msg, string lb_mode, bool is_item = false);
        // 入力時間制限付きのリストボックスを表示
        int LIPS(string lb_caption, string[] list, string lb_info, int time_limit);
        // 複数段のリストボックスを表示
        int MultiColumnListBox(string lb_caption, string[] list, bool is_center);
        // 複数のアイテム選択可能なリストボックスを表示
        int MultiSelectListBox(string lb_caption, string[] list, string lb_info, int max_num);

        // === 画像描画に関する処理 ===

        // 画像をウィンドウに描画
        bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option);
        // 画像バッファを作成
        void MakePicBuf();

        // === 文字列描画に関する処理 ===

        // メインウィンドウに文字列を表示する
        void DrawString(string msg, int X, int Y, bool without_cr = false);
        // メインウィンドウに文字列を表示 (システムメッセージ)
        void DrawSysString(int X, int Y, string msg, bool without_refresh = false);

        // === 画像消去に関する処理 ===

        // 描画した画像を消去できるように元画像を保存する
        void SaveScreen();
        // 描画したグラフィックを消去
        void ClearPicture();
        // 描画したグラフィックの一部を消去
        void ClearPicture2(int x1, int y1, int x2, int y2);

        // === 画面ロックに関する処理 ===

        // ＧＵＩをロックし、プレイヤーからの入力を無効にする
        void LockGUI();
        // ＧＵＩのロックを解除し、プレイヤーからの入力を有効にする
        void UnlockGUI();

        // === マウスカーソルの自動移動に関する処理 ===

        // 現在のマウスカーソルの位置を記録
        void SaveCursorPos();
        // マウスカーソルを移動する
        void MoveCursorPos(string cursor_mode, Unit t = null);
        // マウスカーソルを元の位置に戻す
        void RestoreCursorPos();

        // === タイトル画面表示に関する処理 ===

        // タイトル画面を表示
        void OpenTitleForm();
        // タイトル画面を閉じる
        void CloseTitleForm();

        // === 「Now Loading...」表示に関する処理 ===

        // 「Now Loading...」の画面を表示
        void OpenNowLoadingForm();
        // 「Now Loading...」の画面を消去
        void CloseNowLoadingForm();
        // 「Now Loading...」のバーを１段階進行させる
        void DisplayLoadingProgress();
        // 「Now Loading...」のバーの長さを設定
        void SetLoadImageSize(int new_size);
        // === 画面の解像度変更 ===
        void ChangeDisplaySize(int w, int h);

        // === その他 ===

        // エラーメッセージを表示
        void ErrorMessage(string msg);
        // データ読み込み時のエラーメッセージを表示する
        void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname);
        // マウスの右ボタンが押されているか(キャンセル)判定
        bool IsRButtonPressed(bool ignore_message_wait = false);
        // Telopコマンド用描画ルーチン
        void DisplayTelop(string msg);

        // ウィンドウなどのタイトルを設定する
        void SetTitle(string title);


        // === MainForm 操作へのバイパス ===

        bool MainFormVisible { get; }
        void MainFormShow();
        void MainFormHide();

        void ChangeStatus(GuiStatus status);
        void UpdateScreen();

        // === コンテキストメニュー操作 ===
        void ShowUnitCommandMenu(IList<UiCommand> commands);
        void ShowMapCommandMenu(IList<UiCommand> commands);

        // === 確認ダイアログ ===
        GuiDialogResult Confirm(string message, string title, GuiConfirmOption option);
    }

    public enum GuiStatus
    {
        Default,
        WaitCursor,
        IBeam,
    }
    public enum GuiButton
    {
        None,
        Left,
        Right,
    }
    public enum GuiConfirmOption
    {
        OkCancel,
        Question,
    }
    public enum GuiDialogResult
    {
        Ok,
        Cancel,
    }
    public class ListBoxItem
    {
        // 表示するテキスト
        public string Text { get; set; }

        // 選択でき『ない』かどうか
        // 全要素 false なら無視
        public bool ListItemFlag { get; set; }
        // フォーカス時に表示するコメント
        public string ListItemComment { get; set; }
        // リスト表示の呼び出し側での識別用ID
        public string ListItemID { get; set; }
    }
}
