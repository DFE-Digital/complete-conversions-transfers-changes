import { CreateLocalAuthorityRequest } from "cypress/api/apiDomain";

export class LocalAuthorityBuilder {
    public static createLocalAuthorityRequest(
        options: Partial<CreateLocalAuthorityRequest> = {},
    ): CreateLocalAuthorityRequest {
        return {
            id: { value: crypto.randomUUID() },
            code: "124",
            name: "Test Local Authority",
            address1: "Test Address Line 1",
            address2: "Test Address Line 2",
            address3: "Test Address Line 3",
            addressTown: "Test Town",
            addressCounty: "Test County",
            addressPostcode: "W1A 1AA",
            contactId: { value: crypto.randomUUID() },
            title: "Test Director",
            contactName: "Test DCS Name",
            email: "test@email.com",
            phone: "01234567890",
            ...options,
        };
    }
}
