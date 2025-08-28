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
            advisory_board_date: "2025-08-05",
            advisory_board_conditions: "Default advisory board conditions",
            provisional_conversion_date: "2025-12-01",
            directive_academy_order: false,
            created_by_email: cypressUser.email,
            created_by_first_name: cypressUser.firstName,
            created_by_last_name: cypressUser.lastName,
            prepare_id: Math.floor(Math.random() * 99999) + 1,
            group_id: "",
            incoming_trust_ukprn: dimensionsTrust.ukprn,
            ...options,
        };
    }

    public static createConversionFormAMatProjectRequest(
        options: Partial<CreateConversionFormAMatPrepareRequest> = {},
    ): CreateConversionFormAMatPrepareRequest {
        return {
            urn: 123456,
            advisory_board_date: "2025-08-05",
            advisory_board_conditions: "Default advisory board conditions",
            provisional_conversion_date: "2025-12-01",
            directive_academy_order: false,
            created_by_email: cypressUser.email,
            created_by_first_name: cypressUser.firstName,
            created_by_last_name: cypressUser.lastName,
            prepare_id: Math.floor(Math.random() * 99999) + 1,
            group_id: groupReferenceNumber,
            new_trust_reference_number: dimensionsTrust.referenceNumber,
            new_trust_name: dimensionsTrust.name,
            ...options,
        };
    }

    public static createTransferProjectRequest(
        options: Partial<CreateTransferPrepareRequest> = {},
    ): CreateTransferPrepareRequest {
        return {
            urn: 123456,
            advisory_board_date: "2025-08-05",
            advisory_board_conditions: "Default advisory board conditions",
            provisional_transfer_date: "2025-12-01",
            created_by_email: cypressUser.email,
            created_by_first_name: cypressUser.firstName,
            created_by_last_name: cypressUser.lastName,
            inadequate_ofsted: false,
            financial_safeguarding_governance_issues: false,
            outgoing_trust_ukprn: macclesfieldTrust.ukprn,
            outgoing_trust_to_close: false,
            prepare_id: Math.floor(Math.random() * 99999) + 1,
            group_id: "",
            incoming_trust_ukprn: dimensionsTrust.ukprn,
            ...options,
        };
    }

    public static createTransferFormAMatProjectRequest(
        options: Partial<CreateTransferFormAMatPrepareRequest> = {},
    ): CreateTransferFormAMatPrepareRequest {
        return {
            urn: 123456,
            advisory_board_date: "2025-08-05",
            advisory_board_conditions: "Default advisory board conditions",
            provisional_transfer_date: "2025-12-01",
            created_by_email: cypressUser.email,
            created_by_first_name: cypressUser.firstName,
            created_by_last_name: cypressUser.lastName,
            inadequate_ofsted: false,
            financial_safeguarding_governance_issues: false,
            outgoing_trust_ukprn: macclesfieldTrust.ukprn,
            outgoing_trust_to_close: false,
            prepare_id: Math.floor(Math.random() * 99999) + 1,
            group_id: groupReferenceNumber,
            new_trust_reference_number: dimensionsTrust.referenceNumber,
            new_trust_name: dimensionsTrust.name,
            ...options,
        };
    }
}
