import BasePage from "cypress/pages/basePage";
import { yesNoOption } from "cypress/constants/stringTestConstants";

class EditProjectPage extends BasePage {
    withIncomingTrustUKPRN(ukprn: number): this {
        cy.getById("IncomingTrustUkprn").clear().typeFast(String(ukprn));
        return this;
    }

    withTrustReferenceNumber(trn: string): this {
        cy.getById("NewTrustReferenceNumber").clear().typeFast(trn);
        return this;
    }

    withGroupReferenceNumber(refNumber: string): this {
        cy.getById("GroupReferenceNumber").clear().typeFast(refNumber);
        return this;
    }

    withAdvisoryBoardDate(day: string, month: string, year: string): this {
        cy.enterDate("AdvisoryBoardDate", day, month, year);
        return this;
    }

    withAdvisoryBoardConditions(text: string): this {
        cy.getById("AdvisoryBoardConditions").clear().typeFast(text);
        return this;
    }

    withSchoolOrAcademySharePointLink(link: string): this {
        cy.getById("EstablishmentSharepointLink").clear().typeFast(link);
        return this;
    }

    withIncomingTrustSharePointLink(link: string): this {
        cy.getById("IncomingTrustSharepointLink").clear().typeFast(link);
        return this;
    }

    withHandingOverToRCS(option: yesNoOption): this {
        cy.enterYesNo("IsHandingToRCS", option);
        return this;
    }

    withHandoverComments(text: string): this {
        cy.getById("HandoverComments").clear().typeFast(text);
        return this;
    }

    with2RI(option: yesNoOption): this {
        cy.enterYesNo("TwoRequiresImprovement", option);
        return this;
    }

    continue(): this {
        this.clickButton("Continue");
        return this;
    }
}

export default EditProjectPage;
