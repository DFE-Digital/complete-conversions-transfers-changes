class NavBar {
    private readonly navHeaderId = 'header-navigation';
    goToAllProjects(){
        cy.getById(this.navHeaderId).contains('All projects').click();
        return this;
    }


}

const navBar = new NavBar();

export default navBar;