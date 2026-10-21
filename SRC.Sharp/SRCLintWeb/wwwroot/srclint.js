// srcsharp lint（SRCAuthoring の DataLinter）と静的解析をブラウザで動かす。使い方は README.md
//
//   import { lintZip, preload } from '<置き場所>/srclint.js';
//   const result = await lintZip(file, {
//     systemArchives: ['/files/packs/src-basic/xxxx.zip'],
//     assets: true,
//     onProgress: p => console.log(p.phase, p.file, p.done, p.total),
//     signal: abortController.signal,
//   });
//   // result.diagnostics: [{ file, line, severity: 'error' | 'warning' | 'info', message, source }]
//
// 検査は Web Worker で動かす（ページが固まらない）。Worker を作れない環境では、ページのスレッドで動かす。
// zip の目次を読み、*.eve と *.txt だけを取り出して検査する（画像や音は読まない）。
// systemArchives の zip（URL か Blob）からは汎用データとして Data/System/*.txt（sp.txt など）と Lib/**/*.eve（汎用戦闘アニメなど）を読み込む（検査はしない）。
// assets: true なら、イベントとデータが参照する画像・音・マップのファイルが、シナリオの zip か汎用データの zip の目次にあるかも確かめる（見つからないものは info）。

/**
 * 進み具合。phase は次の順に進む。done は済んだ件数、total は全体の件数、file はそのとき扱っているもの
 * - scenario: シナリオの zip からテキストを取り出す
 * - system: 汎用データの zip を読む（file は zip の名前と、取り出しているファイル）
 * - runtime: 検査の実行環境（.NET の WebAssembly）の読み込みを待つ
 * - data: データとイベントを検査する
 * - analyze: 静的解析（ラベル・include）
 * - assets: 素材の参照
 * @typedef {{ phase: 'scenario' | 'system' | 'runtime' | 'data' | 'analyze' | 'assets', file: string, done: number, total: number }} Progress
 */
export const PHASES = ['scenario', 'system', 'runtime', 'data', 'analyze', 'assets'];

let worker;
let workerFailed = false;
let nextId = 1;
// 検査は 1 つずつ（実行環境は 1 つなので）
let queue = Promise.resolve();

function getWorker() {
  if (workerFailed || typeof Worker === 'undefined') {
    return null;
  }
  if (!worker) {
    try {
      worker = new Worker(new URL('./srclint-worker.js', import.meta.url), { type: 'module' });
    } catch {
      workerFailed = true;
      return null;
    }
  }
  return worker;
}

function stopWorker() {
  worker?.terminate();
  worker = undefined;
}

/** 実行環境を先に読み込む（検査の待ちを短くする） */
export function preload() {
  const w = getWorker();
  if (w) {
    w.postMessage({ type: 'preload' });
    return Promise.resolve();
  }
  return import('./srclint-core.js').then(m => m.preload());
}

/**
 * zip（Blob・File）を検査する。
 * @param {Blob} zip
 * @param {{ systemArchives?: (string | Blob)[], assets?: boolean, onProgress?: (p: Progress) => void, signal?: AbortSignal }} options
 *   systemArchives: 汎用データの zip。URL（Range 要求に応えること）か Blob。assets: 素材の参照も確かめる。
 *   onProgress: 進み具合。signal: 中止（中止すると AbortError で終わる）
 * @returns {Promise<{ diagnostics: { file: string, line: number, severity: 'error' | 'warning' | 'info', message: string, source?: string }[], checkedFiles: number, root: string, systemErrors: string[] }>}
 */
export function lintZip(zip, options = {}) {
  const run = queue.then(() => lintOnce(zip, options));
  queue = run.catch(() => {});
  return run;
}

async function lintOnce(zip, options) {
  const { onProgress, signal, ...rest } = options;
  signal?.throwIfAborted();
  // Worker からは頁の URL が見えないので、URL は絶対にしておく
  const systemArchives = (rest.systemArchives ?? []).map(a => typeof a === 'string' ? new URL(a, location.href).href : a);
  const w = getWorker();
  if (!w) {
    const core = await import('./srclint-core.js');
    return core.lintZip(zip, { ...rest, systemArchives, onProgress });
  }
  const id = nextId++;
  return new Promise((resolve, reject) => {
    const done = () => {
      w.removeEventListener('message', onMessage);
      w.removeEventListener('error', onError);
      signal?.removeEventListener('abort', onAbort);
    };
    const onMessage = e => {
      if (e.data.id !== id) {
        return;
      }
      if (e.data.type === 'progress') {
        onProgress?.(e.data.progress);
      } else if (e.data.type === 'result') {
        done();
        resolve(e.data.result);
      } else if (e.data.type === 'error') {
        done();
        reject(new Error(e.data.message));
      }
    };
    // Worker を読み込めなかった（module の Worker に対応していないなど）。ページのスレッドでやり直す
    const onError = e => {
      e.preventDefault?.();
      done();
      stopWorker();
      workerFailed = true;
      import('./srclint-core.js')
        .then(core => core.lintZip(zip, { ...rest, systemArchives, onProgress }))
        .then(resolve, reject);
    };
    // 中止は Worker ごと止める（C# の検査は途中で止められないため）。次の検査で作り直す
    const onAbort = () => {
      done();
      stopWorker();
      reject(signal.reason ?? new DOMException('中止しました', 'AbortError'));
    };
    w.addEventListener('message', onMessage);
    w.addEventListener('error', onError);
    signal?.addEventListener('abort', onAbort, { once: true });
    w.postMessage({ type: 'lint', id, zip, options: { ...rest, systemArchives } });
  });
}
