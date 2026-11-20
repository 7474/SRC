// データのプレビューの「表で直す」（7474/SRC#1644・#1656）。能力値・地形適応・武器・アビリティの行を、カンマで区切った欄ごとに直す。
// 直した欄だけを書き換え、ほかの欄・空白・行末コメント（//）・行の後ろ（<必要条件> (必要技能)）はそのまま残す。
// 行の続き（末尾の _）を使う行は、表では直さない（テキストで直す）。

/** 武器の行の欄（ユニットデータ.md の書式） */
export const WEAPON_FIELDS = ['name', 'power', 'min', 'max', 'hit', 'bullet', 'en', 'morale', 'adaption', 'critical', 'class'];
/** アビリティの行の欄 */
export const ABILITY_FIELDS = ['name', 'effect', 'range', 'stock', 'en', 'morale', 'class'];

/** 引用符の外側にある行末コメント // を分離する */
export function splitTrailingComment(line) {
  let quote = false;
  for (let i = 0; i < line.length - 1; i++) {
    const c = line[i];
    if (c === '"') {
      quote = !quote;
    } else if (!quote && c === '/' && line[i + 1] === '/') {
      return { code: line.slice(0, i), comment: line.slice(i) };
    }
  }
  return { code: line, comment: '' };
}

/** パイロット 2 行目（愛称, [読み仮名,] [性別,] クラス, 地形適応, 経験値）から地形適応の欄番号を得る */
export function pilotAdaptionIndex(line) {
  const { fields } = splitLine(splitTrailingComment(line).code);
  return fields.length >= 4 && fields.length <= 6 ? fields.length - 2 : -1;
}

/** ユニットの能力値・地形適応の欄（statLines の行を使う） */
export const UNIT_STAT_FIELDS = [
  { field: 'hp', lineKey: 'hp', index: 0 },
  { field: 'en', lineKey: 'hp', index: 1 },
  { field: 'armor', lineKey: 'hp', index: 2 },
  { field: 'mobility', lineKey: 'hp', index: 3 },
  { field: 'speed', lineKey: 'move', index: 1 },
  { field: 'adaption', lineKey: 'adaption', index: 0 },
];

/** パイロットの能力値・地形適応の欄（statLines の行を使う） */
export const PILOT_STAT_FIELDS = [
  { field: 'infight', lineKey: 'stats', index: 0 },
  { field: 'shooting', lineKey: 'stats', index: 1 },
  { field: 'hit', lineKey: 'stats', index: 2 },
  { field: 'dodge', lineKey: 'stats', index: 3 },
  { field: 'technique', lineKey: 'stats', index: 4 },
  { field: 'intuition', lineKey: 'stats', index: 5 },
  { field: 'adaption', lineKey: 'adaption', index: pilotAdaptionIndex },
];

export function resolveFieldIndex(col, line) {
  return typeof col.index === 'function' ? col.index(line) : col.index;
}

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

/** index 番目の欄を value にした行。欄の前後の空白と行末の // コメントは元のまま */
export function replaceField(line, index, value) {
  if (index < 0) {
    return line;
  }
  const { code, comment } = splitTrailingComment(line);
  const { raw } = splitLine(code);
  if (index >= raw.length) {
    return line;
  }
  const old = raw[index];
  const lead = old.match(/^\s*/)[0];
  const trail = old.match(/\s*$/)[0];
  raw[index] = lead + value + trail;
  return raw.join(',') + comment;
}

/** 行の続き（前の行の末尾か、この行の末尾に _）を使っているか */
export function isContinued(lines, lineNo) {
  const line = splitTrailingComment(lines[lineNo - 1] ?? '').code;
  const prev = splitTrailingComment(lines[lineNo - 2] ?? '').code;
  return /_\s*$/.test(line) || /_\s*$/.test(prev);
}
