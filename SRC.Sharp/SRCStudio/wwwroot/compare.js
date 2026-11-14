// データのプレビューの「比べる」（7474/SRC#1644）。書いているユニット・パイロットの数値を、準拠するデータパック（GSC データパックのロボ・拡張）の
// 全ユニット・同じ帯のパイロットの中に置き、上位何 % かを出す。比べる相手は 7474/SRC-Data の Pages の評価の JSON（書式 2 の units・pilots。
// データビューアの「比べる」と同じもの。tools/pack-analysis/README.md の「JSON」）。

/** 評価の JSON の一覧。?analysis= で変えられる（テスト用） */
export const ANALYSIS_INDEX = new URLSearchParams(globalThis.location?.search ?? '').get('analysis')
  ?? 'https://7474.github.io/SRC-Data/analysis/index.json';

let indexPromise;
const sets = new Map();

async function index() {
  indexPromise ??= fetch(ANALYSIS_INDEX).then(r => {
    if (!r.ok) {
      throw new Error(`${ANALYSIS_INDEX}: ${r.status}`);
    }
    return r.json();
  });
  return indexPromise;
}

/** 比べられる区分（ロボ・拡張。ユニット・パイロットの一覧のあるもの） */
export async function analysisSections() {
  const i = await index();
  return i.files.map(f => f.section).filter(s => s !== 'Sharp');
}

async function set(section) {
  if (!sets.has(section)) {
    sets.set(section, (async () => {
      const i = await index();
      const file = i.files.find(f => f.section === section);
      const r = await fetch(new URL(file.file, ANALYSIS_INDEX));
      if (!r.ok) {
        throw new Error(`${file.file}: ${r.status}`);
      }
      return r.json();
    })());
  }
  return sets.get(section);
}

/** values の中で v は上から何 %（同じ値は半分ずつ数える） */
function percentile(values, v) {
  if (v == null || values.length === 0) {
    return null;
  }
  const above = values.filter(x => x > v).length;
  const same = values.filter(x => x === v).length;
  return 100 * (above + same / 2) / values.length;
}

/** パイロットの帯（tools/pack-analysis の Band と同じ） */
export function band(name, exp) {
  if (/\((ザコ|汎用)\)/.test(name)) {
    return '名前なし';
  }
  return exp >= 200 ? '経験値200以上' : exp >= 180 ? '経験値180-199' : exp >= 160 ? '経験値160-179' : exp >= 120 ? '経験値120-159' : '経験値120未満';
}

const isMap = w => /Ｍ/.test(w.class ?? '');

export async function compareUnit(u, section) {
  const data = await set(section);
  const units = data.units ?? [];
  const column = f => units.map(f).filter(x => x != null);
  const maxPower = Math.max(0, ...u.weapons.filter(w => !isMap(w)).map(w => w.power));
  const rows = [
    ['HP', u.hp, x => x.hp],
    ['EN', u.en, x => x.en],
    ['装甲', u.armor, x => x.armor],
    ['運動性', u.mobility, x => x.mobility],
    ['移動力', u.speed, x => x.speed],
    ['最大攻撃力', maxPower, x => x.values?.['最大攻撃力']],
  ].map(([label, value, f]) => ({ label, value, percentile: percentile(column(f), value) }));
  rows.note = `${section} の全ユニット ${units.length} 体と比べています`;
  return rows;
}

export async function comparePilot(p, section) {
  const data = await set(section);
  const b = band(p.name, p.exp);
  const pilots = (data.pilots ?? []).filter(x => x.band === b);
  const rows = Object.entries(p.stats).concat([['SP', p.sp]]).map(([label, value]) => ({
    label,
    value,
    percentile: percentile(pilots.map(x => label === 'SP' ? x.sp : x.stats?.[label]).filter(x => x != null), value),
  }));
  rows.note = `${section} の同じ帯（${b}）のパイロット ${pilots.length} 人と比べています`;
  return rows;
}
