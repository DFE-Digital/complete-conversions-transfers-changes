import {CreateProjectRequest} from "./apiDomain";

export class ProjectBuilder {

    public static createConversionProjectRequest(urn? : number): CreateProjectRequest {
        const today = new Date();
        const nextMonth = new Date(today.setMonth(today.getMonth() + 1));
        const significantDate = nextMonth.toISOString().split('T')[0];
        const urnValue = urn ? urn : 103844;

        return {
            urn: { value: urnValue },
            significantDate: significantDate,
            isSignificantDateProvisional: true,
            incomingTrustUkprn: {
                value: 10058682,
            },
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2025-02-18",
            advisoryBoardConditions: "test",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
            groupReferenceNumber: "GRP_00000006",
            handingOverToRegionalCaseworkService: false,
            handoverComments: "test 2",
            userAdId: Cypress.env("userAdId"),
        };
    }

    public static createTransferProjectRequest(): CreateProjectRequest {
        return {
            urn: { value: 142277 },
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            incomingTrustUkprn: { value: 10058502 },
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com/school",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com/incoming",
            userAdId: Cypress.env("userAdId"),
        };
    }

    public static createConversionFormAMatProjectRequest(): CreateProjectRequest {
        return {
            urn: { value: 149149 },
            significantDate: "2025-02-01",
            isSignificantDateProvisional: false,
            incomingTrustUkprn: { value: 10058682 },
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2025-02-18",
            advisoryBoardConditions: "test",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
            groupReferenceNumber: "GRP_00000006",
            handingOverToRegionalCaseworkService: false,
            handoverComments: "test 2",
            userAdId: Cypress.env("userAdId"),
        };
    }

    public static createTransferFormAMatProjectRequest(): CreateProjectRequest {
        return {
            urn: { value: 136732 },
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            incomingTrustUkprn: { value: 10058502 },
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com/school",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com/incoming",
            userAdId: Cypress.env("userAdId"),
        };
    }

}
