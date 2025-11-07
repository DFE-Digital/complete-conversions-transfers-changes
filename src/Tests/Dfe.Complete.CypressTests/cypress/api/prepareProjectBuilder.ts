import {
    CreateConversionFormAMatPrepareRequest,
    CreateConversionPrepareRequest,
    CreateTransferFormAMatPrepareRequest,
    CreateTransferPrepareRequest,
} from "cypress/api/apiDomain";
import { dimensionsTrust, groupReferenceNumber, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import { cypressUser } from "cypress/constants/cypressConstants";

export class PrepareProjectBuilder {
    public static createConversionProjectRequest(
        options: Partial<CreateConversionPrepareRequest> = {},
    ): CreateConversionPrepareRequest {
        return {
            urn: 123456,
            incomingTrustUkprn: dimensionsTrust.ukprn,
            advisoryBoardDate: "2025-08-05",
            advisoryBoardConditions: "Default advisory board conditions",
            provisionalConversionDate: "2025-12-01",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: Math.floor(Math.random() * 99999) + 1,
            directiveAcademyOrder: false,
            groupId: null,
            ...options,
        };
    }

    public static createConversionFormAMatProjectRequest(
        options: Partial<CreateConversionFormAMatPrepareRequest> = {},
    ): CreateConversionFormAMatPrepareRequest {
        return {
            urn: 123456,
            advisoryBoardDate: "2025-08-05",
            advisoryBoardConditions: "Default advisory board conditions",
            provisionalConversionDate: "2026-04-01",
            directiveAcademyOrder: false,
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            prepareId: Math.floor(Math.random() * 99999) + 1,
            newTrustReferenceNumber: dimensionsTrust.referenceNumber,
            newTrustName: dimensionsTrust.name,
            ...options,
        };
    }

    public static createTransferProjectRequest(
        options: Partial<CreateTransferPrepareRequest> = {},
    ): CreateTransferPrepareRequest {
        return {
            urn: 123456,
            advisoryBoardDate: "2025-08-05",
            advisoryBoardConditions: "Default advisory board conditions",
            provisionalTransferDate: "2025-12-01",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            inadequateOfsted: false,
            financialSafeguardingGovernanceIssues: false,
            outgoingTrustUkprn: macclesfieldTrust.ukprn,
            outgoingTrustToClose: false,
            prepareId: Math.floor(Math.random() * 99999) + 1,
            groupId: null,
            incomingTrustUkprn: dimensionsTrust.ukprn,
            ...options,
        };
    }

    public static createTransferFormAMatProjectRequest(
        options: Partial<CreateTransferFormAMatPrepareRequest> = {},
    ): CreateTransferFormAMatPrepareRequest {
        return {
            urn: 123456,
            advisoryBoardDate: "2025-08-05",
            advisoryBoardConditions: "Default advisory board conditions",
            provisionalTransferDate: "2025-12-01",
            createdByEmail: cypressUser.email,
            createdByFirstName: cypressUser.firstName,
            createdByLastName: cypressUser.lastName,
            inadequateOfsted: false,
            financialSafeguardingGovernanceIssues: false,
            outgoingTrustUkprn: macclesfieldTrust.ukprn,
            outgoingTrustToClose: false,
            prepareId: Math.floor(Math.random() * 99999) + 1,
            groupId: groupReferenceNumber,
            newTrustReferenceNumber: dimensionsTrust.referenceNumber,
            newTrustName: dimensionsTrust.name,
            ...options,
        };
    }
}
