using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database.Interceptors;
using Dfe.Complete.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Complete.Infrastructure.Database;

public partial class CompleteContext2 : DbContext
{
    private readonly IConfiguration? _configuration;
    const string DefaultSchema = "complete";
    private readonly IServiceProvider _serviceProvider = null!;

    public CompleteContext()
    {
    }

    public CompleteContext(DbContextOptions<CompleteContext> options, IConfiguration configuration, IServiceProvider serviceProvider)
        : base(options)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public virtual DbSet<ApiKey> ApiKeys { get; set; }

    public virtual DbSet<ArInternalMetadatum> ArInternalMetadata { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<ConversionTasksDatum> ConversionTasksData { get; set; }

    public virtual DbSet<DaoRevocation> DaoRevocations { get; set; }

    public virtual DbSet<DaoRevocationReason> DaoRevocationReasons { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<GiasEstablishment> GiasEstablishments { get; set; }

    public virtual DbSet<GiasGroup> GiasGroups { get; set; }

    public virtual DbSet<KeyContact> KeyContacts { get; set; }

    public virtual DbSet<LocalAuthority> LocalAuthorities { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectGroup> ProjectGroups { get; set; }

    public virtual DbSet<SchemaMigration> SchemaMigrations { get; set; }

    public virtual DbSet<SignificantDateHistory> SignificantDateHistories { get; set; }

    public virtual DbSet<SignificantDateHistoryReason> SignificantDateHistoryReasons { get; set; }

    public virtual DbSet<TransferTasksDatum> TransferTasksData { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration!.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        optionsBuilder.AddInterceptors(new DomainEventDispatcherInterceptor(mediator));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__api_keys__3213E83F95747D22");

            entity.ToTable("api_keys", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ApiKey1)
                .HasMaxLength(4000)
                .HasColumnName("api_key");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(4000)
                .HasColumnName("description");
            entity.Property(e => e.ExpiresAt)
                .HasPrecision(6)
                .HasColumnName("expires_at");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ArInternalMetadatum>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PK__ar_inter__DFD83CAE30D055CC");

            entity.ToTable("ar_internal_metadata", "complete");

            entity.Property(e => e.Key)
                .HasMaxLength(4000)
                .HasColumnName("key");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.Value)
                .HasMaxLength(4000)
                .HasColumnName("value");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__contacts__3213E83F5AC83AE6");

            entity.ToTable("contacts", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(4000)
                .HasColumnName("email");
            entity.Property(e => e.EstablishmentUrn).HasColumnName("establishment_urn");
            entity.Property(e => e.LocalAuthorityId).HasColumnName("local_authority_id");
            entity.Property(e => e.Name)
                .HasMaxLength(4000)
                .HasColumnName("name");
            entity.Property(e => e.OrganisationName)
                .HasMaxLength(4000)
                .HasColumnName("organisation_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(4000)
                .HasColumnName("phone");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Title)
                .HasMaxLength(4000)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(4000)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Project).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_rails_b0485f0dbc");
        });

        modelBuilder.Entity<ConversionTasksDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__conversi__3213E83FC77813DB");

            entity.ToTable("conversion_tasks_data", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AcademyDetailsName)
                .HasMaxLength(4000)
                .HasColumnName("academy_details_name");
            entity.Property(e => e.ArticlesOfAssociationCleared).HasColumnName("articles_of_association_cleared");
            entity.Property(e => e.ArticlesOfAssociationNotApplicable).HasColumnName("articles_of_association_not_applicable");
            entity.Property(e => e.ArticlesOfAssociationReceived).HasColumnName("articles_of_association_received");
            entity.Property(e => e.ArticlesOfAssociationSaved).HasColumnName("articles_of_association_saved");
            entity.Property(e => e.ArticlesOfAssociationSent).HasColumnName("articles_of_association_sent");
            entity.Property(e => e.ArticlesOfAssociationSigned).HasColumnName("articles_of_association_signed");
            entity.Property(e => e.CheckAccuracyOfHigherNeedsConfirmNumber).HasColumnName("check_accuracy_of_higher_needs_confirm_number");
            entity.Property(e => e.CheckAccuracyOfHigherNeedsConfirmPublishedNumber).HasColumnName("check_accuracy_of_higher_needs_confirm_published_number");
            entity.Property(e => e.ChurchSupplementalAgreementCleared).HasColumnName("church_supplemental_agreement_cleared");
            entity.Property(e => e.ChurchSupplementalAgreementNotApplicable).HasColumnName("church_supplemental_agreement_not_applicable");
            entity.Property(e => e.ChurchSupplementalAgreementReceived).HasColumnName("church_supplemental_agreement_received");
            entity.Property(e => e.ChurchSupplementalAgreementSaved).HasColumnName("church_supplemental_agreement_saved");
            entity.Property(e => e.ChurchSupplementalAgreementSent).HasColumnName("church_supplemental_agreement_sent");
            entity.Property(e => e.ChurchSupplementalAgreementSigned).HasColumnName("church_supplemental_agreement_signed");
            entity.Property(e => e.ChurchSupplementalAgreementSignedDiocese).HasColumnName("church_supplemental_agreement_signed_diocese");
            entity.Property(e => e.ChurchSupplementalAgreementSignedSecretaryState).HasColumnName("church_supplemental_agreement_signed_secretary_state");
            entity.Property(e => e.CommercialTransferAgreementAgreed)
                .HasDefaultValue(false)
                .HasColumnName("commercial_transfer_agreement_agreed");
            entity.Property(e => e.CommercialTransferAgreementQuestionsChecked)
                .HasDefaultValue(false)
                .HasColumnName("commercial_transfer_agreement_questions_checked");
            entity.Property(e => e.CommercialTransferAgreementQuestionsReceived)
                .HasDefaultValue(false)
                .HasColumnName("commercial_transfer_agreement_questions_received");
            entity.Property(e => e.CommercialTransferAgreementSaved)
                .HasDefaultValue(false)
                .HasColumnName("commercial_transfer_agreement_saved");
            entity.Property(e => e.CommercialTransferAgreementSigned)
                .HasDefaultValue(false)
                .HasColumnName("commercial_transfer_agreement_signed");
            entity.Property(e => e.CompleteNotificationOfChangeCheckDocument).HasColumnName("complete_notification_of_change_check_document");
            entity.Property(e => e.CompleteNotificationOfChangeNotApplicable).HasColumnName("complete_notification_of_change_not_applicable");
            entity.Property(e => e.CompleteNotificationOfChangeSendDocument).HasColumnName("complete_notification_of_change_send_document");
            entity.Property(e => e.CompleteNotificationOfChangeTellLocalAuthority).HasColumnName("complete_notification_of_change_tell_local_authority");
            entity.Property(e => e.ConfirmDateAcademyOpenedDateOpened).HasColumnName("confirm_date_academy_opened_date_opened");
            entity.Property(e => e.ConversionGrantCheckVendorAccount).HasColumnName("conversion_grant_check_vendor_account");
            entity.Property(e => e.ConversionGrantNotApplicable).HasColumnName("conversion_grant_not_applicable");
            entity.Property(e => e.ConversionGrantPaymentForm).HasColumnName("conversion_grant_payment_form");
            entity.Property(e => e.ConversionGrantSendInformation).HasColumnName("conversion_grant_send_information");
            entity.Property(e => e.ConversionGrantSharePaymentDate).HasColumnName("conversion_grant_share_payment_date");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.DeedOfVariationCleared).HasColumnName("deed_of_variation_cleared");
            entity.Property(e => e.DeedOfVariationNotApplicable).HasColumnName("deed_of_variation_not_applicable");
            entity.Property(e => e.DeedOfVariationReceived).HasColumnName("deed_of_variation_received");
            entity.Property(e => e.DeedOfVariationSaved).HasColumnName("deed_of_variation_saved");
            entity.Property(e => e.DeedOfVariationSent).HasColumnName("deed_of_variation_sent");
            entity.Property(e => e.DeedOfVariationSigned).HasColumnName("deed_of_variation_signed");
            entity.Property(e => e.DeedOfVariationSignedSecretaryState).HasColumnName("deed_of_variation_signed_secretary_state");
            entity.Property(e => e.DirectionToTransferCleared).HasColumnName("direction_to_transfer_cleared");
            entity.Property(e => e.DirectionToTransferNotApplicable).HasColumnName("direction_to_transfer_not_applicable");
            entity.Property(e => e.DirectionToTransferReceived).HasColumnName("direction_to_transfer_received");
            entity.Property(e => e.DirectionToTransferSaved).HasColumnName("direction_to_transfer_saved");
            entity.Property(e => e.DirectionToTransferSigned).HasColumnName("direction_to_transfer_signed");
            entity.Property(e => e.HandoverMeeting).HasColumnName("handover_meeting");
            entity.Property(e => e.HandoverNotApplicable).HasColumnName("handover_not_applicable");
            entity.Property(e => e.HandoverNotes).HasColumnName("handover_notes");
            entity.Property(e => e.HandoverReview).HasColumnName("handover_review");
            entity.Property(e => e.LandQuestionnaireCleared).HasColumnName("land_questionnaire_cleared");
            entity.Property(e => e.LandQuestionnaireReceived).HasColumnName("land_questionnaire_received");
            entity.Property(e => e.LandQuestionnaireSaved).HasColumnName("land_questionnaire_saved");
            entity.Property(e => e.LandQuestionnaireSigned).HasColumnName("land_questionnaire_signed");
            entity.Property(e => e.LandRegistryCleared).HasColumnName("land_registry_cleared");
            entity.Property(e => e.LandRegistryReceived).HasColumnName("land_registry_received");
            entity.Property(e => e.LandRegistrySaved).HasColumnName("land_registry_saved");
            entity.Property(e => e.MasterFundingAgreementCleared).HasColumnName("master_funding_agreement_cleared");
            entity.Property(e => e.MasterFundingAgreementNotApplicable).HasColumnName("master_funding_agreement_not_applicable");
            entity.Property(e => e.MasterFundingAgreementReceived).HasColumnName("master_funding_agreement_received");
            entity.Property(e => e.MasterFundingAgreementSaved).HasColumnName("master_funding_agreement_saved");
            entity.Property(e => e.MasterFundingAgreementSent).HasColumnName("master_funding_agreement_sent");
            entity.Property(e => e.MasterFundingAgreementSigned).HasColumnName("master_funding_agreement_signed");
            entity.Property(e => e.MasterFundingAgreementSignedSecretaryState).HasColumnName("master_funding_agreement_signed_secretary_state");
            entity.Property(e => e.OneHundredAndTwentyFiveYearLeaseEmail).HasColumnName("one_hundred_and_twenty_five_year_lease_email");
            entity.Property(e => e.OneHundredAndTwentyFiveYearLeaseNotApplicable).HasColumnName("one_hundred_and_twenty_five_year_lease_not_applicable");
            entity.Property(e => e.OneHundredAndTwentyFiveYearLeaseReceive).HasColumnName("one_hundred_and_twenty_five_year_lease_receive");
            entity.Property(e => e.OneHundredAndTwentyFiveYearLeaseSaveLease).HasColumnName("one_hundred_and_twenty_five_year_lease_save_lease");
            entity.Property(e => e.ProposedCapacityOfTheAcademyNotApplicable).HasColumnName("proposed_capacity_of_the_academy_not_applicable");
            entity.Property(e => e.ProposedCapacityOfTheAcademyReceptionToSixYears)
                .HasMaxLength(4000)
                .HasColumnName("proposed_capacity_of_the_academy_reception_to_six_years");
            entity.Property(e => e.ProposedCapacityOfTheAcademySevenToElevenYears)
                .HasMaxLength(4000)
                .HasColumnName("proposed_capacity_of_the_academy_seven_to_eleven_years");
            entity.Property(e => e.ProposedCapacityOfTheAcademyTwelveOrAboveYears)
                .HasMaxLength(4000)
                .HasColumnName("proposed_capacity_of_the_academy_twelve_or_above_years");
            entity.Property(e => e.ReceiveGrantPaymentCertificateCheckCertificate).HasColumnName("receive_grant_payment_certificate_check_certificate");
            entity.Property(e => e.ReceiveGrantPaymentCertificateDateReceived).HasColumnName("receive_grant_payment_certificate_date_received");
            entity.Property(e => e.ReceiveGrantPaymentCertificateSaveCertificate).HasColumnName("receive_grant_payment_certificate_save_certificate");
            entity.Property(e => e.RedactAndSendRedact).HasColumnName("redact_and_send_redact");
            entity.Property(e => e.RedactAndSendSaveRedaction).HasColumnName("redact_and_send_save_redaction");
            entity.Property(e => e.RedactAndSendSendRedaction).HasColumnName("redact_and_send_send_redaction");
            entity.Property(e => e.RedactAndSendSendSolicitors).HasColumnName("redact_and_send_send_solicitors");
            entity.Property(e => e.RiskProtectionArrangementOption)
                .HasMaxLength(4000)
                .HasColumnName("risk_protection_arrangement_option");
            entity.Property(e => e.RiskProtectionArrangementReason)
                .HasMaxLength(4000)
                .HasColumnName("risk_protection_arrangement_reason");
            entity.Property(e => e.SchoolCompletedEmailed).HasColumnName("school_completed_emailed");
            entity.Property(e => e.SchoolCompletedSaved).HasColumnName("school_completed_saved");
            entity.Property(e => e.ShareInformationEmail).HasColumnName("share_information_email");
            entity.Property(e => e.SponsoredSupportGrantInformTrust).HasColumnName("sponsored_support_grant_inform_trust");
            entity.Property(e => e.SponsoredSupportGrantNotApplicable).HasColumnName("sponsored_support_grant_not_applicable");
            entity.Property(e => e.SponsoredSupportGrantPaymentAmount).HasColumnName("sponsored_support_grant_payment_amount");
            entity.Property(e => e.SponsoredSupportGrantPaymentForm).HasColumnName("sponsored_support_grant_payment_form");
            entity.Property(e => e.SponsoredSupportGrantSendInformation).HasColumnName("sponsored_support_grant_send_information");
            entity.Property(e => e.SponsoredSupportGrantType)
                .HasMaxLength(4000)
                .HasColumnName("sponsored_support_grant_type");
            entity.Property(e => e.StakeholderKickOffCheckProvisionalConversionDate).HasColumnName("stakeholder_kick_off_check_provisional_conversion_date");
            entity.Property(e => e.StakeholderKickOffIntroductoryEmails).HasColumnName("stakeholder_kick_off_introductory_emails");
            entity.Property(e => e.StakeholderKickOffLocalAuthorityProforma).HasColumnName("stakeholder_kick_off_local_authority_proforma");
            entity.Property(e => e.StakeholderKickOffMeeting).HasColumnName("stakeholder_kick_off_meeting");
            entity.Property(e => e.StakeholderKickOffSetupMeeting).HasColumnName("stakeholder_kick_off_setup_meeting");
            entity.Property(e => e.SubleasesCleared).HasColumnName("subleases_cleared");
            entity.Property(e => e.SubleasesEmailSigned).HasColumnName("subleases_email_signed");
            entity.Property(e => e.SubleasesNotApplicable).HasColumnName("subleases_not_applicable");
            entity.Property(e => e.SubleasesReceiveSigned).HasColumnName("subleases_receive_signed");
            entity.Property(e => e.SubleasesReceived).HasColumnName("subleases_received");
            entity.Property(e => e.SubleasesSaveSigned).HasColumnName("subleases_save_signed");
            entity.Property(e => e.SubleasesSaved).HasColumnName("subleases_saved");
            entity.Property(e => e.SubleasesSigned).HasColumnName("subleases_signed");
            entity.Property(e => e.SupplementalFundingAgreementCleared).HasColumnName("supplemental_funding_agreement_cleared");
            entity.Property(e => e.SupplementalFundingAgreementReceived).HasColumnName("supplemental_funding_agreement_received");
            entity.Property(e => e.SupplementalFundingAgreementSaved).HasColumnName("supplemental_funding_agreement_saved");
            entity.Property(e => e.SupplementalFundingAgreementSent).HasColumnName("supplemental_funding_agreement_sent");
            entity.Property(e => e.SupplementalFundingAgreementSigned).HasColumnName("supplemental_funding_agreement_signed");
            entity.Property(e => e.SupplementalFundingAgreementSignedSecretaryState).HasColumnName("supplemental_funding_agreement_signed_secretary_state");
            entity.Property(e => e.TenancyAtWillEmailSigned).HasColumnName("tenancy_at_will_email_signed");
            entity.Property(e => e.TenancyAtWillNotApplicable).HasColumnName("tenancy_at_will_not_applicable");
            entity.Property(e => e.TenancyAtWillReceiveSigned).HasColumnName("tenancy_at_will_receive_signed");
            entity.Property(e => e.TenancyAtWillSaveSigned).HasColumnName("tenancy_at_will_save_signed");
            entity.Property(e => e.TrustModificationOrderCleared).HasColumnName("trust_modification_order_cleared");
            entity.Property(e => e.TrustModificationOrderNotApplicable).HasColumnName("trust_modification_order_not_applicable");
            entity.Property(e => e.TrustModificationOrderReceived).HasColumnName("trust_modification_order_received");
            entity.Property(e => e.TrustModificationOrderSaved).HasColumnName("trust_modification_order_saved");
            entity.Property(e => e.TrustModificationOrderSentLegal).HasColumnName("trust_modification_order_sent_legal");
            entity.Property(e => e.UpdateEsfaUpdate).HasColumnName("update_esfa_update");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<DaoRevocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__dao_revo__3213E83F319BDBD1");

            entity.ToTable("dao_revocations", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfDecision).HasColumnName("date_of_decision");
            entity.Property(e => e.DecisionMakersName)
                .HasMaxLength(4000)
                .HasColumnName("decision_makers_name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<DaoRevocationReason>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__dao_revo__3213E83F98F9CEBC");

            entity.ToTable("dao_revocation_reasons", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DaoRevocationId).HasColumnName("dao_revocation_id");
            entity.Property(e => e.ReasonType)
                .HasMaxLength(4000)
                .HasColumnName("reason_type");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__events__3213E83FCA9FAAFF");

            entity.ToTable("events", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.EventableId).HasColumnName("eventable_id");
            entity.Property(e => e.EventableType)
                .HasMaxLength(4000)
                .HasColumnName("eventable_type");
            entity.Property(e => e.Grouping).HasColumnName("grouping");
            entity.Property(e => e.Message)
                .HasMaxLength(4000)
                .HasColumnName("message");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<GiasEstablishment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__gias_est__3213E83F7DB68D44");

            entity.ToTable("gias_establishments", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AddressAdditional)
                .HasMaxLength(4000)
                .HasColumnName("address_additional");
            entity.Property(e => e.AddressCounty)
                .HasMaxLength(4000)
                .HasColumnName("address_county");
            entity.Property(e => e.AddressLocality)
                .HasMaxLength(4000)
                .HasColumnName("address_locality");
            entity.Property(e => e.AddressPostcode)
                .HasMaxLength(4000)
                .HasColumnName("address_postcode");
            entity.Property(e => e.AddressStreet)
                .HasMaxLength(4000)
                .HasColumnName("address_street");
            entity.Property(e => e.AddressTown)
                .HasMaxLength(4000)
                .HasColumnName("address_town");
            entity.Property(e => e.AgeRangeLower).HasColumnName("age_range_lower");
            entity.Property(e => e.AgeRangeUpper).HasColumnName("age_range_upper");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.DioceseCode)
                .HasMaxLength(4000)
                .HasColumnName("diocese_code");
            entity.Property(e => e.DioceseName)
                .HasMaxLength(4000)
                .HasColumnName("diocese_name");
            entity.Property(e => e.EstablishmentNumber)
                .HasMaxLength(4000)
                .HasColumnName("establishment_number");
            entity.Property(e => e.LocalAuthorityCode)
                .HasMaxLength(4000)
                .HasColumnName("local_authority_code");
            entity.Property(e => e.LocalAuthorityName)
                .HasMaxLength(4000)
                .HasColumnName("local_authority_name");
            entity.Property(e => e.Name)
                .HasMaxLength(4000)
                .HasColumnName("name");
            entity.Property(e => e.OpenDate).HasColumnName("open_date");
            entity.Property(e => e.ParliamentaryConstituencyCode)
                .HasMaxLength(4000)
                .HasColumnName("parliamentary_constituency_code");
            entity.Property(e => e.ParliamentaryConstituencyName)
                .HasMaxLength(4000)
                .HasColumnName("parliamentary_constituency_name");
            entity.Property(e => e.PhaseCode)
                .HasMaxLength(4000)
                .HasColumnName("phase_code");
            entity.Property(e => e.PhaseName)
                .HasMaxLength(4000)
                .HasColumnName("phase_name");
            entity.Property(e => e.RegionCode)
                .HasMaxLength(4000)
                .HasColumnName("region_code");
            entity.Property(e => e.RegionName)
                .HasMaxLength(4000)
                .HasColumnName("region_name");
            entity.Property(e => e.StatusName)
                .HasMaxLength(4000)
                .HasColumnName("status_name");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(4000)
                .HasColumnName("type_code");
            entity.Property(e => e.TypeName)
                .HasMaxLength(4000)
                .HasColumnName("type_name");
            entity.Property(e => e.Ukprn).HasColumnName("ukprn");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.Url)
                .HasMaxLength(4000)
                .HasColumnName("url");
            entity.Property(e => e.Urn).HasColumnName("urn");
        });

        modelBuilder.Entity<GiasGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__gias_gro__3213E83FFBDBD925");

            entity.ToTable("gias_groups", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AddressAdditional)
                .HasMaxLength(4000)
                .HasColumnName("address_additional");
            entity.Property(e => e.AddressCounty)
                .HasMaxLength(4000)
                .HasColumnName("address_county");
            entity.Property(e => e.AddressLocality)
                .HasMaxLength(4000)
                .HasColumnName("address_locality");
            entity.Property(e => e.AddressPostcode)
                .HasMaxLength(4000)
                .HasColumnName("address_postcode");
            entity.Property(e => e.AddressStreet)
                .HasMaxLength(4000)
                .HasColumnName("address_street");
            entity.Property(e => e.AddressTown)
                .HasMaxLength(4000)
                .HasColumnName("address_town");
            entity.Property(e => e.CompaniesHouseNumber)
                .HasMaxLength(4000)
                .HasColumnName("companies_house_number");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.GroupIdentifier)
                .HasMaxLength(4000)
                .HasColumnName("group_identifier");
            entity.Property(e => e.OriginalName)
                .HasMaxLength(4000)
                .HasColumnName("original_name");
            entity.Property(e => e.Ukprn).HasColumnName("ukprn");
            entity.Property(e => e.UniqueGroupIdentifier).HasColumnName("unique_group_identifier");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<KeyContact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__key_cont__3213E83F2B11A697");

            entity.ToTable("key_contacts", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ChairOfGovernorsId).HasColumnName("chair_of_governors_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.HeadteacherId).HasColumnName("headteacher_id");
            entity.Property(e => e.IncomingTrustCeoId).HasColumnName("incoming_trust_ceo_id");
            entity.Property(e => e.OutgoingTrustCeoId).HasColumnName("outgoing_trust_ceo_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<LocalAuthority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__local_au__3213E83F9C3793CA");

            entity.ToTable("local_authorities", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address1)
                .HasMaxLength(4000)
                .HasColumnName("address_1");
            entity.Property(e => e.Address2)
                .HasMaxLength(4000)
                .HasColumnName("address_2");
            entity.Property(e => e.Address3)
                .HasMaxLength(4000)
                .HasColumnName("address_3");
            entity.Property(e => e.AddressCounty)
                .HasMaxLength(4000)
                .HasColumnName("address_county");
            entity.Property(e => e.AddressPostcode)
                .HasMaxLength(4000)
                .HasColumnName("address_postcode");
            entity.Property(e => e.AddressTown)
                .HasMaxLength(4000)
                .HasColumnName("address_town");
            entity.Property(e => e.Code)
                .HasMaxLength(4000)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(4000)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__notes__3213E83F16C5DDC4");

            entity.ToTable("notes", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.NotableId).HasColumnName("notable_id");
            entity.Property(e => e.NotableType)
                .HasMaxLength(4000)
                .HasColumnName("notable_type");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.TaskIdentifier)
                .HasMaxLength(4000)
                .HasColumnName("task_identifier");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Project).WithMany(p => p.Notes)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_rails_99e097b079");

            entity.HasOne(d => d.User).WithMany(p => p.Notes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_rails_7f2323ad43");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__projects__3213E83F1AF5857D");

            entity.ToTable("projects", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.AcademyUrn).HasColumnName("academy_urn");
            entity.Property(e => e.AdvisoryBoardConditions).HasColumnName("advisory_board_conditions");
            entity.Property(e => e.AdvisoryBoardDate).HasColumnName("advisory_board_date");
            entity.Property(e => e.AllConditionsMet)
                .HasDefaultValue(false)
                .HasColumnName("all_conditions_met");
            entity.Property(e => e.AssignedAt)
                .HasPrecision(6)
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssignedToId).HasColumnName("assigned_to_id");
            entity.Property(e => e.CaseworkerId).HasColumnName("caseworker_id");
            entity.Property(e => e.CompletedAt)
                .HasPrecision(6)
                .HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.DirectiveAcademyOrder)
                .HasDefaultValue(false)
                .HasColumnName("directive_academy_order");
            entity.Property(e => e.EstablishmentMainContactId).HasColumnName("establishment_main_contact_id");
            entity.Property(e => e.EstablishmentSharepointLink).HasColumnName("establishment_sharepoint_link");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IncomingTrustMainContactId).HasColumnName("incoming_trust_main_contact_id");
            entity.Property(e => e.IncomingTrustSharepointLink).HasColumnName("incoming_trust_sharepoint_link");
            entity.Property(e => e.IncomingTrustUkprn).HasColumnName("incoming_trust_ukprn");
            entity.Property(e => e.LocalAuthorityMainContactId).HasColumnName("local_authority_main_contact_id");
            entity.Property(e => e.MainContactId).HasColumnName("main_contact_id");
            entity.Property(e => e.NewTrustName)
                .HasMaxLength(4000)
                .HasColumnName("new_trust_name");
            entity.Property(e => e.NewTrustReferenceNumber)
                .HasMaxLength(4000)
                .HasColumnName("new_trust_reference_number");
            entity.Property(e => e.OutgoingTrustMainContactId).HasColumnName("outgoing_trust_main_contact_id");
            entity.Property(e => e.OutgoingTrustSharepointLink).HasColumnName("outgoing_trust_sharepoint_link");
            entity.Property(e => e.OutgoingTrustUkprn).HasColumnName("outgoing_trust_ukprn");
            entity.Property(e => e.PrepareId).HasColumnName("prepare_id");
            entity.Property(e => e.Region)
                .HasMaxLength(4000)
                .HasColumnName("region");
            entity.Property(e => e.RegionalDeliveryOfficerId).HasColumnName("regional_delivery_officer_id");
            entity.Property(e => e.SignificantDate).HasColumnName("significant_date");
            entity.Property(e => e.SignificantDateProvisional)
                .HasDefaultValue(true)
                .HasColumnName("significant_date_provisional");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.TasksDataId).HasColumnName("tasks_data_id");
            entity.Property(e => e.TasksDataType)
                .HasMaxLength(4000)
                .HasColumnName("tasks_data_type");
            entity.Property(e => e.Team)
                .HasMaxLength(4000)
                .HasColumnName("team");
            entity.Property(e => e.TwoRequiresImprovement)
                .HasDefaultValue(false)
                .HasColumnName("two_requires_improvement");
            entity.Property(e => e.Type)
                .HasMaxLength(4000)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.Urn).HasColumnName("urn");

            entity.HasOne(d => d.AssignedTo).WithMany(p => p.ProjectAssignedTos)
                .HasForeignKey(d => d.AssignedToId)
                .HasConstraintName("fk_rails_9cf9d80ba9");

            entity.HasOne(d => d.Caseworker).WithMany(p => p.ProjectCaseworkers)
                .HasForeignKey(d => d.CaseworkerId)
                .HasConstraintName("fk_rails_246548228c");

            entity.HasOne(d => d.RegionalDeliveryOfficer).WithMany(p => p.ProjectRegionalDeliveryOfficers)
                .HasForeignKey(d => d.RegionalDeliveryOfficerId)
                .HasConstraintName("fk_rails_bba1c6b145");
        });

        modelBuilder.Entity<ProjectGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__project___3213E83F98E1C87A");

            entity.ToTable("project_groups", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.GroupIdentifier)
                .HasMaxLength(4000)
                .HasColumnName("group_identifier");
            entity.Property(e => e.TrustUkprn).HasColumnName("trust_ukprn");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<SchemaMigration>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PK__schema_m__79B5C94C894CC1F5");

            entity.ToTable("schema_migrations", "complete");

            entity.Property(e => e.Version)
                .HasMaxLength(4000)
                .HasColumnName("version");
        });

        modelBuilder.Entity<SignificantDateHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__signific__3213E83FC4F90D39");

            entity.ToTable("significant_date_histories", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.PreviousDate).HasColumnName("previous_date");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RevisedDate).HasColumnName("revised_date");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<SignificantDateHistoryReason>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__signific__3213E83F0B26EC48");

            entity.ToTable("significant_date_history_reasons", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.ReasonType)
                .HasMaxLength(4000)
                .HasColumnName("reason_type");
            entity.Property(e => e.SignificantDateHistoryId).HasColumnName("significant_date_history_id");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<TransferTasksDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__transfer__3213E83F7202F297");

            entity.ToTable("transfer_tasks_data", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ArticlesOfAssociationCleared).HasColumnName("articles_of_association_cleared");
            entity.Property(e => e.ArticlesOfAssociationNotApplicable).HasColumnName("articles_of_association_not_applicable");
            entity.Property(e => e.ArticlesOfAssociationReceived).HasColumnName("articles_of_association_received");
            entity.Property(e => e.ArticlesOfAssociationSaved).HasColumnName("articles_of_association_saved");
            entity.Property(e => e.ArticlesOfAssociationSent).HasColumnName("articles_of_association_sent");
            entity.Property(e => e.ArticlesOfAssociationSigned).HasColumnName("articles_of_association_signed");
            entity.Property(e => e.BankDetailsChangingYesNo)
                .HasDefaultValue(false)
                .HasColumnName("bank_details_changing_yes_no");
            entity.Property(e => e.CheckAndConfirmFinancialInformationAcademySurplusDeficit)
                .HasMaxLength(4000)
                .HasColumnName("check_and_confirm_financial_information_academy_surplus_deficit");
            entity.Property(e => e.CheckAndConfirmFinancialInformationNotApplicable).HasColumnName("check_and_confirm_financial_information_not_applicable");
            entity.Property(e => e.CheckAndConfirmFinancialInformationTrustSurplusDeficit)
                .HasMaxLength(4000)
                .HasColumnName("check_and_confirm_financial_information_trust_surplus_deficit");
            entity.Property(e => e.ChurchSupplementalAgreementCleared).HasColumnName("church_supplemental_agreement_cleared");
            entity.Property(e => e.ChurchSupplementalAgreementNotApplicable).HasColumnName("church_supplemental_agreement_not_applicable");
            entity.Property(e => e.ChurchSupplementalAgreementReceived).HasColumnName("church_supplemental_agreement_received");
            entity.Property(e => e.ChurchSupplementalAgreementSavedAfterSigningBySecretaryState).HasColumnName("church_supplemental_agreement_saved_after_signing_by_secretary_state");
            entity.Property(e => e.ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese).HasColumnName("church_supplemental_agreement_saved_after_signing_by_trust_diocese");
            entity.Property(e => e.ChurchSupplementalAgreementSignedDiocese).HasColumnName("church_supplemental_agreement_signed_diocese");
            entity.Property(e => e.ChurchSupplementalAgreementSignedIncomingTrust).HasColumnName("church_supplemental_agreement_signed_incoming_trust");
            entity.Property(e => e.ChurchSupplementalAgreementSignedSecretaryState).HasColumnName("church_supplemental_agreement_signed_secretary_state");
            entity.Property(e => e.ClosureOrTransferDeclarationCleared).HasColumnName("closure_or_transfer_declaration_cleared");
            entity.Property(e => e.ClosureOrTransferDeclarationNotApplicable).HasColumnName("closure_or_transfer_declaration_not_applicable");
            entity.Property(e => e.ClosureOrTransferDeclarationReceived).HasColumnName("closure_or_transfer_declaration_received");
            entity.Property(e => e.ClosureOrTransferDeclarationSaved).HasColumnName("closure_or_transfer_declaration_saved");
            entity.Property(e => e.ClosureOrTransferDeclarationSent).HasColumnName("closure_or_transfer_declaration_sent");
            entity.Property(e => e.CommercialTransferAgreementConfirmAgreed).HasColumnName("commercial_transfer_agreement_confirm_agreed");
            entity.Property(e => e.CommercialTransferAgreementConfirmSigned).HasColumnName("commercial_transfer_agreement_confirm_signed");
            entity.Property(e => e.CommercialTransferAgreementQuestionsChecked)
                .HasDefaultValue(false)
                .HasColumnName("commercial_transfer_agreement_questions_checked");
            entity.Property(e => e.CommercialTransferAgreementQuestionsReceived)
                .HasDefaultValue(false)
                .HasColumnName("commercial_transfer_agreement_questions_received");
            entity.Property(e => e.CommercialTransferAgreementSaveConfirmationEmails).HasColumnName("commercial_transfer_agreement_save_confirmation_emails");
            entity.Property(e => e.ConditionsMetBaselineSheetApproved).HasColumnName("conditions_met_baseline_sheet_approved");
            entity.Property(e => e.ConditionsMetCheckAnyInformationChanged).HasColumnName("conditions_met_check_any_information_changed");
            entity.Property(e => e.ConfirmDateAcademyTransferredDateTransferred).HasColumnName("confirm_date_academy_transferred_date_transferred");
            entity.Property(e => e.ConfirmIncomingTrustHasCompletedAllActionsEmailed).HasColumnName("confirm_incoming_trust_has_completed_all_actions_emailed");
            entity.Property(e => e.ConfirmIncomingTrustHasCompletedAllActionsSaved).HasColumnName("confirm_incoming_trust_has_completed_all_actions_saved");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.DeclarationOfExpenditureCertificateCorrect)
                .HasDefaultValue(false)
                .HasColumnName("declaration_of_expenditure_certificate_correct");
            entity.Property(e => e.DeclarationOfExpenditureCertificateDateReceived).HasColumnName("declaration_of_expenditure_certificate_date_received");
            entity.Property(e => e.DeclarationOfExpenditureCertificateNotApplicable)
                .HasDefaultValue(false)
                .HasColumnName("declaration_of_expenditure_certificate_not_applicable");
            entity.Property(e => e.DeclarationOfExpenditureCertificateSaved)
                .HasDefaultValue(false)
                .HasColumnName("declaration_of_expenditure_certificate_saved");
            entity.Property(e => e.DeedOfNovationAndVariationCleared).HasColumnName("deed_of_novation_and_variation_cleared");
            entity.Property(e => e.DeedOfNovationAndVariationReceived).HasColumnName("deed_of_novation_and_variation_received");
            entity.Property(e => e.DeedOfNovationAndVariationSaveAfterSign).HasColumnName("deed_of_novation_and_variation_save_after_sign");
            entity.Property(e => e.DeedOfNovationAndVariationSaved).HasColumnName("deed_of_novation_and_variation_saved");
            entity.Property(e => e.DeedOfNovationAndVariationSignedIncomingTrust).HasColumnName("deed_of_novation_and_variation_signed_incoming_trust");
            entity.Property(e => e.DeedOfNovationAndVariationSignedOutgoingTrust).HasColumnName("deed_of_novation_and_variation_signed_outgoing_trust");
            entity.Property(e => e.DeedOfNovationAndVariationSignedSecretaryState).HasColumnName("deed_of_novation_and_variation_signed_secretary_state");
            entity.Property(e => e.DeedOfTerminationForTheMasterFundingAgreementCleared).HasColumnName("deed_of_termination_for_the_master_funding_agreement_cleared");
            entity.Property(e => e.DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam).HasColumnName("deed_of_termination_for_the_master_funding_agreement_contact_financial_reporting_team");
            entity.Property(e => e.DeedOfTerminationForTheMasterFundingAgreementNotApplicable).HasColumnName("deed_of_termination_for_the_master_funding_agreement_not_applicable");
            entity.Property(e => e.DeedOfTerminationForTheMasterFundingAgreementReceived).HasColumnName("deed_of_termination_for_the_master_funding_agreement_received");
            entity.Property(e => e.DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint).HasColumnName("deed_of_termination_for_the_master_funding_agreement_saved_academy_and_outgoing_trust_sharepoint");
            entity.Property(e => e.DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder).HasColumnName("deed_of_termination_for_the_master_funding_agreement_saved_in_academy_sharepoint_folder");
            entity.Property(e => e.DeedOfTerminationForTheMasterFundingAgreementSigned).HasColumnName("deed_of_termination_for_the_master_funding_agreement_signed");
            entity.Property(e => e.DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState).HasColumnName("deed_of_termination_for_the_master_funding_agreement_signed_secretary_state");
            entity.Property(e => e.DeedOfVariationCleared).HasColumnName("deed_of_variation_cleared");
            entity.Property(e => e.DeedOfVariationNotApplicable).HasColumnName("deed_of_variation_not_applicable");
            entity.Property(e => e.DeedOfVariationReceived).HasColumnName("deed_of_variation_received");
            entity.Property(e => e.DeedOfVariationSaved).HasColumnName("deed_of_variation_saved");
            entity.Property(e => e.DeedOfVariationSent).HasColumnName("deed_of_variation_sent");
            entity.Property(e => e.DeedOfVariationSigned).HasColumnName("deed_of_variation_signed");
            entity.Property(e => e.DeedOfVariationSignedSecretaryState).HasColumnName("deed_of_variation_signed_secretary_state");
            entity.Property(e => e.DeedTerminationChurchAgreementCleared).HasColumnName("deed_termination_church_agreement_cleared");
            entity.Property(e => e.DeedTerminationChurchAgreementNotApplicable).HasColumnName("deed_termination_church_agreement_not_applicable");
            entity.Property(e => e.DeedTerminationChurchAgreementReceived).HasColumnName("deed_termination_church_agreement_received");
            entity.Property(e => e.DeedTerminationChurchAgreementSaved).HasColumnName("deed_termination_church_agreement_saved");
            entity.Property(e => e.DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState).HasColumnName("deed_termination_church_agreement_saved_after_signing_by_secretary_state");
            entity.Property(e => e.DeedTerminationChurchAgreementSignedDiocese).HasColumnName("deed_termination_church_agreement_signed_diocese");
            entity.Property(e => e.DeedTerminationChurchAgreementSignedOutgoingTrust).HasColumnName("deed_termination_church_agreement_signed_outgoing_trust");
            entity.Property(e => e.DeedTerminationChurchAgreementSignedSecretaryState).HasColumnName("deed_termination_church_agreement_signed_secretary_state");
            entity.Property(e => e.FinancialSafeguardingGovernanceIssues)
                .HasDefaultValue(false)
                .HasColumnName("financial_safeguarding_governance_issues");
            entity.Property(e => e.FormMCleared).HasColumnName("form_m_cleared");
            entity.Property(e => e.FormMNotApplicable).HasColumnName("form_m_not_applicable");
            entity.Property(e => e.FormMReceivedFormM).HasColumnName("form_m_received_form_m");
            entity.Property(e => e.FormMReceivedTitlePlans).HasColumnName("form_m_received_title_plans");
            entity.Property(e => e.FormMSaved).HasColumnName("form_m_saved");
            entity.Property(e => e.FormMSigned).HasColumnName("form_m_signed");
            entity.Property(e => e.HandoverMeeting).HasColumnName("handover_meeting");
            entity.Property(e => e.HandoverNotApplicable).HasColumnName("handover_not_applicable");
            entity.Property(e => e.HandoverNotes).HasColumnName("handover_notes");
            entity.Property(e => e.HandoverReview).HasColumnName("handover_review");
            entity.Property(e => e.InadequateOfsted)
                .HasDefaultValue(false)
                .HasColumnName("inadequate_ofsted");
            entity.Property(e => e.LandConsentLetterDrafted).HasColumnName("land_consent_letter_drafted");
            entity.Property(e => e.LandConsentLetterNotApplicable).HasColumnName("land_consent_letter_not_applicable");
            entity.Property(e => e.LandConsentLetterSaved).HasColumnName("land_consent_letter_saved");
            entity.Property(e => e.LandConsentLetterSent).HasColumnName("land_consent_letter_sent");
            entity.Property(e => e.LandConsentLetterSigned).HasColumnName("land_consent_letter_signed");
            entity.Property(e => e.MasterFundingAgreementCleared).HasColumnName("master_funding_agreement_cleared");
            entity.Property(e => e.MasterFundingAgreementNotApplicable).HasColumnName("master_funding_agreement_not_applicable");
            entity.Property(e => e.MasterFundingAgreementReceived).HasColumnName("master_funding_agreement_received");
            entity.Property(e => e.MasterFundingAgreementSaved).HasColumnName("master_funding_agreement_saved");
            entity.Property(e => e.MasterFundingAgreementSigned).HasColumnName("master_funding_agreement_signed");
            entity.Property(e => e.MasterFundingAgreementSignedSecretaryState).HasColumnName("master_funding_agreement_signed_secretary_state");
            entity.Property(e => e.OutgoingTrustToClose)
                .HasDefaultValue(false)
                .HasColumnName("outgoing_trust_to_close");
            entity.Property(e => e.RedactAndSendDocumentsRedact).HasColumnName("redact_and_send_documents_redact");
            entity.Property(e => e.RedactAndSendDocumentsSaved).HasColumnName("redact_and_send_documents_saved");
            entity.Property(e => e.RedactAndSendDocumentsSendToEsfa).HasColumnName("redact_and_send_documents_send_to_esfa");
            entity.Property(e => e.RedactAndSendDocumentsSendToFundingTeam).HasColumnName("redact_and_send_documents_send_to_funding_team");
            entity.Property(e => e.RedactAndSendDocumentsSendToSolicitors).HasColumnName("redact_and_send_documents_send_to_solicitors");
            entity.Property(e => e.RequestNewUrnAndRecordComplete).HasColumnName("request_new_urn_and_record_complete");
            entity.Property(e => e.RequestNewUrnAndRecordGive).HasColumnName("request_new_urn_and_record_give");
            entity.Property(e => e.RequestNewUrnAndRecordNotApplicable).HasColumnName("request_new_urn_and_record_not_applicable");
            entity.Property(e => e.RequestNewUrnAndRecordReceive).HasColumnName("request_new_urn_and_record_receive");
            entity.Property(e => e.RpaPolicyConfirm).HasColumnName("rpa_policy_confirm");
            entity.Property(e => e.SponsoredSupportGrantNotApplicable)
                .HasDefaultValue(false)
                .HasColumnName("sponsored_support_grant_not_applicable");
            entity.Property(e => e.SponsoredSupportGrantType)
                .HasMaxLength(4000)
                .HasColumnName("sponsored_support_grant_type");
            entity.Property(e => e.StakeholderKickOffIntroductoryEmails).HasColumnName("stakeholder_kick_off_introductory_emails");
            entity.Property(e => e.StakeholderKickOffMeeting).HasColumnName("stakeholder_kick_off_meeting");
            entity.Property(e => e.StakeholderKickOffSetupMeeting).HasColumnName("stakeholder_kick_off_setup_meeting");
            entity.Property(e => e.SupplementalFundingAgreementCleared).HasColumnName("supplemental_funding_agreement_cleared");
            entity.Property(e => e.SupplementalFundingAgreementReceived).HasColumnName("supplemental_funding_agreement_received");
            entity.Property(e => e.SupplementalFundingAgreementSaved).HasColumnName("supplemental_funding_agreement_saved");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F2884685E");

            entity.ToTable("users", "complete");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ActiveDirectoryUserGroupIds)
                .HasMaxLength(4000)
                .HasColumnName("active_directory_user_group_ids");
            entity.Property(e => e.ActiveDirectoryUserId)
                .HasMaxLength(4000)
                .HasColumnName("active_directory_user_id");
            entity.Property(e => e.AddNewProject).HasColumnName("add_new_project");
            entity.Property(e => e.AssignToProject)
                .HasDefaultValue(false)
                .HasColumnName("assign_to_project");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(6)
                .HasColumnName("created_at");
            entity.Property(e => e.DeactivatedAt)
                .HasPrecision(6)
                .HasColumnName("deactivated_at");
            entity.Property(e => e.Email)
                .HasMaxLength(4000)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(4000)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(4000)
                .HasColumnName("last_name");
            entity.Property(e => e.LatestSession)
                .HasPrecision(6)
                .HasColumnName("latest_session");
            entity.Property(e => e.ManageConversionUrns)
                .HasDefaultValue(false)
                .HasColumnName("manage_conversion_urns");
            entity.Property(e => e.ManageLocalAuthorities)
                .HasDefaultValue(false)
                .HasColumnName("manage_local_authorities");
            entity.Property(e => e.ManageTeam)
                .HasDefaultValue(false)
                .HasColumnName("manage_team");
            entity.Property(e => e.ManageUserAccounts)
                .HasDefaultValue(false)
                .HasColumnName("manage_user_accounts");
            entity.Property(e => e.Team)
                .HasMaxLength(4000)
                .HasColumnName("team");
            entity.Property(e => e.UpdatedAt)
                .HasPrecision(6)
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
