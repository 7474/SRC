#!/bin/bash

# SRC# Milestone Creation Script
# このスクリプトはGitHub CLIを使用してマイルストーンを一括作成します

set -e

# Repository settings
OWNER="7474"
REPO="SRC"

echo "Creating milestones for $OWNER/$REPO..."

# Phase 1: コア機能完成 (v3.1.0)
echo "Creating Phase 1 milestone..."
gh api repos/$OWNER/$REPO/milestones \
  -f title="Phase 1: コア機能完成 (v3.1.0)" \
  -f description="戦闘システムとユニット・パイロットシステムの基本機能を完成 / Complete core combat and unit-pilot systems" \
  -f due_on="2026-06-30T23:59:59Z" \
  -f state="open" \
  || echo "Milestone already exists or error occurred"

# Phase 2: UI/UX改善 (v3.2.0)
echo "Creating Phase 2 milestone..."
gh api repos/$OWNER/$REPO/milestones \
  -f title="Phase 2: UI/UX改善 (v3.2.0)" \
  -f description="GUI・UIシステムとイベント・コマンドシステムの改善 / Enhance GUI/UI and event-command systems" \
  -f due_on="2026-09-30T23:59:59Z" \
  -f state="open" \
  || echo "Milestone already exists or error occurred"

# Phase 3: 品質向上 (v3.3.0)
echo "Creating Phase 3 milestone..."
gh api repos/$OWNER/$REPO/milestones \
  -f title="Phase 3: 品質向上 (v3.3.0)" \
  -f description="データ管理とバグ修正による品質向上 / Improve quality through data management and bug fixes" \
  -f due_on="2026-12-31T23:59:59Z" \
  -f state="open" \
  || echo "Milestone already exists or error occurred"

# Phase 4: 最適化・完成 (v3.4.0)
echo "Creating Phase 4 milestone..."
gh api repos/$OWNER/$REPO/milestones \
  -f title="Phase 4: 最適化・完成 (v3.4.0)" \
  -f description="VB6レガシーの置換とパフォーマンス最適化 / Replace VB6 legacy and optimize performance" \
  -f due_on="2027-03-31T23:59:59Z" \
  -f state="open" \
  || echo "Milestone already exists or error occurred"

echo "✅ All milestones created successfully!"
echo ""
echo "To view all milestones, run:"
echo "  gh api repos/$OWNER/$REPO/milestones | jq '.[] | {title, due_on, open_issues, closed_issues}'"
