import { CreateConversionProjectRequest, CreateTransferProjectRequest, Region } from "./apiDomain";

export class ProjectBuilder {
    public static createTransferProjectRequest(): CreateTransferProjectRequest {
        return {
            urn: "142277",
            region: Region.WestMidlands,
            schoolSharePointLink: "https://educationgovuk.sharepoint.com/school",
            advisoryBoardDetails: {
                date: "2022-01-01",
                conditions: "Conditions",
            },
            date: "2026-03-01",
            isDateProvisional: true,
            isDueTo2RI: true,
            isDueToIssues: true,
            isDueToOfstedRating: true,
            incomingTrustDetails: {
                sharepointLink: "https://educationgovuk.sharepoint.com/incoming",
                ukprn: "10058502",
            },
            outgoingTrustDetails: {
                sharepointLink: "https://educationgovuk.sharepoint.com/outgoing",
                ukprn: "10061008",
            },
        };
    }

    public static createConversionProjectRequest(): CreateConversionProjectRequest {
        return {
            urn: { value: 103844 },
            significantDate: "2025-02-18",
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
            groupReferenceNumber: "GRP_12345670",
            handingOverToRegionalCaseworkService: false,
            handoverComments: "test 2",
            userAdId: Cypress.env("userAdId"),
        };
    }
}
