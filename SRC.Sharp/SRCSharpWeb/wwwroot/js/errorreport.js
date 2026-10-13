// エラーの収集（docs/gui/web-hosting.md の errorReporting）
// - 未処理の例外・Promise の reject・console.error（.NET の例外や [MONO] の出力もここに出る）を拾う
// - エラーのときだけ、直前のログと実行中のシナリオの行を付けて、OpenTelemetry のトレース（OTLP/HTTP JSON）として送る
// - 送り先は Mackerel のトレース（クライアントトークン）。設定（srcweb.json）が無ければ何も送らない
//   https://mackerel.io/ja/docs/entry/tracing/guide/client-token
// index.html の最初に読み込み、起動前のエラーも拾う。設定は srcwebErrorReporting.configure で渡す
(() => {
    const MAX_BREADCRUMBS = 30;
    const MAX_REPORTS = 10; // 1 回の読み込みで送る上限
    const MAX_TEXT = 4000;
    const breadcrumbs = [];
    const sent = new Set();
    const pending = [];
    let config = null;
    let reports = 0;
    let debugState = null;
    let reporting = false;

    const now = () => Date.now();
    const text = v => {
        if (v instanceof Error) {
            return `${v.name}: ${v.message}`;
        }
        if (typeof v === 'string') {
            return v;
        }
        try {
            return JSON.stringify(v);
        } catch {
            return String(v);
        }
    };
    const clip = s => (s.length > MAX_TEXT ? s.slice(0, MAX_TEXT) + '…' : s);

    const addBreadcrumb = (level, message) => {
        breadcrumbs.push({ time: now(), level, message: clip(message) });
        if (breadcrumbs.length > MAX_BREADCRUMBS) {
            breadcrumbs.shift();
        }
    };

    const hex = bytes => Array.from(crypto.getRandomValues(new Uint8Array(bytes)), b => b.toString(16).padStart(2, '0')).join('');
    const nano = ms => (BigInt(Math.floor(ms)) * 1000000n).toString();
    const attr = (key, value) => ({ key, value: { stringValue: String(value ?? '') } });

    // 実行中のシナリオの行など（WebInterop.DebugState）。.NET が動いていなければ空
    const scenarioState = async () => {
        if (!debugState) {
            return {};
        }
        try {
            const json = await Promise.race([
                debugState(),
                new Promise((_, reject) => setTimeout(() => reject(new Error('timeout')), 1000)),
            ]);
            return JSON.parse(json);
        } catch {
            return {};
        }
    };

    const send = async (type, message, stack) => {
        const key = `${type}:${message}`.slice(0, 500);
        if (sent.has(key) || reports >= MAX_REPORTS) {
            return;
        }
        sent.add(key);
        reports++;
        const state = await scenarioState();
        const time = now();
        const attributes = [
            attr('exception.type', type),
            attr('exception.message', clip(message)),
            attr('exception.stacktrace', clip(stack || '')),
        ];
        const spanAttributes = [
            attr('url.full', location.href),
            attr('user_agent.original', navigator.userAgent),
            attr('srcweb.scenario.line', state.line || ''),
            attr('srcweb.screen', state.screen ? text(state.screen) : ''),
            attr('srcweb.terminated', state.terminated ?? ''),
        ];
        const events = breadcrumbs.map(b => ({
            name: 'log',
            timeUnixNano: nano(b.time),
            attributes: [attr('log.level', b.level), attr('log.message', b.message)],
        }));
        events.push({ name: 'exception', timeUnixNano: nano(time), attributes });
        const body = {
            resourceSpans: [{
                resource: {
                    attributes: [
                        attr('service.name', config.serviceName || 'srcsharp-web'),
                        attr('service.version', config.version || ''),
                        attr('deployment.environment.name', location.host),
                    ],
                },
                scopeSpans: [{
                    scope: { name: 'srcweb.errors' },
                    spans: [{
                        traceId: hex(16),
                        spanId: hex(8),
                        name: `error: ${type}`,
                        kind: 1,
                        startTimeUnixNano: nano(breadcrumbs.length ? breadcrumbs[0].time : time),
                        endTimeUnixNano: nano(time),
                        attributes: spanAttributes,
                        status: { code: 2, message: clip(message) },
                        events,
                    }],
                }],
            }],
        };
        try {
            await fetch(config.endpoint || 'https://otlp-vaxila.mackerelio.com/v1/traces', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-Mackerel-Client-Token': config.clientToken },
                body: JSON.stringify(body),
                keepalive: true,
                mode: 'cors',
            });
        } catch {
            // 送れなくてもゲームは続ける
        }
    };

    const report = (type, message, stack) => {
        if (reporting) {
            return; // 送信中のエラーで再帰しない
        }
        if (!config) {
            // 設定が来る前のエラーは取っておく
            if (pending.length < MAX_REPORTS) {
                pending.push([type, message, stack]);
            }
            return;
        }
        reporting = true;
        send(type, message, stack).finally(() => { reporting = false; });
    };

    // console を包み、ログを残し、エラーを送る
    for (const level of ['log', 'info', 'warn', 'error']) {
        const original = console[level].bind(console);
        console[level] = (...args) => {
            original(...args);
            const message = args.map(text).join(' ');
            addBreadcrumb(level, message);
            if (level === 'error') {
                const error = args.find(a => a instanceof Error);
                report(error ? error.name : 'console.error', message, error?.stack);
            }
        };
    }
    addEventListener('error', e => {
        addBreadcrumb('error', e.message);
        report(e.error?.name || 'Error', e.message, e.error?.stack || `${e.filename}:${e.lineno}:${e.colno}`);
    });
    addEventListener('unhandledrejection', e => {
        const reason = e.reason;
        addBreadcrumb('error', text(reason));
        report(reason?.name || 'UnhandledRejection', reason?.message || text(reason), reason?.stack);
    });

    globalThis.srcwebErrorReporting = {
        /** srcweb.json の errorReporting.mackerel（clientToken が無ければ送らない） */
        configure(options, version) {
            if (!options?.clientToken) {
                pending.length = 0;
                return;
            }
            config = { ...options, version };
            for (const args of pending.splice(0)) {
                report(...args);
            }
        },
        /** 実行中のシナリオの行などを返す関数（srcweb.js が .NET の準備後に渡す） */
        setDebugState(fn) {
            debugState = fn;
        },
        /** .NET 側のエラー（ゲームの例外・イベントエラー）を送る */
        report(type, message, stack) {
            addBreadcrumb('error', `${type}: ${message}`);
            report(type, message, stack);
        },
        /** 画面の操作などを残す（エラーの前後関係を追うため） */
        breadcrumb(message) {
            addBreadcrumb('info', message);
        },
    };
})();
