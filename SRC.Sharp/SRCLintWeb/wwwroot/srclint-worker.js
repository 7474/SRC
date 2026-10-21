// srclint.js が作る Web Worker。検査をページのスレッドの外で動かし、進み具合と結果をメッセージで返す
import { lintZip, preload } from './srclint-core.js';

self.onmessage = async e => {
  const { type, id, zip, options } = e.data;
  if (type === 'preload') {
    preload().catch(() => {});
    return;
  }
  try {
    const result = await lintZip(zip, { ...options, onProgress: p => self.postMessage({ type: 'progress', id, progress: p }) });
    self.postMessage({ type: 'result', id, result });
  } catch (err) {
    self.postMessage({ type: 'error', id, message: String(err?.message ?? err) });
  }
};
