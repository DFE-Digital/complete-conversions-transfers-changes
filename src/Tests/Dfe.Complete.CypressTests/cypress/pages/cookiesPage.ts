class CookiesPage {
    private readonly consentCookieAcceptId = 'cookie-consent-accept';
    private readonly consentCookieDenyId = 'cookie-consent-deny';
    private readonly saveChangesButton = '[data-qa="submit"]';
    private readonly successBannerReturnLink = '[data-test="success-banner-return-link"]';

    public shouldBeOnCookiesPage() {
        cy.url().should('contain', '/public/cookies');
        return this;
    }

    public containsCookiePreferencesHeader() {
        cy.get('h1').should('contain', 'Cookie preferences');
        return this;
    }

    public containsEssentialCookiesHeader() {
        cy.get('h2').should('contain', 'Essential cookies');
        return this;
    }

    public containsAnalyticsCookiesHeader() {
        cy.get('h2').should('contain', 'Analytics cookies (optional)');
        return this;
    }

    public denyAnalyticsCookiesIsChecked() {
        cy.getById(this.consentCookieDenyId).should('be.checked');
        return this;
    }

    public selectAcceptAnalyticsCookies() {
        cy.getById(this.consentCookieAcceptId).check();
        cy.getById(this.consentCookieDenyId).should('not.be.checked');
        cy.getById(this.consentCookieAcceptId).should('be.checked');
        return this;
    }

    public saveChangesIsClickable() {
        cy.get(this.saveChangesButton).should('be.enabled');
        return this;
    }

    public saveChanges() {
        cy.get(this.saveChangesButton).click();
        return this;
    }

    public goBackToThePreviousPage() {
        cy.get(this.successBannerReturnLink).click();
        return this;
    }
}

const cookiesPage = new CookiesPage();

export default cookiesPage;