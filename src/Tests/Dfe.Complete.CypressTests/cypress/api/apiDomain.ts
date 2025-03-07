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
    groupReferenceNumber: string;
    handingOverToRegionalCaseworkService: boolean;
    handoverComments: string;
    userAdId?: string;
};

export type CreateConversionProjectResponse =
    {
        id: string;
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