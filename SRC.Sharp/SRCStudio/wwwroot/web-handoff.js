// かんたん作成のシナリオを、同じサイトの SRC#Web に渡す（7474/SRC#1659）。
// SRC#Web の「追加したアーカイブ」と同じところ（ブラウザの IndexedDB の srcweb・archives。SRCSharpWeb/wwwroot/js/store.js）に zip を入れ、
// SRC#Web を ?stored=名前 で開く。SRC#Web は requires の素材（最小の汎用データなど）を重ねて、すぐに始める。
// IndexedDB はサイト（オリジン）ごとなので、SRC#Studio と SRC#Web が同じサイトにあるときだけ使える（GitHub Pages の /SRC/studio/ と /SRC/web/、SRC-Site の /studio/ と /play/）

let configPromise;

/**
 * サイトごとの設定（SRC#Studio の隣の studio.json）。
 * web: SRC#Web の場所（SRC#Studio からの相対 URL か絶対 URL）。requires: 遊ぶときに重ねる SRC#Web のカタログの素材の id。
 * data: 作品のデータ（7474/SRC-Data の index.json と各ファイル）の場所。scenes: 場面のカタログ（マップのサンプル）の URL の一覧
 */
export function studioConfig() {
  configPromise ??= (async () => {
    const defaults = {
      web: '../web/',
      requires: ['basic-data'],
      data: 'https://raw.githubusercontent.com/7474/SRC-Data/master/',
      scenes: ['https://raw.githubusercontent.com/7474/SRC-Data/master/scenes/index.json'],
    };
    try {
      const response = await fetch(new URL('./studio.json', import.meta.url), { cache: 'no-cache' });
      if (response.ok) {
        return { ...defaults, ...(await response.json()) };
      }
    } catch {
      // 設定が無ければ既定のまま
    }
    return defaults;
  })();
  return configPromise;
}

function openStore() {
  // SRC#Web の store.js と同じ版・同じ作り方。SRC#Web の方が新しい版にしていれば、その版のまま開く
  const create = version => new Promise((resolve, reject) => {
    const request = version ? indexedDB.open('srcweb', version) : indexedDB.open('srcweb');
    request.onupgradeneeded = e => {
      const db = request.result;
      if (e.oldVersion < 1) {
        db.createObjectStore('archives', { keyPath: 'name' });
      }
      if (e.oldVersion < 2) {
        db.createObjectStore('saves', { keyPath: ['scenario', 'name'] });
      }
    };
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error);
  });
  return create(2).catch(e => (e?.name === 'VersionError' ? create(0) : Promise.reject(e)));
}

/** zip を SRC#Web の「追加したアーカイブ」に入れる（同じ名前は上書き。セーブデータは名前で引き継ぐ） */
export async function storeForWeb(name, bytes, requires) {
  const db = await openStore();
  try {
    await new Promise((resolve, reject) => {
      const tx = db.transaction('archives', 'readwrite');
      tx.objectStore('archives').put({ name, kind: 'scenario', size: bytes.length, added: Date.now(), requires, data: new Blob([bytes]) });
      tx.oncomplete = () => resolve();
      tx.onerror = () => reject(tx.error);
      tx.onabort = () => reject(tx.error);
    });
  } finally {
    db.close();
  }
}

/** SRC#Web で、入れたシナリオをすぐに始める URL */
export async function webPlayUrl(name) {
  const config = await studioConfig();
  const url = new URL(config.web, new URL('./', import.meta.url));
  url.searchParams.set('stored', name);
  return url.toString();
}

/** SRC#Web が同じサイトにあるか（違うサイトなら IndexedDB で渡せない） */
export async function webIsSameOrigin() {
  const config = await studioConfig();
  return new URL(config.web, new URL('./', import.meta.url)).origin === location.origin;
}
