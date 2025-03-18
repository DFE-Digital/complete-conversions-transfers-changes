import cookies from "../pages/cookies";
import cookiesPage from "../pages/cookiesPage";

describe("Testing cookie preferences", () => {
    beforeEach(() => {
        cy.login();
        cy.visit("/");
    });

    it("Should have analytics cookies if accepted", () => {
        cookies
            .consentCookieIsNotSet()
            .cookieBannerIsVisible()
            .acceptCookies()
            .consentCookieIsSetToTrue()
            .cookieBannerIsNotVisible();
    });

    it("Should have analytics cookies as false if rejected", () => {
        cookies
            .consentCookieIsNotSet()
            .cookieBannerIsVisible()
            .rejectCookies()
            .consentCookieIsSetToFalse()
            .cookieBannerIsNotVisible();
    });

    it("Should be able to view cookies page", () => {
        cookies.viewCookies()

        cookiesPage.shouldBeOnCookiesPage();

        cookies.cookieBannerIsVisible();

        cookiesPage
            .containsCookiePreferencesHeader()
            .containsEssentialCookiesHeader()
            .containsAnalyticsCookiesHeader()
            .denyAnalyticsCookiesIsChecked()
            .saveChangesIsClickable();
    });

    it.skip("Should be able to view then accept cookies", () => {
        // bug: 201914
        cookies.viewCookies();

        cookiesPage
            .shouldBeOnCookiesPage()
            .selectAcceptAnalyticsCookies()
            .saveChanges();

        cookies
            .consentCookieIsSetToTrue()
            .cookieBannerIsNotVisible(); // This is failing, banner is still visible

        cookiesPage.goBackToThePreviousPage();

        cy.url().should("not.contain", "/public/cookies");

        cookies
            .consentCookieIsSetToTrue()
            .cookieBannerIsNotVisible();
    });

});
