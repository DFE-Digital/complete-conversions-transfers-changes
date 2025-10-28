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
    code: string;
    name: string;
    address1: string;
    address2: string;
    address3: string;
    addressTown: string;
    addressCounty: string;
    addressPostcode: string;
    title: string;
    contactName: string;
    email: string;
    phone: string;
};

// prepare project api domain

export type CreateConversionPrepareRequest = {
    urn: number;
    incomingTrustUkprn: number;
    advisoryBoardDate: string;
    provisionalConversionDate: string;
    createdByEmail: string;
    createdByFirstName: string;
    createdByLastName: string;
    prepareId: number;
    directiveAcademyOrder: boolean;
    advisoryBoardConditions: string;
    groupId: string | null;
};

export type CreateConversionFormAMatPrepareRequest = {
    urn: number;
    advisoryBoardDate: string;
    advisoryBoardConditions: string;
    provisionalConversionDate: string;
    directiveAcademyOrder: boolean;
    createdByEmail: string;
    createdByFirstName: string;
    createdByLastName: string;
    prepareId: number;
    groupId: string | null;
    newTrustReferenceNumber: string;
    newTrustName: string;
};

export type CreateTransferPrepareRequest = {
    urn: number;
    advisoryBoardDate: string;
    advisoryBoardConditions: string;
    provisionalTransferDate: string;
    createdByEmail: string;
    createdByFirstName: string;
    createdByLastName: string;
    inadequateOfsted: boolean;
    financialSafeguardingGovernanceIssues: boolean;
    outgoingTrustUkprn: number;
    outgoingTrustToClose: boolean;
    prepareId: number;
    groupId: string | null;
    incomingTrustUkprn: number;
};

export type CreateTransferFormAMatPrepareRequest = {
    urn: number;
    advisoryBoardDate: string;
    advisoryBoardConditions: string;
    provisionalTransferDate: string;
    createdByEmail: string;
    createdByFirstName: string;
    createdByLastName: string;
    inadequateOfsted: boolean;
    financialSafeguardingGovernanceIssues: boolean;
    outgoingTrustUkprn: number;
    outgoingTrustToClose: boolean;
    prepareId: number;
    groupId: string | null;
    newTrustReferenceNumber: string;
    newTrustName: string;
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
