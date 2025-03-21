export type CreateTransferProjectRequest = {
    urn: string;
    date: string;
    isDateProvisional: boolean;
    schoolSharePointLink: string;
    region: Region;
    isDueTo2RI: boolean;
    isDueToOfstedRating: boolean;
    isDueToIssues: boolean;
    advisoryBoardDetails: AdvisoryBoardDetails;
    incomingTrustDetails: CreateTrustDetails;
    outgoingTrustDetails: CreateTrustDetails;
};

export type CreateTrustDetails = {
    ukprn: string;
    sharepointLink: string;
};

export type AdvisoryBoardDetails = {
    date: string;
    conditions: string;
};

export type CreateTransferProjectResponse =
    {
        id: string;
    };

export type CreateProjectRequest = {
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

export type CreateProjectResponse = {
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
    EastMidlands = 9
}