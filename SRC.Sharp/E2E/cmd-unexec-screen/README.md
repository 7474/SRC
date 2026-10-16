# cmd-unexec-screen

画像や文字を描くコマンドを実行し、`Wait Click` で止めてスナップショットを撮る。描いた結果はスナップショット（TestResults/e2e/cmd-unexec-screen/）で確かめ、各段階の後にイベントが続いていることを `段階` の値で確かめる。

- ShowImage: `Bitmap\Event\テスト画像.png`（左半分が赤、右半分が青）を画面中央に原寸で表示する。幅と高さを指定すると拡大して表示する。元の SRC と同じく、描いた画像はすぐに画面に出る
- ClearImage: 表示した画像を消す
- PaintSysString: マップ座標のマスにシステム用の文字列を表示する。`非同期` は画面を更新せず、Refresh で反映する
