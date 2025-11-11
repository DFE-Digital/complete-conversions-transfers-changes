import {
    CreateConversionProjectRequest,
    CreateMatConversionProjectRequest,
    CreateMatTransferProjectRequest,
    CreateTransferProjectRequest,
    UpdateProjectHandoverAssignRequest,
} from "./apiDomain";
import { dimensionsTrust, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import { cypressUser } from "cypress/constants/cypressConstants";

export class ProjectBuilder {
    public static createConversionProjectRequest(
        options: Partial<CreateConversionProjectRequest> = {},
    ): CreateConversionProjectRequest {
        return {
            urn: 0, // specify a valid URN in request options
            incomingTrustUkprn: macclesfieldTrust.ukprn,
            provisionalConversionDate: "2027-03-01",
            advisoryBoardDate: "2025-02-18",
            advisoryBoardConditions: "test",
            groupId: null,
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: 1,
            directiveAcademyOrder: false,
            ...options,
        };
    }

    public static createTransferProjectRequest(
        options: Partial<CreateTransferProjectRequest> = {},
    ): CreateTransferProjectRequest {
        return {
            urn: 0, // specify a valid URN in request options
            outgoingTrustUkprn: macclesfieldTrust.ukprn,
            incomingTrustUkprn: dimensionsTrust.ukprn,
            inadequateOfsted: false,
            outgoingTrustToClose: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "test",
            groupId: null,
            provisionalTransferDate: "2027-05-01",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: 1,
            financialSafeguardingGovernanceIssues: false,
            ...options,
        };
    }

    public static createConversionFormAMatProjectRequest(
        options: Partial<CreateMatConversionProjectRequest> = {},
    ): CreateMatConversionProjectRequest {
        return {
            urn: 0, // specify a valid URN in request options
            newTrustName: macclesfieldTrust.name,
            newTrustReferenceNumber: macclesfieldTrust.referenceNumber,
            directiveAcademyOrder: false,
            advisoryBoardDate: "2023-05-01",
            provisionalConversionDate: "2027-05-01",
            advisoryBoardConditions: "none.",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: 1,
            ...options,
        };
    }

    public static createTransferFormAMatProjectRequest(
        options: Partial<CreateMatTransferProjectRequest> = {},
    ): CreateMatTransferProjectRequest {
        return {
            urn: 0, // specify a valid URN in request options
            newTrustName: dimensionsTrust.name,
            newTrustReferenceNumber: dimensionsTrust.referenceNumber,
            outgoingTrustUkprn: macclesfieldTrust.ukprn,
            provisionalTransferDate: "2027-03-01",
            inadequateOfsted: false,
            financialSafeguardingGovernanceIssues: false,
            outgoingTrustToClose: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: 1,
            ...options,
        };
    }

    public static updateConversionProjectHandoverAssignRequest(
        options: Partial<UpdateProjectHandoverAssignRequest> = {},
    ): UpdateProjectHandoverAssignRequest {
        return {
            projectId: { value: "" }, // specify a valid projectId in request options
            schoolSharepointLink: "https://educationgovuk.sharepoint.com/1",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com/2",
            assignedToRegionalCaseworkerTeam: false,
            handoverComments: "Default handover comments",
            userId: { value: cypressUser.id },
            userTeam: "London",
            twoRequiresImprovement: false,
            ...options,
        };
    }

    public static updateTransferProjectHandoverAssignRequest(
        options: Partial<UpdateProjectHandoverAssignRequest> = {},
    ): UpdateProjectHandoverAssignRequest {
        return {
            projectId: { value: "" }, // specify a valid projectId in request options
            schoolSharepointLink: "https://educationgovuk.sharepoint.com/1",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com/2",
            outgoingTrustSharepointLink: "https://educationgovuk.sharepoint.com/3",
            assignedToRegionalCaseworkerTeam: false,
            handoverComments: "Default handover comments",
            userId: { value: cypressUser.id },
            userTeam: "London",
            twoRequiresImprovement: false,
            ...options,
        };
    }
}
