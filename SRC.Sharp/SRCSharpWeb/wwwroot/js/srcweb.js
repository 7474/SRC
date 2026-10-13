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
    canvas.addEventListener('mousedown', e => {
        canvas.focus();
        const [x, y] = position(e);
        gui.Pointer(0, x, y, e.button, e.detail || 1);
        e.preventDefault();
    });
    canvas.addEventListener('mouseup', e => {
        const [x, y] = position(e);
        gui.Pointer(1, x, y, e.button, 0);
        e.preventDefault();
    });
    canvas.addEventListener('mousemove', e => {
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
