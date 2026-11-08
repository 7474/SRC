# Status.bas

VB 版の `SRC/SRC_20121125/Status.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub DisplayGlobalStatus | 37 | 219 | `SRCRender/ScreenGUI.status.cs` ScreenGUI.DisplayGlobalStatus<br>`SRCSharpForm/Forms/Main.guistatus.cs` frmMain.DisplayGlobalStatus | 修正済み | 文字の大きさを 12 にした（10 だった）。地形の名称を (37, 65) から書き、続く行を左端から書く（x=37 にまとめて書いていた）。SRCSharpForm も同じに直した（#1498）。新ＧＵＩの分岐は SRC#で不要 |
| Sub DisplayUnitStatus | 259 | 3300 | `SRCRender/ScreenGUI.status.cs` ScreenGUI.DisplayUnitStatus<br>`SRCSharpForm/Forms/Main.guistatus.cs` frmMain.DisplayUnitStatus | 修正済み | 直したもの（SRCSharpForm にも同じ直しを入れた。#1498）: 愛称を 10.5 で書く、「レベル」「気力」を Term で書く、行動回数を「 Ｗ」と書く、ＳＰとスペシャルパワーの行で改行する、能力値を 19 桁ずつ続けて同じ行に書く（112 ピクセルの段組みで行がずれていた）、適応の後に Space$(8)、ＨＰ・ＥＮのゲージを VB の位置（x = 116〜206）と色で描く（88 * 値 \ 最大値）、有効な能力の色を vbBlue にした、スペシャルパワー自動発動の付加分は SkillData を読む、オーラ・超能力は DisplayedPilotInd = 1 のときだけ見る、切り払い・浄化は Disable の武器を除く、ハイパーモードの気力は CInt(10 * Lv) + 100、レベル指定の無い盾は「盾(1/1)」、防御属性の後で n = 0 に戻す、攻撃の予想の行を続けて書く、武器一覧を VB の挿入ソート（攻撃力の小さい順、同じなら弾数の多い順）で並べる、武器とアビリティの一覧は CurrentY > 420 で打ち切る、右端で折り返さない。合わせないもの: 武器のソートの最後の比較の .Weapon(i - j)（warray の書き間違いとみなす）、ハイパーモードの .FeatureLevel(i) の番号の取り違え。新ＧＵＩの分岐（StatusWindow の色・枠）は SRC#で不要（#1520） |
| Sub DisplayPilotStatus | 3561 | 33 | `SRCRender/ScreenGUI.status.cs` ScreenGUI.DisplayPilotStatus | 修正済み | VB と同じく、メイン・サブ・サポートの順に番号を探し、どれでもなければ追加サポートを表示する（追加サポートが無ければ何もしない）。Unit.StatusPilotIndex もメインパイロットを先に見るように直した（#1498） |
| Sub InstantUnitStatusDisplay | 3596 | 27 | `SRCRender/ScreenGUI.status.cs` ScreenGUI.InstantUnitStatusDisplay | 一致 | 発進コマンドのときの置き換えと、同じユニットなら表示しない判定が同じ |
| Sub ClearUnitStatus | 3625 | 16 | `SRCRender/ScreenGUI.status.cs` ScreenGUI.ClearUnitStatus | 一致 | MainWidth=15 の分岐に当たる |
| Function GetGlobalStatusSize | 3644 | 29 | （なし） | SRC#で不要 | 新ＧＵＩだけで使うウィンドウの高さの計算。SRC# は新ＧＵＩを持たない |
| Sub GlobalVariableLoad | 3675 | 44 | （なし） | SRC#で不要 | 新ＧＵＩだけで使う StatusWindow(…) の色の読み込み。VB で StatusFontColor・StatusWindow を使うのはすべて NewGUIMode の中なので、SRC# がコマンドの設定を読まないのは VB と同じ動作（#1520） |
