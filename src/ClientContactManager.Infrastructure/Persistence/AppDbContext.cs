using Microsoft.EntityFrameworkCore;
using ClientContactManager.Domain.Entities;
using ClientContactManager.Domain.Interfaces;

namespace ClientContactManager.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<ClientContact> ClientContacts => Set<ClientContact>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ClientCode).IsRequired().HasMaxLength(6);
            entity.HasIndex(e => e.ClientCode).IsUnique().HasDatabaseName("IX_Clients_ClientCode");
            entity.HasIndex(e => e.Name).HasDatabaseName("IX_Clients_Name");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contacts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Surname).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(254);
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Contacts_Email");
            entity.HasIndex(e => new { e.Surname, e.Name }).HasDatabaseName("IX_Contacts_FullName");
            entity.Ignore(e => e.FullName);
        });

        modelBuilder.Entity<ClientContact>(entity =>
        {
            entity.ToTable("ClientContacts");
            entity.HasKey(e => new { e.ClientId, e.ContactId });

            entity.HasOne(e => e.Client)
                .WithMany(c => c.ClientContacts)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Contact)
                .WithMany(c => c.ClientContacts)
                .HasForeignKey(e => e.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ContactId).HasDatabaseName("IX_ClientContacts_ContactId");
        });
    }
}
