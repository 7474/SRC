// データのプレビューの「表で直す」（7474/SRC#1644）。武器・アビリティの行を、カンマで区切った欄ごとに直す。
// 直した欄だけを書き換え、ほかの欄・空白・行の後ろ（<必要条件> (必要技能)）はそのまま残す。
// 行の続き（末尾の _）を使う行は、表では直さない（テキストで直す）。

/** 武器の行の欄（ユニットデータ.md の書式） */
export const WEAPON_FIELDS = ['name', 'power', 'min', 'max', 'hit', 'bullet', 'en', 'morale', 'adaption', 'critical', 'class'];
/** アビリティの行の欄 */
export const ABILITY_FIELDS = ['name', 'effect', 'range', 'stock', 'en', 'morale', 'class'];

/** 行を欄に分ける。{ fields: [欄の中身（前後の空白を除く）], raw: [欄（空白を含む）] } */
export function splitLine(line) {
  const raw = [];
  let depth = 0;
  let quote = false;
  let start = 0;
  for (let i = 0; i < line.length; i++) {
    const c = line[i];
    if (c === '"') {
      quote = !quote;
    } else if (!quote && (c === '(' || c === '（' || c === '<' || c === '＜')) {
      depth++;
    } else if (!quote && (c === ')' || c === '）' || c === '>' || c === '＞')) {
      depth = Math.max(0, depth - 1);
    } else if (!quote && depth === 0 && c === ',') {
      raw.push(line.slice(start, i));
      start = i + 1;
    }
  }
  raw.push(line.slice(start));
  return { raw, fields: raw.map(x => x.trim()) };
}

/** index 番目の欄を value にした行。欄の前後の空白は元のまま */
export function replaceField(line, index, value) {
  const { raw } = splitLine(line);
  if (index >= raw.length) {
    return line;
  }
  const old = raw[index];
  const lead = old.match(/^\s*/)[0];
  const trail = old.match(/\s*$/)[0];
  raw[index] = lead + value + trail;
  return raw.join(',');
}

/** 行の続き（前の行の末尾か、この行の末尾に _）を使っているか */
export function isContinued(lines, lineNo) {
  const line = lines[lineNo - 1] ?? '';
  const prev = lines[lineNo - 2] ?? '';
  return /_\s*$/.test(line) || /_\s*$/.test(prev);
}
