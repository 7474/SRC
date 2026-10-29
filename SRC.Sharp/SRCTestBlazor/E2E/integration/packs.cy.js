describe('Published viewer mounted at /viewer/', () => {
    beforeEach(() => {
        // Only the unrelated legacy Titles index is mocked; catalog and ZIP use real HTTP.
        cy.intercept('GET', 'https://raw.githubusercontent.com/7474/SRC-Data/master/index.json', { titles: [] });
        cy.intercept('GET', 'https://raw.githubusercontent.com/7474/SRC-Data/master/index-bitmap.json', { folders: [] });
    });

    function checkMedia() {
        cy.contains('Fixture author').should('exist');
        cy.contains('Fixture terms').should('exist');
        cy.contains('outside.txt').should('not.exist');
        cy.contains('button', 'Bitmap/unreferenced.bmp').click();
        cy.get('img[alt="Bitmap/unreferenced.bmp"]').should($img => {
            expect($img[0].naturalWidth).to.equal(1);
        });
        cy.contains('button', 'Sound/tone.wav').click();
        cy.get('audio').should($audio => expect($audio[0].readyState).to.be.greaterThan(0));
        cy.get('audio').then($audio => $audio[0].play());
        cy.get('audio').should($audio => expect($audio[0].ended).to.equal(true));
        cy.get('#blazor-error-ui').should('not.be.visible');
    }

    it('opens a cold deep link and reloads with correct request paths', () => {
        cy.visit('/viewer/packs?pack=media');
        cy.get('base').should('have.attr', 'href', '/viewer/');
        cy.get('#catalog-url').should('have.value', `${Cypress.config('baseUrl')}/api/packs.json`);
        checkMedia();
        // Check worker installation separately without Cypress's HTML/JS proxy instrumentation.
        cy.reload();
        checkMedia();
        cy.request('/__requests').then(({ body }) => {
            const paths = body.map(request => request.path);
            expect(paths.filter(p => p === '/api/packs.json').length).to.be.at.least(2);
            expect(paths.filter(p => p === '/files/packs/media/v1.zip').length).to.be.at.least(2);
            expect(paths).to.include('/viewer/_framework/blazor.webassembly.js');
            expect(paths).to.include('/viewer/pack-media.js');
            expect(paths).not.to.include('/viewer/api/packs.json');
            expect(paths).not.to.include('/viewer/files/packs/media/v1.zip');
            expect(paths).not.to.include('/_framework/blazor.webassembly.js');
        });
    });

    it('keeps API and asset 404s distinct from HTML navigation fallback', () => {
        cy.request({ url: '/viewer', followRedirect: false }).its('redirectedToUrl').should('match', /\/viewer\/$/);
        cy.request({ url: '/viewer/packs?pack=media', headers: { Accept: 'text/html' } })
            .its('body').should('contain', '<base href="/viewer/" />');
        cy.request({ url: '/viewer/index.html', method: 'HEAD', followRedirect: false }).its('status').should('equal', 200);
        cy.request('/viewer/index.ghp.html').its('body').should('contain', '<script src="pack-media.js"></script>');
        for (const url of ['/viewer/missing.js', '/api/missing', '/files/packs/missing/v1.zip']) {
            cy.request({ url, failOnStatusCode: false }).its('status').should('equal', 404);
        }
        cy.request({ url: '/api/missing', headers: { Accept: 'text/html' }, failOnStatusCode: false })
            .its('status').should('equal', 404);
    });
});
