// 追加したアーカイブ（zip）とセーブデータをブラウザの IndexedDB に保存する
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
        setTitle(title) {
            document.title = title;
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
    };
})();
