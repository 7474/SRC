import * as vscode from 'vscode';

/** 出来事のフォームの項目（srcsharp event-text の --json。かんたん作成の出来事と同じ） */
export interface EventFormValue {
  trigger: 'turn' | 'attack' | 'damaged' | 'destroyed';
  turn?: number;
  phase?: string;
  unit?: string;
  target?: string;
  ratio?: number;
  talk: { speaker: string; text: string }[];
  retreat?: boolean;
}

/** テスト用: フォームの答え（QuickPick は選ぶ項目のラベル、InputBox は入れる文字）。あれば画面を出さずに順に使う。undefined はやめたとき */
let scripted: (string | undefined)[] | undefined;

export function setFormAnswers(answers: (string | undefined)[] | undefined): void {
  scripted = answers ? [...answers] : undefined;
}

type Pickable = string | vscode.QuickPickItem;

function showQuickPick<T extends Pickable>(items: T[], options: vscode.QuickPickOptions): Thenable<T | undefined> {
  if (scripted) {
    const answer = scripted.shift();
    return Promise.resolve(answer === undefined ? undefined : items.find((x) => (typeof x === 'string' ? x : x.label) === answer));
  }
  return vscode.window.showQuickPick(items as vscode.QuickPickItem[], options) as Thenable<T | undefined>;
}

function showInputBox(options: vscode.InputBoxOptions): Thenable<string | undefined> {
  if (scripted) {
    const answer = scripted.shift();
    return Promise.resolve(answer !== undefined && options.validateInput && options.validateInput(answer) ? undefined : answer);
  }
  return vscode.window.showInputBox(options);
}

/** 開いているイベントに出てくる人（Create・Launch・Talk・Ride のパイロット） */
function castOf(doc: vscode.TextDocument): string[] {
  const cast: string[] = [];
  for (const raw of doc.getText().split(/\r?\n/)) {
    const t = raw.trim().split(/[\s　]+/);
    // Create 陣営 ユニット ランク パイロット レベル X Y のパイロットは 5 つ目
    const name = /^(Talk|AutoTalk|Launch|Ride)$/i.test(t[0]) ? t[1] : /^Create$/i.test(t[0]) ? t[4] : undefined;
    const unquoted = name?.replace(/^"(.*)"$/, '$1');
    if (unquoted && !/^[-\d$]/.test(unquoted) && !cast.includes(unquoted)) {
      cast.push(unquoted);
    }
  }
  return cast;
}

/**
 * 出来事のフォーム（7474/SRC#1692）。きっかけ・会話・撤退を QuickPick と InputBox で選ぶ。
 * initial を渡すと、その値を選んだ状態から始める（出来事を直す）。やめたら undefined
 */
export async function eventForm(doc: vscode.TextDocument, heading: string, initial?: EventFormValue): Promise<EventFormValue | undefined> {
  const title = (s: string) => `SRC# ${heading}: ${s}`;
  const cast = castOf(doc);
  const other = '（名前を入力する）';
  const first = (items: string[], value?: string) => (value ? [value, ...items.filter((x) => x !== value)] : items);
  const person = async (s: string, value?: string): Promise<string | undefined> => {
    const picked = await showQuickPick([...first(cast, value), other], { title: title(s) });
    if (picked !== other) {
      return picked;
    }
    const typed = (await showInputBox({ title: title(s), prompt: 'パイロットの名前', value }))?.trim();
    if (typed && !cast.includes(typed)) {
      cast.push(typed);
    }
    return typed || undefined;
  };
  const triggers: { label: string; description: string; value: EventFormValue['trigger'] }[] = [
    { label: 'ターン', description: 'N ターン目の味方・敵のフェイズの始め', value: 'turn' },
    { label: '戦うとき', description: '2 人が戦う前（どちらから仕掛けても）', value: 'attack' },
    { label: '損傷率', description: 'ユニットの損傷率が N% 以上になったとき', value: 'damaged' },
    { label: '落ちたとき', description: 'パイロットのユニットが破壊されたとき', value: 'destroyed' },
  ];
  const now = triggers.find((x) => x.value === initial?.trigger);
  const trigger = await showQuickPick(now ? [{ ...now, description: `${now.description}（今のまま）` }, ...triggers.filter((x) => x !== now)] : triggers, { title: title('きっかけ') });
  if (!trigger) {
    return undefined;
  }
  const ev: EventFormValue = { trigger: trigger.value, talk: [] };
  if (trigger.value === 'turn') {
    const turn = await showInputBox({ title: title('何ターン目'), value: String(initial?.turn ?? 2), validateInput: (v) => (/^\d+$/.test(v.trim()) && +v > 0 ? undefined : '1 以上の数') });
    const phase = turn && (await showQuickPick(first(['敵', '味方'], initial?.phase), { title: title('どちらのフェイズの始め') }));
    if (!turn || !phase) {
      return undefined;
    }
    ev.turn = +turn;
    ev.phase = phase;
  } else {
    const unit = await person(trigger.value === 'attack' ? '戦う 1 人目' : 'だれの', initial?.unit);
    if (!unit) {
      return undefined;
    }
    ev.unit = unit;
    if (trigger.value === 'attack') {
      const target = await person('戦う相手', initial?.target);
      if (!target) {
        return undefined;
      }
      ev.target = target;
    }
    if (trigger.value === 'damaged') {
      const ratio = await showInputBox({ title: title('損傷率（%）'), value: String(initial?.ratio ?? 50), validateInput: (v) => (/^\d+$/.test(v.trim()) && +v > 0 && +v < 100 ? undefined : '1〜99') });
      if (!ratio) {
        return undefined;
      }
      ev.ratio = +ratio;
    }
  }
  // 会話。直すときは、今の会話をそのまま・書き直す・後ろに足すから選ぶ
  const talk: { speaker: string; text: string }[] = [];
  let ask = true;
  if (initial && initial.talk.length > 0) {
    const preview = initial.talk.map((l) => `${l.speaker}「${l.text}」`).join(' ');
    const how = await showQuickPick(
      [
        { label: 'そのまま', description: `${initial.talk.length} 行`, detail: preview, value: 'keep' },
        { label: '後ろに足す', description: '今の会話の後ろにセリフを足す', value: 'append' },
        { label: '書き直す', description: '会話を一から書く（今の会話は消える）', value: 'rewrite' },
      ],
      { title: title('会話') },
    );
    if (!how) {
      return undefined;
    }
    if (how.value !== 'rewrite') {
      talk.push(...initial.talk);
    }
    ask = how.value !== 'keep';
  }
  const done = '（会話はここまで）';
  while (ask) {
    const picked = await showQuickPick([done, ...first(cast, talk[talk.length - 1]?.speaker), other], { title: title(`会話 ${talk.length + 1} 行目を話す人`) });
    if (picked === undefined) {
      return undefined;
    }
    if (picked === done) {
      break;
    }
    const speaker = picked === other ? (await showInputBox({ title: title('話す人'), prompt: 'パイロットの名前' }))?.trim() : picked;
    const text = speaker && (await showInputBox({ title: title(`${speaker} のセリフ`) }));
    if (speaker && text) {
      if (!cast.includes(speaker)) {
        cast.push(speaker);
      }
      talk.push({ speaker, text });
    }
  }
  ev.talk = talk;
  if (trigger.value === 'attack' || trigger.value === 'damaged') {
    const options = first(['撤退させない', '会話のあとで撤退させる'], initial?.retreat ? '会話のあとで撤退させる' : undefined);
    const retreat = await showQuickPick(options, { title: title(`${ev.unit} を撤退させるか`) });
    if (!retreat) {
      return undefined;
    }
    ev.retreat = retreat !== '撤退させない';
  }
  return ev;
}
