// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using DbLocalizationProvider.Migrations;

namespace DbLocalizationProvider
{
    public class LanguageEntities : DbContext
    {
        public LanguageEntities() : this(ConfigurationContext.Current.DbContextConnectionString) { }

        public LanguageEntities(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LanguageEntities, Configuration>());
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

            Database.Initialize(false);
        }

        public virtual DbSet<LocalizationResource> LocalizationResources { get; set; }

        public virtual DbSet<LocalizationResourceTranslation> LocalizationResourceTranslations { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            var resource = builder.Entity<LocalizationResource>();
            resource.HasKey(r => r.Id);
            resource.Property(r => r.ResourceKey)
                    .IsRequired()
                    .HasMaxLength(1700)
                    .HasColumnType("VARCHAR")
                    .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                                         new IndexAnnotation(new IndexAttribute("IX_ResourceKey", 1)
                                                             {
                                                                 IsUnique = true
                                                             }));
            resource.HasMany(r => r.Translations)
                    .WithOptional()
                    .HasForeignKey(t => t.ResourceId);

            var translation = builder.Entity<LocalizationResourceTranslation>();
            translation.HasKey(t => t.Id);
            translation.Property(t => t.ResourceId).IsRequired();
            translation.Property(t => t.Language)
                       .HasColumnType("VARCHAR")
                       .HasMaxLength(10);
        }
    }
}
