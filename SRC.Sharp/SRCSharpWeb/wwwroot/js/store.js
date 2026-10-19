// 追加したアーカイブ（zip）とセーブデータをブラウザの IndexedDB に保存する。カタログのアーカイブは Cache Storage に置く
(() => {
    const open = () => new Promise((resolve, reject) => {
        const request = indexedDB.open('srcweb', 2);
        request.onupgradeneeded = e => {
            const db = request.result;
            if (e.oldVersion < 1) {
                db.createObjectStore('archives', { keyPath: 'name' });
            }
            if (e.oldVersion < 2) {
                // シナリオごとのセーブデータ。name はシナリオのフォルダからの相対パス
                db.createObjectStore('saves', { keyPath: ['scenario', 'name'] });
            }
        };
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });

    const run = async (mode, action, storeName = 'archives') => {
        const db = await open();
        try {
            return await new Promise((resolve, reject) => {
                const tx = db.transaction(storeName, mode);
                const request = action(tx.objectStore(storeName));
                tx.oncomplete = () => resolve(request.result);
                tx.onerror = () => reject(tx.error);
                tx.onabort = () => reject(tx.error);
            });
        } finally {
            db.close();
        }
    };

    window.srcwebStore = {
        async list() {
            const items = await run('readonly', store => store.getAll());
            return items
                .map(x => ({ name: x.name, size: x.size, added: x.added, kind: x.kind || 'scenario' }))
                .sort((a, b) => a.name.localeCompare(b.name));
        },
        // kind: scenario（シナリオ）・asset（汎用グラフィック集などの素材。どのシナリオにも重ねる）
        async put(name, bytes, kind) {
            if (navigator.storage && navigator.storage.persist) {
                // ブラウザの判断で消されないよう求める（許可されない場合もある）
                navigator.storage.persist();
            }
            await run('readwrite', store => store.put({ name, kind: kind || 'scenario', size: bytes.length, added: Date.now(), data: new Blob([bytes]) }));
        },
        async get(name) {
            const item = await run('readonly', store => store.get(name));
            return item ? new Uint8Array(await item.data.arrayBuffer()) : null;
        },
        async remove(name) {
            await run('readwrite', store => store.delete(name));
        },
        async listSaves(scenario) {
            const range = IDBKeyRange.bound([scenario, ''], [scenario, '\uffff']);
            const items = await run('readonly', store => store.getAll(range), 'saves');
            return items
                .map(x => ({ name: x.name, size: x.size, updated: x.updated }))
                .sort((a, b) => b.updated - a.updated);
        },
        async putSave(scenario, name, bytes) {
            await run('readwrite', store => store.put({ scenario, name, size: bytes.length, updated: Date.now(), data: new Blob([bytes]) }), 'saves');
        },
        async getSave(scenario, name) {
            const item = await run('readonly', store => store.get([scenario, name]), 'saves');
            return item ? new Uint8Array(await item.data.arrayBuffer()) : null;
        },
        async removeSave(scenario, name) {
            await run('readwrite', store => store.delete([scenario, name]), 'saves');
        },
        // 利用者が追加したカタログの URL（localStorage）
        listCatalogs() {
            try {
                return JSON.parse(localStorage.getItem('srcweb.catalogs') || '[]');
            } catch {
                return [];
            }
        },
        saveCatalogs(urls) {
            localStorage.setItem('srcweb.catalogs', JSON.stringify(urls));
        },
        // 画面のテーマ（95・xp）。index.html が描画の前に同じ値を html の data-theme に付ける
        getTheme() {
            return document.documentElement.dataset.theme === 'xp' ? 'xp' : '95';
        },
        setTheme(theme) {
            theme = theme === 'xp' ? 'xp' : '95';
            document.documentElement.dataset.theme = theme;
            try {
                localStorage.setItem('srcweb.theme', theme);
            } catch {
                // localStorage を使えない（この表示の間だけ変える）
            }
        },
        // 操作の種類の設定（auto・touch・mouse）。index.html の srcwebInput が html の data-input に反映する
        getInputMode() {
            return globalThis.srcwebInput?.setting ?? 'auto';
        },
        setInputMode(mode) {
            globalThis.srcwebInput?.set(mode);
        },
        setTitle(title) {
            document.title = title;
        },
        // ゲームの画面にキー入力を戻す
        focus(id) {
            document.getElementById(id)?.focus();
        },
        // ゲームの画面（とその下のメニュー）を全画面表示にする・戻す
        async toggleFullscreen(id) {
            if (document.fullscreenElement) {
                await document.exitFullscreen();
                return;
            }
            const target = document.getElementById(id)?.closest('.play') ?? document.documentElement;
            await target.requestFullscreen?.();
        },
        // ファイルとして書き出す（ダウンロード）
        download(name, bytes) {
            const url = URL.createObjectURL(new Blob([bytes], { type: 'application/octet-stream' }));
            const a = document.createElement('a');
            a.href = url;
            a.download = name;
            document.body.appendChild(a);
            a.click();
            a.remove();
            setTimeout(() => URL.revokeObjectURL(url), 10000);
        },
        // 読み込み中のダイアログの「キャンセル」で、取得中の zip のダウンロードを止める（fetchArchive は AbortError で失敗する）
        abortFetches() {
            for (const controller of fetches) {
                controller.abort();
            }
            fetches.clear();
        },
        async fetchArchive(url, version) {
            const controller = new AbortController();
            fetches.add(controller);
            try {
                return await fetchArchive(url, version, controller.signal);
            } finally {
                fetches.delete(controller);
            }
        },
    };

    // 取得中のダウンロード（abortFetches で止める）
    const fetches = new Set();

    // カタログのアーカイブ（zip）を取得する。一度取得したものは Cache Storage に置き、次からはネットワークを使わない。
    // version（カタログの version）が置いたときと違えば取得し直す。Blob を返す（大きいものはブラウザがディスクに置く）
    const fetchArchive = async (url, version, signal) => {
        let cache = null;
        try {
            cache = await caches.open(archiveCacheName);
        } catch {
            // 安全でない接続（http）などで Cache Storage が使えない
        }
        if (!cache) {
            return await fetchBlob(url, signal);
        }
        version = version || '';
        const hit = await cache.match(url);
        if (hit && hit.headers.get(versionHeader) === version) {
            return await hit.blob();
        }
        const response = await fetchOk(url, signal);
        if (navigator.storage && navigator.storage.persist) {
            navigator.storage.persist();
        }
        try {
            await cache.put(url, new Response(response.body, { headers: { 'Content-Type': 'application/zip', [versionHeader]: version } }));
            const stored = await cache.match(url);
            if (stored) {
                return await stored.blob();
            }
        } catch (e) {
            // 途中まで置いたものは消す。取り消したときはそのまま終える
            await cache.delete(url).catch(() => { });
            if (signal.aborted) {
                throw e;
            }
            // 容量が足りないなど。置けなければキャッシュせずに取得し直す
            console.warn('アーカイブをキャッシュできません', url, e);
        }
        return await fetchBlob(url, signal);
    };

    const fetchOk = async (url, signal) => {
        const response = await fetch(url, { signal });
        if (!response.ok) {
            throw new Error(`${response.status} ${response.statusText} (${url})`);
        }
        return response;
    };
    const fetchBlob = async (url, signal) => await (await fetchOk(url, signal)).blob();

    const archiveCacheName = 'srcweb-archives';
    const versionHeader = 'X-SRCWeb-Version';
})();

// シナリオに同梱された文書のリンク（ScenarioDocuments）。
// 別の文書へのリンク（data-srcweb-doc）は .NET の OpenLinkedDocument で開き、ページの中の移動（#）はその文書の中でスクロールする。
// .htm・.html は iframe（allow-scripts なし・allow-same-origin あり）に入れるので、中のスクリプトは動かず、ここから中のクリックを受け取れる
window.srcwebDocs = {
    bind(id, dotnet) {
        const view = document.getElementById(id);
        if (!view) {
            return;
        }
        const handle = (e, root) => {
            const a = e.target.closest && e.target.closest('a[href]');
            if (!a) {
                return;
            }
            const doc = a.getAttribute('data-srcweb-doc');
            const href = a.getAttribute('href') || '';
            if (doc) {
                e.preventDefault();
                e.stopPropagation();
                dotnet.invokeMethodAsync('OpenLinkedDocument', doc);
            } else if (href.startsWith('#')) {
                e.preventDefault();
                e.stopPropagation();
                const name = decodeURIComponent(href.slice(1));
                const target = name ? root.getElementById(name) || root.getElementsByName(name)[0] : null;
                if (target) {
                    target.scrollIntoView();
                } else if (!name) {
                    (root.scrollingElement || view).scrollTop = 0;
                }
            }
        };
        if (view.tagName === 'IFRAME') {
            const attach = () => {
                const inner = view.contentDocument;
                if (inner && !inner.srcwebBound) {
                    inner.srcwebBound = true;
                    inner.addEventListener('click', e => handle(e, inner), true);
                }
            };
            view.addEventListener('load', attach);
            if (view.contentDocument && view.contentDocument.readyState === 'complete') {
                attach();
            }
        } else if (!view.srcwebBound) {
            view.srcwebBound = true;
            view.addEventListener('click', e => handle(e, document), true);
        }
    },
    // 編集 - すべて選択。iframe（.htm・.html）なら中の文書を選ぶ
    selectAll(id) {
        const view = document.getElementById(id);
        if (!view) {
            return;
        }
        const doc = view.tagName === 'IFRAME' ? view.contentDocument : document;
        const target = view.tagName === 'IFRAME' ? doc && doc.body : view;
        if (!doc || !target) {
            return;
        }
        const range = doc.createRange();
        range.selectNodeContents(target);
        const selection = (view.tagName === 'IFRAME' ? view.contentWindow : window).getSelection();
        selection.removeAllRanges();
        selection.addRange(range);
    },
    // アドレス欄に外の URL を入れたときは別のタブで開く
    openExternal(url) {
        window.open(url, '_blank', 'noopener');
    },
};
