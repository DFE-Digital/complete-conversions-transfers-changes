import EditProjectPage from "cypress/pages/projects/edit/editProjectPage";

class EditConversionProjectPage extends EditProjectPage {
    public withAcademyOrder(option: "Directive academy order" | "Academy order"): this {
        if (option == "Directive academy order") cy.getById("DirectiveAcademyOrder").click();
        if (option == "Academy order") cy.getById("DirectiveAcademyOrder-2").click();
        return this;
    }


}

const editConversionProjectPage = new EditConversionProjectPage();

export default editConversionProjectPage;