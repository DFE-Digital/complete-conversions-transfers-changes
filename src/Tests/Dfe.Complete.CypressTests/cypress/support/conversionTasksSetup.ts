import { ProjectBuilder } from "cypress/api/projectBuilder";
import projectApi from "cypress/api/projectApi";
import projectRemover from "cypress/api/projectRemover";
import { rdoLondonUser } from "cypress/constants/cypressConstants";
import { urnPool } from "cypress/constants/testUrns";
import { ProjectType } from "cypress/api/taskApi";
import {
    ContactCategory,
    CreateContactRequest,
    CreateConversionProjectRequest,
    CreateMatConversionProjectRequest,
} from "cypress/api/apiDomain";
import contactApi from "cypress/api/contactApi";
import { ContactBuilder } from "cypress/api/contactBuilder";

export interface ConversionTasksSetup {
    project: CreateConversionProjectRequest;
    projectId: string;
    taskId: string;
    otherUserProject: CreateMatConversionProjectRequest;
    otherUserProjectId: string;
    projectType: ProjectType;
    projectWithoutContact?: CreateConversionProjectRequest;
    projectWithoutContactId?: string;
    contact?: CreateContactRequest;
}

export class ConversionTasksTestSetup {
    private static instance: ConversionTasksSetup;

    public static getSetup(): ConversionTasksSetup {
        if (!this.instance) {
            this.instance = {
                project: ProjectBuilder.createConversionProjectRequest({
                    urn: urnPool.conversionTasks.spen,
                }),
                projectId: "",
                taskId: "",
                otherUserProject: ProjectBuilder.createConversionFormAMatProjectRequest({
                    urn: urnPool.conversionTasks.grylls,
                }),
                otherUserProjectId: "",
                projectType: ProjectType.Conversion,
                projectWithoutContact: ProjectBuilder.createConversionProjectRequest({
                    urn: urnPool.conversionTasks.huddersfield,
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

        projectApi.createAndUpdateConversionProject(setup.project).then((createResponse) => {
            setup.projectId = createResponse.value;
            projectApi.getProject(setup.project.urn).then((response) => {
                setup.taskId = response.body.tasksDataId.value;
            });
        });

        projectApi.createAndUpdateMatConversionProject(setup.otherUserProject, rdoLondonUser).then((createResponse) => {
            setup.otherUserProjectId = createResponse.value;
        });
    }

    public static setupProjectsWithoutTaskId(): void {
        const setup = this.getSetup();

        projectRemover.removeProjectIfItExists(setup.project.urn);
        projectRemover.removeProjectIfItExists(setup.otherUserProject.urn);

        projectApi.createAndUpdateConversionProject(setup.project).then((createResponse) => {
            setup.projectId = createResponse.value;
        });

        projectApi.createAndUpdateMatConversionProject(setup.otherUserProject, rdoLondonUser).then((createResponse) => {
            setup.otherUserProjectId = createResponse.value;
        });
    }

    public static setupConfirmContactProjects(contact: CreateContactRequest, contactCategory: ContactCategory): void {
        const setup = this.getSetup();
        setup.contact = contact;

        projectRemover.removeProjectIfItExists(setup.project.urn);
        projectRemover.removeProjectIfItExists(setup.projectWithoutContact!.urn);
        projectRemover.removeProjectIfItExists(setup.otherUserProject.urn);

        projectApi.createAndUpdateConversionProject(setup.project).then((createResponse) => {
            setup.projectId = createResponse.value;
            setup.contact!.projectId = { value: setup.projectId };
            contactApi.createContact(setup.contact!);
        });
        projectApi.createAndUpdateMatConversionProject(setup.otherUserProject, rdoLondonUser).then((createResponse) => {
            setup.otherUserProjectId = createResponse.value;
            contactApi.createContact(
                ContactBuilder.createContactRequest({
                    projectId: { value: setup.otherUserProjectId },
                    category: contactCategory,
                }),
            );
        });
        projectApi.createAndUpdateConversionProject(setup.projectWithoutContact!).then((createResponse) => {
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
