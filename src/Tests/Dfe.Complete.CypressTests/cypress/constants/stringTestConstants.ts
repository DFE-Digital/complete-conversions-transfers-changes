import { EnvUrl } from "cypress/constants/cypressConstants";

export const specialCharsTestString = ',"(){}<>,!@£$%^&*+-';
export type yesNoOption = "Yes" | "No";
export const groupReferenceNumber = "GRP_00000001";
export const ukprn = 10058689;
export const trust = "The Macclesfield Academy";
export const ukprn2 = Cypress.env(EnvUrl).includes("test") ? 10089355 : 10058682;
export const today = new Date();
export const nextMonth = new Date(today.setMonth(today.getMonth() + 1));
export const rdoLondonUserEmail = "test.cypress-london@education.gov.uk";
export const rdoLondonUserAdId = "TEST-AD-ID-2";
export const rdoLondonUserName = "cypress test-london";
export const regionalCaseworkerUserEmail = "test.cypress-rcs@education.gov.uk";
export const regionalCaseworkerUserAdId = "TEST-AD-ID-2";
export const regionalCaseWorkerUserName = "cypress test-rcs";
