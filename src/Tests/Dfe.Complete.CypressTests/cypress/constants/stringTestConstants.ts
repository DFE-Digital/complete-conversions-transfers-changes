import { EnvUrl } from "cypress/constants/cypressConstants";

export const specialCharsTestString = ',"(){}<>,!@£$%^&*+-';
export type yesNoOption = "Yes" | "No";
export const groupReferenceNumber = "GRP_00000001";
export const ukprn = 10058689;
export const ukprn2 = Cypress.env(EnvUrl).includes("test") ? 10089355 : 10058682;
export const trust = "The Macclesfield Academy";
export const testTrustName = "Test Trust";
export const testTrustReferenceNumber = "TR09999";
export const today = new Date();
export const nextMonth = new Date(today.setMonth(today.getMonth() + 1));
export const serviceSupportUserAdId = "TEST-AD-ID-SS";
