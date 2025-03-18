import NewProjectPage from "./newProjectPage";
import {yesNoOption} from "../../../constants/stringTestConstants";

class NewTransferPage extends NewProjectPage {
    public withAcademyURN(urn: string): this {
        cy.getById("URN").typeFast(urn);
        return this;
    }

    public withOutgoingTrustUKPRN(ukprn: string): this {
        cy.getById("OutgoingUKPRN").typeFast(ukprn);
        return this;
    }

    public withIncomingTrustUKPRN (ukprn: string): this {
        cy.getById("IncomingUKPRN").typeFast(ukprn);
        return this;
    }

    public withAcademySharepointLink(link: string): this {
        cy.getById("AcademySharePointLink").typeFast(link);
        return this;
    }

    public withOutgoingTrustSharepointLink(link: string): this {
        cy.getById("OutgoingTrustSharePointLink").typeFast(link);
        return this;
    }

    public withProvisionalTransferDate(day: string, month: string, year: string): this {
        cy.enterDate("SignificantDate", day, month, year);
        return this;
    }

    public withTransferDueToInadequateOfstedRating(option: yesNoOption): this {
        cy.enterYesNo("IsDueToInedaquateOfstedRating", option);
        return this;
    }

    public withTransferDueToFinancialSafeguardingGovernanceIssues(option: yesNoOption): this {
        cy.enterYesNo("IsDueToIssues", option);
        return this;
    }

    public withOutgoingTrustWillCloseAfterTransfer(option: yesNoOption): this {
        cy.enterYesNo("OutgoingTrustWillClose", option);
        return this;
    }
}

const newTransferPage = new NewTransferPage();

export default newTransferPage;