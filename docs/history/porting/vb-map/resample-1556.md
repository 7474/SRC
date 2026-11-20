# 「一致」の抜き取りの読み直し（7474/SRC#1556）

対応表で「一致」とした 854 件から 50 件を選び、前の判断（対応表のメモ）を見ずに、元の SRC（VB5。`SRC/SRC_20121125`）と C# を分岐ごとに読み直した。読み直したあとで前の判断と比べ、食い違いを数えた。

## 結果

- 食い違い（前の判断は「一致」で、読み直すと直すべき違いがあった）: **2 件 / 50 件（4%）**
  - Command.bas の GlobalMapCommand（全体マップの背景色。SRCSharpForm は縮小マップの位置と縦横も）
  - Expression.bas の EvalTerm（引用符で囲んだ項を数値にするときの変換。「"NaN"」が NaN になる）
- どちらも元の SRC の動作を期待値にしたテストを足し、直す前に落ちることを確かめてから直した。対応表の状態を「修正済み」にした
- 判断: 目安（50 件のうち 2 件以下）に収まるので、**ここで打ち切る**。同じモジュールの「一致」には広げない。見つかった 2 件は、どちらも画面（GUI）と数値の変換という、読み比べで見落としやすい種類のもので、分岐の読み違いではなかった

## 選び方

1. `docs/history/porting/vb-map/*_*.md` の状態が「一致」の 854 件を、VB の行数が多い順に並べた
2. テストのカバレッジの代わりに、SRCCoreTests の中で C# のメソッド名が出てくる回数を grep で数えた（下の表の「テスト」。`coverage` の結果は手元で取らなかった）。少ないものを優先した
3. 検証用のシナリオ（`SRC.Sharp/Compat` の torres・probe の `.eve`）で使っていないイベントコマンドを優先した（下の表の「シナリオ」。○は使っている、×は使っていない。コマンド以外の手続きは grep では判断できないので「—」）
4. 次のものは外した
   - 別の担当が同時に進めている CallFunction の Format と文字列関数（Expressions/Functions の String・Format 系）
   - 1,000 行を超える演出の手続き（AttackAnimation・HitAnimation など。名前の文字列の比較が大半で、分岐の読み違いを測るには向かない）と、C# が GUI のプロジェクトにしかない手続き（UpdateMessageForm・ListBox など）
5. モジュールが偏らないようにした（Unit.cls 9・CmdData.cls 12・Effect.bas 3・Command.bas 4・Expression.bas 2・COM.bas 3・Map.bas 4・Pilot.cls 4・Pilots.cls 1・SpecialPowerData.cls 2・Units.cls 1・Sound.bas 1・SRC.bas 1・Event.bas 1・Intermission.bas 1・Item.cls 1）

## 読み直した手続きと結果

「前の判断」は読み直す前の対応表の状態とメモ、「読み直し」は前のメモを見ずに読んだ結果。「食い違い」は、前の判断と読み直しの判断が違ったかどうか。

| # | 手続き | 行数 | テスト | シナリオ | 前の判断 | 読み直し | 食い違い | 直したか |
|---|---|---|---|---|---|---|---|---|
| 1 | Unit.cls CheckBlowAttack | 313 | 11 | — | 一致（摩擦の `t.Area = Class` は VB の誤りとして再現しない） | 同じ違いを見つけた。それ以外の分岐（Ｍ移の `sx = ty`・攻撃側の位置の地形を見る誤り・激突と巻き添えのダメージ）は VB のとおり | 無し | — |
| 2 | Unit.cls CheckAutoAttack | 256 | 3 | — | 一致 | 一致 | 無し | — |
| 3 | Unit.cls CheckAutoHyperMode | 178 | 6 | — | 一致 | 一致（CInt は偶数丸め。`With UDList.Item` の中の IsFeatureAvailable は自分を見る） | 無し | — |
| 4 | Unit.cls CheckAdditionalAttack | 175 | 3 | — | 一致 | 一致 | 無し | — |
| 5 | Unit.cls CheckAutoNormalMode | 139 | 2 | — | 一致 | 一致 | 無し | — |
| 6 | Unit.cls CheckShieldFeature | 105 | 3 | — | 一致 | 一致（dmg は ByVal で、呼び出し元のダメージは変わらない。VB も C# も同じ） | 無し | — |
| 7 | Unit.cls IsTargetReachable | 70 | 1 | — | 一致 | 一致（`Abs(.y - i)` の誤りも同じ） | 無し | — |
| 8 | Unit.cls MaxSkillLevel | 69 | 0 | — | 一致 | 一致（メインパイロットが追加パイロットのとき Pilot(1) を見ないのも同じ） | 無し | — |
| 9 | Unit.cls UpdateCondition | 59 | 0 | — | 一致 | 一致。For Each の中で Collection から消す動作（すべて消え、次の要素を飛ばさない）は VB5 の実機で確かめた（#1550） | 無し | — |
| 10 | CmdData.cls ExecChangeLayerCmd | 177 | 1 | × | 一致（引数の数のエラーの文言は VB の誤りとして再現しない） | 同じ違いを見つけた。ほかに、引数を評価する順（VB は Y 座標を X 座標の範囲の確認より先に評価する）が違うが、引数の式にエラーや副作用があるときだけの違い | 無し | — |
| 11 | CmdData.cls ExecExecCmd | 169 | 1 | × | 一致 | 一致 | 無し | — |
| 12 | CmdData.cls ExecSetSkillCmd | 150 | 3 | × | 一致 | 一致 | 無し | — |
| 13 | CmdData.cls ExecRideCmd | 138 | 1 | ○ | 一致 | 一致（乗り込みの履歴に今のパイロットを含めて調べるが、そのパイロットは降りた後なので結果は同じ） | 無し | — |
| 14 | CmdData.cls ExecRankUpCmd | 118 | 1 | ○ | 一致（MaxHP・MaxEN が 0 のときだけ違う） | 同じ違いを見つけた | 無し | — |
| 15 | CmdData.cls ExecCopyArrayCmd | 113 | 4 | ○ | 一致 | 一致（`name1 = Mid$(name2, 2)` の誤りも同じ） | 無し | — |
| 16 | CmdData.cls ExecRemoveUnitCmd | 106 | 1 | ○ | 一致 | 一致 | 無し | — |
| 17 | CmdData.cls ExecUseAbilityCmd | 98 | 1 | ○ | 一致 | 一致 | 無し | — |
| 18 | CmdData.cls ExecSwapCmd | 87 | 6 | × | 一致 | 一致 | 無し | — |
| 19 | CmdData.cls ExecClearSkillCmd | 84 | 3 | × | 一致 | 一致 | 無し | — |
| 20 | CmdData.cls ExecSetRelationCmd | 61 | 1 | × | 一致 | 一致 | 無し | — |
| 21 | CmdData.cls ExecDisableCmd | 72 | 1 | × | 一致 | 一致 | 無し | — |
| 22 | Effect.bas CountAttack0 | 150 | 3 | — | 一致 | 一致（「５連」で Exit Function が無く 2 回になる VB の動作も同じ） | 無し | — |
| 23 | Effect.bas SpellColor | 104 | 4 | — | 一致 | 一致 | 無し | — |
| 24 | Effect.bas IsBeamWeapon | 81 | 3 | — | 一致 | 一致 | 無し | — |
| 25 | Command.bas SplitCommand | 204 | 5 | — | 一致（最後の RedrawScreen は SRC# の都合） | 同じ | 無し | — |
| 26 | Command.bas MapAttackCommand | 169 | 3 | — | 一致 | 一致 | 無し | — |
| 27 | Command.bas GlobalMapCommand | 126 | 1 | — | 一致（GUI.DisplayGlobalMap が VB と同じ） | **違いあり**: VB は画面を灰色（&HC0C0C0）で塗るが、SRCRender・SRCSharpForm は黒で塗る。SRCSharpForm は縮小マップの位置を MainPWidth でなく MapPWidth で求め、幅と高さを入れ替えて描く | **有り** | 直した |
| 28 | Command.bas FinishFixCommand | 67 | 3 | — | 一致 | 一致 | 無し | — |
| 29 | Expression.bas UndefineVariable | 165 | 21 | — | 一致 | 一致 | 無し | — |
| 30 | Expression.bas EvalTerm | 86 | 2 | — | 一致（1 文字だけの「"」「`」は VB ではエラー 5） | **違いあり**: 「"…"」「`…`」「(…」を数値として求めるとき、VB は StrToDbl（IsNumeric で読めなければ 0）、C# は Conversions.ToDouble で、「"NaN"」が NaN になる | **有り** | 直した |
| 31 | COM.bas TryFix | 237 | 19 | — | 一致（`For k = 2 To u.FeatureData("修理不可")` は再現しない） | 同じ違いを見つけた（VB は修理不可を持つユニットが修理の候補になると型の不一致のエラーになる） | 無し | — |
| 32 | COM.bas CountTargetInRange | 95 | 0 | — | 一致 | 一致（陣営を見るのに `.Mode` を使う VB の書き方も同じ） | 無し | — |
| 33 | COM.bas SearchNearestEnemy | 65 | 15 | — | 一致 | 一致 | 無し | — |
| 34 | Map.bas RestoreMapData | 135 | 0 | — | 一致（違い 3 つを理由付きで残す） | 一致。ClearUnitBitmap が無いことは同じく見つけた | 無し | — |
| 35 | Map.bas AreaInSector | 121 | 8 | — | 一致 | 一致 | 無し | — |
| 36 | Map.bas AreaInWideCross | 76 | 2 | — | 一致 | 一致 | 無し | — |
| 37 | Map.bas AreaWithUnit | 51 | 3 | — | 一致 | 一致 | 無し | — |
| 38 | Pilot.cls IsAbleToRide | 68 | 0 | — | 一致（必要技能を見る順は違うが結果は同じ） | 同じ | 無し | — |
| 39 | Pilot.cls IsSupport | 81 | 2 | — | 一致 | 一致 | 無し | — |
| 40 | Pilot.cls GetOff | 46 | 1 | — | 一致 | 一致 | 無し | — |
| 41 | Pilot.cls SpecialPowerCost | 66 | 18 | — | 一致 | 一致（番号で引く SkillLevel は ＳＰ消費減少 の付加・強化を足さない） | 無し | — |
| 42 | Pilots.cls UpdateSupportMod | 93 | 38 | — | 一致 | 一致 | 無し | — |
| 43 | SpecialPowerData.cls Useful | 109 | 12 | — | 一致 | 一致 | 無し | — |
| 44 | SpecialPowerData.cls CountTarget | 106 | 12 | — | 一致 | 一致 | 無し | — |
| 45 | Units.cls Update | 332 | 0 | — | 一致 | 一致。For Each の中で Delete する動作は VB5 の実機で確かめていない（#1550 で確かめたのは UpdateCondition・Unit.Escape の Remove） | 無し | — |
| 46 | Sound.bas PlayWave | 150 | 0 | — | 一致（null.mp3 と MP3 の効果音で MP3 の BGM を止める動作は合わせない） | 同じ違いを見つけた | 無し | — |
| 47 | SRC.bas SearchDataFolder | 61 | 8 | — | 一致 | 一致 | 無し | — |
| 48 | Event.bas SearchLabel | 273 | 30 | — | 一致 | 一致（入れ替えて比べ直す流れは関数に分けたが、結果は同じ） | 無し | — |
| 49 | Intermission.bas RankUpCost | 129 | 30 | — | 一致 | 一致 | 無し | — |
| 50 | Item.cls IsAvailable | 218 | 70 | — | 一致 | 一致 | 無し | — |

## 直したもの

- **GlobalMapCommand**（`SRC.Sharp/SRCRender/ScreenGUI.map.cs`・`SRC.Sharp/SRCSharpForm/Forms/Main.guimap.cs`）: 縮小マップの外を VB と同じ灰色（&HC0C0C0）で塗る。SRCSharpForm は、縮小マップを画面（MainPWidth × MainPHeight）の中央に、幅 mwidth・高さ mheight で描く。テストは `SRCRenderTests/GlobalMapScreenTests.cs`（直す前は背景が黒で落ちた）。SRCSharpForm は Linux でテストを動かせないので、ビルドだけを確かめた
- **EvalTerm**（`SRC.Sharp/SRCCore/Expressions/Eval.cs`）: 「"…"」「`…`」「(…」の項を数値にするとき、VB と同じく StrToDbl を使う（NaN・無限大は 0）。テストは `SRCCoreTests/Expressions/EvalTermResampleTests.cs`（直す前は「"NaN"」が NaN、「"NaN" + 1」が NaN で落ちた）

## 直さないもの（「一致」のまま、メモに理由がある違い）

読み直しで見つけた違いのうち、前の判断がすでに理由を付けて残していたもの。どれも VB の明らかな不具合か、実行環境の違いによる。

| 手続き | 違い | 理由 |
|---|---|---|
| CheckBlowAttack | 摩擦の条件の `t.Area = Class` を地形のクラスと比べる | VB は With の中でドットを落とし、攻撃側のユニットクラスと比べている（明らかな誤り） |
| ExecChangeLayerCmd | 引数の数のエラーの文言 | VB の文言「ChangeTerrainコマンドの…」は書き写しの誤り |
| ExecRankUpCmd | MaxHP・MaxEN が 0 のとき割合を 100 にする | VB は 0 での割り算（エラーか不定の値。実機では確かめていない） |
| TryFix | 修理不可の名前を一覧として読む | VB は `For k = 2 To <データの文字列>` で、型の不一致のエラーになる |
| PlayWave | null.mp3 と MP3 の効果音で MP3 の BGM を止めない | VB は MP3 を 1 系統でしか鳴らせないための動作。SRC# は BGM と効果音を別の系統で鳴らす |

このうち、摩擦（CheckBlowAttack）・修理不可（TryFix）・ChangeLayer の文言・RankUp の 0 の扱いは、[SRCとの互換性](../../../../SRC.Sharp.Help/src/SRCとの互換性.md) に載っていない（MP3 は載っている）。#1548 の完了の条件（わざと残した違いが互換性の一覧から引ける）のために、ヘルプの担当が載せるかを判断する。

## 読み直しで気付いたこと

- 「一致」には、上の表のように、理由を付けてわざと残した違いを含むものがある（50 件のうち 6 件）。状態の意味（「分岐ごとに突き合わせ、違いが無い」）とは少しずれるが、メモから引けるので、状態は変えない
- 食い違いの 2 件は、C# の本体（SRCCore の分岐）ではなく、GUI のプロジェクトにある処理と、数値の変換の関数の選び方だった。分岐の読み比べだけでは見落としやすい。同じ型の誤り（Conversions.ToDouble と StrToDbl の取り違え）が SRCCore のほかの場所にあるかは、別に grep で確かめるとよい
- For Each の中で Collection から消す動作は、UpdateCondition と Unit.Escape を VB5 の実機で確かめた（すべて消え、次の要素を飛ばさない。#1550。SRC.Sharp/Compat/PROBE.md の VB5.ForEach.*）。Units.Update・UndefineVariable は同じ VB5 の Collection の動作なので同じと読むが、直には確かめていない
