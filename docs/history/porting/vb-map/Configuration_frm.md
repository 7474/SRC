# Configuration.frm

VB 版の `SRC/SRC_20121125/Configuration.frm` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub chkBattleAnimation_Click | 247 | 10 | `SRCRender/ScreenGUI.config.cs` ScreenGUI.UpdateConfigEnabled<br>`SRCSharpForm/Forms/Configuration.cs` frmConfiguration.chkBattleAnimation_CheckStateChanged | 修正済み | SRCRender は一致。SRCSharpForm は処理が空だったので、拡張戦闘アニメ・武器準備アニメを使えなくする処理を入れた（#1498） |
| Sub cmdCancel_Click | 259 | 10 | `SRCRender/ScreenGUI.config.cs` ScreenGUI.ConfigCancel<br>`SRCSharpForm/Forms/Configuration.cs` frmConfiguration.cmdCancel_Click | 一致 | MP3 の音量を元に戻して閉じる。vbmp3_setVolume は Sound.Player.SoundVolume に当たる |
| Sub cmdOK_Click | 271 | 116 | `SRCRender/ScreenGUI.config.cs` ScreenGUI.ConfigOK<br>`SRCSharpForm/Forms/Configuration.cs` frmConfiguration.cmdOK_Click | 修正済み | SRCSharpForm で、戦闘アニメを選び直したら BattleAnimationUnavailable を外すようにした（SRCRender と同じ。#1498）。合わせないもの: 拡張戦闘アニメを Off にしたときに VB が書き間違いのキー "Extendednimation" に書く不具合。UseDirectMusic・MidiReset は SRC# では使わない（#483） |
| Sub Form_Load | 388 | 119 | `SRCRender/ScreenGUI.config.cs` ScreenGUI.Configure<br>`SRCSharpForm/Forms/Configuration.cs` frmConfiguration.frmConfiguration_Load | 修正済み | SRCSharpForm で、ターン表示と「敵フェイズ中にＢＧＭを変更しない」の初期値が設定されていなかったのを直した（#1498）。汎用戦闘アニメが無いとき VB はチェックを灰色（Value=2）にするが、SRC# は外したチェックとして見せる（OK で Off になるのは同じ） |
| Sub hscMP3Volume_Change | 509 | 7 | `SRCRender/ScreenGUI.config.cs` ScreenGUI.SetConfigVolume<br>`SRCSharpForm/Forms/Configuration.cs` frmConfiguration.hscMP3Volume_Change | 一致 | その場で音量を変え、テキストに値を出す |
| Sub hscMP3Volume_Scroll | 517 | 7 | `SRCRender/ScreenGUI.config.cs` ScreenGUI.SetConfigVolume<br>`SRCSharpForm/Forms/Configuration.cs` frmConfiguration.hscMP3Volume_Scroll | 一致 | 同上 |
| Sub txtMP3Volume_Change | 525 | 17 | `SRCSharpForm/Forms/Configuration.cs` frmConfiguration.txtMP3Volume_TextChanged | 一致 | 0〜100 に収めてスクロールバーに反映する。SRCRender の欄は入力のできない表示なので、SetConfigVolume で同じ範囲に収める |
