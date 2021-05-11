
const ROOT_URL = 'https://7474.github.io/SRC/';

describe('Init Blazor Application', function () {
    it('Blazorアプリケーションとしてのレンダリングが行われる', function () {
        cy.visit(ROOT_URL);
        cy.get('input[type="file"]').should('exist');
    });
});

describe('Load Data', function () {
    it('適当なデータが読み込まれる', function () {
        cy.visit(ROOT_URL);
        cy.get('.navbar-burger ').click();
        cy.contains('Titles').trigger('mouseover');
        cy.contains('∀ガンダム').click();
        cy.get('body');
        cy.contains('∀ガンダム(前期)').should('exist');
    });
});
