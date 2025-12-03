import { ContactCategory, ContactType, CreateContactRequest } from "cypress/api/apiDomain";

export class ContactBuilder {
    public static createContactRequest(options: Partial<CreateContactRequest> = {}): CreateContactRequest {
        return {
            fullName: "Testy McTestface",
            role: "Headteacher",
            email: "testy@education.gov.uk",
            phoneNumber: "01234567890",
            category: ContactCategory.SchoolOrAcademy,
            isPrimaryContact: false,
            projectId: { value: "" }, // specify a valid projectId in request options
            organisationName: "Test Organisation", // typically same as school / LA / Trust name
            type: ContactType.Project,
            establishmentUrn: null,
            localAuthorityId: null,
            ...options,
        };
    }
}
