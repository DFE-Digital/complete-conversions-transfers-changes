import BasePage from "cypress/pages/basePage";
import { yesNoOption } from "cypress/constants/stringTestConstants";

class HandoverForm extends BasePage {
    withWillRCSManageProject(option: yesNoOption): this {
        cy.enterYesNo("AssignedToRegionalCaseworkerTeam", option);
        return this;
    }

    withHandoverComments(text: string): this {
        cy.getById("HandoverComments").typeFast(text);
        return this;
    }

    withSchoolSharePointLink(link: string): this {
        cy.getById("SchoolSharePointLink").typeFast(link);
        return this;
    }

    withIncomingTrustSharePointLink(link: string): this {
        cy.getById("IncomingTrustSharePointLink").typeFast(link);
        return this;
    }

    confirm(): this {
        this.clickButton("Confirm");
        return this;
    }
}

export default HandoverForm;
