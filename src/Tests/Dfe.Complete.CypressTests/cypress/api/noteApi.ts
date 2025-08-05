import { ApiBase } from "cypress/api/apiBase";
import { EnvApi } from "cypress/constants/cypressConstants";

interface NoteIdObject {
    value: string;
}

interface NoteCreateRequest {
    projectId: NoteIdObject;
    userId: NoteIdObject;
    body: string;
    taskIdentifier?: string;
}

interface NoteDeleteRequest {
    noteId: NoteIdObject;
}

export interface NoteResponse {
    value: string;
}

export interface NoteDetail {
    id: { value: string };
    body: string;
    projectId: { value: string };
    userId: { value: string };
    userFullName: string;
    createdAt: string;
}

export type NotesResponse = NoteDetail[];

class NoteApi extends ApiBase {
    private readonly noteUrl = `${Cypress.env(EnvApi)}/v1/Projects/Notes`;

    public removeAllNotesForProject(projectId: string) {
        return this.getProjectNotes(projectId).then((response) => {
            const noteIds: string[] = response.map((note: NoteDetail) => note.id.value);
            noteIds.forEach((id) => {
                this.deleteNote(id);
            });
        });
    }

    public getProjectNotes(projectId: string): Cypress.Chainable<NotesResponse> {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<NotesResponse>({
                    method: "GET",
                    url: `${this.noteUrl}?ProjectId.Value=${projectId}`,
                    headers,
                })
                .then((response) => {
                    expect(response.status, `Expected GET request to return 200 but got ${response.status}`).to.eq(200);
                    return response.body;
                });
        });
    }

    public createNote(projectId: string, userId: string, note: string, taskIdentifier?: string) {
        const requestBody: NoteCreateRequest = {
            projectId: { value: projectId },
            userId: { value: userId },
            body: note,
        };
        if (taskIdentifier) {
            requestBody.taskIdentifier = taskIdentifier;
        }
        return this.noteBaseRequest<NoteResponse>("POST", this.noteUrl, requestBody, 201);
    }

    public deleteNote(noteId: string) {
        const requestBody: NoteDeleteRequest = {
            noteId: { value: noteId },
        };

        return this.noteBaseRequest<void>("DELETE", this.noteUrl, requestBody, 200); // bug should be 204
    }

    private noteBaseRequest<T>(
        method: string,
        url: string,
        body: NoteCreateRequest | NoteDeleteRequest,
        expectedStatus: number,
    ) {
        return this.authenticatedRequest().then((headers) => {
            return cy
                .request<T>({
                    method,
                    url,
                    headers,
                    body,
                })
                .then((response) => {
                    expect(
                        response.status,
                        `Expected ${method} request to return ${expectedStatus} but got ${response.status}`,
                    ).to.eq(expectedStatus);
                    return response.body;
                });
        });
    }
}

const noteApi = new NoteApi();

export default noteApi;
