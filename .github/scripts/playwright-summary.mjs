// Playwright の JSON の結果（reporter の json）から、ジョブのサマリと注釈を作る。
// やり直して通ったテスト（flaky）は、通ったことで見えなくならないよう一覧にして warning の注釈も出す（docs/testing.md）
// 使い方: node .github/scripts/playwright-summary.mjs <results.json> >> "$GITHUB_STEP_SUMMARY"
import fs from 'node:fs';

const [file, title = 'Playwright'] = process.argv.slice(2);
if (!file || !fs.existsSync(file)) {
  console.log(`## ${title}\n\n結果のファイル（${file}）がありません。`);
  process.exit(0);
}
const report = JSON.parse(fs.readFileSync(file, 'utf8'));
const tests = [];
const perf = {};
const walk = (suite, names) => {
  for (const spec of suite.specs ?? []) {
    for (const test of spec.tests ?? []) {
      tests.push({ title: [...names, spec.title].filter(Boolean).join(' › '), file: spec.file, line: spec.line, status: test.status });
      // 計測（annotation の type が perf で「名前=値」）。やり直したときは最後の値
      for (const a of (test.results?.at(-1)?.annotations ?? test.annotations ?? []).filter(a => a.type === 'perf')) {
        const [name, value] = a.description.split('=');
        perf[name] = Number(value);
      }
    }
  }
  for (const child of suite.suites ?? []) {
    walk(child, [...names, child.title]);
  }
};
for (const suite of report.suites ?? []) {
  walk(suite, []);
}
const count = status => tests.filter(t => t.status === status).length;
const flaky = tests.filter(t => t.status === 'flaky');
const lines = [
  `## ${title}`,
  '',
  `| 通った | やり直して通った（flaky） | 失敗 | 飛ばした |`,
  `|---|---|---|---|`,
  `| ${count('expected')} | ${flaky.length} | ${count('unexpected')} | ${count('skipped')} |`,
];
if (flaky.length) {
  lines.push('', '**やり直して通ったテスト**（不安定。原因を直す）', '');
  for (const t of flaky) {
    lines.push(`- ${t.title}（${t.file}:${t.line}）`);
    // 注釈は stderr に出しても GitHub Actions が拾う。サマリ（stdout）と混ぜない
    console.error(`::warning file=SRC.Sharp/SRCSharpWeb.E2E/tests/${t.file},line=${t.line}::やり直して通った（flaky）: ${t.title}`);
  }
}
// 計測の値を基準（.github/perf-baseline.json の PERF_MODE ごと）と比べ、1.5 倍を超えたら warning を出す。止めはしない（#1365）
if (Object.keys(perf).length) {
  const mode = process.env.PERF_MODE ?? 'default';
  let baseline = {};
  try {
    baseline = JSON.parse(fs.readFileSync(new URL('../perf-baseline.json', import.meta.url), 'utf8'))[mode] ?? {};
  } catch {
    // 基準が無ければ値を出すだけ
  }
  lines.push('', `**計測**（${mode}）`, '', '| 項目 | 値 | 基準 |', '|---|---|---|');
  for (const [name, value] of Object.entries(perf)) {
    const base = baseline[name];
    lines.push(`| ${name} | ${value} | ${base ?? '—'} |`);
    if (base && value > base * 1.5) {
      console.error(`::warning::${name} が ${value}（基準 ${base} の 1.5 倍を超えた。.github/perf-baseline.json）`);
    }
  }
}
console.log(lines.join('\n'));
