// ***********************************************************
// This example support/index.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import "./commands";
import { RuleObject } from "axe-core";
import { yesNoOption } from "../constants/stringTestConstants";
import { TestUser } from "cypress/constants/TestUser";

Cypress.on("url:changed", (url) => {
    url = url.replace(`${Cypress.config("baseUrl")}`, "");
    url = url.split("#")[0]; // Remove any hash fragments
    url = url.split("?")[0]; // Remove any query parameters
    if (!Cypress.env("visitedUrls")) {
        Cypress.env("visitedUrls", new Set());
    }
    Cypress.env("visitedUrls").add(url);
});

declare global {
    namespace Cypress {
        interface Chainable {
            getByTestId(id: string): Chainable<Element>;

            getByDataCy(dataCy: string): Chainable<Element>;

            getById(id: string): Chainable<Element>;

            getByClass(className: string): Chainable<Element>;

            getByName(name: string): Chainable<Element>;

            getByRole(role: string): Chainable<Element>;

            getByLabelFor(labelFor: string): Chainable<Element>;

            getByRadioOption(radioText: string): Chainable<Element>;

            getProjectTableRow(schoolName: string): Chainable<JQuery<HTMLTableRowElement>>;

            login(user?: TestUser): Chainable<Element>;

            loginWithCredentials(): Chainable<Element>;

            loginRuby(): Chainable<Element>;

            notAuthorisedToPerformAction(): Chainable<Element>;

            acceptCookies(): Chainable<Element>;

            assertChildList(selector: string, values: string[]): Chainable<Element>;

            executeAccessibilityTests(ruleExclusions?: RuleObject): Chainable<Element>;

            enterDate(idPrefix: string, day: string, month: string, year: string): Chainable<Element>;

            enterYesNo(idPrefix: string, option: yesNoOption): Chainable<Element>;

            hasAddress(id: string, line1: string, line2: string, line3: string): Chainable<Element>;

            typeFast(text: string): Chainable<Element>;

            shouldHaveText(expectedText: string | number): Chainable<Element>;

            revisitCurrentUrl(): Chainable<Element>;
        }
    }
}
