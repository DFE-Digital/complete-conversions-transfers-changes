class ValidationComponent {
    public hasValidationError(message: string): this {
        cy.get(".govuk-error-summary").should("contain.text", message);
        return this;
    }

    public hasNoValidationErrors(): this {
        cy.get(".govuk-error-summary").should("not.exist");
        return this;
    }

    public hasErrorMessage(message: string): this {
        cy.getByClass("govuk-error-message").should("contain.text", message);
        return this;
    }

    public hasLinkedValidationError(message: string): this {
        cy.get(".govuk-error-summary")
            .contains(message)
            .parent()
            .find("a")
            .invoke("attr", "href")
            .then((href: string | undefined) => {
                if (href && href.includes(".")) {
                    // example href="#AdvisoryBoardDate.Day" -> id=AdvisoryBoardDate-error
                    href = href.split(".")[0];
                }
                cy.get(href as string).should("exist");
                cy.get((href as string) + "-error").should("contain.text", message);
            });

        return this;
    }

    public hasLinkedValidationErrorForField(fieldId: string, message: string): this {
        cy.get(`.govuk-error-summary a[href="#${fieldId}"]`)
            .contains(message)
            .should("exist")
            .invoke("attr", "href")
            .then((href: string | undefined) => {
                cy.get(href as string).should("exist");
                cy.get((href as string) + "-error").should("contain.text", message);
            });

        return this;
    }
}

const validationComponent = new ValidationComponent();
export default validationComponent;
