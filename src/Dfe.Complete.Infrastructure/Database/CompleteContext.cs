using Dfe.Complete.Domain.Contacts;
using Dfe.Complete.Domain.Entities.Projects;
using Dfe.Complete.Domain.Entities.Schools;
using Dfe.Complete.Domain.Notes;
using Dfe.Complete.Domain.Users;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.Complete.Infrastructure.Database;

public class CompleteContext : DbContext
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


    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Note> Notes { get; set; } = null!;
    public DbSet<Contact> Contacts { get; set; } = null!;


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
        modelBuilder.Entity<Project>(ConfigureProject);
        modelBuilder.Entity<User>(ConfigureUser);
        modelBuilder.Entity<Note>(ConfigureNotes);
        modelBuilder.Entity<Contact>(ConfigureContact);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureProject(EntityTypeBuilder<Project> projectConfiguration)
    {
        projectConfiguration.HasKey(s => s.Id);
        projectConfiguration.ToTable("Projects", DefaultSchema);
        projectConfiguration.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v!.Value,
                v => new ProjectId(v));

        projectConfiguration.Property(e => e.Urn)
           .HasConversion(
               v => v!.Value,
               v => new Urn(v));

        projectConfiguration.Property(e => e.TeamLeaderId)
           .HasConversion(
               v => v!.Value,
               v => new UserId(v));

        projectConfiguration.Property(e => e.IncomingTrustUkprn)
          .HasConversion(
              v => v!.Value,
              v => new Ukprn(v));

        projectConfiguration.Property(e => e.RegionalDeliveryOfficerId)
          .HasConversion(
              v => v!.Value,
              v => new UserId(v));

        projectConfiguration.Property(e => e.CaseworkerId)
          .HasConversion(
              v => v!.Value,
              v => new UserId(v));

        projectConfiguration.Property(e => e.AssignedToId)
          .HasConversion(
              v => v!.Value,
              v => new UserId(v));

        projectConfiguration.Property(e => e.AcademyUrn)
          .HasConversion(
              v => v!.Value,
              v => new Urn(v));

        projectConfiguration.Property(e => e.OutgoingTrustUkprn)
          .HasConversion(
              v => v!.Value,
              v => new Ukprn(v));

        projectConfiguration.Property(e => e.MainContactId)
          .HasConversion(
              v => v!.Value,
              v => new UserId(v));

        projectConfiguration.Property(e => e.EstablishmentMainContactId)
          .HasConversion(
              v => v!.Value,
              v => new UserId(v));

        projectConfiguration.Property(e => e.IncomingTrustMainContactId)
          .HasConversion(
              v => v!.Value,
              v => new UserId(v));

        projectConfiguration.Property(e => e.OutgoingTrustMainContactId)
          .HasConversion(
              v => v!.Value,
              v => new UserId(v));

        projectConfiguration.Property(e => e.ChairOfGovernorsContactId)
          .HasConversion(
              v => v!.Value,
              v => new UserId(v));

        //projectConfiguration
        //    .HasOne(c => c.TeamLeaderId)
        //    .WithOne()
        //    .HasForeignKey<User>(c => c.Id);

        projectConfiguration
            .HasOne(c => c.MainContactId)
            .WithOne()
            .HasForeignKey<User>(c => c.Id);

        projectConfiguration
            .HasOne(c => c.EstablishmentMainContactId)
            .WithOne()
            .HasForeignKey<User>(c => c.Id);


        projectConfiguration
            .HasOne(c => c.IncomingTrustMainContactId)
            .WithOne()
            .HasForeignKey<User>(c => c.Id);


        projectConfiguration
            .HasOne(c => c.OutgoingTrustMainContactId)
            .WithOne()
            .HasForeignKey<User>(c => c.Id);


        projectConfiguration
            .HasOne(c => c.ChairOfGovernorsContactId)
            .WithOne()
            .HasForeignKey<User>(c => c.Id);
    }

    private static void ConfigureUser(EntityTypeBuilder<User> projectConfiguration)
    {
        projectConfiguration.HasKey(s => s.Id);
        projectConfiguration.ToTable("User", DefaultSchema);
        projectConfiguration.Property(e => e.Id)
            //.ValueGeneratedOnAdd() Is this auto gen
            .HasConversion(
                v => v!.Value,
                v => new UserId(v));

        projectConfiguration
            .HasMany(c => c.Notes)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);

        projectConfiguration
            .HasMany(c => c.ProjectAssignedTos)
            .WithOne(e => e.AssignedTo)
            .HasForeignKey(e => e.AssignedToId);
    }

    private static void ConfigureNotes(EntityTypeBuilder<Note> projectConfiguration)
    {
        projectConfiguration.HasKey(s => s.Id);
        projectConfiguration.ToTable("Note", DefaultSchema);
        projectConfiguration.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v!.Value,
                v => new NoteId(v));
    }

    private static void ConfigureContact(EntityTypeBuilder<Contact> projectConfiguration)
    {
        projectConfiguration.HasKey(s => s.Id);
        projectConfiguration.ToTable("Contact", DefaultSchema);
        projectConfiguration.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v!.Value,
                v => new ContactId(v));
    }

}
