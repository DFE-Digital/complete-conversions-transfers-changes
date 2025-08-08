import HandoverForm from "cypress/pages/projects/handover/handoverForm";

class TransferHandoverForm extends HandoverForm {
    withOutgoingTrustSharePointLink(link: string): this {
        cy.getById("OutgoingTrustSharePointLink").typeFast(link);
        return this;
    }
}

const transferHandoverForm = new TransferHandoverForm();

export default transferHandoverForm;
