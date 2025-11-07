import ProjectTable from "cypress/pages/projects/tables/projectTable";

class UsersPage extends ProjectTable {
    editUser(userName: string) {
        this.clickButtonInRow(userName, "Edit user");
        return this;
    }

    withUser(username: string) {
        return this.withSchool(username);
    }
}

const usersPage = new UsersPage();

export default usersPage;
