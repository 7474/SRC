# GitHub Projects 設定ガイド / GitHub Projects Setup Guide

本ドキュメントは、SRC# 移植完了プロジェクトをGitHub Projectsで管理するための設定ガイドです。

## プロジェクトボードの作成 / Creating Project Board

### 1. 新規Projectの作成

1. GitHubリポジトリページで `Projects` タブを開く
2. `New project` をクリック
3. `Board` テンプレートを選択
4. プロジェクト名: `SRC# Migration Completion`

### 2. カラムの設定 / Column Setup

以下のカラムを作成します：

| カラム名 | 説明 | 自動化 |
|---------|------|--------|
| **📋 Backlog** | 未着手のIssue | - |
| **🔍 Ready** | 着手準備完了 | - |
| **🚧 In Progress** | 作業中 | Issue/PRが自動移動 |
| **👀 In Review** | レビュー中 | PRが自動移動 |
| **✅ Done** | 完了 | Issue/PRクローズで自動移動 |

### 3. カラムの自動化設定 / Automation Settings

#### In Progress カラム
- Issue/PRがアサインされたら自動移動
- ラベル `status:in-progress` が付与されたら自動移動

#### In Review カラム
- PRがレビュー待ちになったら自動移動
- ラベル `status:review` が付与されたら自動移動

#### Done カラム
- Issue/PRがクローズされたら自動移動

## ラベルの作成 / Creating Labels

### Epic ラベル / Epic Labels

以下のラベルを作成します：

```bash
# Epic labels (色: #0052CC - 青)
epic:combat          # 戦闘システム / Combat System
epic:unit-pilot      # ユニット・パイロット / Unit & Pilot
epic:ui              # GUI・UI
epic:events          # イベント・コマンド / Events & Commands
epic:data            # データ管理 / Data Management
epic:vb6-legacy      # VB6レガシー / VB6 Legacy
epic:performance     # パフォーマンス / Performance
epic:bugfix          # バグ修正 / Bug Fixes

# Type labels (色: #FBCA04 - 黄)
type:epic            # Epic Issue
type:feature         # 新機能 / Feature
type:enhancement     # 改善 / Enhancement
type:bugfix          # バグ修正 / Bug Fix
type:refactor        # リファクタリング / Refactoring
type:docs            # ドキュメント / Documentation

# Priority labels (色: #D93F0B - 赤系)
priority:critical    # 重大 / Critical
priority:high        # 高 / High
priority:medium      # 中 / Medium
priority:low         # 低 / Low

# Size labels (色: #006B75 - 青緑)
size:xs              # ~100行 / ~100 lines
size:s               # 200-400行 / 200-400 lines
size:m               # 400-700行 / 400-700 lines
size:l               # 700-1000行 / 700-1000 lines
size:xl              # 1000行以上 / 1000+ lines

# Status labels (色: #5319E7 - 紫)
status:blocked       # ブロック中 / Blocked
status:in-progress   # 作業中 / In Progress
status:review        # レビュー中 / In Review
status:on-hold       # 保留中 / On Hold
```

### ラベル作成スクリプト / Label Creation Script

以下のスクリプトを使用してラベルを一括作成できます：

```bash
#!/bin/bash

# Repository settings
OWNER="7474"
REPO="SRC"

# Epic labels
gh label create "epic:combat" --description "戦闘システム / Combat System" --color "0052CC" --repo $OWNER/$REPO
gh label create "epic:unit-pilot" --description "ユニット・パイロット / Unit & Pilot" --color "0052CC" --repo $OWNER/$REPO
gh label create "epic:ui" --description "GUI・UI" --color "0052CC" --repo $OWNER/$REPO
gh label create "epic:events" --description "イベント・コマンド / Events & Commands" --color "0052CC" --repo $OWNER/$REPO
gh label create "epic:data" --description "データ管理 / Data Management" --color "0052CC" --repo $OWNER/$REPO
gh label create "epic:vb6-legacy" --description "VB6レガシー / VB6 Legacy" --color "0052CC" --repo $OWNER/$REPO
gh label create "epic:performance" --description "パフォーマンス / Performance" --color "0052CC" --repo $OWNER/$REPO
gh label create "epic:bugfix" --description "バグ修正 / Bug Fixes" --color "0052CC" --repo $OWNER/$REPO

# Type labels
gh label create "type:epic" --description "Epic Issue" --color "FBCA04" --repo $OWNER/$REPO
gh label create "type:feature" --description "新機能 / Feature" --color "FBCA04" --repo $OWNER/$REPO
gh label create "type:enhancement" --description "改善 / Enhancement" --color "FBCA04" --repo $OWNER/$REPO
gh label create "type:bugfix" --description "バグ修正 / Bug Fix" --color "FBCA04" --repo $OWNER/$REPO
gh label create "type:refactor" --description "リファクタリング / Refactoring" --color "FBCA04" --repo $OWNER/$REPO
gh label create "type:docs" --description "ドキュメント / Documentation" --color "FBCA04" --repo $OWNER/$REPO

# Priority labels
gh label create "priority:critical" --description "重大 / Critical" --color "D93F0B" --repo $OWNER/$REPO
gh label create "priority:high" --description "高 / High" --color "E99695" --repo $OWNER/$REPO
gh label create "priority:medium" --description "中 / Medium" --color "FBCA04" --repo $OWNER/$REPO
gh label create "priority:low" --description "低 / Low" --color "0E8A16" --repo $OWNER/$REPO

# Size labels
gh label create "size:xs" --description "~100行 / ~100 lines" --color "006B75" --repo $OWNER/$REPO
gh label create "size:s" --description "200-400行 / 200-400 lines" --color "006B75" --repo $OWNER/$REPO
gh label create "size:m" --description "400-700行 / 400-700 lines" --color "006B75" --repo $OWNER/$REPO
gh label create "size:l" --description "700-1000行 / 700-1000 lines" --color "006B75" --repo $OWNER/$REPO
gh label create "size:xl" --description "1000行以上 / 1000+ lines" --color "006B75" --repo $OWNER/$REPO

# Status labels
gh label create "status:blocked" --description "ブロック中 / Blocked" --color "5319E7" --repo $OWNER/$REPO
gh label create "status:in-progress" --description "作業中 / In Progress" --color "5319E7" --repo $OWNER/$REPO
gh label create "status:review" --description "レビュー中 / In Review" --color "5319E7" --repo $OWNER/$REPO
gh label create "status:on-hold" --description "保留中 / On Hold" --color "5319E7" --repo $OWNER/$REPO

echo "Labels created successfully!"
```

## マイルストーンの作成 / Creating Milestones

### Phase 1: コア機能完成 (v3.1.0)
- 期限: 2026年6月30日
- 説明: 戦闘システムとユニット・パイロットシステムの基本機能を完成

### Phase 2: UI/UX改善 (v3.2.0)
- 期限: 2026年9月30日
- 説明: GUI・UIシステムとイベント・コマンドシステムの改善

### Phase 3: 品質向上 (v3.3.0)
- 期限: 2026年12月31日
- 説明: データ管理とバグ修正による品質向上

### Phase 4: 最適化・完成 (v3.4.0)
- 期限: 2027年3月31日
- 説明: VB6レガシーの置換とパフォーマンス最適化

### マイルストーン作成スクリプト / Milestone Creation Script

```bash
#!/bin/bash

OWNER="7474"
REPO="SRC"

gh api repos/$OWNER/$REPO/milestones \
  -f title="Phase 1: コア機能完成 (v3.1.0)" \
  -f description="戦闘システムとユニット・パイロットシステムの基本機能を完成 / Complete core combat and unit-pilot systems" \
  -f due_on="2026-06-30T23:59:59Z"

gh api repos/$OWNER/$REPO/milestones \
  -f title="Phase 2: UI/UX改善 (v3.2.0)" \
  -f description="GUI・UIシステムとイベント・コマンドシステムの改善 / Enhance GUI/UI and event-command systems" \
  -f due_on="2026-09-30T23:59:59Z"

gh api repos/$OWNER/$REPO/milestones \
  -f title="Phase 3: 品質向上 (v3.3.0)" \
  -f description="データ管理とバグ修正による品質向上 / Improve quality through data management and bug fixes" \
  -f due_on="2026-12-31T23:59:59Z"

gh api repos/$OWNER/$REPO/milestones \
  -f title="Phase 4: 最適化・完成 (v3.4.0)" \
  -f description="VB6レガシーの置換とパフォーマンス最適化 / Replace VB6 legacy and optimize performance" \
  -f due_on="2027-03-31T23:59:59Z"

echo "Milestones created successfully!"
```

## Epicの作成方法 / How to Create Epics

### 1. Epic Issueの作成

1. `Issues` タブを開く
2. `New issue` をクリック
3. `Epic Issue Template` を選択
4. テンプレートに従って内容を記入
5. 以下のラベルを付与：
   - `type:epic`
   - `epic:[対応するepic名]`
   - `priority:[優先度]`

### 2. 子Issueの作成と紐付け

1. 各機能のIssueを作成
2. Issue本文に `Related to #[Epic Issue番号]` を記載
3. Epic Issueの本文に `- #[子Issue番号]` を追加

### Epic Issue例 / Epic Issue Example

```markdown
## Epic 概要 / Epic Overview

戦闘システムの完全実装を目指します。
攻撃タイプ、援護攻撃、合体技などの実装を含みます。

## 含まれる機能 / Included Features

- [ ] 回避攻撃の実装
- [ ] 受け流し攻撃の実装
- [ ] ダミー攻撃の実装
...

## 主要な対象ファイル / Key Target Files

- `SRC.Sharp/SRCCore/Units/Unit.attackcheck.cs` (13 TODOs)
- `SRC.Sharp/SRCCore/Units/Unit.attack.cs`
...

## 推定作業量 / Estimated Effort

- Issue数: 15個
- 総差分行数: 約3,500-5,000行
- 推定期間: 3-4ヶ月

## 関連Issue / Related Issues

- #1 - 回避攻撃の実装
- #2 - 受け流し攻撃の実装
...

## Labels

- `type:epic`
- `epic:combat`
- `priority:high`
```

## プロジェクトビューの設定 / Project View Settings

### ボードビュー / Board View

デフォルトのボードビューで、カラム間でドラッグ&ドロップでIssueを移動できます。

### テーブルビュー / Table View

以下のカスタムフィールドを追加することを推奨：

| フィールド名 | タイプ | 説明 |
|------------|--------|------|
| Epic | Single Select | 所属するEpic |
| Size | Single Select | PR差分サイズ (xs/s/m/l/xl) |
| Phase | Single Select | フェーズ (1/2/3/4) |
| 推定工数 | Number | 推定作業時間 |
| 実績工数 | Number | 実際の作業時間 |

### ロードマップビュー / Roadmap View

マイルストーンの日程を視覚化します：

1. `Roadmap` ビューを追加
2. マイルストーンをタイムライン上に配置
3. Epicごとに色分け

## 進捗管理 / Progress Tracking

### 週次更新 / Weekly Updates

毎週、以下の情報を更新：

1. 完了したIssueの数
2. 進行中のIssueの状態
3. ブロッカーの確認
4. 次週の計画

### 月次レビュー / Monthly Review

毎月、以下を確認：

1. マイルストーンの進捗率
2. 各Epicの完了率
3. リスクと課題
4. スケジュールの調整

## 自動化の活用 / Using Automation

### GitHub Actions

以下のワークフローを設定することを推奨：

#### 1. Issue自動ラベリング

```yaml
name: Auto Label Issues
on:
  issues:
    types: [opened]

jobs:
  label:
    runs-on: ubuntu-latest
    steps:
      - name: Label based on title
        uses: actions/labeler@v4
        with:
          repo-token: ${{ secrets.GITHUB_TOKEN }}
```

#### 2. PR自動レビュー依頼

```yaml
name: Auto Request Review
on:
  pull_request:
    types: [opened]

jobs:
  review:
    runs-on: ubuntu-latest
    steps:
      - name: Request reviewers
        uses: kentaro-m/auto-assign-action@v1.2.5
```

#### 3. Issue統計レポート

週次で進捗レポートを自動生成：

```yaml
name: Weekly Progress Report
on:
  schedule:
    - cron: '0 0 * * 1'  # 毎週月曜日

jobs:
  report:
    runs-on: ubuntu-latest
    steps:
      - name: Generate report
        run: |
          # 進捗統計を生成
          gh issue list --state all --json number,title,state,labels
```

## Tips & Best Practices

### 1. Issue作成時のチェックリスト

- [ ] 明確なタイトル（ファイル名: 実装内容）
- [ ] 適切なラベル（epic, priority, type, size）
- [ ] マイルストーンの設定
- [ ] 関連Epicへのリンク
- [ ] 推定工数の記載

### 2. PR作成時のチェックリスト

- [ ] Issue番号を記載（`Closes #XXX`）
- [ ] 変更内容の説明
- [ ] テスト結果の記載
- [ ] スクリーンショット（UI変更の場合）
- [ ] レビュアーの指定

### 3. レビュー時のチェックリスト

- [ ] コードの品質
- [ ] テストの網羅性
- [ ] ドキュメントの更新
- [ ] PR差分が1000行以下（横断的な関心事を除く）
- [ ] 既存機能への影響確認

## トラブルシューティング / Troubleshooting

### Issue がプロジェクトボードに表示されない

1. Issueがプロジェクトに追加されているか確認
2. フィルター設定を確認
3. ページをリロード

### 自動化が動作しない

1. GitHub Actions のログを確認
2. 権限設定を確認
3. ワークフローファイルの構文エラーをチェック

## 参考資料 / References

- [GitHub Projects Documentation](https://docs.github.com/en/issues/planning-and-tracking-with-projects)
- [GitHub Labels Best Practices](https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work/managing-labels)
- [移植完了計画](./migration-plan.md)
- [個別Issue詳細](./issue-breakdown.md)

---

最終更新: 2026-02-19
