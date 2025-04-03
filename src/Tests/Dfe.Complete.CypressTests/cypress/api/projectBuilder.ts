import { CreateProjectRequest } from "./apiDomain";
import { EnvUserAdId } from "../constants/cypressConstants";
import { groupReferenceNumber, ukprn } from "cypress/constants/stringTestConstants";

export class ProjectBuilder {
    public static createConversionProjectRequest(
        significantDate: Date,
        urn?: number,
        userAdId?: string,
    ): CreateProjectRequest {
        const significantDateFormatted = significantDate.toISOString().split("T")[0];
        const urnValue = urn ? urn : 103844;
        const userAdIdValue = userAdId ? userAdId : Cypress.env(EnvUserAdId);

        return {
            urn: { value: urnValue },
            significantDate: significantDateFormatted,
            isSignificantDateProvisional: true,
            incomingTrustUkprn: {
                value: ukprn,
            },
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2025-02-18",
            advisoryBoardConditions: "test",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
            groupReferenceNumber: groupReferenceNumber,
            handingOverToRegionalCaseworkService: false,
            handoverComments: "test 2",
            userAdId: userAdIdValue,
        };
    }

    public static createTransferProjectRequest(): CreateProjectRequest {
        return {
            urn: { value: 105601 },
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            incomingTrustUkprn: { value: 10058502 },
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com/school",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com/incoming",
            userAdId: Cypress.env(EnvUserAdId),
        };
    }

    public static createConversionFormAMatProjectRequest(): CreateProjectRequest {
        return {
            urn: { value: 149149 },
            significantDate: "2025-02-01",
            isSignificantDateProvisional: false,
            incomingTrustUkprn: { value: ukprn },
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2025-02-18",
            advisoryBoardConditions: "test",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
            groupReferenceNumber: groupReferenceNumber,
            handingOverToRegionalCaseworkService: false,
            handoverComments: "test 2",
            userAdId: Cypress.env(EnvUserAdId),
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
            userAdId: Cypress.env(EnvUserAdId),
        };
    }
}
