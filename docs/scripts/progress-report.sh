#!/bin/bash

# SRC# Progress Report Script
# このスクリプトは現在の進捗状況をレポートします

set -e

# Repository settings
OWNER="7474"
REPO="SRC"

echo "========================================="
echo "SRC# Migration Progress Report"
echo "Generated: $(date '+%Y-%m-%d %H:%M:%S')"
echo "========================================="
echo ""

# Total issues
echo "📊 Overall Statistics"
echo "-------------------------------------"
TOTAL_ISSUES=$(gh issue list --repo $OWNER/$REPO --state all --json number --jq '. | length')
OPEN_ISSUES=$(gh issue list --repo $OWNER/$REPO --state open --json number --jq '. | length')
CLOSED_ISSUES=$(gh issue list --repo $OWNER/$REPO --state closed --json number --jq '. | length')

echo "Total Issues: $TOTAL_ISSUES"
echo "Open Issues: $OPEN_ISSUES"
echo "Closed Issues: $CLOSED_ISSUES"

if [ $TOTAL_ISSUES -gt 0 ]; then
  COMPLETION_RATE=$(echo "scale=2; $CLOSED_ISSUES * 100 / $TOTAL_ISSUES" | bc)
  echo "Completion Rate: $COMPLETION_RATE%"
fi
echo ""

# Epic-wise progress
echo "📈 Progress by Epic"
echo "-------------------------------------"

EPICS=("combat" "unit-pilot" "ui" "events" "data" "vb6-legacy" "performance" "bugfix")
EPIC_NAMES=(
  "戦闘システム / Combat"
  "ユニット・パイロット / Unit-Pilot"
  "GUI・UI"
  "イベント・コマンド / Events"
  "データ管理 / Data"
  "VB6レガシー / VB6 Legacy"
  "パフォーマンス / Performance"
  "バグ修正 / Bug Fixes"
)

for i in "${!EPICS[@]}"; do
  EPIC="${EPICS[$i]}"
  NAME="${EPIC_NAMES[$i]}"
  
  EPIC_TOTAL=$(gh issue list --repo $OWNER/$REPO --label "epic:$EPIC" --state all --json number --jq '. | length')
  EPIC_OPEN=$(gh issue list --repo $OWNER/$REPO --label "epic:$EPIC" --state open --json number --jq '. | length')
  EPIC_CLOSED=$(gh issue list --repo $OWNER/$REPO --label "epic:$EPIC" --state closed --json number --jq '. | length')
  
  if [ $EPIC_TOTAL -gt 0 ]; then
    EPIC_RATE=$(echo "scale=2; $EPIC_CLOSED * 100 / $EPIC_TOTAL" | bc)
    echo "$NAME: $EPIC_CLOSED/$EPIC_TOTAL ($EPIC_RATE%)"
  else
    echo "$NAME: No issues yet"
  fi
done
echo ""

# Milestone progress
echo "🎯 Progress by Milestone"
echo "-------------------------------------"
gh api repos/$OWNER/$REPO/milestones \
  --jq '.[] | "\(.title): \(.closed_issues)/\(.open_issues + .closed_issues) (\(if (.open_issues + .closed_issues) > 0 then ((.closed_issues * 100) / (.open_issues + .closed_issues) | floor) else 0 end)%)"' \
  2>/dev/null || echo "No milestones found"
echo ""

# Priority breakdown
echo "🚨 Issues by Priority"
echo "-------------------------------------"
for priority in "critical" "high" "medium" "low"; do
  COUNT=$(gh issue list --repo $OWNER/$REPO --label "priority:$priority" --state open --json number --jq '. | length')
  echo "Priority $priority: $COUNT open"
done
echo ""

# Recently closed issues
echo "✅ Recently Closed (Last 5)"
echo "-------------------------------------"
gh issue list --repo $OWNER/$REPO --state closed --limit 5 --json number,title,closedAt --jq '.[] | "#\(.number): \(.title) (closed: \(.closedAt[:10]))"' || echo "No closed issues"
echo ""

# Currently in progress
echo "🚧 Currently In Progress"
echo "-------------------------------------"
gh issue list --repo $OWNER/$REPO --label "status:in-progress" --json number,title,assignees --jq '.[] | "#\(.number): \(.title) (\(.assignees | map(.login) | join(", ")))"' || echo "No issues in progress"
echo ""

echo "========================================="
echo "Report complete!"
echo "========================================="
