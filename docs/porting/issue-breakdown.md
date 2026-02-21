# 個別Issue詳細 / Detailed Issue Breakdown

本ドキュメントは、[移植完了計画](./migration-plan.md)で定義されたEpicごとの具体的なIssueリストです。

## Epic 1: 戦闘システム完成 / Combat System Completion

### Issue 1.1: 回避攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl Dodge
```

#### 実装内容:
- `IsDodgeAttack()` メソッドの実装
- 回避判定ロジックの追加
- テストケースの追加

---

### Issue 1.2: 受け流し攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl Parry
```

#### 実装内容:
- `IsParryAttack()` メソッドの実装
- 受け流し判定ロジック
- テストケースの追加

---

### Issue 1.3: ダミー攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 150-250行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO Impl Dummy
```

#### 実装内容:
- ダミーターゲット攻撃の実装
- ダミー判定ロジック

---

### Issue 1.4: シールド防御の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl ShieldDefense
```

#### 実装内容:
- シールド防御判定
- ダメージ軽減計算

---

### Issue 1.5: 追加攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl AdditionalAttack
```

#### 実装内容:
- 追加攻撃判定
- 連続攻撃ロジック

---

### Issue 1.6: 反撃攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl CounterAttack
```

#### 実装内容:
- 反撃判定ロジック
- 反撃タイミング制御

---

### Issue 1.7: 吹き飛ばし攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO Impl Blow
```

#### 実装内容:
- 吹き飛ばし効果の実装
- 位置移動ロジック

---

### Issue 1.8: 引き寄せ攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO Impl Draw
```

#### 実装内容:
- 引き寄せ効果の実装
- 位置移動ロジック

---

### Issue 1.9: テレポート攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO Impl TeleportAway
```

#### 実装内容:
- テレポート効果の実装
- ランダム位置決定ロジック

---

### Issue 1.10: 変身攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO Impl Metamorph
```

#### 実装内容:
- 変身効果の実装
- ユニット変換ロジック

---

### Issue 1.11: 援護攻撃システムの実装
**ファイル**: `Unit.lookup.cs`, `Command.attack.cs`  
**推定規模**: 400-600行 (size:m)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl Attack Help
// TODO Impl Support Guard
```

#### 実装内容:
- 援護攻撃判定
- 援護ユニット選択ロジック
- 援護防御システム

---

### Issue 1.12: 合体技の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 500-700行 (size:l)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO Impl Combination Attack
```

#### 実装内容:
- 合体技判定
- 複数ユニット連携ロジック
- 合体技ダメージ計算

---

### Issue 1.13: 効果解除攻撃の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO Impl Effect Removal
```

#### 実装内容:
- 状態異常解除攻撃
- 効果解除判定

---

### Issue 1.14: 攻撃マップ計算の改善
**ファイル**: `Unit.attackmap.cs`  
**推定規模**: 300-500行 (size:m)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO 攻撃可能マップの最適化
```

#### 実装内容:
- 攻撃範囲計算の効率化
- キャッシング機構の追加

---

### Issue 1.15: パートナー弾薬消費の実装
**ファイル**: `Unit.attackcheck.cs`  
**推定規模**: 150-250行 (size:s)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO パートナーの弾薬消費
```

#### 実装内容:
- パートナーユニットの弾薬管理
- 消費ロジックの実装

---

## Epic 2: ユニット・パイロットシステム完成 / Unit & Pilot System Completion

### Issue 2.1: IsSkillAvailable() の実装
**ファイル**: `Pilot.skill.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl IsSkillAvailable()
```

#### 実装内容:
- スキル使用可能判定の実装
- ユニット状態チェック
- コメントアウトコードの有効化

---

### Issue 2.2: SkillName() の実装
**ファイル**: `Pilot.skill.cs`  
**推定規模**: 150-250行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl SkillName()
```

#### 実装内容:
- スキル名取得機能
- データベースとの連携

---

### Issue 2.3: IsAbilityAvailable() の実装
**ファイル**: `Unit.ability.cs`, `Unit.lookup.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl IsAbilityAvailable()
```

#### 実装内容:
- アビリティ使用可能判定
- 条件チェックロジック

---

### Issue 2.4: AbilityName() の実装
**ファイル**: `Unit.ability.cs`, `Unit.lookup.cs`  
**推定規模**: 150-250行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl AbilityName()
```

#### 実装内容:
- アビリティ名取得機能
- データベースとの連携

---

### Issue 2.5: IsAbleToEnter() の実装
**ファイル**: `Unit.lookup.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl IsAbleToEnter()
```

#### 実装内容:
- マップ進入可能判定
- 地形・状態チェック

---

### Issue 2.6: IsAvailable() の実装
**ファイル**: `Unit.lookup.cs`  
**推定規模**: 150-250行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO Impl IsAvailable()
```

#### 実装内容:
- ユニット使用可能判定
- 状態チェックロジック

---

### Issue 2.7: 変身システムの完成
**ファイル**: `Unit.otherform.cs`  
**推定規模**: 400-600行 (size:m)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 変身処理の改善
```

#### 実装内容:
- 変身可能判定
- 形態切替ロジック
- 能力値変換

---

### Issue 2.8: パイロット搭乗システムの改善
**ファイル**: `Unit.pilot.cs`  
**推定規模**: 300-500行 (size:m)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 搭乗処理の改善
```

#### 実装内容:
- 搭乗可能判定
- パイロット割り当てロジック

---

### Issue 2.9: ユニット行動可能判定の実装
**ファイル**: `Unit.action.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO 行動可能判定
```

#### 実装内容:
- 行動可能チェック
- 状態異常の考慮

---

### Issue 2.10: ユニット特性システムの完成
**ファイル**: `Unit.feature.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: Medium

#### 実装内容:
- コメントアウトコードの有効化
- `FeatureNecessarySkill()` の実装
- `AllFeatureName()` の実装

---

### Issue 2.11: 召喚システムの改善
**ファイル**: `Unit.summon.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 召喚処理の改善
```

#### 実装内容:
- 召喚可能判定
- 召喚ユニット管理

---

### Issue 2.12: アイテムシステムの完成
**ファイル**: `Unit.item.cs`, `Items.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO アイテム管理の改善
```

#### 実装内容:
- アイテム使用可能判定
- 装備管理ロジック

---

## Epic 3: GUI・UIシステム改善 / GUI & UI System Enhancement

### Issue 3.1: 武器リストボックスの実装
**ファイル**: `Main.gui.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 武器リストボックス
```

#### 実装内容:
- 武器選択UI
- リストボックスコントロール

---

### Issue 3.2: アビリティリストボックスの実装
**ファイル**: `Main.gui.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO アビリティリストボックス
```

#### 実装内容:
- アビリティ選択UI
- リストボックスコントロール

---

### Issue 3.3: メッセージフォーム状態管理
**ファイル**: `SRCSharpFormGUI.message.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO メッセージ状態保存
```

#### 実装内容:
- メッセージ表示状態の保存
- 状態復元機能

---

### Issue 3.4: 背景設定とフィルターの実装
**ファイル**: `SRCSharpFormGUI.draw.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO 背景設定
```

#### 実装内容:
- 背景画像の設定
- カラーフィルター適用

---

### Issue 3.5: ステータス表示の完成
**ファイル**: `Status.cs`  
**推定規模**: 400-600行 (size:m)  
**優先度**: Medium

#### 実装内容:
- コメントアウトコードの精査
- `DisplayGlobalStatus()` の再実装検討
- 不要コードの削除

---

### Issue 3.6: ダイアログシステムの改善
**ファイル**: `Dialog.cs`, `Main.gui.cs`  
**推定規模**: 300-500行 (size:m)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO ダイアログ再生
```

#### 実装内容:
- ダイアログ表示制御
- 再生タイミング管理

---

### Issue 3.7: マップ描画の最適化
**ファイル**: `Main.guimap.cs`  
**推定規模**: 300-500行 (size:m)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO マップ描画最適化
```

#### 実装内容:
- 描画パフォーマンス改善
- キャッシング機構

---

### Issue 3.8: 設定画面の改善
**ファイル**: `Configuration.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO 設定項目の追加
```

#### 実装内容:
- 新規設定項目の追加
- UI改善

---

## Epic 4: イベント・コマンドシステム完成 / Event & Command System Completion

### Issue 4.1: Question コマンドの実装
**ファイル**: `QuestionCmd.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: High

#### 実装内容:
- コメントアウトコードの有効化と改善
- 選択肢表示機能
- 選択結果の処理

---

### Issue 4.2: イベントデータ読込の改善
**ファイル**: `Event.data.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO イベントファイル読込制約
```

#### 実装内容:
- ファイル読込制約の緩和
- エラーハンドリング改善

---

### Issue 4.3: PaintString コマンドの最適化
**ファイル**: `PaintStringCmd.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO パース最適化
```

#### 実装内容:
- コマンドパースの高速化
- メモリ使用量の削減

---

### Issue 4.4: イベントラベル処理の改善
**ファイル**: `Event.label.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO ラベル処理
```

#### 実装内容:
- ラベル検索の最適化
- ジャンプ処理の改善

---

### Issue 4.5: 未実装コマンドの実装 (Batch 1)
**ファイル**: Various `*Cmd.cs` files  
**推定規模**: 400-600行 (size:m)  
**優先度**: Medium

#### 実装内容:
- NotImplementedCmd のうち優先度高のコマンド実装
- 5-8個のコマンドを一括実装

---

### Issue 4.6: 未実装コマンドの実装 (Batch 2)
**ファイル**: Various `*Cmd.cs` files  
**推定規模**: 400-600行 (size:m)  
**優先度**: Low

#### 実装内容:
- NotImplementedCmd のうち優先度低のコマンド実装
- 残りのコマンドを一括実装

---

### Issue 4.7: イベント変数管理の改善
**ファイル**: `Event.variable.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 変数管理の改善
```

#### 実装内容:
- 変数スコープ管理
- 変数検索の最適化

---

### Issue 4.8: イベント描画の改善
**ファイル**: `Event.draw.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO イベント描画
```

#### 実装内容:
- イベント中の描画処理改善
- 特殊効果の実装

---

## Epic 5: データ管理・永続化 / Data Management & Persistence

### Issue 5.1: セーブデータの改善
**ファイル**: `SRC.save.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: High

#### TODOコメント:
```csharp
// TODO セーブ処理の改善
```

#### 実装内容:
- セーブデータフォーマット改善
- 互換性の確保

---

### Issue 5.2: パス正規化の実装
**ファイル**: `SRC.config.cs`, `LocalFileConfig.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO パス正規化
```

#### 実装内容:
- クロスプラットフォーム対応
- パス処理の統一

---

### Issue 5.3: 設定管理システムの改善
**ファイル**: `SRC.config.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 設定管理
```

#### 実装内容:
- 設定の永続化
- デフォルト値管理

---

### Issue 5.4: エラーハンドリングの強化
**ファイル**: Various files  
**推定規模**: 300-500行 (size:m)  
**優先度**: Medium

#### 実装内容:
- 例外処理の統一
- エラーメッセージの改善

---

### Issue 5.5: ロード処理の最適化
**ファイル**: `SRC.save.cs`, `Event.data.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO ロード最適化
```

#### 実装内容:
- ロード速度の改善
- メモリ使用量の削減

---

## Epic 6: VB6レガシー関数置換 / VB6 Legacy Function Replacement

### Issue 6.1: バイト単位文字列関数の実装
**ファイル**: `VB/Strings.cs`  
**推定規模**: 400-600行 (size:m)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO Instrb, Instrrevb, Leftb, Lenb, Midb, Rightb
```

#### 実装内容:
- バイト単位文字列操作関数の実装
- マルチバイト文字対応

---

### Issue 6.2: ファイルダイアログの実装
**ファイル**: `Lib/FileSystem.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO LoadFileDialog, SaveFileDialog
```

#### 実装内容:
- ファイル選択ダイアログ
- クロスプラットフォーム対応

---

### Issue 6.3: VB互換関数の完成
**ファイル**: `VB/Information.cs`, `VB/Conversions.cs`  
**推定規模**: 300-400行 (size:m)  
**優先度**: Low

#### 実装内容:
- 未実装のVB関数の追加
- 互換性の確保

---

## Epic 7: パフォーマンス最適化 / Performance Optimization

### Issue 7.1: Sound システムのキャッシング
**ファイル**: `Sound.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 重複検索の削除
```

#### 実装内容:
- サウンドファイル検索のキャッシング
- メモリ使用量の最適化

---

### Issue 7.2: ランダムシーケンスの実装
**ファイル**: `COM.cs` or new file  
**推定規模**: 150-250行 (size:s)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO ランダムシーケンス
```

#### 実装内容:
- 再現性のある乱数生成
- シード管理

---

### Issue 7.3: エイリアス参照の改善
**ファイル**: `Models/AliasData.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO エイリアス参照の改善
```

#### 実装内容:
- 参照の一元化
- 検索の最適化

---

### Issue 7.4: 配列操作の最適化
**ファイル**: `VB/SrcArray.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Low

#### 実装内容:
- 配列操作の高速化
- メモリ効率の改善

---

## Epic 8: バグ修正・エッジケース対応 / Bug Fixes & Edge Cases

### Issue 8.1: 武器選択失敗の修正
**ファイル**: `Unit.weapon.cs`  
**推定規模**: 150-250行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 武器選択失敗
```

#### 実装内容:
- 武器選択エラーの修正
- フォールバック処理

---

### Issue 8.2: レベルベース除算の修正
**ファイル**: Various files  
**推定規模**: 200-300行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO レベルベース除算
```

#### 実装内容:
- ゼロ除算の防止
- エッジケース対応

---

### Issue 8.3: 1オフセット処理の改善
**ファイル**: `Event.data.cs`, Various files  
**推定規模**: 300-500行 (size:m)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO 1オフセット
```

#### 実装内容:
- 配列オフセットの統一
- 境界チェック強化

---

### Issue 8.4: イベントファイル読込制約の緩和
**ファイル**: `Event.data.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Low

#### 実装内容:
- ファイルサイズ制限の緩和
- エラー処理改善

---

### Issue 8.5: SE選択ロジックの改善
**ファイル**: `Unit.se.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Low

#### 実装内容:
- コメントアウトコードの精査
- SE選択ロジックの改善

---

### Issue 8.6: アニメーション処理の改善
**ファイル**: `Unit.anime.cs`  
**推定規模**: 200-300行 (size:s)  
**優先度**: Low

#### TODOコメント:
```csharp
// TODO アニメーション処理
```

#### 実装内容:
- アニメーション再生の改善
- タイミング制御

---

### Issue 8.7: マップ移動処理の修正
**ファイル**: `Unit.map.cs`  
**推定規模**: 250-350行 (size:s)  
**優先度**: Medium

#### TODOコメント:
```csharp
// TODO マップ移動
```

#### 実装内容:
- 移動判定の修正
- 境界チェック

---

## 作業の進め方 / How to Proceed

### 推奨作業順序 / Recommended Order:

1. **Phase 1 (v3.1.0)**: 
   - Epic 1: Issues 1.1-1.6 (基本的な攻撃タイプ)
   - Epic 2: Issues 2.1-2.6 (基本的なユニット・パイロット機能)

2. **Phase 2 (v3.2.0)**:
   - Epic 1: Issues 1.7-1.15 (高度な攻撃機能)
   - Epic 2: Issues 2.7-2.12 (高度なユニット機能)
   - Epic 4: Issues 4.1-4.4 (重要なイベントコマンド)

3. **Phase 3 (v3.3.0)**:
   - Epic 3: Issues 3.1-3.6 (UI改善)
   - Epic 5: Issues 5.1-5.4 (データ管理)
   - Epic 8: Issues 8.1-8.4 (重要なバグ修正)

4. **Phase 4 (v3.4.0)**:
   - Epic 4: Issues 4.5-4.8 (残りのイベントコマンド)
   - Epic 6: All issues (VB6レガシー)
   - Epic 7: All issues (最適化)
   - Epic 8: Issues 8.5-8.7 (残りのバグ修正)

### 各Issueのテンプレート / Issue Template:

```markdown
## 概要 / Overview
[機能の説明]

## TODOコメント / TODO Comment
```csharp
[該当するTODOコメントを引用]
```

## 実装内容 / Implementation Details
- [ ] [実装項目1]
- [ ] [実装項目2]
- [ ] [実装項目3]

## テスト方針 / Test Plan
- [ ] [テスト項目1]
- [ ] [テスト項目2]

## 関連ファイル / Related Files
- `[ファイルパス1]`
- `[ファイルパス2]`

## 推定工数 / Estimated Effort
- 推定: [X]時間
- PR差分: [Y]行

## Labels
- `epic:[epic名]`
- `priority:[優先度]`
- `type:[タイプ]`
- `size:[サイズ]`
```

---

## まとめ / Summary

- **総Issue数**: 約70個
- **総推定作業量**: 大 (18,000-25,000行の変更)
- **推奨期間**: 12-18ヶ月
- **チーム規模**: 2-3名での並行作業を推奨

各Issueは独立して作業可能なように設計されており、複数の開発者が並行して作業できます。
