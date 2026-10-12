// 追加したアーカイブ（zip）をブラウザの IndexedDB に保存する
(() => {
    const open = () => new Promise((resolve, reject) => {
        const request = indexedDB.open('srcweb', 1);
        request.onupgradeneeded = () => {
            request.result.createObjectStore('archives', { keyPath: 'name' });
        };
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });

    const run = async (mode, action) => {
        const db = await open();
        try {
            return await new Promise((resolve, reject) => {
                const tx = db.transaction('archives', mode);
                const request = action(tx.objectStore('archives'));
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
                .map(x => ({ name: x.name, size: x.size, added: x.added }))
                .sort((a, b) => a.name.localeCompare(b.name));
        },
        async put(name, bytes) {
            if (navigator.storage && navigator.storage.persist) {
                // ブラウザの判断で消されないよう求める（許可されない場合もある）
                navigator.storage.persist();
            }
            await run('readwrite', store => store.put({ name, size: bytes.length, added: Date.now(), data: new Blob([bytes]) }));
        },
        async get(name) {
            const item = await run('readonly', store => store.get(name));
            return item ? new Uint8Array(await item.data.arrayBuffer()) : null;
        },
        async remove(name) {
            await run('readwrite', store => store.delete(name));
        },
    };
})();
