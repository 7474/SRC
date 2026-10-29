const http = require('node:http');
const fs = require('node:fs/promises');
const path = require('node:path');
const crypto = require('node:crypto');
const { zip } = require('./cypress/fixtures/pack-media.json');

// Model SRC-Site's assembly and HTML-only fallback, not its Cloudflare runtime.
async function startViewerHost(directory) {
    if (!directory) throw new Error('SRCVIEWER_DIR must point to a published viewer wwwroot.');
    const root = path.resolve(directory);
    const index = (await fs.readFile(path.join(root, 'index.html'), 'utf8'))
        .replace('<base href="/" />', '<base href="/viewer/" />');
    if (!index.includes('<base href="/viewer/" />')) throw new Error('Unexpected base href.');
    const hash = `sha256-${crypto.createHash('sha256').update(index).digest('base64')}`;
    const manifest = (await fs.readFile(path.join(root, 'service-worker-assets.js'), 'utf8'))
        .replace(/("hash"\s*:\s*")[^"]+("\s*,\s*"url"\s*:\s*"index\.html")/, `$1${hash}$2`);
    if (!manifest.includes(hash)) throw new Error('Could not update index.html integrity.');
    const types = {
        '.html': 'text/html', '.js': 'text/javascript', '.json': 'application/json',
        '.wasm': 'application/wasm', '.css': 'text/css', '.png': 'image/png',
        '.woff': 'font/woff', '.woff2': 'font/woff2'
    };
    const requests = [];
    const catalog = {
        format: 1, name: 'Site pack contract',
        entries: [{
            id: 'media', kind: 'asset', title: 'Published media pack',
            archive: '/files/packs/media/v1.zip', version: 'v1', root: 'bundle',
            author: 'Fixture author', license: 'Fixture terms'
        }]
    };
    const server = http.createServer(async (req, res) => {
        const url = new URL(req.url, 'http://localhost');
        const name = decodeURIComponent(url.pathname);
        const send = (status, type, body) => {
            res.writeHead(status, { 'Content-Type': type, 'Cache-Control': 'no-store' });
            res.end(req.method === 'HEAD' ? undefined : body);
        };
        try {
            if (name === '/__requests') return send(200, 'application/json', JSON.stringify(requests));
            requests.push({ method: req.method, path: name });
            if (name === '/api/packs.json') return send(200, 'application/json', JSON.stringify(catalog));
            if (name === '/files/packs/media/v1.zip') return send(200, 'application/zip', Buffer.from(zip, 'base64'));
            if (name === '/viewer') {
                res.writeHead(302, { Location: '/viewer/' });
                return res.end();
            }
            if (!name.startsWith('/viewer/')) return send(404, 'text/plain', 'Not found');
            const relative = name.substring('/viewer/'.length);
            if (relative === '' || relative === 'index.html') return send(200, 'text/html', index);
            const file = path.resolve(root, relative);
            if (!file.startsWith(root + path.sep) || /\.(br|gz)$/.test(file)) {
                return send(404, 'text/plain', 'Not found');
            }
            if (relative === 'service-worker-assets.js') return send(200, 'text/javascript', manifest);
            let contents;
            try {
                contents = await fs.readFile(file);
            } catch (error) {
                if (error.code !== 'ENOENT' && error.code !== 'EISDIR') throw error;
                if ((req.headers.accept || '').includes('text/html')) return send(200, 'text/html', index);
                return send(404, 'text/plain', 'Not found');
            }
            return send(200, types[path.extname(file)] || 'application/octet-stream', contents);
        } catch (error) {
            console.error(error);
            return send(500, 'text/plain', error.message);
        }
    });
    await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
    return {
        url: `http://127.0.0.1:${server.address().port}`,
        close: () => new Promise((resolve, reject) => {
            server.close(error => error ? reject(error) : resolve());
            server.closeAllConnections();
        })
    };
}

module.exports = { startViewerHost };
