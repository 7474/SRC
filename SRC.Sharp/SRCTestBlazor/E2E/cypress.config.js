const { defineConfig } = require('cypress')

module.exports = defineConfig({
  e2e: {
    baseUrl: 'https://srcv.7474.jp',
    defaultCommandTimeout: 20000,
    supportFile: 'cypress/support/index.js'
  }
})