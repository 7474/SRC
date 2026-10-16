# cmd-unexec-save

ステージの終わりに `Exec 幕間.eve` で自作のインターミッションに入り、SaveData と CallIntermissionCommand で本体の機能を呼び出す（ヘルプ「CallIntermissionCommandコマンド」の例と同じ使い方）。

- SaveData 保存.src: シナリオフォルダに 保存.src ができ（`Dir("保存.src")` が「保存.src」を返す）、中身にパイロットとユニットが入っている（Open・LineRead で読んで確かめる）
- SaveData（ファイル名なし）・CallIntermissionCommand データセーブ: 保存先の選択がキャンセルされた（テストのホストの既定）のでセーブされない
- CallIntermissionCommand ユニットの強化・機体改造・乗り換え・アイテム交換・換装: それぞれのリストを出し、Esc で閉じるとイベントが続く
- CallIntermissionCommand に対象外のコマンド名を渡すと何もしない
- CallIntermissionCommand ユニットステータス: Lib\ユニットステータス表示.eve に切り替わってユニットが並び、呼び出したイベントの続きは実行されない

自作のインターミッションから「インターミッションに戻る」と、元の SRC と同じく本体のインターミッションには入らずに終わるので、ここでは戻らない。
