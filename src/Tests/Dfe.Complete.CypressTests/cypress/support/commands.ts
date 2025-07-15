import "cypress-localstorage-commands";
import "cypress-axe";
import { AuthenticationInterceptor } from "../auth/authenticationInterceptor";
import { Logger } from "../common/logger";
import { RuleObject } from "axe-core";
import { yesNoOption } from "../constants/stringTestConstants";
import Chainable = Cypress.Chainable;

Cypress.Commands.add("getByTestId", (id) => {
    cy.get(`[data-testid="${id}"]`);
});

Cypress.Commands.add("containsByTestId", (id) => {
    cy.get(`[data-testid*="${id}"]`);
});

Cypress.Commands.add("getById", (id) => {
    cy.get(`[id="${id}"]`);
});

Cypress.Commands.add("containsById", (id) => {
    cy.get(`[id*="${id}"]`);
});

Cypress.Commands.add("getByClass", (className) => {
    cy.get(`[class~="${className}"]`);
});

Cypress.Commands.add("getByName", (name) => {
    cy.get(`[name="${name}"]`);
});

Cypress.Commands.add("getByRole", (role) => {
    cy.get(`[role="${role}"]`);
});

Cypress.Commands.add("getByLabelFor", (labelFor) => {
    cy.get(`[for="${labelFor}"]`);
});

Cypress.Commands.add("getProjectTableRow", (schoolName: string): Chainable<JQuery<HTMLTableRowElement>> => {
    return cy.getByClass("govuk-table").contains(schoolName).closest("tr");
});

Cypress.Commands.add("getByRadioOption", (radioText: string) => {
    cy.contains(radioText)
        .invoke("attr", "for")
        .then((id) => {
            cy.get("#" + id);
        });
});

Cypress.Commands.add("assertChildList", (selector: string, values: string[]) => {
    cy.getByTestId(selector)
        .children()
        .should("have.length", values.length)
        .each((el, i) => {
            expect(el.text()).to.equal(values[i]);
        });
});

Cypress.Commands.add("login", (user) => {
    cy.clearCookies();
    cy.clearLocalStorage();

    // // Intercept all browser requests and add our special auth header
    // // Means we don't have to use azure to authenticate
    new AuthenticationInterceptor().register(user);
});

Cypress.Commands.add("loginRuby", () => {
    cy.clearCookies();
    cy.clearLocalStorage();

    cy.visit("/");
    cy.contains("button", "Sign in with your DfE Microsoft account").click();
});

Cypress.Commands.add("notAuthorisedToPerformAction", () => {
    cy.get("h1").contains("You do not have access to this service");
});

Cypress.Commands.add("acceptCookies", () => {
    cy.setCookie("ACCEPT_OPTIONAL_COOKIES", "True");
});

Cypress.Commands.add("executeAccessibilityTests", (ruleOverride?: RuleObject) => {
    const continueOnFail = false;

    let ruleConfiguration: RuleObject = {
        region: { enabled: false },
    };

    if (ruleOverride) {
        ruleConfiguration = { ...ruleConfiguration, ...ruleOverride };
    }

    // Ensure that the axe dependency is available in the browser
    Logger.log("Injecting Axe and checking accessibility");
    cy.injectAxe();

    cy.checkA11y(
        undefined,
        {
            runOnly: {
                type: "tag",
                values: ["wcag2aa"],
            },
            rules: ruleConfiguration,
        },
        undefined,
        continueOnFail,
    );
});

Cypress.Commands.add("typeFast", { prevSubject: "element" }, (subject: JQuery<HTMLElement>, text: string) => {
    cy.wrap(subject).invoke("val", text);
});

Cypress.Commands.add("enterDate", (idPrefix: string, day: string, month: string, year: string) => {
    cy.getById(`${idPrefix}.Day`).typeFast(day);
    cy.getById(`${idPrefix}.Month`).typeFast(month);
    cy.getById(`${idPrefix}.Year`).typeFast(year);
});

Cypress.Commands.add("enterYesNo", (idPrefix: string, option: yesNoOption) => {
    if (option == "Yes") cy.getById(idPrefix).click();
    if (option == "No") cy.getById(`${idPrefix}-2`).click();
});

Cypress.Commands.add("hasAddress", (id: string, line1: string, line2: string, line3: string) => {
    if (line1 === "Empty") {
        cy.getByTestId(id).should("contain.text", "Empty");

        return;
    }

    cy.getByTestId(id).find("[data-testid='address-line1']").should("contain.text", line1);
    cy.getByTestId(id).find("[data-testid='address-line2']").should("contain.text", line2);
    cy.getByTestId(id).find("[data-testid='address-line3']").should("contain.text", line3);
});

Cypress.Commands.add("isInViewport", { prevSubject: true }, (subject) => {
    const rect = subject[0].getBoundingClientRect();

    expect(
        rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= Cypress.config("viewportHeight") &&
            rect.right <= Cypress.config("viewportWidth"),
        "Element was not found in the viewport",
    ).to.be.true;

    return subject;
});

Cypress.Commands.add("revisitCurrentUrl", () => {
    cy.url().then((url: string) => {
        cy.visit(url);
    });
});
