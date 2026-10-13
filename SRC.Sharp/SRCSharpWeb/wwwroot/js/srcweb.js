// canvas への画面の書き込みと、マウス・キー入力の受け渡し（SRCSharpWeb/WebInterop.cs）
let canvas;
let ctx;
let imageData;
let imageBytes;

export function setTitle(title) {
    document.title = title;
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
    start();
}

async function start() {
    const runtime = await globalThis.getDotnetRuntime(0);
    const exports = await runtime.getAssemblyExports('SRCSharpWeb.dll');
    const gui = exports.SRCSharpWeb.WebInterop;

    const position = e => {
        const rect = canvas.getBoundingClientRect();
        return [
            (e.clientX - rect.left) * canvas.width / rect.width,
            (e.clientY - rect.top) * canvas.height / rect.height,
        ];
    };

    // タッチ操作: タップで左クリック、長押しで右クリック、指を動かすとカーソルの移動
    let lastTouch = -Infinity;
    let touch = null;
    let lastTap = null;
    const LONG_PRESS_MS = 500;
    const MOVE_TOLERANCE = 12; // CSS ピクセル
    const click = (x, y, button, clicks) => {
        gui.Pointer(2, x, y, 0, 0);
        gui.Pointer(0, x, y, button, clicks);
        gui.Pointer(1, x, y, button, 0);
        globalThis.srcwebTouchClicks = [...(globalThis.srcwebTouchClicks || []), { button, clicks }];
    };
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
            x,
            y,
            moved: false,
            held: false,
            timer: setTimeout(() => {
                if (touch && !touch.moved) {
                    touch.held = true;
                    hideRing();
                    navigator.vibrate?.(20);
                    click(touch.x, touch.y, 2, 1);
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
        if (!cancelled && !touch.moved && !touch.held) {
            // 近くを続けてタップしたらダブルクリック
            const now = performance.now();
            const double = lastTap && now - lastTap.time < 400 && Math.hypot(e.clientX - lastTap.x, e.clientY - lastTap.y) < 24;
            click(touch.x, touch.y, 0, double ? 2 : 1);
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

// 文字入力・ファイル選択のモーダル（SRCSharpWeb/WebScreenHost.cs の IScreenDialogHost）
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

export function showInput(title, message, value) {
    return modal(title || '入力', (box, close) => {
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
        buttons(box, [['OK', () => close(input.value)], ['キャンセル', () => close(null)]]);
        setTimeout(() => input.focus(), 0);
    });
}

export function showChoice(title, items) {
    return modal(title || '選択', (box, close) => {
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
        buttons(box, [['キャンセル', () => close(null)]]);
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
