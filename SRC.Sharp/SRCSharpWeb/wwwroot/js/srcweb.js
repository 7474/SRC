// canvas への画面の書き込みと、マウス・キー入力の受け渡し（SRCSharpWeb/WebInterop.cs）
let canvas;
let ctx;
let imageData;
let imageBytes;

export function setTitle(title) {
    document.title = title;
}

// .NET 側のエラー（ゲームの例外・イベントエラー）を送る（errorreport.js）
export function reportError(type, message, stack) {
    globalThis.srcwebErrorReporting?.report(type, message, stack);
}

export function drawFrame(view, width, height) {
    if (!ctx) {
        return;
    }
    if (!imageData || imageData.width !== width || imageData.height !== height) {
        imageData = ctx.createImageData(width, height);
        // copyTo は Uint8Array しか受け付けない
        imageBytes = new Uint8Array(imageData.data.buffer);
    }
    // .NET のメモリは SharedArrayBuffer のため、ImageData へ複写してから描く
    view.copyTo(imageBytes);
    ctx.putImageData(imageData, 0, 0);
    // 描いた回数（テスト用）
    globalThis.srcwebFrames = (globalThis.srcwebFrames || 0) + 1;
}

export function bindCanvas(id) {
    canvas = document.getElementById(id);
    ctx = canvas.getContext('2d');
    // 表示の大きさ（画面の拡大率を含む）が変わったら、拡大の方法を選び直す
    new ResizeObserver(updateScaling).observe(canvas);
    window.addEventListener('resize', updateScaling);
    updateScaling();
    start();
}

// 表示倍率（システムメニューで選ぶ。localStorage に残す）。
// 'fit' はビューポートに合わせる（app.css）。'1'〜'4' は実際の画素で canvas の 1 画素を N×N 画素にする（ピクセル等倍は '1'）
const DISPLAY_SCALE_SETTING = 'srcweb.displayScale';
const DISPLAY_SCALES = ['fit', '1', '2', '3', '4'];

function getDisplayScale() {
    try {
        const saved = localStorage.getItem(DISPLAY_SCALE_SETTING);
        return DISPLAY_SCALES.includes(saved) ? saved : 'fit';
    } catch {
        return displayScaleFallback;
    }
}
let displayScaleFallback = 'fit';

globalThis.srcwebGetDisplayScale = getDisplayScale;
globalThis.srcwebSetDisplayScale = scale => {
    scale = DISPLAY_SCALES.includes(scale) ? scale : 'fit';
    displayScaleFallback = scale;
    try {
        localStorage.setItem(DISPLAY_SCALE_SETTING, scale);
    } catch {
        // 保存できなくても、この表示の間は変える
    }
    updateScaling();
};

// 倍率を選んでいれば、画面と下のバーの幅を「canvas の幅 × 倍率 ÷ devicePixelRatio」（CSS の px）にする。
// devicePixelRatio はブラウザの拡大縮小・画面の移動で変わるため、表示の大きさが変わるたびに計算し直す
function applyDisplayScale() {
    const scale = getDisplayScale();
    const width = scale === 'fit' ? '' : `${canvas.width * Number(scale) / (window.devicePixelRatio || 1)}px`;
    for (const element of [canvas.closest('.screen-wrap'), document.querySelector('.screen-bar')]) {
        if (element && element.style.width !== width) {
            element.style.width = width;
        }
    }
}

// 実際の画素で canvas の大きさ以上に表示するとき（拡大）は、補間せずドットのまま拡大する
function updateScaling() {
    if (!canvas?.isConnected) {
        return;
    }
    applyDisplayScale();
    const devicePixels = canvas.clientWidth * (window.devicePixelRatio || 1);
    canvas.classList.toggle('pixelated', devicePixels >= canvas.width);
}

async function start() {
    const runtime = await globalThis.getDotnetRuntime(0);
    const exports = await runtime.getAssemblyExports('SRCSharpWeb.dll');
    const gui = exports.SRCSharpWeb.WebInterop;
    // エラーを送るときに、実行中のシナリオの行を添える（errorreport.js）
    globalThis.srcwebErrorReporting?.setDebugState(() => gui.DebugState());

    const position = e => {
        const rect = canvas.getBoundingClientRect();
        return [
            (e.clientX - rect.left) * canvas.width / rect.width,
            (e.clientY - rect.top) * canvas.height / rect.height,
        ];
    };

    // タッチ操作: タップで左クリック、長押しで右クリック、指を滑らせるとマップのスクロール（とカーソルの移動）
    let lastTouch = -Infinity;
    let touch = null;
    let lastTap = null;
    const LONG_PRESS_MS = 500;
    const MOVE_TOLERANCE = 12; // CSS ピクセル
    // シナリオの KeyState(1)・KeyState(2) はボタンが押されている間を調べるため、押した状態を保ってから離す
    const TAP_HOLD_MS = 200;
    const press = (x, y, button, clicks) => {
        gui.Pointer(2, x, y, 0, 0);
        gui.Pointer(0, x, y, button, clicks);
        globalThis.srcwebTouchClicks = [...(globalThis.srcwebTouchClicks || []), { button, clicks }];
    };
    const release = (x, y, button) => gui.Pointer(1, x, y, button, 0);
    canvas.addEventListener('pointerdown', e => {
        if (e.pointerType === 'mouse') {
            return;
        }
        lastTouch = performance.now();
        if (touch) {
            // 2 本目の指は使わない
            return;
        }
        canvas.focus();
        resumeAudio();
        canvas.setPointerCapture?.(e.pointerId);
        const [x, y] = position(e);
        gui.Pointer(2, x, y, 0, 0);
        touch = {
            id: e.pointerId,
            startX: e.clientX,
            startY: e.clientY,
            downX: x,
            downY: y,
            x,
            y,
            moved: false,
            held: false,
            timer: setTimeout(() => {
                if (touch && !touch.moved) {
                    // 右ボタンを押したままにし、指を離したら離す（押し続けてメッセージを送る操作にもなる）
                    touch.held = true;
                    hideRing();
                    navigator.vibrate?.(20);
                    press(touch.x, touch.y, 2, 1);
                }
            }, LONG_PRESS_MS),
        };
        showRing(e.clientX, e.clientY);
        e.preventDefault();
    });
    canvas.addEventListener('pointermove', e => {
        if (!touch || e.pointerId !== touch.id) {
            return;
        }
        lastTouch = performance.now();
        const [x, y] = position(e);
        touch.x = x;
        touch.y = y;
        if (!touch.moved && Math.hypot(e.clientX - touch.startX, e.clientY - touch.startY) > MOVE_TOLERANCE) {
            touch.moved = true;
            clearTimeout(touch.timer);
            hideRing();
            // 触れた位置からマップをドラッグする（マウスの左ボタンのドラッグと同じスクロール。クリックにはしない）
            gui.Pointer(3, touch.downX, touch.downY, 0, 0);
        }
        if (touch.moved) {
            gui.Pointer(2, x, y, 0, 0);
        }
        e.preventDefault();
    });
    const endTouch = (e, cancelled) => {
        if (!touch || e.pointerId !== touch.id) {
            return;
        }
        lastTouch = performance.now();
        clearTimeout(touch.timer);
        hideRing();
        if (touch.moved) {
            gui.Pointer(4, touch.x, touch.y, 0, 0);
        }
        if (touch.held) {
            release(touch.x, touch.y, 2);
        } else if (!cancelled && !touch.moved) {
            // 近くを続けてタップしたらダブルクリック
            const now = performance.now();
            const double = lastTap && now - lastTap.time < 400 && Math.hypot(e.clientX - lastTap.x, e.clientY - lastTap.y) < 24;
            const { x, y } = touch;
            press(x, y, 0, double ? 2 : 1);
            setTimeout(() => release(x, y, 0), TAP_HOLD_MS);
            lastTap = double ? null : { time: now, x: e.clientX, y: e.clientY };
        }
        touch = null;
        e.preventDefault();
    };
    canvas.addEventListener('pointerup', e => endTouch(e, false));
    canvas.addEventListener('pointercancel', e => endTouch(e, true));

    // 長押しの間、触れた位置に輪を出す（右クリックになることを知らせる）
    const ring = document.createElement('div');
    ring.className = 'touch-ring';
    document.body.appendChild(ring);
    function showRing(clientX, clientY) {
        ring.style.left = `${clientX}px`;
        ring.style.top = `${clientY}px`;
        ring.style.setProperty('--hold', `${LONG_PRESS_MS}ms`);
        ring.classList.remove('active');
        void ring.offsetWidth;
        ring.classList.add('active');
    }
    function hideRing() {
        ring.classList.remove('active');
    }

    // タッチの後にブラウザが作る互換のマウスイベントは無視する（タッチは下の Pointer Events で扱う）
    const fromTouch = () => performance.now() - lastTouch < 1000;
    canvas.addEventListener('mousedown', e => {
        if (fromTouch()) {
            e.preventDefault();
            return;
        }
        canvas.focus();
        resumeAudio();
        const [x, y] = position(e);
        gui.Pointer(0, x, y, e.button, e.detail || 1);
        e.preventDefault();
    });
    canvas.addEventListener('mouseup', e => {
        if (fromTouch()) {
            e.preventDefault();
            return;
        }
        const [x, y] = position(e);
        gui.Pointer(1, x, y, e.button, 0);
        e.preventDefault();
    });
    canvas.addEventListener('mousemove', e => {
        if (fromTouch()) {
            return;
        }
        const [x, y] = position(e);
        gui.Pointer(2, x, y, 0, 0);
    });
    canvas.addEventListener('contextmenu', e => e.preventDefault());
    canvas.addEventListener('wheel', e => {
        const [x, y] = position(e);
        const delta = -Math.sign(e.deltaY);
        if (delta !== 0) {
            gui.Wheel(x, y, delta);
        }
        e.preventDefault();
    }, { passive: false });

    // SRC が使うキー（矢印・Enter・Space・Esc・BackSpace・Delete）はブラウザの既定の動作（スクロールなど）を止める
    const handled = new Set([8, 13, 27, 32, 37, 38, 39, 40, 46]);
    canvas.addEventListener('keydown', e => {
        resumeAudio();
        gui.Key(0, e.keyCode);
        if (handled.has(e.keyCode)) {
            e.preventDefault();
        }
    });
    canvas.addEventListener('keyup', e => gui.Key(1, e.keyCode));
    canvas.addEventListener('blur', () => gui.Key(2, 0));

    buildVirtualPad(gui);
    buildTouchUi(gui);

    // .NET は UI スレッドとは別のスレッドで動くため、呼び出しは非同期になる。前の書き込みが終わってから次を求める
    const loop = async () => {
        try {
            await gui.Tick();
        } finally {
            requestAnimationFrame(loop);
        }
    };
    requestAnimationFrame(loop);
    canvas.focus();
}

// タッチ向けの選択画面（拡大表示）。ゲームの画面に描く小さなリスト・メニュー・ダイアログを、大きなボタンの一覧でも出す。
// 中身は WebInterop.TouchUi で読み、選んだものは WebInterop.TouchChoose でゲームに渡す
// （自動プレイと同じ操作。ゲームの画面の描画は変えない）。
// 仮想パッドと場所が重なるため、既定では出さない。システムメニューで切り替え、設定は localStorage に残す
const TOUCH_UI_INTERVAL = 150;
const TOUCH_UI_LIST = 0;
const TOUCH_UI_MENU = 1;
const TOUCH_UI_DIALOG = 2;
const TOUCH_UI_CANCEL = 3;
const TOUCH_UI_SETTING = 'srcweb.touchUi';

function buildTouchUi(gui) {
    const panel = document.getElementById('touch-ui');
    if (!panel) {
        return;
    }
    let last = '{}';
    let busy = false;
    // 選んだ直後は、ゲームが次の表示に移るまで同じものを出さない
    let suppressUntil = 0;
    let enabled = false;
    try {
        enabled = localStorage.getItem(TOUCH_UI_SETTING) === 'on';
    } catch {
        // 保存先を読めなければ出さない
    }
    globalThis.srcwebToggleTouchUi = () => {
        enabled = !enabled;
        last = '{}';
        if (!enabled) {
            panel.hidden = true;
        }
        try {
            localStorage.setItem(TOUCH_UI_SETTING, enabled ? 'on' : 'off');
        } catch {
            // 保存できなくても表示は切り替える
        }
    };

    const choose = (kind, index) => {
        panel.hidden = true;
        suppressUntil = performance.now() + 400;
        resumeAudio();
        gui.TouchChoose(kind, index);
        canvas?.focus();
    };

    const button = (text, onClick, className) => {
        const b = document.createElement('button');
        b.type = 'button';
        b.className = className;
        b.textContent = text;
        b.addEventListener('click', onClick);
        return b;
    };

    const render = state => {
        panel.replaceChildren();
        let kind;
        let title;
        let text;
        let items;
        if (state.dialog) {
            kind = TOUCH_UI_DIALOG;
            title = state.dialog.title;
            text = state.dialog.text;
            items = state.dialog.buttons.map(x => ({ text: x }));
        } else if (state.menu) {
            kind = TOUCH_UI_MENU;
            items = state.menu.items.map(x => ({ text: x }));
        } else if (state.list) {
            kind = TOUCH_UI_LIST;
            title = state.list.caption;
            text = state.list.info;
            items = state.list.items;
        } else {
            panel.hidden = true;
            return;
        }
        if (title) {
            const caption = document.createElement('div');
            caption.className = 'touch-ui-title titlebar';
            caption.textContent = title;
            panel.appendChild(caption);
        }
        if (text && kind !== TOUCH_UI_LIST) {
            const p = document.createElement('p');
            p.className = 'touch-ui-text';
            p.textContent = text;
            panel.appendChild(p);
        }
        const list = document.createElement('div');
        list.className = kind === TOUCH_UI_DIALOG ? 'touch-ui-buttons' : 'touch-ui-items';
        // 見出しの行（武器の一覧の「名称 攻撃 命中…」など）があるリストは、桁をそろえて出し、横にスクロールできるようにする
        const columns = kind === TOUCH_UI_LIST && !!text;
        list.classList.toggle('columns', columns);
        if (columns) {
            const header = document.createElement('div');
            header.className = 'touch-ui-header';
            header.textContent = text;
            list.appendChild(header);
            text = null;
        }
        items.forEach((item, i) => {
            const b = button(item.text, () => choose(kind, i), 'touch-ui-item');
            b.disabled = !!item.disabled;
            list.appendChild(b);
        });
        panel.appendChild(list);
        if (kind !== TOUCH_UI_DIALOG) {
            // 右クリックと同じ（キャンセルできないリストでは何も起きない）
            panel.appendChild(button(panel.dataset.cancel || 'Cancel', () => choose(TOUCH_UI_CANCEL, 0), 'touch-ui-cancel'));
        }
        panel.hidden = false;
        panel.scrollTop = 0;
    };

    setInterval(async () => {
        if (busy) {
            return;
        }
        busy = true;
        try {
            const json = enabled ? await gui.TouchUi() : '{}';
            const state = JSON.parse(json);
            if (state.error) {
                return;
            }
            if (performance.now() < suppressUntil) {
                return;
            }
            if (json !== last || (panel.hidden && json !== '{}')) {
                last = json;
                render(state);
            }
        } catch (e) {
            console.error(e);
        } finally {
            busy = false;
        }
    }, TOUCH_UI_INTERVAL);
}

// 仮想パッド（タッチの端末向け）。左に十字キー、右に A（決定）・B（取消）、中央にシナリオ向けのキーを並べる。
// 物理キーと同じく WebInterop.Key に送るため、シナリオの KeyState とゲームの操作（カーソル・メニュー・ダイアログ）の
// どちらにも効く。code は Windows の仮想キーコード
const VIRTUAL_PAD = [
    { label: '▲', code: 38, area: 'up', repeat: true, title: '↑' },
    { label: '◀', code: 37, area: 'left', repeat: true, title: '←' },
    { label: '▼', code: 40, area: 'down', repeat: true, title: '↓' },
    { label: '▶', code: 39, area: 'right', repeat: true, title: '→' },
    { label: 'A', code: 13, area: 'a', title: 'A（Enter）' },
    { label: 'B', code: 27, area: 'b', title: 'B（Esc）' },
    { label: 'Z', code: 90, area: 'z' },
    { label: 'X', code: 88, area: 'x' },
    { label: 'Space', code: 32, area: 'space' },
    { label: 'Ctrl', code: 17, area: 'ctrl' },
    { label: 'Shift', code: 16, area: 'shift' },
];
const VIRTUAL_PAD_SETTING = 'srcweb.virtualPad';
// 以前の名前（仮想キー）で保存した設定も読む
const OLD_VIRTUAL_PAD_SETTING = 'srcweb.virtualKeys';

/** タッチ向けの画面か（index.html が html の data-input に付ける。利用者が切り替えられる） */
const isTouchMode = () => document.documentElement.dataset.input === 'touch';

function buildVirtualPad(gui) {
    const container = document.getElementById('virtual-pad');
    if (!container) {
        return;
    }
    for (const def of VIRTUAL_PAD) {
        const key = document.createElement('button');
        key.type = 'button';
        key.className = `vkey vkey-${def.area}`;
        key.textContent = def.label;
        key.dataset.code = def.code;
        if (def.title) {
            key.title = def.title;
            key.setAttribute('aria-label', def.title);
        }
        key.tabIndex = -1;
        let repeat = null;
        let down = false;
        const release = () => {
            if (!down) {
                return;
            }
            down = false;
            clearTimeout(repeat);
            clearInterval(repeat);
            key.classList.remove('down');
            gui.Key(1, def.code);
        };
        key.addEventListener('pointerdown', e => {
            // フォーカスをゲームの画面から移さない（移すと押しているキーが解除される）
            e.preventDefault();
            key.setPointerCapture?.(e.pointerId);
            resumeAudio();
            down = true;
            key.classList.add('down');
            gui.Key(0, def.code);
            if (def.repeat) {
                // 押し続けたら物理キーと同じく繰り返す
                repeat = setTimeout(() => {
                    repeat = setInterval(() => gui.Key(0, def.code), 80);
                }, 400);
            }
        });
        key.addEventListener('pointerup', release);
        key.addEventListener('pointercancel', release);
        key.addEventListener('lostpointercapture', release);
        key.addEventListener('contextmenu', e => e.preventDefault());
        container.appendChild(key);
    }
    let visible;
    try {
        const saved = localStorage.getItem(VIRTUAL_PAD_SETTING) ?? localStorage.getItem(OLD_VIRTUAL_PAD_SETTING);
        visible = saved === null ? isTouchMode() : saved === 'on';
    } catch {
        visible = isTouchMode();
    }
    container.hidden = !visible;
    globalThis.srcwebToggleVirtualPad = () => {
        container.hidden = !container.hidden;
        try {
            localStorage.setItem(VIRTUAL_PAD_SETTING, container.hidden ? 'off' : 'on');
        } catch {
            // 保存できなくても表示は切り替える
        }
    };
}

// 文字入力・ファイル選択のモーダル（SRCSharpWeb/WebScreenHost.cs の IScreenDialogHost）
// 見出し・ボタンの文言は .NET 側（Resources/WebStrings.resx）から渡す
function modal(title, build) {
    return new Promise(resolve => {
        const overlay = document.createElement('div');
        overlay.className = 'modal';
        const box = document.createElement('div');
        box.className = 'modal-box';
        const heading = document.createElement('div');
        heading.className = 'modal-title';
        heading.textContent = title;
        box.appendChild(heading);
        overlay.appendChild(box);
        const close = value => {
            overlay.remove();
            if (canvas) {
                canvas.focus();
            }
            resolve(value);
        };
        build(box, close);
        document.body.appendChild(overlay);
    });
}

function buttons(box, items) {
    const row = document.createElement('div');
    row.className = 'modal-buttons';
    for (const [label, action] of items) {
        const b = document.createElement('button');
        b.textContent = label;
        b.addEventListener('click', action);
        row.appendChild(b);
    }
    box.appendChild(row);
}

export function showInput(title, message, value, ok, cancel) {
    return modal(title, (box, close) => {
        const p = document.createElement('p');
        p.textContent = message;
        const input = document.createElement('input');
        input.type = 'text';
        input.value = value ?? '';
        input.id = 'modal-input';
        input.addEventListener('keydown', e => {
            if (e.key === 'Enter' && !e.isComposing) {
                close(input.value);
            } else if (e.key === 'Escape') {
                close(null);
            }
        });
        box.append(p, input);
        buttons(box, [[ok, () => close(input.value)], [cancel, () => close(null)]]);
        setTimeout(() => input.focus(), 0);
    });
}

export function showChoice(title, items, cancel) {
    return modal(title, (box, close) => {
        const list = document.createElement('ul');
        list.className = 'modal-list';
        for (const item of items) {
            const li = document.createElement('li');
            const b = document.createElement('button');
            b.textContent = item;
            b.addEventListener('click', () => close(item));
            li.appendChild(b);
            list.appendChild(li);
        }
        box.appendChild(list);
        buttons(box, [[cancel, () => close(null)]]);
    });
}

// 音声（SRCSharpWeb/WebSoundPlayer.cs）。ブラウザは操作があるまで音を鳴らさないため、最初のクリック・キー入力で開始する
let audio;
let master;
const voices = new Map();
// 鳴らした数（テスト用）。encoded: WAV・MP3、pcm: 合成した MIDI の断片
globalThis.srcwebSound = { encoded: 0, pcm: 0 };

function ensureAudio() {
    if (!audio) {
        audio = new AudioContext();
        master = audio.createGain();
        master.connect(audio.destination);
    }
    return audio;
}

export function resumeAudio() {
    if (audio && audio.state === 'suspended') {
        audio.resume();
    }
}

export function soundStop(channel) {
    const voice = voices.get(channel);
    if (voice) {
        for (const node of voice.nodes) {
            try {
                node.stop();
            } catch {
                // 開始前など
            }
        }
        voices.delete(channel);
    }
}

function newVoice(channel) {
    soundStop(channel);
    const voice = { nodes: new Set(), next: 0 };
    voices.set(channel, voice);
    return voice;
}

function startNode(voice, node, when) {
    voice.nodes.add(node);
    node.onended = () => voice.nodes.delete(node);
    node.connect(master);
    node.start(when);
}

export function soundPlayEncoded(channel, view, loop) {
    ensureAudio();
    const voice = newVoice(channel);
    // MemoryView は呼び出しの間だけ有効なので複写する
    const bytes = view.slice();
    audio.decodeAudioData(bytes.buffer).then(buffer => {
        if (voices.get(channel) !== voice) {
            return;
        }
        const node = audio.createBufferSource();
        node.buffer = buffer;
        node.loop = loop;
        startNode(voice, node, 0);
        globalThis.srcwebSound.encoded++;
    }).catch(e => console.warn('音声を再生できません', e));
}

export function soundQueuePcm(channel, view, channels, sampleRate) {
    ensureAudio();
    let voice = voices.get(channel);
    if (!voice) {
        voice = newVoice(channel);
    }
    const samples = new Float32Array(view.slice().buffer);
    const frames = Math.floor(samples.length / channels);
    if (frames === 0) {
        return;
    }
    const buffer = audio.createBuffer(channels, frames, sampleRate);
    for (let c = 0; c < channels; c++) {
        const data = buffer.getChannelData(c);
        for (let i = 0; i < frames; i++) {
            data[i] = samples[i * channels + c];
        }
    }
    const node = audio.createBufferSource();
    node.buffer = buffer;
    const when = Math.max(voice.next, audio.currentTime + 0.05);
    startNode(voice, node, when);
    voice.next = when + buffer.duration;
    globalThis.srcwebSound.pcm++;
}

// 送ってあるがまだ鳴らしていない長さ（秒）
export function soundAhead(channel) {
    const voice = voices.get(channel);
    return voice && audio ? Math.max(0, voice.next - audio.currentTime) : 0;
}

export function soundVolume(volume) {
    ensureAudio();
    master.gain.value = volume;
}
