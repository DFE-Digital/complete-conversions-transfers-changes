import {
    CreateConversionProjectRequest,
    CreateMatConversionProjectRequest,
    CreateMatTransferProjectRequest,
    CreateTransferProjectRequest,
} from "./apiDomain";
import { dimensionsTrust, groupReferenceNumber, macclesfieldTrust } from "cypress/constants/stringTestConstants";
import { cypressUser } from "cypress/constants/cypressConstants";

export class ProjectBuilder {
    public static createConversionProjectRequest(
        significantDate: Date,
        urn?: number,
        userAdId?: string,
    ): CreateConversionProjectRequest {
        // force significant date to be first day of the month
        significantDate.setDate(1);
        const significantDateFormatted = significantDate.toISOString().split("T")[0];
        const urnValue = urn ?? 103844;
        const userAdIdValue = userAdId ?? cypressUser.adId;

        return {
            urn: { value: urnValue },
            significantDate: significantDateFormatted,
            isSignificantDateProvisional: false,
            incomingTrustUkprn: {
                value: macclesfieldTrust.ukprn,
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

    public static createTransferProjectRequest(
        options: Partial<CreateTransferProjectRequest> = {},
    ): CreateTransferProjectRequest {
        return {
            urn: { value: 105601 },
            outgoingTrustUkprn: { value: macclesfieldTrust.ukprn },
            incomingTrustUkprn: { value: dimensionsTrust.ukprn },
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            isDueTo2Ri: false,
            isDueToInedaquateOfstedRating: false,
            isDueToIssues: false,
            outGoingTrustWillClose: false,
            handingOverToRegionalCaseworkService: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "test",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com/1",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com/2",
            outgoingTrustSharepointLink: "https://educationgovuk.sharepoint.com/3",
            groupReferenceNumber: groupReferenceNumber,
            handoverComments: "test 2",
            userAdId: cypressUser.adId,
            ...options,
        };
    }

    public static createConversionFormAMatProjectRequest(
        options: Partial<CreateMatConversionProjectRequest> = {},
    ): CreateMatConversionProjectRequest {
        return {
            urn: { value: 147800 },
            newTrustName: macclesfieldTrust.name,
            newTrustReferenceNumber: macclesfieldTrust.referenceNumber,
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com/1",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com/2",
            handingOverToRegionalCaseworkService: false,
            handoverComments: "test 2",
            userAdId: cypressUser.adId,
            ...options,
        };
    }

    public static createTransferFormAMatProjectRequest(
        options: Partial<CreateMatTransferProjectRequest> = {},
    ): CreateMatTransferProjectRequest {
        return {
            urn: { value: 149460 },
            newTrustName: dimensionsTrust.name,
            newTrustReferenceNumber: dimensionsTrust.referenceNumber,
            outgoingTrustUkprn: { value: macclesfieldTrust.ukprn },
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            isDueTo2Ri: false,
            isDueToInedaquateOfstedRating: false,
            isDueToIssues: false,
            handingOverToRegionalCaseworkService: false,
            outGoingTrustWillClose: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com/1",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com/2",
            outgoingTrustSharepointLink: "https://educationgovuk.sharepoint.com/3",
            handoverComments: "test 2",
            userAdId: cypressUser.adId,
            ...options,
        };
    }
}
