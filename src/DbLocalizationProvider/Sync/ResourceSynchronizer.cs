using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Sync
{
    public class ResourceSynchronizer
    {
        public void DiscoverAndRegister()
        {
            if(!ConfigurationContext.Current.DiscoverAndRegisterResources)
                return;

            var discoveredTypes = TypeDiscoveryHelper.GetTypes(t => t.GetCustomAttribute<LocalizedResourceAttribute>() != null,
                                                               t => t.GetCustomAttribute<LocalizedModelAttribute>() != null);

            var discoveredResources = discoveredTypes[0];
            var foreignResources = ConfigurationContext.Current.ForeignResources;
            if(foreignResources != null && foreignResources.Any())
            {
                discoveredResources.AddRange(foreignResources.Select(x => x.ResourceType));
            }

            // initialize db structures first (issue #53)
            using (var ctx = new LanguageEntities())
            {
                var tmp = ctx.LocalizationResources.FirstOrDefault();
            }

            ResetSyncStatus();

            Parallel.Invoke(() => RegisterDiscoveredResources(discoveredResources),
                            () => RegisterDiscoveredResources(discoveredTypes[1]));

            if(ConfigurationContext.Current.PopulateCacheOnStartup)
                PopulateCache();
        }

        public void RegisterManually(IEnumerable<ManualResource> resources)
        {
            using (var db = new LanguageEntities())
            {
                var defaultCulture = new DetermineDefaultCulture.Query().Execute();

                foreach (var resource in resources)
                    RegisterIfNotExist(db, resource.Key, resource.Translation, defaultCulture, "manual");

                db.SaveChanges();
            }
        }

        private void PopulateCache()
        {
            var c = new ClearCache.Command();
            c.Execute();

            var allResources = new GetAllResources.Query().Execute();

            foreach (var resource in allResources)
            {
                var key = CacheKeyHelper.BuildKey(resource.ResourceKey);
                ConfigurationContext.Current.CacheManager.Insert(key, resource);
            }
        }

        private void ResetSyncStatus()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[ConfigurationContext.Current.ConnectionName].ConnectionString))
            {
                var cmd = new SqlCommand("UPDATE dbo.LocalizationResources SET FromCode = 0", conn);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void RegisterDiscoveredResources(IEnumerable<Type> types)
        {
            var helper = new TypeDiscoveryHelper();
            var properties = types.SelectMany(type => helper.ScanResources(type)).DistinctBy(r => r.Key);

            var allResources = new GetAllResources.Query().Execute();

            // split work queue by 400 resources each
            var groupedProperties = properties.SplitByCount(400);

            Parallel.ForEach(groupedProperties,
                             group =>
                             {
                                 var sb = new StringBuilder();
                                 sb.AppendLine("declare @resourceId int");

                                 foreach (var property in group)
                                 {
                                     var existingResource = allResources.FirstOrDefault(r => r.ResourceKey == property.Key);

                                     if(existingResource == null)
                                     {
                                         sb.Append($@"
set @resourceId = isnull((select id from localizationresources where [resourcekey] = '{property.Key}'), -1)
if (@resourceId = -1)
begin
    insert into localizationresources ([resourcekey], modificationdate, author, fromcode, ismodified, ishidden)
    values ('{property.Key}', getutcdate(), 'type-scanner', 1, 0, {Convert.ToInt32(property.IsHidden)})
    set @resourceId = SCOPE_IDENTITY()");

                                         // add all translations
                                         foreach (var propertyTranslation in property.Translations)
                                         {
                                             sb.Append($@"
    insert into localizationresourcetranslations (resourceid, [language], [value]) values (@resourceId, '{propertyTranslation.Culture}', N'{propertyTranslation.Translation.Replace("'", "''")}')
");
                                         }


                                         sb.Append(@"
end
");
                                     }

                                     if(existingResource != null)
                                     {
                                         sb.AppendLine($"update localizationresources set fromcode = 1, ishidden = {Convert.ToInt32(property.IsHidden)} where [id] = {existingResource.Id}");

                                         if(existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                                         {
                                             foreach (var propertyTranslation in property.Translations)
                                                 AddTranslationScript(existingResource, sb, propertyTranslation);
                                         }
                                     }
                                 }

                                 using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[ConfigurationContext.Current.ConnectionName].ConnectionString))
                                 {
                                     var cmd = new SqlCommand(sb.ToString(), conn) { CommandTimeout = 60 };

                                     conn.Open();
                                     cmd.ExecuteNonQuery();
                                     conn.Close();
                                 }
                             });
        }

        private static void AddTranslationScript(LocalizationResource existingResource, StringBuilder buffer, DiscoveredTranslation resource)
        {
            var existingTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == resource.Culture);
            if(existingTranslation == null)
            {
                buffer.Append($@"
insert into localizationresourcetranslations (resourceid, [language], [value]) values ({existingResource.Id}, '{resource.Culture}', N'{resource.Translation.Replace("'", "''")}')
");
            }
            else if(!existingTranslation.Value.Equals(resource.Translation))
            {
                buffer.Append($@"
update localizationresourcetranslations set [value] = N'{resource.Translation.Replace("'", "''")}' where resourceid={existingResource.Id} and [language]='{resource.Culture}'
");
            }
        }

        private void RegisterIfNotExist(LanguageEntities db, string resourceKey, string resourceValue, string defaultCulture, string author = "type-scanner")
        {
            var existingResource = db.LocalizationResources.Include(r => r.Translations).FirstOrDefault(r => r.ResourceKey == resourceKey);

            if(existingResource != null)
            {
                existingResource.FromCode = true;

                // if resource is not modified - we can sync default value from code
                if(existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                {
                    existingResource.ModificationDate = DateTime.UtcNow;
                    var defaultTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == defaultCulture);
                    if(defaultTranslation != null)
                    {
                        defaultTranslation.Value = resourceValue;
                    }
                }

                var fromCodeTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == ConfigurationContext.CultureForTranslationsFromCode);
                if(fromCodeTranslation != null)
                {
                    fromCodeTranslation.Value = resourceValue;
                }
                else
                {
                    fromCodeTranslation = new LocalizationResourceTranslation
                    {
                        Language = ConfigurationContext.CultureForTranslationsFromCode,
                        Value = resourceValue
                    };

                    existingResource.Translations.Add(fromCodeTranslation);
                }
            }
            else
            {
                // create new resource
                var resource = new LocalizationResource(resourceKey)
                {
                    ModificationDate = DateTime.UtcNow,
                    Author = author,
                    FromCode = true,
                    IsModified = false
                };

                resource.Translations.Add(new LocalizationResourceTranslation
                {
                    Language = defaultCulture,
                    Value = resourceValue
                });

                resource.Translations.Add(new LocalizationResourceTranslation
                {
                    Language = ConfigurationContext.CultureForTranslationsFromCode,
                    Value = resourceValue
                });

                db.LocalizationResources.Add(resource);
            }
        }
    }
}
