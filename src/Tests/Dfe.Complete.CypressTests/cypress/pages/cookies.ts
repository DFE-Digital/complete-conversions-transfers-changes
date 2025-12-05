class Cookies {
    private readonly consentCookie = "ACCEPT_OPTIONAL_COOKIES";
    private readonly acceptCookiesTestId = "cookie-banner-accept";
    private readonly rejectCookiesTestId = "cookie-banner-reject";
    private readonly viewCookiesLinkTestId = "cookie-banner-link";
    private readonly cookieBannerId = "appCookieBanner";

    public consentCookieIsSetToTrue() {
        cy.getCookie(this.consentCookie).should("exist").should("have.property", "value", "true");
        return this;
    }

    public consentCookieIsSetToFalse() {
        cy.getCookie(this.consentCookie).should("exist").should("have.property", "value", "false");
        return this;
    }

    public consentCookieIsNotSet() {
        cy.getCookie(this.consentCookie).should("not.exist");
        return this;
    }

    public acceptCookies() {
        cy.getByTestId(this.acceptCookiesTestId).click();
        return this;
    }

    public rejectCookies() {
        cy.getByTestId(this.rejectCookiesTestId).click();
        return this;
    }

    public viewCookies() {
        cy.getByTestId(this.viewCookiesLinkTestId).click();
        return this;
    }

    public cookieBannerIsVisible() {
        cy.getById(this.cookieBannerId).should("be.visible");
        return this;
    }

    public cookieBannerIsNotVisible() {
        cy.getById(this.cookieBannerId).should("not.exist");
        return this;
    }
}

const cookies = new Cookies();

export default cookies;
