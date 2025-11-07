import {
    CreateConversionProjectRequest,
    CreateMatConversionProjectRequest,
    CreateMatTransferProjectRequest,
    CreateTransferProjectRequest,
} from "./apiDomain";
import { dimensionsTrust, groupReferenceNumber, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import { cypressUser } from "cypress/constants/cypressConstants";
import { getSignificantDateString } from "cypress/support/formatDate";

export class ProjectBuilder {
    public static createConversionProjectRequest(): CreateConversionProjectRequest {
        return {
            urn: 0, // specify a valid URN in request options
            incomingTrustUkprn: macclesfieldTrust.ukprn,
            provisionalConversionDate: "2025-02-18",
            advisoryBoardDate: "2025-02-18",
            advisoryBoardConditions: "test",
            groupId: groupReferenceNumber,
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: 1,
            directiveAcademyOrder: false,
        };
    }

    public static createTransferProjectRequest(): CreateTransferProjectRequest {
        return {
            urn: 0, // specify a valid URN in request options
            outgoingTrustUkprn: macclesfieldTrust.ukprn,
            incomingTrustUkprn: dimensionsTrust.ukprn,
            inadequateOfsted: false,
            outgoingTrustToClose: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "test",
            groupId: groupReferenceNumber,
            provisionalTransferDate: "2023-05-01",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: 1,
            financialSafeguardingGovernanceIssues: false,
        };
    }

    public static createConversionFormAMatProjectRequest(): CreateMatConversionProjectRequest {
        return {
            urn: 0, // specify a valid URN in request options
            newTrustName: macclesfieldTrust.name,
            newTrustReferenceNumber: macclesfieldTrust.referenceNumber,
            directiveAcademyOrder: false,
            advisoryBoardDate: "2023-05-01",
            provisionalConversionDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: 1
        };
    }

    public static createTransferFormAMatProjectRequest(): CreateMatTransferProjectRequest {
        return {
            urn: 0, // specify a valid URN in request options
            newTrustName: dimensionsTrust.name,
            newTrustReferenceNumber: dimensionsTrust.referenceNumber,
            outgoingTrustUkprn: macclesfieldTrust.ukprn,
            provisionalTransferDate: "2026-03-01",
            inadequateOfsted: false,
            financialSafeguardingGovernanceIssues: false,
            outgoingTrustToClose: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: 1
        };
    }
}
