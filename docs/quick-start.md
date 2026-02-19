# クイックスタートガイド / Quick Start Guide

SRC# 移植完了プロジェクトを始めるための簡易ガイドです。

## 📋 ドキュメント一覧 / Document List

1. **[移植完了計画](./migration-plan.md)** - プロジェクト全体の概要とEpic定義
2. **[個別Issue詳細](./issue-breakdown.md)** - 70個の具体的なIssueリスト
3. **[GitHub Projects設定ガイド](./github-projects-setup.md)** - プロジェクト管理の設定方法

## 🚀 今すぐ始める / Getting Started

### ステップ1: ラベルとマイルストーンの設定

```bash
# リポジトリのルートディレクトリで実行
cd /path/to/SRC
bash docs/scripts/create-labels.sh
bash docs/scripts/create-milestones.sh
```

### ステップ2: Epic Issueの作成

8つのEpic Issueを作成します：

1. **Epic 1: 戦闘システム完成** (`epic:combat`)
2. **Epic 2: ユニット・パイロットシステム完成** (`epic:unit-pilot`)
3. **Epic 3: GUI・UIシステム改善** (`epic:ui`)
4. **Epic 4: イベント・コマンドシステム完成** (`epic:events`)
5. **Epic 5: データ管理・永続化** (`epic:data`)
6. **Epic 6: VB6レガシー関数置換** (`epic:vb6-legacy`)
7. **Epic 7: パフォーマンス最適化** (`epic:performance`)
8. **Epic 8: バグ修正・エッジケース対応** (`epic:bugfix`)

各Epicには `.github/ISSUE_TEMPLATE/epic-template.md` を使用してください。

### ステップ3: GitHub Projectsの設定

1. リポジトリの `Projects` タブを開く
2. `New project` → `Board` テンプレートを選択
3. プロジェクト名: `SRC# Migration Completion`
4. カラムを作成: Backlog / Ready / In Progress / In Review / Done

詳細は [GitHub Projects設定ガイド](./github-projects-setup.md) を参照してください。

### ステップ4: 最初のIssueの作成

Phase 1 の最優先Issueから始めます：

1. **Issue 1.1: 回避攻撃の実装** (Epic 1)
   - ファイル: `Unit.attackcheck.cs`
   - 推定: 200-300行
   - ラベル: `epic:combat`, `priority:high`, `type:feature`, `size:s`

2. **Issue 2.1: IsSkillAvailable() の実装** (Epic 2)
   - ファイル: `Pilot.skill.cs`
   - 推定: 200-300行
   - ラベル: `epic:unit-pilot`, `priority:high`, `type:feature`, `size:s`

各Issueには `.github/ISSUE_TEMPLATE/feature-template.md` を使用してください。

## 📊 推奨作業順序 / Recommended Work Order

### Phase 1: コア機能完成 (2026 Q2)

**Epic 1: 戦闘システム** (優先)
- Issue 1.1 → 1.2 → 1.3 → 1.4 → 1.5 → 1.6

**Epic 2: ユニット・パイロット** (並行)
- Issue 2.1 → 2.2 → 2.3 → 2.4 → 2.5 → 2.6

### Phase 2: UI/UX改善 (2026 Q3)

**Epic 1: 戦闘システム** (継続)
- Issue 1.7 → 1.8 → 1.9 → 1.10 → 1.11 → 1.12

**Epic 4: イベント・コマンド**
- Issue 4.1 → 4.2 → 4.3 → 4.4

### Phase 3: 品質向上 (2026 Q4)

**Epic 3: GUI・UI**
- Issue 3.1 → 3.2 → 3.3 → 3.4 → 3.5 → 3.6

**Epic 5: データ管理**
- Issue 5.1 → 5.2 → 5.3 → 5.4

### Phase 4: 最適化・完成 (2027 Q1)

**Epic 6: VB6レガシー**
- All issues

**Epic 7: パフォーマンス**
- All issues

## 🏷️ ラベル一覧 / Labels Reference

### Epic ラベル
- `epic:combat` - 戦闘システム
- `epic:unit-pilot` - ユニット・パイロット
- `epic:ui` - GUI・UI
- `epic:events` - イベント・コマンド
- `epic:data` - データ管理
- `epic:vb6-legacy` - VB6レガシー
- `epic:performance` - パフォーマンス
- `epic:bugfix` - バグ修正

### 優先度ラベル
- `priority:critical` - 重大
- `priority:high` - 高
- `priority:medium` - 中
- `priority:low` - 低

### タイプラベル
- `type:epic` - Epic Issue
- `type:feature` - 新機能
- `type:enhancement` - 改善
- `type:bugfix` - バグ修正
- `type:refactor` - リファクタリング
- `type:docs` - ドキュメント

### サイズラベル
- `size:xs` - ~100行
- `size:s` - 200-400行
- `size:m` - 400-700行
- `size:l` - 700-1000行
- `size:xl` - 1000行以上

## 📈 進捗確認方法 / How to Check Progress

### ボードビュー
- GitHub Projects の `Board` ビューで視覚的に確認

### 統計情報
```bash
# 全Issue数
gh issue list --state all | wc -l

# Epic別Issue数
gh issue list --label "epic:combat" --state all

# 完了率
gh issue list --label "epic:combat" --state closed | wc -l
```

### マイルストーンの進捗
- GitHub の `Milestones` ページで各Phaseの進捗率を確認

## 🤝 コントリビューションガイドライン / Contribution Guidelines

### Issue作成時
1. 適切なテンプレートを使用
2. 必須ラベルを付与（epic, priority, type, size）
3. マイルストーンを設定
4. 関連Epicへのリンク

### PR作成時
1. Issue番号を記載（`Closes #XXX`）
2. 差分は1000行以下を目安
3. テストを追加
4. 既存テストを実行して全て通過することを確認

### レビュー時
1. コードの品質
2. テストの網羅性
3. ドキュメントの更新
4. 既存機能への影響

## 📚 参考資料 / Additional Resources

### ドキュメント
- [移植完了計画](./migration-plan.md) - プロジェクト全体像
- [個別Issue詳細](./issue-breakdown.md) - 全Issueリスト
- [GitHub Projects設定](./github-projects-setup.md) - プロジェクト管理

### リポジトリ
- [SRC 公式サイト](http://www.src-srpg.jpn.org/)
- [SRC# GitHub](https://github.com/7474/SRC)
- [SRC# デモ](https://7474.github.io/SRC/)

### Issue テンプレート
- [Epic テンプレート](../.github/ISSUE_TEMPLATE/epic-template.md)
- [Feature テンプレート](../.github/ISSUE_TEMPLATE/feature-template.md)
- [Bugfix テンプレート](../.github/ISSUE_TEMPLATE/bugfix-template.md)

## 💡 Tips

### チーム作業の場合
- Issue を分担する際は依存関係に注意
- 週次ミーティングで進捗を共有
- ブロッカーは早めに報告

### 個人作業の場合
- 小さいIssueから始める（size:xs, size:s）
- 1つのIssueを完了させてから次へ
- 定期的に進捗を記録

### モチベーション維持
- 小さな達成を祝う
- 進捗を可視化する
- コミュニティと共有

## ❓ よくある質問 / FAQ

### Q1: どのEpicから始めればいいですか？
**A**: Epic 1（戦闘システム）から始めることを推奨します。これがコア機能であり、他の機能に影響を与えるためです。

### Q2: PR差分が1000行を超えそうです
**A**: Issueを分割するか、横断的な関心事（リファクタリングなど）の場合は例外として進めてください。

### Q3: TODOコメントの内容が不明確です
**A**: 元のVB6コードや周辺のコードを参照し、必要に応じてIssueで質問してください。

### Q4: テストはどこに書けばいいですか？
**A**: `SRC.Sharp/SRCCoreTests/` 配下に既存のテストパターンに従って追加してください。

### Q5: ドキュメントは更新が必要ですか？
**A**: API変更やユーザー向け機能の変更があれば、関連ドキュメントを更新してください。

## 🎯 次のアクション / Next Actions

1. [ ] ラベルを作成
2. [ ] マイルストーンを作成
3. [ ] GitHub Projects をセットアップ
4. [ ] Epic 1 の親Issueを作成
5. [ ] Issue 1.1 を作成して作業開始

---

**質問や提案があれば、Issueで報告してください！**

最終更新: 2026-02-19
