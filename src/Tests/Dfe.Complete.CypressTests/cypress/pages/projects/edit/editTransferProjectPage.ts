import EditProjectPage from "cypress/pages/projects/edit/editProjectPage";
import { yesNoOption } from "cypress/constants/stringTestConstants";

class EditTransferProjectPage extends EditProjectPage {
    withOutgoingTrustUKPRN(ukprn: number) {
        cy.getById("OutgoingTrustUkprn").clear().typeFast(String(ukprn));
        return this;
    }

    withOutgoingTrustSharePointLink(link: string) {
        cy.getById("OutgoingTrustSharepointLink").clear().typeFast(link);
        return this;
    }

    withTransferDueToInadequateOfstedRating(option: yesNoOption): this {
        cy.enterYesNo("InadequateOfsted", option);
        return this;
    }

    withTransferDueToFinancialSafeguardingGovernanceIssues(option: yesNoOption): this {
        cy.enterYesNo("FinancialSafeguardingGovernanceIssues", option);
        return this;
    }

    withOutgoingTrustWillCloseAfterTransfer(option: yesNoOption): this {
        cy.enterYesNo("OutgoingTrustToClose", option);
        return this;
    }

}

const editTransferProjectPage = new EditTransferProjectPage();

export default editTransferProjectPage;