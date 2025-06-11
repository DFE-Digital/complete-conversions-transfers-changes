export type yesNoOption = "Yes" | "No";
export const groupReferenceNumber = "GRP_00000001";
export const ukprn = 10058689;
export const ukprn2 = 10058682;

export const macclesfieldTrust = {
    name: "The Macclesfield Academy",
    referenceNumber: "TR01369",
    ukprn: 10058689,
    number: "07597883",
    address: "Macclesfield",
};
export const dimensionsTrust = {
    name: "5 Dimensions Trust",
    referenceNumber: "TR01904",
    ukprn: 10058682,
    number: "07595434",
    address: "Milton Keynes",
};
export const trust = "The Macclesfield Academy";
export const trust2 = "5 Dimensions Trust";
export const testTrustName = "Test Trust";
export const testTrustReferenceNumber = "TR09999";
export const today = new Date();
export const currentMonthLong = `${today.toLocaleString("default", { month: "long" })} ${today.getFullYear()}`;
export const currentMonthShort = `${today.toLocaleString("default", { month: "short" })} ${today.getFullYear()}`;
export const nextMonth = new Date(today.setMonth(today.getMonth() + 1));
export const nextMonthLong = `${nextMonth.toLocaleString("default", { month: "long" })} ${nextMonth.getFullYear()}`;
export const nextMonthShort = `${nextMonth.toLocaleString("default", { month: "short" })} ${nextMonth.getFullYear()}`;
