import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import { urnPool } from "cypress/constants/testUrns";
import { ProjectType } from "cypress/api/taskApi";
import { ContactCategory, CreateContactRequest, CreateTransferProjectRequest } from "cypress/api/apiDomain";
import contactApi from "cypress/api/contactApi";
import { ContactBuilder } from "cypress/api/contactBuilder";

export interface TransferTasksSetup {
    project: CreateTransferProjectRequest;
    projectId: string;
    taskId: string;
    otherUserProject: CreateTransferProjectRequest;
    otherUserProjectId: string;
    projectType: ProjectType;
    projectWithoutContact?: CreateTransferProjectRequest;
    projectWithoutContactId?: string;
    contact?: CreateContactRequest;
}

export class TransferTasksTestSetup {
    private static instance: TransferTasksSetup;

    public static getSetup(): TransferTasksSetup {
        if (!this.instance) {
            this.instance = {
                project: ProjectBuilder.createTransferProjectRequest({
                    urn: urnPool.transferTasks.coquet,
                }),
                projectId: "",
                taskId: "",
                otherUserProject: ProjectBuilder.createTransferProjectRequest({
                    urn: urnPool.transferTasks.marden,
                }),
                otherUserProjectId: "",
                projectType: ProjectType.Transfer,
                projectWithoutContact: ProjectBuilder.createTransferProjectRequest({
                    urn: urnPool.transferTasks.whitley,
                }),
                projectWithoutContactId: "",
            };
        }
        return this.instance;
    }

    public static setupProjects(): void {
        const setup = this.getSetup();

        projectRemover.removeProjectIfItExists(setup.project.urn);
        projectRemover.removeProjectIfItExists(setup.otherUserProject.urn);

        projectApi.createAndUpdateTransferProject(setup.project).then((createResponse) => {
            setup.projectId = createResponse.value;
            projectApi.getProject(setup.project.urn).then((response) => {
                setup.taskId = response.body.tasksDataId.value;
            });
        });

        projectApi.createAndUpdateTransferProject(setup.otherUserProject, rdoLondonUser).then((createResponse) => {
            setup.otherUserProjectId = createResponse.value;
        });
    }

    public static setupProjectsWithoutTaskId(): void {
        const setup = this.getSetup();

        projectRemover.removeProjectIfItExists(setup.project.urn);
        projectRemover.removeProjectIfItExists(setup.otherUserProject.urn);

        projectApi.createAndUpdateTransferProject(setup.project).then((createResponse) => {
            setup.projectId = createResponse.value;
        });

        projectApi.createAndUpdateTransferProject(setup.otherUserProject, rdoLondonUser).then((createResponse) => {
            setup.otherUserProjectId = createResponse.value;
        });
    }

    public static setupConfirmContactProjects(contact: CreateContactRequest, contactCategory: ContactCategory): void {
        const setup = this.getSetup();
        setup.contact = contact;

        projectRemover.removeProjectIfItExists(setup.project.urn);
        projectRemover.removeProjectIfItExists(setup.projectWithoutContact!.urn);
        projectRemover.removeProjectIfItExists(setup.otherUserProject.urn);

        projectApi.createAndUpdateTransferProject(setup.project).then((createResponse) => {
            setup.projectId = createResponse.value;
            setup.contact!.projectId = { value: setup.projectId };
            contactApi.createContact(setup.contact!);
        });
        projectApi.createAndUpdateTransferProject(setup.otherUserProject, rdoLondonUser).then((createResponse) => {
            setup.otherUserProjectId = createResponse.value;
            contactApi.createContact(
                ContactBuilder.createContactRequest({
                    projectId: { value: setup.otherUserProjectId },
                    category: contactCategory,
                }),
            );
        });
        projectApi.createAndUpdateTransferProject(setup.projectWithoutContact!).then((createResponse) => {
            setup.projectWithoutContactId = createResponse.value;
        });
    }

    public static setupBeforeEach(taskPath: string): void {
        const setup = this.getSetup();
        cy.login();
        cy.acceptCookies();
        cy.visit(`projects/${setup.projectId}/tasks/${taskPath}`);
    }
}
