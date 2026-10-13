const { test } = require('@playwright/test');
test('debug', async ({ page }) => {
  page.on('console', m => console.log('console.' + m.type() + ':', m.text()));
  page.on('pageerror', e => console.log('pageerror:', e.stack));
  await page.goto('./');
  await page.waitForTimeout(40000);
});
