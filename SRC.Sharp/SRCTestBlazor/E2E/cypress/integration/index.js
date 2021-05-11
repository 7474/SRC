
describe('Init Blazor Application', function () {
    it('Blazorアプリケーションとしてのレンダリングが行われる', function () {
        cy.visit('https://7474.github.io/SRC/');
        cy.get('input[type="file"]').should('exist');
    });
});

describe('Load Data', function () {
    it('適当なデータが読み込まれる', function () {
        // TODO
        cy.visit('https://7474.github.io/SRC/');
    });
});
