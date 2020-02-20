// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.Data.SqlClient;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public class ResourceSynchronizer : IQueryHandler<SyncResources.Query, IEnumerable<LocalizationResource>>
    {
        public IEnumerable<LocalizationResource> Execute(SyncResources.Query query)
        {
            ConfigurationContext.Current.Logger?.Info("Starting to sync resources...");

            var discoveredResources = query.DiscoveredResources;
            var discoveredModels = query.DiscoveredModels;

            ResetSyncStatus();

            var allResources = new GetAllResources.Query(true).Execute();
            Parallel.Invoke(() => RegisterDiscoveredResources(discoveredResources, allResources),
                () => RegisterDiscoveredResources(discoveredModels, allResources));

            return MergeLists(allResources, discoveredResources.ToList(), discoveredModels.ToList());
        }

        internal IEnumerable<LocalizationResource> MergeLists(IEnumerable<LocalizationResource> databaseResources,
            List<DiscoveredResource> discoveredResources,
            List<DiscoveredResource> discoveredModels)
        {
            if (discoveredResources == null || discoveredModels == null || !discoveredResources.Any() || !discoveredModels.Any())
                return databaseResources;

            var result = new List<LocalizationResource>(databaseResources);
            var dic = result.ToDictionary(r => r.ResourceKey, r => r);

            // run through resources
            CompareAndMerge(ref discoveredResources, dic, ref result);
            CompareAndMerge(ref discoveredModels, dic, ref result);

            return result;
        }

        private static void CompareAndMerge(ref List<DiscoveredResource> discoveredResources,
            Dictionary<string, LocalizationResource> dic,
            ref List<LocalizationResource> result)
        {
            while (discoveredResources.Count > 0)
            {
                var discoveredResource = discoveredResources[0];
                if (!dic.ContainsKey(discoveredResource.Key))
                {
                    // there is no resource by this key in db - we can safely insert
                    result.Add(new LocalizationResource(discoveredResource.Key)
                    {
                        Translations = discoveredResource.Translations.Select(t =>
                            new LocalizationResourceTranslation
                            {
                                Language = t.Culture, Value = t.Translation
                            }).ToList()
                    });
                }
                else
                {
                    // resource exists in db - we need to merge only unmodified translations
                    var existingRes = dic[discoveredResource.Key];
                    if (!existingRes.IsModified.HasValue || !existingRes.IsModified.Value)
                    {
                        // resource is unmodified in db - overwrite
                        foreach (var translation in discoveredResource.Translations)
                        {
                            var t = existingRes.Translations.FindByLanguage(translation.Culture);
                            if (t == null)
                            {
                                existingRes.Translations.Add(new LocalizationResourceTranslation
                                {
                                    Language = translation.Culture, Value = translation.Translation
                                });
                            }
                            else
                            {
                                t.Language = translation.Culture;
                                t.Value = translation.Translation;
                            }
                        }
                    }
                    else
                    {
                        // resource exists in db, is modified - we need to update only invariant translation
                        var t = existingRes.Translations.FindByLanguage(CultureInfo.InvariantCulture);
                        var invariant = discoveredResource.Translations.FirstOrDefault(t2 => t.Language == string.Empty);
                        if (t != null && invariant != null)
                        {
                            t.Language = invariant.Culture;
                            t.Value = invariant.Translation;
                        }
                    }
                }

                discoveredResources.Remove(discoveredResource);
            }
        }

        private void ResetSyncStatus()
        {
            using(var conn = new SqlConnection(Settings.DbContextConnectionString))
            {
                var cmd = new SqlCommand("UPDATE [dbo].[LocalizationResources] SET FromCode = 0", conn);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void RegisterDiscoveredResources(ICollection<DiscoveredResource> properties, IEnumerable<LocalizationResource> allResources)
        {
            // split work queue by 400 resources each
            var groupedProperties = properties.SplitByCount(400);

            Parallel.ForEach(groupedProperties,
                             group =>
                             {
                                 var sb = new StringBuilder();
                                 sb.AppendLine("declare @resourceId int");

                                 var refactoredResources = group.Where(r => !string.IsNullOrEmpty(r.OldResourceKey));
                                 foreach(var refactoredResource in refactoredResources)
                                 {
                                     sb.Append($@"
        if exists(select 1 from LocalizationResources with(nolock) where ResourceKey = '{refactoredResource.OldResourceKey}')
        begin
            update dbo.LocalizationResources set ResourceKey = '{refactoredResource.Key}', FromCode = 1 where ResourceKey = '{refactoredResource.OldResourceKey}'
        end
        ");
                                 }

                                 foreach(var property in group)
                                 {
                                     var existingResource = allResources.FirstOrDefault(r => r.ResourceKey == property.Key);

                                     if(existingResource == null)
                                     {
                                         sb.Append($@"
        set @resourceId = isnull((select id from LocalizationResources where [ResourceKey] = '{property.Key}'), -1)
        if (@resourceId = -1)
        begin
            insert into LocalizationResources ([ResourceKey], ModificationDate, Author, FromCode, IsModified, IsHidden)
            values ('{property.Key}', getutcdate(), 'type-scanner', 1, 0, {Convert.ToInt32(property.IsHidden)})
            set @resourceId = SCOPE_IDENTITY()");

                                                 // add all translations
                                                 foreach(var propertyTranslation in property.Translations)
                                         {
                                             sb.Append($@"
            insert into LocalizationResourceTranslations (ResourceId, [Language], [Value]) values (@resourceId, '{propertyTranslation.Culture}', N'{
                                                               propertyTranslation.Translation.Replace("'", "''")
                                                           }')
        ");
                                         }

                                         sb.Append(@"
        end
        ");
                                     }

                                     if(existingResource != null)
                                     {
                                         sb.AppendLine($"update LocalizationResources set FromCode = 1, IsHidden = {Convert.ToInt32(property.IsHidden)} where [Id] = {existingResource.Id}");

                                         var invariantTranslation = property.Translations.First(t => t.Culture == string.Empty);
                                         sb.AppendLine($"update LocalizationResourceTranslations set [Value] = N'{invariantTranslation.Translation.Replace("'", "''")}' where ResourceId={existingResource.Id} and [Language]='{invariantTranslation.Culture}'");

                                         if(existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                                         {
                                             foreach(var propertyTranslation in property.Translations)
                                                 AddTranslationScript(existingResource, sb, propertyTranslation);
                                         }
                                     }
                                 }

                                 using(var conn = new SqlConnection(Settings.DbContextConnectionString))
                                 {
                                     var cmd = new SqlCommand(sb.ToString(), conn)
                                     {
                                         CommandTimeout = 60
                                     };

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
        INSERT INTO [dbo].[LocalizationResourceTranslations] (ResourceId, [Language], [Value]) VALUES ({existingResource.Id}, '{resource.Culture}', N'{resource.Translation.Replace("'", "''")}')");
            }
            else if(!existingTranslation.Value.Equals(resource.Translation))
            {
                buffer.Append($@"
        UPDATE [dbo].[LocalizationResourceTranslations] SET [Value] = N'{resource.Translation.Replace("'", "''")}' WHERE ResourceId={existingResource.Id} and [Language]='{resource.Culture}'");
            }
        }
    }
}
