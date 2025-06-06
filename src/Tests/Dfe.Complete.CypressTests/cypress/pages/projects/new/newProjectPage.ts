import { yesNoOption } from "cypress/constants/stringTestConstants";
import BasePage from "cypress/pages/basePage";

class NewProjectPage extends BasePage {
    public withTrustReferenceNumber(trn: string): this {
        cy.getById("TrustReferenceNumber").typeFast(trn);
        return this;
    }

    public withGroupReferenceNumber(grp: string): this {
        cy.getById("GroupReferenceNumber").typeFast(grp);
        return this;
    }

    public withAdvisoryBoardDate(day: string, month: string, year: string): this {
        cy.enterDate("AdvisoryBoardDate", day, month, year);

        return this;
    }

    public withAdvisoryBoardConditions(text: string): this {
        cy.getById("AdvisoryBoardConditions").typeFast(text);
        return this;
    }

    public withIncomingTrustSharePointLink(link: string): this {
        cy.getById("IncomingTrustSharePointLink").typeFast(link);
        return this;
    }

    public withHandingOverToRCS(option: yesNoOption): this {
        cy.enterYesNo("IsHandingToRCS", option);
        return this;
    }

    public with2RI(option: yesNoOption): this {
        cy.enterYesNo("IsDueTo2RI", option);
        return this;
    }

    public withHandoverComments(text: string): this {
        cy.getById("HandoverComments").typeFast(text);
        return this;
    }

    public withTrustName(name: string): this {
        cy.getById("TrustName").typeFast(name);
        return this;
    }

    public continue(): this {
        this.clickButton("Continue");
        return this;
    }
}

export default NewProjectPage;
