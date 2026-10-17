// @ts-check
const { test, expect } = require('@playwright/test');
const fs = require('node:fs');
const path = require('node:path');
const { makeZip } = require('./zip');

const root = path.resolve(__dirname, '..', process.env.SRCWEB_ROOT ?? '../SRCSharpWeb/bin/web/wwwroot');
// SRC#Web を置いたフォルダ（サブパスに置く場合はサイトのルートの下）
const webRoot = path.join(root, process.env.SRCWEB_BASE ?? '/');

/** 初回は coi-serviceworker を有効にするために再読み込みされる。シナリオの一覧が出るまで待つ */
async function open(page) {
  page.on('pageerror', e => console.log('pageerror:', e.message));
  page.on('console', m => {
    if (m.type() === 'error') {
      console.log('console.error:', m.text());
    }
  });
  // baseURL（サブパスを含む）からの相対で開く
  await page.goto('./');
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  expect(await page.evaluate(() => self.crossOriginIsolated)).toBe(true);
}

/** canvas へ書き込んだ回数 */
function frames(page) {
  return page.evaluate(() => globalThis.srcwebFrames ?? 0);
}

/**
 * 画面とイベントの状態（WebInterop.DebugState の JSON 文字列）。
 * 読み込み直した直後は .NET の公開する関数がまだ揃っていないことがあるため、そのときは空の状態を返す（expect.poll で待つ）
 */
function debugState(page) {
  return page.evaluate(async () => {
    const runtime = await globalThis.getDotnetRuntime?.(0);
    const exports = await runtime?.getAssemblyExports('SRCSharpWeb.dll');
    const state = exports?.SRCSharpWeb?.WebInterop?.DebugState;
    return typeof state === 'function' ? state() : '{}';
  });
}

async function screenState(page) {
  return JSON.parse(await debugState(page));
}

/** リストが出るまでメッセージを送り、項目を矢印キーで選んで決定する */
async function chooseListItem(page, caption, item) {
  await page.locator('#screen').focus();
  await expect.poll(async () => {
    const s = await screenState(page);
    if (s.list && (s.list.caption.includes(caption) || (s.list.info ?? '').includes(caption))) {
      return true;
    }
    if (s.message !== undefined) {
      await page.keyboard.press('Enter');
    }
    return false;
  }, { intervals: [500, 1000] }).toBe(true);
  await expect.poll(async () => {
    const s = await screenState(page);
    const target = s.list.items.findIndex(x => x.includes(item));
    if (target < 0) {
      throw new Error(`リストに「${item}」がありません: ${JSON.stringify(s.list)}`);
    }
    if (s.list.highlight === target) {
      return true;
    }
    await page.keyboard.press(s.list.highlight < target ? 'ArrowDown' : 'ArrowUp');
    return false;
  }, { intervals: [200, 500] }).toBe(true);
  await page.keyboard.press('Enter');
}

async function attachScreen(page, testInfo, name) {
  await testInfo.attach(name, { body: await page.locator('#screen').screenshot(), contentType: 'image/png' });
}

/** Enter でメッセージを送り、イベントが先へ進むことを確かめる */
async function expectAdvancesWithEnter(page) {
  await expect.poll(() => frames(page)).toBeGreaterThan(0);
  const before = await debugState(page);
  await page.locator('#screen').focus();
  await expect.poll(async () => {
    await page.keyboard.press('Enter');
    return debugState(page);
  }, { intervals: [500, 1000, 2000] }).not.toBe(before);
}

/** プレイ中に画面の右下のシステムメニューからシナリオ選択へ戻る（確認に「はい」と答える） */
/** 選択画面の拡大表示（既定では出さない）をシステムメニューで出す */
async function enableTouchUi(page) {
  await page.locator('#system-menu-button').click();
  await page.getByRole('menuitem', { name: '選択画面の拡大表示の切り替え' }).click();
}

async function backToMenu(page) {
  await page.locator('#system-menu-button').click();
  await page.getByRole('menuitem', { name: 'シナリオ選択へ戻る…' }).click();
  await page.getByRole('alertdialog').getByRole('button', { name: 'はい' }).click();
}

test('登録済シナリオを選んでプレイする', async ({ page }, testInfo) => {
  await open(page);
  await testInfo.attach('menu', { body: await page.screenshot({ fullPage: true }), contentType: 'image/png' });
  await page.getByRole('button', { name: 'battle-basic' }).click();
  await expect(page.locator('#screen')).toBeVisible();
  // 実寸（720 ピクセル）で表示しているときは補間しない。縮小するときは補間する
  await expect(page.locator('#screen')).toHaveClass(/pixelated/);
  await page.setViewportSize({ width: 600, height: 800 });
  await expect(page.locator('#screen')).not.toHaveClass(/pixelated/);
  await page.setViewportSize({ width: 800, height: 800 });
  await expect(page.locator('#screen')).toHaveClass(/pixelated/);
  await expect.poll(() => debugState(page)).toContain('test.eve');
  await expectAdvancesWithEnter(page);
  await expect(page).toHaveTitle(/SRC#Web/);
  await attachScreen(page, testInfo, 'battle-basic');
});

test('テーマを XP に切り替え、開き直しても保つ', async ({ page }) => {
  await open(page);
  await expect(page.locator('html')).toHaveAttribute('data-theme', '95');
  await page.selectOption('#theme', 'xp');
  await expect(page.locator('html')).toHaveAttribute('data-theme', 'xp');
  await open(page);
  await expect(page.locator('html')).toHaveAttribute('data-theme', 'xp');
  await expect(page.locator('#theme')).toHaveValue('xp');
});

test('表示言語を英語にし、選択欄で日本語に戻す', async ({ page }) => {
  // 未設定のときだけ英語にする（選択欄で変えた値は再読み込み後も残す）
  await page.addInitScript(() => {
    if (localStorage.getItem('srcweb.language') === null) {
      localStorage.setItem('srcweb.language', 'en');
    }
  });
  await open(page);
  await expect(page.getByRole('heading', { name: 'Bundled scenarios' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Added archives' })).toBeVisible();
  await expect(page.locator('#language')).toHaveValue('en');
  // 対応言語（SRCCore/UiLanguages.cs）と「自動」
  await expect(page.locator('#language option')).toHaveCount(16);
  expect(await page.evaluate(() => document.documentElement.lang)).toBe('en');

  await page.locator('#language').selectOption('ja');
  // ページが読み込み直される
  await expect(page.getByRole('heading', { name: '登録済シナリオ' })).toBeVisible({ timeout: 180 * 1000 });
  await expect(page.locator('#language')).toHaveValue('ja');
  expect(await page.evaluate(() => localStorage.getItem('srcweb.language'))).toBe('ja');
});

test.describe('韓国語のブラウザ', () => {
  test.use({ locale: 'ko-KR' });

  test('ブラウザの言語に従う（auto）と、地域付きの言語も対応する訳で表示する', async ({ page }) => {
    // ko-KR のブラウザで auto にすると韓国語（訳は埋め込んだリソースから引く）
    await page.addInitScript(() => localStorage.setItem('srcweb.language', 'auto'));
    await open(page);
    await expect(page.getByRole('heading', { name: '등록된 시나리오' })).toBeVisible();
    await expect(page.locator('#language')).toHaveValue('auto');
    expect(await page.evaluate(() => document.documentElement.lang)).toBe('ko');
  });
});

test('アーカイブを追加し、再読み込み後もプレイできる', async ({ page }, testInfo) => {
  const name = 'テスト追加.zip';
  await open(page);
  await page.setInputFiles('#add-archive', {
    name,
    mimeType: 'application/zip',
    buffer: fs.readFileSync(path.join(webRoot, 'scenarios', 'battle-victory.zip')),
  });
  await expect(page.locator('#archives li')).toContainText(name);

  // ブラウザ（IndexedDB）に残る
  await page.reload();
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await expect(page.locator('#archives li')).toContainText(name);

  await page.locator('#archives li', { hasText: name }).getByRole('button', { name }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await expect.poll(() => debugState(page)).toContain('archive-' + name);
  await expectAdvancesWithEnter(page);
  await attachScreen(page, testInfo, 'archive');

  // 削除する
  await backToMenu(page);
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name: '削除' }).click();
  await expect(page.locator('#archives li')).toHaveCount(0);
});

test('インターミッションでセーブし、ブラウザに残ったセーブデータから再開する', async ({ page }, testInfo) => {
  const scenario = 'catalog-intermission-save';
  await open(page);
  await page.evaluate(id => window.srcwebStore.listSaves(id)
    .then(saves => Promise.all(saves.map(x => window.srcwebStore.removeSave(id, x.name)))), scenario);
  await page.getByRole('button', { name: 'intermission-save' }).click();
  await expect(page.locator('#screen')).toBeVisible();

  // データセーブ → ファイル名の入力 → OK
  await chooseListItem(page, 'コマンド', 'データセーブ');
  await expect(page.locator('#modal-input')).toBeVisible();
  await attachScreen(page, testInfo, 'save-input');
  await page.locator('#modal-input').press('Enter');

  // ゲームが書いたセーブデータがブラウザに保存される（ステージ開始時のリスタート用の .srcq も保存される）
  const saveNames = () => page.evaluate(id => window.srcwebStore.listSaves(id).then(x => x.map(s => s.name)), scenario);
  await expect.poll(async () => (await saveNames()).filter(x => x.endsWith('.srcs'))).toHaveLength(1);
  const saveName = (await saveNames()).find(x => x.endsWith('.srcs'));

  // 読み込み直して、セーブデータから再開する（インターミッションに戻る）
  await backToMenu(page);
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.getByRole('button', { name: 'intermission-save' }).click();
  await page.locator('#saves li', { hasText: saveName }).getByRole('button', { name: saveName }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await expect.poll(async () => (await screenState(page)).list?.info ?? '').toContain('コマンド');
  await attachScreen(page, testInfo, 'resumed');

  // 次のステージへ進んで終わる
  await chooseListItem(page, 'コマンド', '次のステージへ');
  await expect.poll(async () => (await screenState(page)).dialog ?? '').toContain('次のステージへ進みますか');
  await page.keyboard.press('Enter');
  await expect.poll(async () => {
    const s = await screenState(page);
    if (s.message !== undefined) {
      await page.keyboard.press('Enter');
    }
    return s.terminated;
  }, { intervals: [500, 1000] }).toBe(true);
});

test('インターミッションのコマンドを開き、仮想パッドと同じキー操作でインターミッションへ戻る', async ({ page }, testInfo) => {
  await open(page);
  await page.getByRole('button', { name: 'intermission-commands' }).click();
  await expect(page.locator('#screen')).toBeVisible();

  /** マップのコマンドのメニューを Esc（右クリック）で開き、項目をキーで選ぶ */
  const chooseMapCommand = async item => {
    await expect.poll(async () => {
      const s = await screenState(page);
      if (!s.menu && s.locked === false) {
        await page.keyboard.press('Escape');
      }
      return !!s.menu;
    }, { intervals: [500, 1000] }).toBe(true);
    await expect.poll(async () => {
      const s = await screenState(page);
      const target = s.menu.items.findIndex(x => x.includes(item));
      if (target < 0) {
        throw new Error(`メニューに「${item}」がありません: ${JSON.stringify(s.menu)}`);
      }
      if (s.menu.highlight === target) {
        return true;
      }
      await page.keyboard.press('ArrowDown');
      return false;
    }, { intervals: [200, 500] }).toBe(true);
    await page.keyboard.press('Enter');
  };

  for (const command of ['ユニットステータス', 'パイロットステータス']) {
    await chooseListItem(page, 'コマンド', command);
    // メインウィンドウを出し、一覧のマップを操作できる
    await expect.poll(async () => {
      const s = await screenState(page);
      return s.mainFormVisible === true && s.locked === false && !s.list;
    }, { intervals: [500, 1000] }).toBe(true);
    await attachScreen(page, testInfo, command);
    await chooseMapCommand('インターミッションに戻る');
    await expect.poll(async () => (await screenState(page)).list?.info ?? '').toContain('コマンド');
  }

  // ユーザー定義のコマンドも戻ってこられる
  await chooseListItem(page, 'コマンド', '部隊名簿');
  await expect.poll(async () => {
    const st = await screenState(page);
    if (st.message !== undefined) {
      await page.keyboard.press('Enter');
    }
    return st.list?.info ?? '';
  }, { intervals: [500, 1000] }).toContain('コマンド');
  const s = await screenState(page);
  expect(s.error).toBeUndefined();
});

const corpus = path.resolve(__dirname, '../../E2E');
const soundFont = process.env.SRC_SOUNDFONT ?? '/usr/share/sounds/sf2/TimGM6mb.sf2';

function soundCounts(page) {
  return page.evaluate(() => globalThis.srcwebSound);
}

test('効果音（WAV）を鳴らす。SoundFont が無ければ MIDI は鳴らさずに進む', async ({ page }) => {
  await open(page);
  await page.getByRole('button', { name: 'sound', exact: true }).click();
  await expect(page.locator('#screen')).toBeVisible();
  // ブラウザは操作があるまで音を鳴らさない
  await page.locator('#screen').click({ position: { x: 600, y: 300 } });
  await expect.poll(async () => (await soundCounts(page)).encoded).toBeGreaterThan(0);
  await expectAdvancesWithEnter(page);
  expect((await soundCounts(page)).pcm).toBe(0);
});

test('アーカイブの SoundFont で MIDI を合成して鳴らす', async ({ page }) => {
  test.skip(!fs.existsSync(soundFont), `SoundFont が無い: ${soundFont}`);
  const files = ['test.eve', 'test.map', 'Sound/se.wav', 'Midi/bgm.mid']
    .map(name => ({ name: 'サウンド/' + name, data: fs.readFileSync(path.join(corpus, 'sound', name)) }));
  files.push({ name: 'サウンド/Midi/test.sf2', data: fs.readFileSync(soundFont) });
  const name = 'MIDI のテスト.zip';
  await open(page);
  await page.setInputFiles('#add-archive', { name, mimeType: 'application/zip', buffer: makeZip(files) });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await page.locator('#screen').click({ position: { x: 600, y: 300 } });
  await expect.poll(async () => (await soundCounts(page)).pcm, { timeout: 120 * 1000 }).toBeGreaterThan(0);
  await backToMenu(page);
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name: '削除' }).click();
});

// ── 外部ホスティング（docs/gui/web-hosting.md） ──

/**
 * page.route は Service Worker を通る要求を捕まえられないため、Service Worker（coi-serviceworker）を止め、
 * クロスオリジン分離のヘッダは応答に付ける。個別の route はこの後に登録する（後のものが優先される）
 */
async function isolateWithHeaders(page) {
  await page.route('**/*', async route => {
    // 前のテストの直後などに、テスト用のサーバーへの接続が切られる（socket hang up）ことがあるため、取り直す
    let response;
    for (let attempt = 1; ; attempt++) {
      try {
        response = await route.fetch();
        break;
      } catch (e) {
        if (attempt >= 3) {
          throw e;
        }
        await new Promise(resolve => setTimeout(resolve, 200 * attempt));
      }
    }
    await route.fulfill({
      response,
      headers: {
        ...response.headers(),
        'cross-origin-opener-policy': 'same-origin',
        'cross-origin-embedder-policy': 'require-corp',
      },
    });
  });
}

test('遊べるシナリオ（ランタン号）を、組み込みの最小の汎用データを重ねて始める', async ({ page }, testInfo) => {
  await open(page);
  await expect(page.locator('#catalog button').first()).toHaveText('灯の巡航船ランタン号');
  // 素材は一覧に出さない
  await expect(page.locator('#catalog button', { hasText: 'SRC# の最小の汎用データ' })).toHaveCount(0);
  await page.getByRole('button', { name: '灯の巡航船ランタン号' }).click();
  await expect(page.locator('#screen')).toBeVisible({ timeout: 60 * 1000 });
  await expect.poll(async () => (await screenState(page)).line ?? '', { timeout: 60 * 1000 }).toContain('01_灯の港.eve');
  // タイトル画面まで進んでも、データの読み込みのエラーが出ない
  await page.waitForTimeout(3000);
  expect((await screenState(page)).dialog).toBeUndefined();
  await attachScreen(page, testInfo, 'lantern');
  // 素材として重ねた SoundFont（TimGM6mb）で、タイトルの BGM（MIDI）を合成して鳴らす
  await page.locator('#screen').click({ position: { x: 600, y: 450 } });
  await expect.poll(async () => (await soundCounts(page)).pcm, { timeout: 120 * 1000 }).toBeGreaterThan(0);
});

test('URL の play で登録済シナリオを直接始める', async ({ page }) => {
  page.on('pageerror', e => console.log('pageerror:', e.message));
  await page.goto('./?play=battle-basic');
  await expect(page.locator('#screen')).toBeVisible({ timeout: 180 * 1000 });
  await expect.poll(() => debugState(page)).toContain('catalog-battle-basic');
  await expectAdvancesWithEnter(page);
});

test('シナリオを読み込んでいる間はほかの操作を受け付けない', async ({ page }) => {
  await open(page);
  // シナリオの zip の取得を遅らせ、読み込み中の状態を確かめる
  let release;
  const gate = new Promise(resolve => (release = resolve));
  await page.route('**/*.zip', async route => {
    await gate;
    await route.continue();
  });
  await page.getByRole('button', { name: 'battle-basic' }).click();
  const body = page.locator('.explorer-body');
  await expect(body).toHaveAttribute('inert', '');
  await expect(body).toHaveAttribute('aria-busy', 'true');
  // inert の間は、ほかのシナリオを押しても始まらない
  await page.getByRole('button', { name: 'sound', exact: true }).click({ force: true, timeout: 2000 }).catch(() => {});
  release();
  await expect(page.locator('#screen')).toBeVisible({ timeout: 180 * 1000 });
  await expect.poll(() => debugState(page)).toContain('catalog-battle-basic');
});

test('URL の log でログのレベルを変える', async ({ page }) => {
  const messages = [];
  page.on('console', m => messages.push({ type: m.type(), text: m.text() }));
  await page.goto('./?play=battle-basic&log=Debug');
  await expect(page.locator('#screen')).toBeVisible({ timeout: 180 * 1000 });
  await expect.poll(() => debugState(page)).toContain('catalog-battle-basic');
  // 既定（Information）では出ない、イベントの実行（SRCCore.Event の Debug）が出る
  await expect.poll(() => messages.some(m => m.text.includes('SRCCore.Event')), { timeout: 60 * 1000 }).toBe(true);
});

test.describe('サイトの設定とカタログ', () => {
test.use({ serviceWorkers: 'block' });

test('サイトの設定とカタログで配信し、シナリオが求める素材を重ねる', async ({ page }) => {
  // シナリオには効果音を入れず、素材（汎用データ）の zip に入れる
  const scenario = makeZip(['test.eve', 'test.map', 'Midi/bgm.mid']
    .map(name => ({ name: 'シナリオ/' + name, data: fs.readFileSync(path.join(corpus, 'sound', name)) })));
  const asset = makeZip([{ name: '汎用データ/Sound/se.wav', data: fs.readFileSync(path.join(corpus, 'sound', 'Sound/se.wav')) }]);
  const json = body => ({ status: 200, contentType: 'application/json', body: JSON.stringify(body) });
  await isolateWithHeaders(page);
  await page.route('**/srcweb.json', route => route.fulfill(json({
    title: 'テストの配信',
    catalogs: ['dist/catalog.json'],
    allowUserArchives: false,
    allowUserCatalogs: false,
  })));
  await page.route('**/dist/catalog.json', route => route.fulfill(json({
    format: 1,
    name: 'テストの配信元',
    entries: [
      { id: 'sound-with-asset', title: '素材を使うシナリオ', archive: 'scenario.zip', root: 'シナリオ', eve: 'test.eve', requires: ['common'] },
      { id: 'common', kind: 'asset', title: '汎用データ', archive: 'assets/common.zip', root: '汎用データ' },
    ],
  })));
  await page.route('**/dist/scenario.zip', route => route.fulfill({ status: 200, contentType: 'application/zip', body: scenario }));
  await page.route('**/dist/assets/common.zip', route => route.fulfill({ status: 200, contentType: 'application/zip', body: asset }));

  await open(page);
  await expect(page).toHaveTitle('テストの配信');
  await expect(page.locator('h2').first()).toHaveText('テストの配信元');
  // 素材は一覧に出さない。利用者の追加は許していない
  await expect(page.locator('#catalog button')).toHaveText(['素材を使うシナリオ']);
  await expect(page.locator('#add-archive')).toHaveCount(0);
  await expect(page.locator('#catalog-url')).toHaveCount(0);

  await page.getByRole('button', { name: '素材を使うシナリオ' }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await page.locator('#screen').click({ position: { x: 600, y: 300 } });
  // 素材の効果音が鳴る
  await expect.poll(async () => (await soundCounts(page)).encoded).toBeGreaterThan(0);
});

test('素材（requires）の SoundFont で MIDI を合成して鳴らす', async ({ page }) => {
  test.skip(!fs.existsSync(soundFont), `SoundFont が無い: ${soundFont}`);
  // シナリオには SoundFont を入れず、素材の zip に入れる
  const scenario = makeZip(['test.eve', 'test.map', 'Sound/se.wav', 'Midi/bgm.mid']
    .map(name => ({ name: 'シナリオ/' + name, data: fs.readFileSync(path.join(corpus, 'sound', name)) })));
  const asset = makeZip([{ name: 'SoundFont/TimGM6mb.sf2', data: fs.readFileSync(soundFont) }]);
  const json = body => ({ status: 200, contentType: 'application/json', body: JSON.stringify(body) });
  await isolateWithHeaders(page);
  await page.route('**/srcweb.json', route => route.fulfill(json({ catalogs: ['dist/catalog.json'] })));
  await page.route('**/dist/catalog.json', route => route.fulfill(json({
    format: 1,
    name: 'SoundFont のテスト',
    entries: [
      { id: 'midi-with-soundfont', title: 'MIDI のシナリオ', archive: 'scenario.zip', root: 'シナリオ', eve: 'test.eve', requires: ['soundfont'] },
      { id: 'soundfont', kind: 'asset', title: 'SoundFont', archive: 'assets/soundfont.zip' },
    ],
  })));
  await page.route('**/dist/scenario.zip', route => route.fulfill({ status: 200, contentType: 'application/zip', body: scenario }));
  await page.route('**/dist/assets/soundfont.zip', route => route.fulfill({ status: 200, contentType: 'application/zip', body: asset }));

  await open(page);
  await page.getByRole('button', { name: 'MIDI のシナリオ' }).click();
  await expect(page.locator('#screen')).toBeVisible();
  await page.locator('#screen').click({ position: { x: 600, y: 300 } });
  await expect.poll(async () => (await soundCounts(page)).pcm, { timeout: 120 * 1000 }).toBeGreaterThan(0);
});

test('取得したアーカイブはキャッシュし、version が変わるまで取得し直さない', async ({ page }) => {
  const zip = fs.readFileSync(path.join(webRoot, 'scenarios', 'battle-victory.zip'));
  const json = body => ({ status: 200, contentType: 'application/json', body: JSON.stringify(body) });
  let version = 'v1';
  let downloads = 0;
  await isolateWithHeaders(page);
  await page.route('**/srcweb.json', route => route.fulfill(json({ catalogs: ['dist/catalog.json'] })));
  await page.route('**/dist/catalog.json', route => route.fulfill(json({
    format: 1,
    entries: [{ id: 'cached', title: 'キャッシュするシナリオ', archive: 'cached.zip', version, eve: 'test.eve' }],
  })));
  await page.route('**/dist/cached.zip', route => {
    downloads++;
    return route.fulfill({ status: 200, contentType: 'application/zip', body: zip });
  });
  const play = async () => {
    await page.getByRole('button', { name: 'キャッシュするシナリオ' }).click();
    await expect(page.locator('#screen')).toBeVisible({ timeout: 60 * 1000 });
    await expect.poll(() => debugState(page)).toContain('catalog-cached');
  };

  await open(page);
  await play();
  expect(downloads).toBe(1);

  // 開き直しても取得しない
  await open(page);
  await play();
  expect(downloads).toBe(1);

  // version が変われば取得し直す
  version = 'v2';
  await open(page);
  await play();
  expect(downloads).toBe(2);
});

test('URL の archive で任意の zip を直接始める', async ({ page }) => {
  const zip = fs.readFileSync(path.join(webRoot, 'scenarios', 'battle-victory.zip'));
  await isolateWithHeaders(page);
  await page.route('**/external/victory.zip', route => route.fulfill({ status: 200, contentType: 'application/zip', body: zip }));
  page.on('pageerror', e => console.log('pageerror:', e.message));
  await page.goto('./?archive=' + encodeURIComponent('external/victory.zip') + '&eve=test.eve');
  await expect(page.locator('#screen')).toBeVisible({ timeout: 180 * 1000 });
  await expect.poll(() => debugState(page)).toContain('url-');
  await expectAdvancesWithEnter(page);
});

test('エラーは直前のログとシナリオの行を付けて OTLP で送る', async ({ page }) => {
  const reports = [];
  await isolateWithHeaders(page);
  await page.route('**/srcweb.json', async route => {
    const site = await (await route.fetch()).json().catch(() => ({}));
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({ ...site, errorReporting: { mackerel: { clientToken: 'test-token', endpoint: 'otlp-test/v1/traces' } } }),
    });
  });
  await page.route('**/otlp-test/v1/traces', route => {
    reports.push({ headers: route.request().headers(), body: route.request().postDataJSON() });
    return route.fulfill({ status: 200, contentType: 'application/json', body: '{}' });
  });

  await page.goto('./?play=battle-basic');
  await expect(page.locator('#screen')).toBeVisible({ timeout: 180 * 1000 });
  await expect.poll(() => debugState(page)).toContain('catalog-battle-basic');
  await page.evaluate(() => {
    console.log('エラーの前のログ');
    console.error('E2E のエラー');
  });
  await expect.poll(() => reports.length).toBeGreaterThan(0);

  const report = reports.find(r => JSON.stringify(r.body).includes('E2E のエラー'));
  expect(report).toBeTruthy();
  expect(report.headers['x-mackerel-client-token']).toBe('test-token');
  const span = report.body.resourceSpans[0].scopeSpans[0].spans[0];
  const attribute = key => span.attributes.find(a => a.key === key)?.value.stringValue;
  expect(attribute('srcweb.scenario.line')).toContain('catalog-battle-basic');
  expect(span.status.code).toBe(2);
  expect(span.events.map(e => e.name)).toContain('exception');
  expect(JSON.stringify(span.events)).toContain('エラーの前のログ');
});
});

test('システムメニューから戻るときは確認し、「いいえ」ならプレイを続ける', async ({ page }, testInfo) => {
  await open(page);
  await page.getByRole('button', { name: 'battle-basic' }).click();
  await expect(page.locator('#screen')).toBeVisible();
  // プレイ中に目立つ「戻る」ボタンは無い
  await expect(page.getByRole('button', { name: 'シナリオ選択へ戻る' })).toHaveCount(0);
  await page.locator('#system-menu-button').click();
  await testInfo.attach('system-menu', { body: await page.screenshot(), contentType: 'image/png' });
  await page.getByRole('menuitem', { name: 'シナリオ選択へ戻る…' }).click();
  await expect(page.getByRole('alertdialog')).toBeVisible();
  await testInfo.attach('confirm', { body: await page.screenshot(), contentType: 'image/png' });
  await page.getByRole('alertdialog').getByRole('button', { name: 'いいえ' }).click();
  await expect(page.getByRole('alertdialog')).toHaveCount(0);
  await expect(page.locator('#screen')).toBeVisible();
  await expectAdvancesWithEnter(page);
  // メニューの外を押すと閉じる
  await page.locator('#system-menu-button').click();
  await expect(page.getByRole('menu')).toBeVisible();
  await page.mouse.click(10, 10);
  await expect(page.getByRole('menu')).toHaveCount(0);
});

test.describe('タッチ向けの画面', () => {
  test.use({ hasTouch: true, isMobile: true, viewport: { width: 390, height: 844 } });

  test('ゲームのリスト・ダイアログを、タッチ向けの選択画面の大きなボタンで選ぶ', async ({ page }, testInfo) => {
    await open(page);
    await page.getByRole('button', { name: 'intermission-save' }).click();
    await expect(page.locator('#screen')).toBeVisible();
    // スマートフォンの既定は仮想パッドだけを出す。拡大表示はシステムメニューで出す
    await expect(page.locator('#virtual-pad')).toBeVisible();
    await enableTouchUi(page);
    const panel = page.locator('#touch-ui');
    // メッセージは画面をタップして送る。インターミッションのコマンドの一覧が選択画面に出る
    await expect.poll(async () => {
      if (await panel.isVisible()) {
        return true;
      }
      const s = await screenState(page);
      if (s.message !== undefined) {
        await page.locator('#screen').tap({ position: { x: 200, y: 200 } });
      }
      return false;
    }, { intervals: [500, 1000] }).toBe(true);
    // 元の SRC と同じく、表題は「インターミッション： 総ターン数…」、「コマンド」は見出しの行
    await expect(panel.locator('.touch-ui-title')).toContainText('インターミッション');
    await expect(panel).toContainText('コマンド');
    const item = panel.getByRole('button', { name: '次のステージへ' });
    expect((await item.boundingBox()).height).toBeGreaterThanOrEqual(44);
    await attachScreen(page, testInfo, 'touch-list');
    await item.click();

    // 確認のダイアログも選択画面に出る
    await expect(panel).toContainText('次のステージへ進みますか');
    await attachScreen(page, testInfo, 'touch-dialog');
    await panel.locator('.touch-ui-buttons button').first().click();
    await expect.poll(async () => {
      const s = await screenState(page);
      if (s.message !== undefined) {
        await page.locator('#screen').tap({ position: { x: 200, y: 200 } });
      }
      return s.terminated;
    }, { intervals: [500, 1000] }).toBe(true);
  });

  test('ユニットのコマンドのメニューを、タッチ向けの選択画面で選ぶ', async ({ page }) => {
    await open(page);
    await page.getByRole('button', { name: 'battle-basic' }).click();
    await expect(page.locator('#screen')).toBeVisible();
    await enableTouchUi(page);
    // 「戦闘開始」のメッセージを送り、味方フェイズにする
    let shown = false;
    await expect.poll(async () => {
      const s = await screenState(page);
      if (s.message !== undefined) {
        shown = true;
        await page.locator('#screen').tap({ position: { x: 200, y: 200 } });
      }
      return shown && s.message === undefined && s.locked === false;
    }, { intervals: [500, 1000], timeout: 60 * 1000 }).toBe(true);
    // 味方ユニット（6,6）をタップする
    const s = await screenState(page);
    const box = await page.locator('#screen').boundingBox();
    const scale = box.width / 720;
    const cell = n => ((n - s.mapX + 7) * 32 + 16) * scale;
    await page.locator('#screen').tap({ position: { x: cell(6), y: (6 - s.mapY + 7) * 32 * scale + 16 * scale } });
    const panel = page.locator('#touch-ui');
    await expect(panel.getByRole('button', { name: '移動' })).toBeVisible();
    // キャンセルで閉じる
    await panel.getByRole('button', { name: 'キャンセル' }).click();
    await expect(panel).toBeHidden();
    await expect.poll(async () => (await page.locator('#touch-ui').isVisible())).toBe(false);
  });

  test('タッチの端末では押しやすい大きさにし、「操作」でマウス向けに切り替えられる', async ({ page }) => {
    await open(page);
    await expect(page.locator('html')).toHaveAttribute('data-input', 'touch');
    const rowHeight = () => page.locator('#catalog li').first().evaluate(x => x.getBoundingClientRect().height);
    expect(await rowHeight()).toBeGreaterThanOrEqual(44);
    await expect(page.locator('.addressbar')).toBeHidden();

    await page.selectOption('#input-mode', 'mouse');
    await expect(page.locator('html')).toHaveAttribute('data-input', 'mouse');
    expect(await rowHeight()).toBeLessThan(44);
    await expect(page.locator('.addressbar')).toBeVisible();
    // 開き直しても保つ
    await open(page);
    await expect(page.locator('html')).toHaveAttribute('data-input', 'mouse');
    await expect(page.locator('#input-mode')).toHaveValue('mouse');

    // タッチ向けに戻すと、プレイ中は仮想パッドを出す（表示を切り替えていなければ）
    await page.selectOption('#input-mode', 'touch');
    await expect(page.locator('html')).toHaveAttribute('data-input', 'touch');
    // 切り替えで一覧を描き直している間に押すと、押した先のハンドラーが無くなっていて開かないことがあるため、開くまで押し直す
    await expect(async () => {
      if (!(await page.locator('#screen').isVisible())) {
        await page.getByRole('button', { name: 'battle-basic' }).click({ timeout: 5000 });
      }
      await expect(page.locator('#screen')).toBeVisible({ timeout: 15000 });
    }).toPass({ timeout: 60000 });
    await expect(page.locator('#virtual-pad')).toBeVisible();
  });
});

test.describe('タッチ操作', () => {
  test.use({ hasTouch: true });

  test('タップで左クリック、長押しで右クリックになる', async ({ page }) => {
    await open(page);
    await page.getByRole('button', { name: 'battle-basic' }).click();
    await expect(page.locator('#screen')).toBeVisible();
    await expect.poll(() => frames(page)).toBeGreaterThan(0);
    const box = await page.locator('#screen').boundingBox();
    const x = box.x + box.width / 2;
    const y = box.y + box.height * 0.9;
    const touchClicks = () => page.evaluate(() => globalThis.srcwebTouchClicks ?? []);

    // タップでメッセージが進む（左クリック）
    const before = await debugState(page);
    await expect.poll(async () => {
      await page.touchscreen.tap(x, y);
      return debugState(page);
    }, { intervals: [500, 1000, 2000] }).not.toBe(before);
    expect((await touchClicks()).some(c => c.button === 0)).toBe(true);
    expect((await touchClicks()).some(c => c.button === 2)).toBe(false);

    // 長押しは右クリック
    const cdp = await page.context().newCDPSession(page);
    await cdp.send('Input.dispatchTouchEvent', { type: 'touchStart', touchPoints: [{ x, y }] });
    await page.waitForTimeout(800);
    await cdp.send('Input.dispatchTouchEvent', { type: 'touchEnd', touchPoints: [] });
    await expect.poll(async () => (await touchClicks()).filter(c => c.button === 2).length).toBe(1);

    // ページはスクロールしない
    expect(await page.evaluate(() => window.scrollY)).toBe(0);
  });

  test('指を滑らせるとマップがスクロールし、クリックにはならない', async ({ page }) => {
    await open(page);
    await page.getByRole('button', { name: 'battle-basic' }).click();
    await expect(page.locator('#screen')).toBeVisible();
    await expect.poll(() => frames(page)).toBeGreaterThan(0);
    const box = await page.locator('#screen').boundingBox();
    // 「戦闘開始」のメッセージを送り、味方フェイズ（マップ操作を受け付ける状態）にする
    let shown = false;
    await expect.poll(async () => {
      const s = await screenState(page);
      if (s.message !== undefined) {
        shown = true;
        await page.touchscreen.tap(box.x + box.width / 2, box.y + box.height * 0.9);
      }
      return shown && s.locked === false && s.message === undefined;
    }, { intervals: [500, 1000, 2000], timeout: 60 * 1000 }).toBe(true);
    const before = await screenState(page);
    const clicks = (await page.evaluate(() => globalThis.srcwebTouchClicks ?? [])).length;

    // マップ上で左上へ指を滑らせると、表示位置は右下へ動く（マウスの左ボタンのドラッグと同じ）
    const scale = box.width / (await page.locator('#screen').evaluate(c => c.width));
    const startX = box.x + 300 * scale;
    const startY = box.y + 300 * scale;
    const cdp = await page.context().newCDPSession(page);
    await cdp.send('Input.dispatchTouchEvent', { type: 'touchStart', touchPoints: [{ x: startX, y: startY }] });
    // ドラッグは半マス以上動いたときに反映されるため、1 マスずつ動かす
    for (const [dx, dy] of [[1, 1], [2, 2], [3, 2]]) {
      await cdp.send('Input.dispatchTouchEvent', {
        type: 'touchMove',
        touchPoints: [{ x: startX - 32 * dx * scale, y: startY - 32 * dy * scale }],
      });
      await page.waitForTimeout(100);
    }
    await cdp.send('Input.dispatchTouchEvent', { type: 'touchEnd', touchPoints: [] });
    await expect.poll(async () => {
      const s = await screenState(page);
      return [s.mapX - before.mapX, s.mapY - before.mapY];
    }).toEqual([3, 2]);
    // スワイプはクリックにならない
    expect((await page.evaluate(() => globalThis.srcwebTouchClicks ?? [])).length).toBe(clicks);
    expect(await page.evaluate(() => window.scrollY)).toBe(0);
  });

  test('シナリオの KeyState でタップ（左ボタン）と長押し（右ボタン）を検出できる', async ({ page }) => {
    // KeyState をループで調べるシナリオ（ロボダンのクリック領域などと同じ作り）
    const eve = [
      'プロローグ:',
      'ChangeMap test.map',
      'Global 左',
      'Global 右',
      'Set 左 0',
      'Set 右 0',
      'Do While (左 = 0 Or 右 = 0)',
      'If KeyState(1) = 1 Then',
      'Set 左 1',
      'Endif',
      'If KeyState(2) = 1 Then',
      'Set 右 1',
      'Endif',
      'Wait 1',
      'Loop',
      // 結果は選択肢の見出しに出す（ループ中のクリックはメッセージを送ってしまうため）
      'Ask "L=$(左) R=$(右)"',
      'OK',
      'End',
      'Quit',
      '',
    ].join('\r\n');
    const name = 'KeyState のテスト.zip';
    await open(page);
    await page.setInputFiles('#add-archive', {
      name,
      mimeType: 'application/zip',
      buffer: makeZip([
        { name: 'keystate/test.eve', data: Buffer.from(eve, 'utf8') },
        { name: 'keystate/test.map', data: fs.readFileSync(path.join(corpus, 'sound', 'test.map')) },
      ]),
    });
    await page.locator('#archives li', { hasText: name }).getByRole('button', { name }).click();
    await expect(page.locator('#screen')).toBeVisible();
    await expect.poll(() => frames(page)).toBeGreaterThan(0);
    const box = await page.locator('#screen').boundingBox();
    const x = box.x + box.width / 2;
    const y = box.y + box.height / 2;
    // KeyState を調べるループに入ってから操作する（ChangeMap の途中で押しても検出されない）
    await expect.poll(() => debugState(page), { timeout: 120 * 1000 }).toMatch(/test\.eve:(7|8|9|1[0-5]) /);

    await page.touchscreen.tap(x, y);
    const cdp = await page.context().newCDPSession(page);
    await cdp.send('Input.dispatchTouchEvent', { type: 'touchStart', touchPoints: [{ x, y }] });
    await page.waitForTimeout(900);
    await cdp.send('Input.dispatchTouchEvent', { type: 'touchEnd', touchPoints: [] });
    await expect.poll(() => debugState(page)).toContain('L=1 R=1');

    await backToMenu(page);
    await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
    await page.locator('#archives li', { hasText: name }).getByRole('button', { name: '削除' }).click();
  });
});

test('仮想パッドをシステムメニューで表示し、押したキーがシナリオの KeyState に届く', async ({ page }) => {
  const eve = [
    'プロローグ:',
    'ChangeMap test.map',
    'Global ゼット',
    'Global 上',
    'Set ゼット 0',
    'Set 上 0',
    'Do While (ゼット = 0 Or 上 = 0)',
    'If KeyState(90) = 1 Then',
    'Set ゼット 1',
    'Endif',
    'If KeyState(38) = 1 Then',
    'Set 上 1',
    'Endif',
    'Wait 1',
    'Loop',
    'Ask "Z=$(ゼット) U=$(上)"',
    'OK',
    'End',
    'Quit',
    '',
  ].join('\r\n');
  const name = '仮想パッドのテスト.zip';
  await open(page);
  await page.setInputFiles('#add-archive', {
    name,
    mimeType: 'application/zip',
    buffer: makeZip([
      { name: 'vkeys/test.eve', data: Buffer.from(eve, 'utf8') },
      { name: 'vkeys/test.map', data: fs.readFileSync(path.join(corpus, 'sound', 'test.map')) },
    ]),
  });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name }).click();
  await expect(page.locator('#screen')).toBeVisible();

  // マウスの端末では既定で出さない。システムメニューで出す
  await expect(page.locator('#virtual-pad')).toBeHidden();
  await page.locator('#system-menu-button').click();
  await page.getByRole('menuitem', { name: '仮想パッドの表示の切り替え' }).click();
  await expect(page.locator('#virtual-pad')).toBeVisible();
  await expect(page.locator('#virtual-pad .vkey')).toHaveCount(11);

  await expect.poll(() => debugState(page), { timeout: 120 * 1000 }).toMatch(/test\.eve:(7|8|9|1[0-5]) /);
  for (const selector of ['.vkey-z', '.vkey-up']) {
    const box = await page.locator(selector).boundingBox();
    await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
    await page.mouse.down();
    await page.waitForTimeout(400);
    await page.mouse.up();
  }
  await expect.poll(() => debugState(page)).toContain('Z=1 U=1');

  // 表示の設定は次回も残る
  await page.reload();
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name }).click();
  await expect(page.locator('#virtual-pad')).toBeVisible();
  await backToMenu(page);
  await page.waitForSelector('#catalog button', { timeout: 180 * 1000 });
  await page.locator('#archives li', { hasText: name }).getByRole('button', { name: '削除' }).click();
});
