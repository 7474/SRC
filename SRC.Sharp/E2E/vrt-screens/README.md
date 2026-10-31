# vrt-screens

画面の VRT（#1358）のためのシナリオ。`play.txt` の `snapshot` で撮った画面を、`GameScreenVrtTests` が基準画像（`SRCRenderTests/Baselines/vrt-screens-<名前>.png`）と比べる。

| snapshot | 画面 |
|---|---|
| `message-face` | 顔の絵（`Bitmap/Pilot/テスト.bmp`）つきのメッセージ。長い文の折り返し |
| `unit-menu` | ユニットを選んだときのコマンドのメニューと、ステータスの欄 |
| `feature-list` | 特殊能力一覧（バリア・ＨＰ回復・弱点、パイロットの底力） |
| `feature-help` | 特殊能力（バリア）の解説 |
| `weapon-list` | 武装一覧（格闘・ビーム・気力の要る実弾） |

画像（顔・ユニット）は VRT 用に作った単純な図形。
