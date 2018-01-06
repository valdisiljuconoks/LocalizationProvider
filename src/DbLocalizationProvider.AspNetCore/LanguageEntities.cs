using Microsoft.EntityFrameworkCore;

namespace DbLocalizationProvider.AspNetCore
{
    public class LanguageEntities : DbContext
    {
        private readonly string _connectionString;

        public LanguageEntities() : this(ConfigurationContext.Current.DbContextConnectionString)
        {
        }

        public LanguageEntities(DbContextOptions<LanguageEntities> options) : base(options)
        {
        }

        public LanguageEntities(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DbSet<LocalizationResource> LocalizationResources { get; set; }

        public virtual DbSet<LocalizationResourceTranslation> LocalizationResourceTranslations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var resource = builder.Entity<LocalizationResource>();
            resource.HasKey(r => r.Id);
            resource.Property(r => r.ResourceKey)
                .IsRequired()
                .HasMaxLength(1700);

            resource.Property(r => r.Author)
                .HasMaxLength(100);

            resource.HasIndex(r => r.ResourceKey)
                .HasName("IX_ResourceKey")
                .IsUnique();

            resource.HasMany(r => r.Translations)
                .WithOne(t => t.LocalizationResource)
                .IsRequired(false);

            var translation = builder.Entity<LocalizationResourceTranslation>();
            translation.HasKey(t => t.Id);
            translation.HasOne(t => t.LocalizationResource)
                .WithMany(r => r.Translations)
                .HasForeignKey(t => t.ResourceId);

            translation.Property(t => t.ResourceId).IsRequired();
            translation.Property(t => t.Language)
                .HasMaxLength(10);
        }
    }
}
