# SRCCore の TODO・XXX・コメントアウトされたコード

VB 版との突き合わせ（[対応表](README.md)）のときに、その手続きに当たるものを 1 つずつ判断する。`python3 tools/vb_port_map.py` で作り直す（手で書いた「状態」「メモ」は、ファイルと本文が同じなら残る）。

- 種類: `TODO`・`XXX`・`FIXME` はコメント、`コード` はコメントアウトされたコード（続いた行は 1 つにまとめ、最初の行と行数を書く）
- 状態: `未確認`・`直した`（PR の番号）・`SRC#で不要`（理由）・`Issue`（番号）
- 「未確認」の行があると CI（`port-map-check`）が落ちる。TODO・XXX・コメントアウトされたコードを足したら、直すか、この表に状態とメモを書く（7474/SRC#1555）

| 場所 | 種類 | 本文 | 状態 | メモ |
|---|---|---|---|---|
| `SRCCore/CmdDatas/Commands/Control/DoCmd.cs:17` | コード | Do - Loop While | SRC#で不要 | 分岐の説明のコメントで、VB のコードの残りではない |
| `SRCCore/CmdDatas/Commands/Other/LoadCmd.cs:35` | XXX | XXX 読み込み順序の解決がシナリオ読み込み時と大分食い違っているはず | Issue | #1518 |
| `SRCCore/Commands/Command.attack.cs:302` | XXX | XXX COMの攻撃と同じところは切り出しておきたい。。。 | SRC#で不要 | 作り直しの希望で、動作の違いではない（VB も COM と同じ処理を重ねて書いている） |
| `SRCCore/Config/LocalFileConfig.cs:10` | TODO | TODO 項目の説明を設定ファイルに書けるようにする | SRC#で不要 | SRC# の設定ファイル（JSON）の機能の希望で、VB の ReadIni・WriteIni との違いではない（#1498 の GeneralLib の項目で確かめた） |
| `SRCCore/Config/LocalFileConfig.cs:11` | XXX | XXX 説明をどこでエントリーするか？　別にJSONでなくてもいい。 | SRC#で不要 | 同上 |
| `SRCCore/Config/LocalFileConfig.cs:30` | XXX | XXX Configの余地ないしConfigのプロパティではないかもな。 | SRC#で不要 | 設計の覚え書きで、動作に関係しない |
| `SRCCore/Maps/Map.cs:38` | XXX | XXX シリアライズから復元するのか保存用データクラス作るのかはっきりしたほうがよさそう | SRC#で不要 | JSON の Map から復元する形で動いている。RestoreMapData と突き合わせ済み |
| `SRCCore/Maps/Map.cs:116` | XXX | XXX | SRC#で不要 | IsStatusView は VB の `MapFileName = ""` の判定をまとめたもの |
| `SRCCore/Maps/Map.cs:467` | XXX | XXX MapのGUIインタフェースと実装に追い出す | SRC#で不要 | GUIMap.SetMapSize に移してある |
| `SRCCore/Maps/Map.cs:477` | XXX | XXX 1というよりは0にしたいが0長配列は作れない | SRC#で不要 | VB の ClearMap も MapWidth・MapHeight を 1 にする |
| `SRCCore/Models/PilotData.cs:495` | XXX | XXX Readonlyにするとか | SRC#で不要 | SRC# で足した DefinedSpecialPowers の設計のメモ。VB に当たるコードは無い |
| `SRCCore/Models/PilotData.cs:688` | XXX | XXX 例外処理要らんの？ | SRC#で不要 | VB の Feature にもエラーの処理は無く、無ければ実行時エラーになる |
| `SRCCore/Pilots/Pilot.cs:76` | XXX | XXX こうしてるところシリアライズの時だけID解決のほうがいいかな。 | SRC#で不要 | Unit を UnitId から引くという SRC# の保存の設計についての覚え書き。VB の動作とは関係しない |
| `SRCCore/Pilots/Pilot.unit.cs:135` | XXX | XXX 外から操作したいものではない | SRC#で不要 | 作りについてのメモ。VB の `Set MapDataForUnit(.X, .Y) = Nothing` と同じ動作 |
| `SRCCore/Pilots/Pilots.cs:209` | XXX | XXX Clone List | SRC#で不要 | C# は列挙中にコレクションを変えられないので、写してから回す。VB と同じく全員を見る |
| `SRCCore/Units/Unit.ability.cs:32` | XXX | XXX 攻撃同様にしたけれど、UnitXxxを用意するならそっちが持ってるべきだったかもしれない。 | SRC#で不要 | メソッドの置き場所の設計のメモで、動作に関係しない |
| `SRCCore/Units/Unit.feature.cs:18` | コード | For i = .AdditionalFeaturesNum + 1 To .CountAllFeature / fname = .AllFeatureName(i)) | SRC#で不要 | StatusFeatures の説明として VB の参照元（Status.bas）を書いたコメント。コメントアウトしたコードではない |
| `SRCCore/Units/Unit.lookup.cs:1859` | XXX | XXX もうちょいいい感じにしたいもんだ | SRC#で不要 | 書き方についての感想で、動作は VB と同じ |
| `SRCCore/Units/UnitAbility.cs:631` | XXX | XXX 変形技が妥当に定義されていなかった場合 | SRC#で不要 | VB もデータの誤りで実行時エラーになる |
| `SRCCore/Units/UnitWeapon.cs:162` | XXX | XXX この辺の参照考えると UnitWeapon として切り出したのイマイチだったかもなぁ。 | SRC#で不要 | 設計の感想で、動作に関係しない |
| `SRCCore/Units/UnitWeapon.cs:3968` | XXX | XXX この辺の判断切り出せそうな気はする | SRC#で不要 | 処理の切り出しの覚え書きで、動作に関係しない |
| `SRCCore/Units/UnitWeapon.cs:4288` | XXX | XXX 処理簡便にしておきたい | SRC#で不要 | バリア・フィールド・プロテクションの整理の覚え書きで、動作に関係しない |
| `SRCCore/VB/Information.cs:25` | XXX | XXX VB の IsNumeric は &H・&O・括弧（(1)）・通貨記号も数値とみなすが、ここでは読まない（7474/SRC#1545）。 | Issue | #1545。指数表記は #1534 で、読み方の固定は #1554 で直した。&H・&O・括弧の負の数・通貨記号が残る |
