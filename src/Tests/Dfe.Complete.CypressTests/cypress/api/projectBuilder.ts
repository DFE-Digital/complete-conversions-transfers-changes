import {
    CreateConversionProjectRequest,
    CreateMatConversionProjectRequest,
    CreateMatTransferProjectRequest,
    CreateTransferProjectRequest,
} from "./apiDomain";
import {
    groupReferenceNumber,
    testTrustName,
    testTrustReferenceNumber,
    ukprn,
    ukprn2,
} from "cypress/constants/stringTestConstants";
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

    public static createTransferProjectRequest(
        options: Partial<CreateTransferProjectRequest> = {},
    ): CreateTransferProjectRequest {
        return {
            urn: { value: 105601 },
            outgoingTrustUkprn: { value: ukprn },
            incomingTrustUkprn: { value: ukprn2 },
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            isDueTo2Ri: false,
            isDueToInedaquateOfstedRating: false,
            isDueToIssues: false,
            outGoingTrustWillClose: false,
            handingOverToRegionalCaseworkService: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "test",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
            outgoingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
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
            newTrustName: testTrustName,
            newTrustReferenceNumber: testTrustReferenceNumber,
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            isDueTo2Ri: false,
            hasAcademyOrderBeenIssued: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
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
            newTrustName: testTrustName,
            newTrustReferenceNumber: testTrustReferenceNumber,
            outgoingTrustUkprn: { value: ukprn },
            significantDate: "2026-03-01",
            isSignificantDateProvisional: false,
            isDueTo2Ri: false,
            isDueToInedaquateOfstedRating: false,
            isDueToIssues: false,
            handingOverToRegionalCaseworkService: false,
            outGoingTrustWillClose: false,
            advisoryBoardDate: "2023-05-01",
            advisoryBoardConditions: "none.",
            establishmentSharepointLink: "https://educationgovuk.sharepoint.com",
            incomingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
            outgoingTrustSharepointLink: "https://educationgovuk.sharepoint.com",
            handoverComments: "test 2",
            userAdId: cypressUser.adId,
            ...options,
        };
    }
}
