// Synthetic pack: a red 1px BMP, silent PCM WAV, unsupported MIDI and data text.
describe('Pack resources', () => {
    const catalog = {
        format: 1,
        name: 'Test packs',
        entries: [{
            id: 'media', kind: 'asset', title: 'Media fixture',
            archive: '/files/packs/media/v1.zip', version: 'v1', root: 'bundle',
            author: 'Fixture author', license: 'Fixture terms', description: 'Unreferenced media'
        }]
    };

    beforeEach(() => {
        cy.intercept('GET', 'https://raw.githubusercontent.com/7474/SRC-Data/master/index.json', { titles: [] });
        cy.intercept('GET', 'https://raw.githubusercontent.com/7474/SRC-Data/master/index-bitmap.json', { folders: [] });
        cy.intercept('GET', '**/api/packs.json', catalog);
        cy.fixture('pack-media').then(({ zip }) => {
            const bytes = Cypress.Buffer.from(zip, 'base64');
            cy.intercept('GET', '**/files/packs/media/v1.zip', {
                body: bytes.buffer.slice(bytes.byteOffset, bytes.byteOffset + bytes.byteLength),
                headers: { 'content-type': 'application/zip' }
            });
        });
    });

    it('browses every file under root and previews images and audio without data selection', () => {
        cy.visit('/packs?pack=media');
        cy.contains('Fixture author').should('exist');
        cy.contains('Fixture terms').should('exist');
        cy.contains('outside.txt').should('not.exist');
        cy.contains('button', 'Bitmap/unreferenced.bmp').click();
        cy.get('img[alt="Bitmap/unreferenced.bmp"]').should($img => {
            expect($img[0].naturalWidth).to.equal(1);
        });
        cy.get('a[download]').should('have.attr', 'href').and('match', /^blob:/);
        cy.screenshot('pack-media-preview');
        cy.contains('button', 'Sound/tone.wav').click();
        cy.get('audio').should($audio => {
            expect($audio[0].readyState).to.be.greaterThan(0);
            expect($audio[0].duration).to.be.closeTo(0.1, 0.01);
        });
        cy.get('audio').then($audio => $audio[0].play());
        cy.get('audio').should($audio => expect($audio[0].ended).to.equal(true));
        cy.contains('button', 'Midi/test.mid').click();
        cy.get('audio').should('not.exist');
        cy.get('a[download]').should('have.attr', 'download', 'test.mid');
        cy.get('#resource-filter').type('robot');
        cy.contains('button', 'Data/robot.txt').should('exist');
        cy.contains('button', 'Bitmap/unreferenced.bmp').should('not.exist');
        cy.window().then(win => cy.spy(win.URL, 'revokeObjectURL').as('revoke'));
        cy.get('.navbar-brand a').first().click();
        cy.get('@revoke').should('have.been.called');
    });

    it('shows explicit catalog errors and can retry', () => {
        cy.intercept('GET', '**/api/packs.json', { statusCode: 404 });
        cy.visit('/packs?pack=media');
        cy.get('[role="alert"]').should('be.visible');
        cy.intercept('GET', '**/api/packs.json', catalog);
        cy.get('form button').click();
        cy.contains('button', 'Sound/tone.wav').should('exist');
        cy.get('[role="alert"]').should('not.exist');
    });

    it('opens local ZIP files without a catalog or upload', () => {
        cy.visit('/packs');
        cy.fixture('pack-media').then(({ zip }) => {
            cy.get('#pack-zip').selectFile({
                contents: Cypress.Buffer.from(zip, 'base64'),
                fileName: 'local.zip', mimeType: 'application/zip'
            });
        });
        cy.contains('button', 'bundle/Bitmap/unreferenced.bmp').should('exist');
        cy.contains('button', 'outside.txt').should('exist');
    });

    it('reports corrupt ZIP files', () => {
        cy.intercept('GET', '**/files/packs/media/v1.zip', 'not a zip');
        cy.visit('/packs?pack=media');
        cy.get('[role="alert"]').should('be.visible');
        cy.get('#resource-filter').should('not.exist');
    });

    it('rejects unsupported catalog versions and missing roots', () => {
        cy.intercept('GET', '**/api/packs.json', { ...catalog, format: 2 });
        cy.visit('/packs?pack=media');
        cy.get('[role="alert"]').should('contain.text', 'format-1');
        cy.intercept('GET', '**/api/packs.json', {
            ...catalog, entries: [{ ...catalog.entries[0], root: 'missing' }]
        });
        cy.get('form button').click();
        cy.get('[role="alert"]').should('contain.text', 'root');
        cy.get('#resource-filter').should('not.exist');
    });

    it('does not expand a resource beyond the individual size limit', () => {
        cy.fixture('pack-media').then(({ zip }) => {
            const bytes = Cypress.Buffer.from(zip, 'base64');
            // Change the first central-directory entry's declared uncompressed size.
            const offset = bytes.indexOf(Cypress.Buffer.from([0x50, 0x4b, 0x01, 0x02]));
            bytes.writeUInt32LE(16 * 1024 * 1024 + 1, offset + 24);
            cy.intercept('GET', '**/files/packs/media/v1.zip', {
                body: bytes.buffer.slice(bytes.byteOffset, bytes.byteOffset + bytes.byteLength)
            });
        });
        cy.visit('/packs?pack=media');
        cy.contains('button', 'Bitmap/unreferenced.bmp').click();
        cy.get('[role="alert"]').should('contain.text', '16 MiB');
        cy.get('a[download]').should('not.exist');
        cy.get('img[alt="Bitmap/unreferenced.bmp"]').should('not.exist');
    });

    it('updates selection when navigating to another pack query on the same page', () => {
        cy.visit('/packs?pack=media');
        cy.contains('Fixture author').should('exist');
        cy.document().then(doc => {
            const link = doc.createElement('a');
            link.href = '/packs?pack=missing';
            link.id = 'other-pack-link';
            link.textContent = 'Other pack';
            doc.body.appendChild(link);
        });
        cy.get('#other-pack-link').click();
        cy.get('[role="alert"]').should('contain.text', 'not found');
        cy.get('#resource-filter').should('not.exist');
    });
});
