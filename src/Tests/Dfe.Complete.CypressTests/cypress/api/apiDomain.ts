export type CreateConversionProjectRequest = {
    urn: { value: number };
    significantDate: string;
    isSignificantDateProvisional: boolean;
    incomingTrustUkprn: { value: number };
    isDueTo2Ri: boolean;
    hasAcademyOrderBeenIssued: boolean;
    advisoryBoardDate: string;
    advisoryBoardConditions: string;
    establishmentSharepointLink: string;
    incomingTrustSharepointLink: string;
    groupReferenceNumber?: string;
    handingOverToRegionalCaseworkService?: boolean;
    handoverComments?: string;
    userAdId?: string;
};

export type CreateTransferProjectRequest = {
    urn: { value: number };
    outgoingTrustUkprn: { value: number };
    incomingTrustUkprn: { value: number };
    significantDate: string;
    isSignificantDateProvisional: boolean;
    isDueTo2Ri: boolean;
    isDueToInedaquateOfstedRating: boolean;
    isDueToIssues: boolean;
    outGoingTrustWillClose: boolean;
    handingOverToRegionalCaseworkService?: boolean;
    advisoryBoardDate: string;
    advisoryBoardConditions: string;
    establishmentSharepointLink: string;
    incomingTrustSharepointLink: string;
    outgoingTrustSharepointLink: string;
    groupReferenceNumber?: string;
    handoverComments?: string;
    userAdId?: string;
};

export type CreateMatConversionProjectRequest = {
    urn: { value: number };
    newTrustName: string;
    newTrustReferenceNumber: string;
    significantDate: string;
    isSignificantDateProvisional: boolean;
    isDueTo2Ri: boolean;
    hasAcademyOrderBeenIssued: boolean;
    advisoryBoardDate: string;
    advisoryBoardConditions: string;
    establishmentSharepointLink: string;
    incomingTrustSharepointLink: string;
    handingOverToRegionalCaseworkService: boolean;
    handoverComments: string;
    userAdId: string;
};

export type CreateMatTransferProjectRequest = {
    urn: { value: number };
    newTrustName: string;
    newTrustReferenceNumber: string;
    outgoingTrustUkprn: { value: number };
    significantDate: string;
    isSignificantDateProvisional: boolean;
    isDueTo2Ri: boolean;
    isDueToInedaquateOfstedRating: boolean;
    isDueToIssues: boolean;
    handingOverToRegionalCaseworkService: boolean;
    outGoingTrustWillClose: boolean;
    advisoryBoardDate: string;
    advisoryBoardConditions: string;
    establishmentSharepointLink: string;
    incomingTrustSharepointLink: string;
    outgoingTrustSharepointLink: string;
    handoverComments: string;
    userAdId: string;
};

export type ProjectRequest =
    | CreateConversionProjectRequest
    | CreateTransferProjectRequest
    | CreateMatConversionProjectRequest
    | CreateMatTransferProjectRequest;

export type CreateProjectResponse = {
    value: string;
};

export type GetProjectResponse = {
    id: { value: string };
    urn: { value: number };
    createdAt: string;
    updatedAt: string;
    incomingTrustUkprn: { value: number };
    regionalDeliveryOfficerId: { value: string };
    caseworkerId: string | null;
    assignedAt: string;
    advisoryBoardDate: string;
    advisoryBoardConditions: string;
    establishmentSharepointLink: string;
    completedAt: string | null;
    incomingTrustSharepointLink: string;
    type: string;
    assignedToId: { value: string };
    significantDate: string;
    significantDateProvisional: boolean;
    directiveAcademyOrder: boolean;
    region: string;
    academyUrn: string | null;
    tasksDataId: { value: string };
    tasksDataType: string;
    outgoingTrustUkprn: { value: number };
    team: string;
    twoRequiresImprovement: boolean;
    outgoingTrustSharepointLink: string;
    allConditionsMet: boolean;
    mainContactId: string | null;
    establishmentMainContactId: string | null;
    incomingTrustMainContactId: string | null;
    outgoingTrustMainContactId: string | null;
    newTrustReferenceNumber: string | null;
    newTrustName: string | null;
    state: string;
    prepareId: string | null;
    localAuthorityMainContactId: string | null;
    groupId: string | null;
    assignedTo: string | null;
    caseworker: string | null;
    contacts: never[];
    notes: never[];
    regionalDeliveryOfficer: string | null;
};

export enum Region {
    London = 1,
    SouthEast = 2,
    YorkshireAndTheHumber = 3,
    NorthWest = 4,
    EastOfEngland = 5,
    WestMidlands = 6,
    NorthEast = 7,
    SouthWest = 8,
    EastMidlands = 9,
}

export type CreateLocalAuthorityRequest = {
    id: {
        value: string;
    };
    code: string;
    name: string;
    address1: string;
    address2: string;
    address3: string;
    addressTown: string;
    addressCounty: string;
    addressPostcode: string;
    contactId: {
        value: string;
    };
    title: string;
    contactName: string;
    email: string;
    phone: string;
};

// prepare project api domain

export type CreateConversionPrepareRequest = {
    urn: number;
    advisory_board_date: string;
    advisory_board_conditions: string;
    provisional_conversion_date: string;
    directive_academy_order: boolean;
    created_by_email: string;
    created_by_first_name: string;
    created_by_last_name: string;
    prepare_id: number;
    group_id: string;
    incoming_trust_ukprn: number;
};

export type CreateConversionFormAMatPrepareRequest = {
    urn: number;
    advisory_board_date: string;
    advisory_board_conditions: string;
    provisional_conversion_date: string;
    directive_academy_order: boolean;
    created_by_email: string;
    created_by_first_name: string;
    created_by_last_name: string;
    prepare_id: number;
    group_id: string;
    new_trust_reference_number: string;
    new_trust_name: string;
};

export type CreateTransferPrepareRequest = {
    urn: number;
    advisory_board_date: string;
    advisory_board_conditions: string;
    provisional_transfer_date: string;
    created_by_email: string;
    created_by_first_name: string;
    created_by_last_name: string;
    inadequate_ofsted: boolean;
    financial_safeguarding_governance_issues: boolean;
    outgoing_trust_ukprn: number;
    outgoing_trust_to_close: boolean;
    prepare_id: number;
    group_id: string;
    incoming_trust_ukprn: number;
};

export type CreateTransferFormAMatPrepareRequest = {
    urn: number;
    advisory_board_date: string;
    advisory_board_conditions: string;
    provisional_transfer_date: string;
    created_by_email: string;
    created_by_first_name: string;
    created_by_last_name: string;
    inadequate_ofsted: boolean;
    financial_safeguarding_governance_issues: boolean;
    outgoing_trust_ukprn: number;
    outgoing_trust_to_close: boolean;
    prepare_id: number;
    group_id: string;
    new_trust_reference_number: string;
    new_trust_name: string;
};

export type PrepareProjectRequest =
    | CreateConversionPrepareRequest
    | CreateTransferPrepareRequest
    | CreateConversionFormAMatPrepareRequest
    | CreateTransferFormAMatPrepareRequest;

export type CreateConversionPrepareResponse = {
    conversion_project_id: string;
};

export type CreateTransferPrepareResponse = {
    transfer_project_id: string;
};

export type CreatePrepareProjectResponse = CreateConversionPrepareResponse | CreateTransferPrepareResponse;
