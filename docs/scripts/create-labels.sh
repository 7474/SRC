#!/bin/bash

# SRC# Label Creation Script
# このスクリプトはGitHub CLIを使用してラベルを一括作成します

set -e

# Repository settings
OWNER="7474"
REPO="SRC"

echo "Creating labels for $OWNER/$REPO..."

# Epic labels (color: #0052CC - blue)
echo "Creating Epic labels..."
gh label create "epic:combat" --description "戦闘システム / Combat System" --color "0052CC" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "epic:unit-pilot" --description "ユニット・パイロット / Unit & Pilot" --color "0052CC" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "epic:ui" --description "GUI・UI" --color "0052CC" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "epic:events" --description "イベント・コマンド / Events & Commands" --color "0052CC" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "epic:data" --description "データ管理 / Data Management" --color "0052CC" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "epic:vb6-legacy" --description "VB6レガシー / VB6 Legacy" --color "0052CC" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "epic:performance" --description "パフォーマンス / Performance" --color "0052CC" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "epic:bugfix" --description "バグ修正 / Bug Fixes" --color "0052CC" --repo $OWNER/$REPO || echo "Label already exists"

# Type labels (color: #FBCA04 - yellow)
echo "Creating Type labels..."
gh label create "type:epic" --description "Epic Issue" --color "FBCA04" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "type:feature" --description "新機能 / Feature" --color "FBCA04" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "type:enhancement" --description "改善 / Enhancement" --color "FBCA04" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "type:bugfix" --description "バグ修正 / Bug Fix" --color "FBCA04" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "type:refactor" --description "リファクタリング / Refactoring" --color "FBCA04" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "type:docs" --description "ドキュメント / Documentation" --color "FBCA04" --repo $OWNER/$REPO || echo "Label already exists"

# Priority labels (color: red variants)
echo "Creating Priority labels..."
gh label create "priority:critical" --description "重大 / Critical" --color "D93F0B" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "priority:high" --description "高 / High" --color "E99695" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "priority:medium" --description "中 / Medium" --color "FBCA04" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "priority:low" --description "低 / Low" --color "0E8A16" --repo $OWNER/$REPO || echo "Label already exists"

# Size labels (color: #006B75 - teal)
echo "Creating Size labels..."
gh label create "size:xs" --description "~100行 / ~100 lines" --color "006B75" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "size:s" --description "200-400行 / 200-400 lines" --color "006B75" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "size:m" --description "400-700行 / 400-700 lines" --color "006B75" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "size:l" --description "700-1000行 / 700-1000 lines" --color "006B75" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "size:xl" --description "1000行以上 / 1000+ lines" --color "006B75" --repo $OWNER/$REPO || echo "Label already exists"

# Status labels (color: #5319E7 - purple)
echo "Creating Status labels..."
gh label create "status:blocked" --description "ブロック中 / Blocked" --color "5319E7" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "status:in-progress" --description "作業中 / In Progress" --color "5319E7" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "status:review" --description "レビュー中 / In Review" --color "5319E7" --repo $OWNER/$REPO || echo "Label already exists"
gh label create "status:on-hold" --description "保留中 / On Hold" --color "5319E7" --repo $OWNER/$REPO || echo "Label already exists"

echo "✅ All labels created successfully!"
echo ""
echo "To view all labels, run:"
echo "  gh label list --repo $OWNER/$REPO"
