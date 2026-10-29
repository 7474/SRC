const { defineConfig } = require('cypress');
const { startViewerHost } = require('./published-viewer-host.cjs');

module.exports = defineConfig({
    allowCypressEnv: false,
    video: false,
    e2e: {
        specPattern: 'integration/**/*.cy.js',
        supportFile: false,
        defaultCommandTimeout: 20000,
        async setupNodeEvents(on, config) {
            const host = await startViewerHost(process.env.SRCVIEWER_DIR);
            config.baseUrl = host.url;
            on('after:run', () => host.close());
            return config;
        }
    }
});
