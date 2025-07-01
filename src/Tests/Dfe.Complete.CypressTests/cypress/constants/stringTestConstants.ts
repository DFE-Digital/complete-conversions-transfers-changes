export type yesNoOption = "Yes" | "No";
export const groupReferenceNumber = "GRP_00000001";
export const macclesfieldTrust = {
    name: "The Macclesfield Academy",
    referenceNumber: "TR01369",
    ukprn: 10058689,
    companiesHouseNumber: "07597883",
    address: "Macclesfield College Macclesfield SK11 8LF",
    groupReferenceNumber: "GRP_07597883",
};
export const dimensionsTrust = {
    name: "5 Dimensions Trust",
    referenceNumber: "TR01904",
    ukprn: 10058682,
    companiesHouseNumber: "07595434",
    address: "The Hazeley Academy Emperor Drive Milton Keynes MK8 0PT",
    groupReferenceNumber: "GRP_12345670",
};
export const testTrust = {
    name: "Test Trust",
    referenceNumber: "TR09999",
};
export const today = new Date();
export const currentMonthLong = `${today.toLocaleString("default", { month: "long" })} ${today.getFullYear()}`;
export const currentMonthShort = `${today.toLocaleString("default", { month: "short" })} ${today.getFullYear()}`;
export const nextMonth = new Date(today.setMonth(today.getMonth() + 1));
export const nextMonthLong = `${nextMonth.toLocaleString("default", { month: "long" })} ${nextMonth.getFullYear()}`;
export const nextMonthShort = `${nextMonth.toLocaleString("default", { month: "short" })} ${nextMonth.getFullYear()}`;
export const giasUrl = "https://get-information-schools.service.gov.uk";
