# intermission-commands

インターミッションのコマンドをひととおり実行し、どれも操作できるままインターミッションへ戻れることを確かめる。

- ユニットステータス・パイロットステータス: Lib\ユニットステータス表示.eve・Lib\パイロットステータス表示.eve を開き、マップのコマンドの「並べ替え」「インターミッションに戻る」を使う。
  Lib は SRC#（`SRC.Sharp/Content/basic-data/Lib`）のものを、SRC 本体と同じく AppPath に置いて使う（E2ECorpusTests が置く）
- 部隊名簿（ユーザー定義のインターミッションコマンド）: roster.eve の `MakeUnitList` と `Talk` を経て戻る
- 機体改造: ユニット選択のリストを出し、Esc で戻る
