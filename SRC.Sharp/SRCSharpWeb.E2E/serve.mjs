// 発行した SRC#Web（静的ファイル）を配信する。GitHub Pages と同じくヘッダは付けず、クロスオリジン分離は coi-serviceworker に任せる
import { createServer } from 'node:http';
import { readFile, stat } from 'node:fs/promises';
import { extname, join, normalize, resolve } from 'node:path';

const root = resolve(process.argv[2] ?? '../SRCSharpWeb/bin/web/wwwroot');
const port = Number(process.argv[3] ?? 5280);
// 拡張子の無い道（/archive?id=… など）でファイルが無ければ、ルートの index.html を返す（データビューアのような SPA の頁。SPA_FALLBACK=1）
const spaFallback = !!process.env.SPA_FALLBACK;
const types = {
  '.html': 'text/html; charset=utf-8',
  '.js': 'text/javascript',
  '.mjs': 'text/javascript',
  '.json': 'application/json',
  '.wasm': 'application/wasm',
  '.css': 'text/css',
  '.dat': 'application/octet-stream',
  '.zip': 'application/zip',
  '.ttf': 'font/ttf',
  '.png': 'image/png',
  '.bmp': 'image/bmp',
  '.wav': 'audio/wav',
  '.mid': 'audio/midi',
  '.txt': 'text/plain; charset=utf-8',
};

createServer(async (req, res) => {
  const path = decodeURIComponent(new URL(req.url, 'http://localhost').pathname);
  let file = normalize(join(root, path));
  if (!file.startsWith(root)) {
    res.writeHead(403).end();
    return;
  }
  try {
    if ((await stat(file)).isDirectory()) {
      file = join(file, 'index.html');
    }
    const body = await readFile(file);
    res.writeHead(200, { 'Content-Type': types[extname(file)] ?? 'application/octet-stream' }).end(body);
  } catch {
    if (spaFallback && !extname(path)) {
      res.writeHead(200, { 'Content-Type': types['.html'] }).end(await readFile(join(root, 'index.html')));
      return;
    }
    res.writeHead(404).end();
  }
}).listen(port, '127.0.0.1', () => console.log(`serving ${root} on http://127.0.0.1:${port}/`));
