
describe('Init Blazor Application', function () {
    it('Blazorアプリケーションとしてのレンダリングが行われる', function () {
        cy.visit('/');
        cy.get('input[type="file"]').should('exist');
        cy.percySnapshot();
    });
});

describe('Load Data', function () {
    it('適当なデータが読み込まれる', function () {
        // XXX cy.contains('∀ガンダム').click() でアンカー要素のデタッチエラーが出るのでリダイレクトからの読み込みにしてみている
        //cy.visit();
        //cy.get('.navbar-burger ').click();
        //cy.contains('Titles').trigger('mouseover');
        //cy.contains('∀ガンダム').click();
        //cy.get('body');
        cy.visit('/titles/https%3a%2f%2fraw.githubusercontent.com%2f7474%2fSRC-Data%2fmaster%2fGSC%e3%83%87%e3%83%bc%e3%82%bf%e3%83%91%e3%83%83%e3%82%af%2f%e3%83%ad%e3%83%9c%2f%e2%88%80%e3%82%ac%e3%83%b3%e3%83%80%e3%83%a0/alias.txt&animation.txt&item.txt&non_pilot.txt&pilot.txt&pilot_dialog.txt&pilot_message.txt&robot.txt&%e6%9b%b4%e6%96%b0%e5%b1%a5%e6%ad%b4.txt&%e6%a9%9f%e4%bd%93%e8%a7%a3%e8%aa%ac.txt', {
            failOnStatusCode: false,
        });
        cy.contains('∀ガンダム(前期)').should('exist');
        cy.percySnapshot();
    });
});
