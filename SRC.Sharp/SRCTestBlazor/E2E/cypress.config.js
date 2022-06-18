const { defineConfig } = require('cypress')

module.exports = defineConfig({
  e2e: {
    projectId: 'a4eukb',
    baseUrl: 'https://srcv.7474.jp',
    defaultCommandTimeout: 20000,
  }
})