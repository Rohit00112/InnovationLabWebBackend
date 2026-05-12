using InnovationLab.Landing.Models;
using InnovationLab.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace InnovationLab.Landing.DbContexts;

public class LandingDbContext(DbContextOptions<LandingDbContext> options) : DbContext(options)
{
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventGallery> EventGallery { get; set; }
    public DbSet<EventAgenda> EventAgendas { get; set; }
    public DbSet<AgendaItem> AgendaItems { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Faq> Faqs { get; set; }
    public DbSet<About> About { get; set; }
    public DbSet<CoreValue> CoreValues { get; set; }
    public DbSet<JourneyItem> JourneyItems { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(DatabaseSchemas.LandingSchema);
        base.OnModelCreating(builder);
    }
}