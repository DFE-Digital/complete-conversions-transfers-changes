import HandoverForm from "cypress/pages/projects/handover/handoverForm";
import { yesNoOption } from "cypress/constants/stringTestConstants";

class ConversionHandoverForm extends HandoverForm {
    withDueToInterventionFollowing2RI(option: yesNoOption): this {
        cy.enterYesNo("TwoRequiresImprovement", option);
        return this;
    }
}

const conversionHandoverForm = new ConversionHandoverForm();

export default conversionHandoverForm;
