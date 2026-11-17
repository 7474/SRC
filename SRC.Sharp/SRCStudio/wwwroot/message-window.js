// SRC のメッセージウィンドウのプレビュー（7474/SRC#1651・#1655 で共用）。
// ScreenGUI.message.cs の仕様に合わせ、417×70px（12pt ＭＳ Ｐ明朝相当）・64×64px 顔グラ・
// 「;」「:」「.」の区切り・ーー/――→──・タグ（<B><I><BIG><SMALL><SIZE><COLOR><LT><GT>）を再現する。
import { t } from './i18n.js';

const MESSAGE_BOX_WIDTH = 417;
const BASE_PT = 12;
const FONT_FAMILY = '"ＭＳ Ｐ明朝", "MS PMincho", "Yu Mincho", "Hiragino Mincho ProN", serif';
const NAMED_COLORS = {
  black: '#000000', gray: '#808080', silver: '#c0c0c0', white: '#ffffff',
  red: '#ff0000', yellow: '#ffff00', lime: '#00ff00', aqua: '#00ffff',
  blue: '#0000ff', fuchsia: '#ff00ff', maroon: '#800000', olive: '#808000',
  green: '#008000', teal: '#008080', navy: '#000080', purple: '#800080',
};
const KINSOKU = new Set(['。', '、', '…', '‥', '・', '･', '～', 'ー', '－', '！', '？', '」', '』', '）', ')', ' ', '.']);

let measureCanvas;
function measureTextPx(text, pt = BASE_PT, bold = false, italic = false) {
  if (!text) return 0;
  if (typeof document !== 'undefined') {
    measureCanvas ??= document.createElement('canvas');
  }
  const ctx = measureCanvas?.getContext('2d');
  if (!ctx) {
    // テスト環境等で 2D コンテキストが無いときの概算（全角 16px・半角 8px）
    let w = 0;
    for (const ch of text) w += ch.charCodeAt(0) > 127 ? 16 : 8;
    return w * (pt / BASE_PT);
  }
  const px = (pt * 96) / 72;
  ctx.font = `${italic ? 'italic ' : ''}${bold ? 'bold ' : ''}${px.toFixed(1)}px ${FONT_FAMILY}`;
  return ctx.measureText(text).width;
}

/** 改行判定のためにタグを外した文字列（<LT>/<GT> は 1 文字に数える） */
export function stripMessageTags(str) {
  let out = '';
  let inTag = false;
  let tagBuf = '';
  for (let i = 0; i < str.length; i++) {
    const ch = str[i];
    if (!inTag && ch === '<') {
      inTag = true;
      tagBuf = '';
    } else if (inTag && ch === '>') {
      inTag = false;
      const lower = tagBuf.toLowerCase();
      if (lower === 'lt') out += '<';
      else if (lower === 'gt') out += '>';
      else if (!isKnownTag(lower)) out += `<${tagBuf}>`;
    } else if (inTag) {
      tagBuf += ch;
    } else {
      out += ch;
    }
  }
  if (inTag) out += '<' + tagBuf;
  return out;
}

function isKnownTag(lower) {
  return lower === 'b' || lower === '/b' || lower === 'i' || lower === '/i'
    || lower === 'big' || lower === '/big' || lower === 'small' || lower === '/small'
    || lower === '/color' || lower === '/size' || lower === 'lt' || lower === 'gt'
    || lower.startsWith('color=') || lower.startsWith('size=');
}

/** 元の SRC と同じく「――」「ーー」を「──」にし、変数サンプル置換（任意）を適用 */
export function formatSrcMessageText(text, sampleVars) {
  let s = String(text ?? '');
  if (sampleVars) {
    for (const [k, v] of Object.entries(sampleVars)) {
      s = s.split(k).join(v);
    }
  }
  return s.replace(/――/g, '──').replace(/ーー/g, '──');
}

/**
 * 1 つのメッセージ文字列を「;」「:」のページ単位に分解する。
 * @returns {{ text: string, effectOnly?: string, cancelled?: boolean }[]}
 */
export function splitMessagePages(rawText, sampleVars) {
  const raw = String(rawText ?? '').trim();
  if (raw === '-') {
    return [{ text: '', cancelled: true }];
  }
  const formatted = formatSrcMessageText(raw, sampleVars);
  const pages = [];
  for (const semi of formatted.split(';')) {
    const trimmedSemi = semi.trim();
    if (!trimmedSemi) continue;
    // 画像・音声・数字（ウェイト）だけの効果指定はメッセージ欄には出ない
    if (isEffectToken(trimmedSemi)) {
      pages.push({ text: '', effectOnly: trimmedSemi });
      continue;
    }
    const colonParts = semi.split(':');
    if (colonParts.length === 1) {
      pages.push({ text: semi });
    } else {
      for (let i = 1; i <= colonParts.length; i++) {
        pages.push({ text: colonParts.slice(0, i).join('') });
      }
    }
  }
  return pages.length > 0 ? pages : [{ text: formatted }];
}

function isEffectToken(s) {
  const first = s.split(/\s+/)[0]?.toLowerCase() ?? '';
  if (/\.(bmp|jpg|gif|png|wav|mp3|mid)$/i.test(first)) return true;
  if (/^\d+(\.\d+)?$/.test(s)) return true;
  return false;
}

/**
 * ScreenGUI.message.cs に沿って、話者の「…」付与と 417px 幅での自動改行を行う。
 * @returns {string[]} 描画する本文行の配列（話者行 【...】 は別）
 */
export function layoutMessageLines(rawPageText, hasSpeaker = true) {
  let buf = String(rawPageText ?? '');
  if (!buf) return [];
  if (hasSpeaker) {
    if (buf.startsWith('(') && buf.endsWith(')')) {
      buf = '（' + buf.slice(1, -1) + '）';
    } else if (buf.startsWith('(')) {
      buf = '（' + buf.slice(1);
    } else if (buf.startsWith('「') && buf.endsWith('」')) {
      // そのまま
    } else if (buf.startsWith('『') && buf.endsWith('』')) {
      // そのまま
    } else {
      buf = '「' + buf + '」';
    }
  }

  const lines = [];
  let lineHead = 0;
  let inTag = false;
  const maxWidth = MESSAGE_BOX_WIDTH * 0.95;

  const currentLineWidth = (endIdx) => {
    const prefix = lines.length > 0 && hasSpeaker ? '  ' : '';
    return measureTextPx(prefix + stripMessageTags(buf.slice(lineHead, endIdx)));
  };

  for (let j = 0; j < buf.length; j++) {
    const ch = buf[j];
    if (ch === '.' && !inTag) {
      if (j !== lineHead) {
        lines.push((lines.length > 0 && hasSpeaker ? '  ' : '') + buf.slice(lineHead, j));
      }
      lineHead = j + 1;
      continue;
    }
    if (ch === '<') {
      inTag = true;
      continue;
    }
    if (ch === '>') {
      inTag = false;
    } else if (inTag) {
      continue;
    }

    if (currentLineWidth(j + 1) > maxWidth && j > lineHead) {
      lines.push((lines.length > 0 && hasSpeaker ? '  ' : '') + buf.slice(lineHead, j));
      lineHead = j;
      continue;
    }

    const next1 = buf[j + 1] ?? '';
    const next2 = buf[j + 2] ?? '';
    const next3 = buf[j + 3] ?? '';
    if (KINSOKU.has(next1) || KINSOKU.has(next2) || next3 === '.') {
      continue;
    }

    if (currentLineWidth(buf.length) <= maxWidth) {
      continue;
    }

    const w = currentLineWidth(j + 1);
    if (ch === '。' && w > MESSAGE_BOX_WIDTH * 0.6) {
      lines.push((lines.length > 0 && hasSpeaker ? '  ' : '') + buf.slice(lineHead, j + 1));
      lineHead = j + 1;
    } else if ((ch === '、' || ch === '！' || ch === '？' || ch === ' ') && w > MESSAGE_BOX_WIDTH * 0.8) {
      lines.push((lines.length > 0 && hasSpeaker ? '  ' : '') + buf.slice(lineHead, j + 1));
      lineHead = j + 1;
    }
  }
  if (lineHead < buf.length) {
    lines.push((lines.length > 0 && hasSpeaker ? '  ' : '') + buf.slice(lineHead));
  }
  return lines;
}

/** タグを解釈して DOM ノードの span 列を組み立てる（XSS 安全） */
export function renderTaggedLine(lineText, parent) {
  let pt = BASE_PT;
  let bold = false;
  let italic = false;
  let color = null;
  let buf = '';
  let inTag = false;
  let tagBuf = '';

  const flush = () => {
    if (!buf) return;
    const span = document.createElement('span');
    span.textContent = buf;
    if (bold) span.style.fontWeight = 'bold';
    if (italic) span.style.fontStyle = 'italic';
    if (color) span.style.color = color;
    if (pt !== BASE_PT) span.style.fontSize = `${((pt * 96) / 72).toFixed(1)}px`;
    parent.append(span);
    buf = '';
  };

  for (let i = 0; i < lineText.length; i++) {
    const ch = lineText[i];
    if (!inTag && ch === '<') {
      flush();
      inTag = true;
      tagBuf = '';
    } else if (inTag && ch === '>') {
      inTag = false;
      const lower = tagBuf.toLowerCase();
      if (lower === 'b') bold = true;
      else if (lower === '/b') bold = false;
      else if (lower === 'i') italic = true;
      else if (lower === '/i') italic = false;
      else if (lower === 'big') pt += 2;
      else if (lower === '/big') pt = Math.max(6, pt - 2);
      else if (lower === 'small') pt = Math.max(6, pt - 2);
      else if (lower === '/small') pt += 2;
      else if (lower === '/color') color = null;
      else if (lower === '/size') pt = BASE_PT;
      else if (lower === 'lt') { buf += '<'; flush(); }
      else if (lower === 'gt') { buf += '>'; flush(); }
      else if (lower.startsWith('color=')) {
        const c = lower.slice(6).trim();
        if (NAMED_COLORS[c]) color = NAMED_COLORS[c];
        else if (/^#[0-9a-f]{6}$/i.test(c)) color = c;
      } else if (lower.startsWith('size=')) {
        const n = Number(lower.slice(5));
        if (Number.isFinite(n) && n >= 6 && n <= 48) pt = n;
      } else {
        buf += `<${tagBuf}>`;
        flush();
      }
    } else if (inTag) {
      tagBuf += ch;
    } else {
      buf += ch;
    }
  }
  if (inTag) buf += '<' + tagBuf;
  flush();
}

/**
 * メッセージウィンドウのプレビュー部品を描画・操作する。
 * 戦闘メッセージ・ダイアログ（#1651）と会話イベントのプレビュー（#1655）の両方で使う。
 */
export class MessageWindowPreview {
  constructor(element, options = {}) {
    this.element = element;
    this.options = options;
    this.frames = [];
    this.index = 0;
    this.substituteVars = true;
    this.loadSeq = 0;

    element.classList.add('mw-preview');
    element.innerHTML = `
      <div class="mw-controls">
        <div class="mw-nav">
          <button type="button" class="mw-prev">${t('msg.prev')}</button>
          <span class="mw-step"></span>
          <button type="button" class="mw-next">${t('msg.next')}</button>
        </div>
        <label class="mw-vars-toggle" hidden><input type="checkbox" checked> ${t('msg.substituteVars')}</label>
      </div>
      <div class="mw-frame">
        <div class="mw-face-wrap"><canvas class="mw-face" width="64" height="64"></canvas></div>
        <div class="mw-box">
          <div class="mw-speaker"></div>
          <div class="mw-lines"></div>
        </div>
      </div>
      <div class="mw-meta"></div>`;

    element.querySelector('.mw-prev').addEventListener('click', () => {
      if (this.index > 0) {
        this.index--;
        this.renderFrame();
      }
    });
    element.querySelector('.mw-next').addEventListener('click', () => {
      if (this.index + 1 < this.frames.length) {
        this.index++;
        this.renderFrame();
      }
    });
    const varCb = element.querySelector('.mw-vars-toggle input');
    varCb.addEventListener('change', () => {
      this.substituteVars = varCb.checked;
      this.setMessages(this.lastMessages, this.lastSampleVars);
    });
  }

  /**
   * @param {{ speaker?: string, nickname?: string, bitmap?: string, text: string }[]} messages
   * @param {Record<string, string>} sampleVars
   */
  setMessages(messages, sampleVars = {}) {
    this.lastMessages = messages ?? [];
    this.lastSampleVars = sampleVars;
    const hasVar = this.lastMessages.some(m => /\$\([^)]+\)/.test(m.text ?? ''));
    this.element.querySelector('.mw-vars-toggle').hidden = !hasVar;

    const vars = this.substituteVars ? sampleVars : null;
    this.frames = [];
    for (const m of this.lastMessages) {
      const displaySpeaker = m.nickname && m.nickname !== 'システム' && m.speaker !== 'ナレーション' ? m.nickname : '';
      for (const page of splitMessagePages(m.text, vars)) {
        this.frames.push({
          speaker: displaySpeaker,
          rawSpeaker: m.speaker ?? '',
          bitmap: m.bitmap ?? '',
          page,
        });
      }
    }
    this.index = 0;
    this.renderFrame();
  }

  renderFrame() {
    const prevBtn = this.element.querySelector('.mw-prev');
    const nextBtn = this.element.querySelector('.mw-next');
    const stepEl = this.element.querySelector('.mw-step');
    const speakerEl = this.element.querySelector('.mw-speaker');
    const linesEl = this.element.querySelector('.mw-lines');
    const metaEl = this.element.querySelector('.mw-meta');
    const canvas = this.element.querySelector('.mw-face');

    const total = this.frames.length;
    prevBtn.disabled = this.index <= 0;
    nextBtn.disabled = this.index + 1 >= total;
    stepEl.textContent = total > 0 ? t('msg.step', this.index + 1, total) : '0 / 0';
    this.element.querySelector('.mw-nav').hidden = total <= 1;

    speakerEl.textContent = '';
    linesEl.textContent = '';
    metaEl.textContent = '';
    metaEl.className = 'mw-meta';

    const frame = this.frames[this.index];
    if (!frame) {
      const ctx = canvas.getContext('2d');
      ctx?.clearRect(0, 0, canvas.width, canvas.height);
      return;
    }

    if (frame.speaker) {
      speakerEl.textContent = `【${frame.speaker}】`;
    }

    if (frame.page.cancelled) {
      const p = document.createElement('p');
      p.className = 'mw-special';
      p.textContent = t('msg.cancelled');
      linesEl.append(p);
    } else if (frame.page.effectOnly) {
      const p = document.createElement('p');
      p.className = 'mw-special';
      p.textContent = t('msg.effectOnly', frame.page.effectOnly);
      linesEl.append(p);
    } else {
      const bodyLines = layoutMessageLines(frame.page.text, Boolean(frame.speaker));
      for (const line of bodyLines) {
        const div = document.createElement('div');
        div.className = 'mw-line';
        renderTaggedLine(line, div);
        linesEl.append(div);
      }
      const maxBodyLines = frame.speaker ? 3 : 4;
      if (bodyLines.length > maxBodyLines) {
        metaEl.className = 'mw-meta mw-overflow';
        metaEl.textContent = t('msg.overflow', bodyLines.length + (frame.speaker ? 1 : 0));
      }
    }

    const seq = ++this.loadSeq;
    const ctx = canvas.getContext('2d');
    ctx?.clearRect(0, 0, canvas.width, canvas.height);
    if (frame.bitmap && frame.bitmap !== '-.bmp' && this.options.loadImage) {
      this.options.loadImage('pilot', frame.bitmap).then(img => {
        if (!img || seq !== this.loadSeq || !ctx) return;
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.imageSmoothingEnabled = false;
        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
      }, () => {});
    }
  }
}
