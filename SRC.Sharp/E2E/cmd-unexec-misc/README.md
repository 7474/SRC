# cmd-unexec-misc

画面の見た目を変える設定のコマンドと、制御・デバッグ用のコマンドを確かめる。

- SetWindowColor: `#RRGGBB` を VB の色（`&HBBGGRR`）に直して `StatusWindow(FrameColor)`・`StatusWindow(BackBolor)` に記録する。対象を省略すると両方、`枠`・`背景` はそれぞれだけ
- SetWindowFrameWidth: `StatusWindow(FrameWidth)` に記録する
- SetStatusStringColor: `通常`・`能力名`・`有効`・`無効` の文字色を `StatusWindow(StringColor)` などに記録する
- Debug: ログに書くだけでイベントは続く
- ElseIf: If・1 つ目の ElseIf・2 つ目の ElseIf・Else のどれか一つだけを実行する。Else の無い ElseIf でどれも成り立たない場合、入れ子の If の中の ElseIf
- Suspend: Talk / AutoTalk をメッセージウィンドウを閉じずに終える。Suspend の時点では後のコマンドはまだ実行されておらず、間のコマンドを実行してから次の Talk / AutoTalk が続く
