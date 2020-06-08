// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Npgsql;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    public class ResourceSynchronizer : IQueryHandler<SyncResources.Query, IEnumerable<LocalizationResource>>
    {
        public IEnumerable<LocalizationResource> Execute(SyncResources.Query query)
        {
            ConfigurationContext.Current.Logger?.Debug("Starting to sync resources...");
            var sw = new Stopwatch();
            sw.Start();

            var discoveredResources = query.DiscoveredResources;
            var discoveredModels = query.DiscoveredModels;

            ResetSyncStatus();

            var allResources = new GetAllResources.Query(true).Execute();
            Parallel.Invoke(() => RegisterDiscoveredResources(discoveredResources, allResources),
                () => RegisterDiscoveredResources(discoveredModels, allResources));

            var result = MergeLists(allResources, discoveredResources.ToList(), discoveredModels.ToList());
            sw.Stop();

            ConfigurationContext.Current.Logger?.Debug($"Resource synchronization took: {sw.ElapsedMilliseconds}ms");

            return result;
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
                                Language = t.Culture,
                                Value = t.Translation
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
                                    Language = translation.Culture,
                                    Value = translation.Translation
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
            using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
            {
                var cmd = new NpgsqlCommand(@"UPDATE public.""LocalizationResources"" SET ""FromCode"" = '0'", conn);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void RegisterDiscoveredResources(ICollection<DiscoveredResource> properties, IEnumerable<LocalizationResource> allResources)
        {
            // split work queue by 400 resources each
            var groupedProperties = properties.SplitByCount(400);

            Parallel.ForEach(groupedProperties, group =>
                             {
                                 var sb = new StringBuilder();
                                 sb.AppendLine("DO $$");
                                 sb.AppendLine("DECLARE resourceId integer;");
                                 sb.AppendLine("BEGIN");

                                 var refactoredResources = group.Where(r => !string.IsNullOrEmpty(r.OldResourceKey));
                                 foreach (var refactoredResource in refactoredResources)
                                 {
                                     sb.Append($@"
        IF EXISTS(SELECT 1 FROM public.""LocalizationResources"" WHERE ""ResourceKey"" = '{refactoredResource.OldResourceKey}') THEN
            UPDATE public.""LocalizationResources"" SET ""ResourceKey"" = '{refactoredResource.Key}', ""FromCode"" = '1' WHERE ""ResourceKey"" = '{refactoredResource.OldResourceKey}';
        END IF;
        ");
                                 }

                                 foreach (var property in group)
                                 {
                                     var existingResource = allResources.FirstOrDefault(r => r.ResourceKey == property.Key);

                                     if (existingResource == null)
                                     {
                                         sb.Append($@"
        resourceId := coalesce((SELECT ""Id"" FROM public.""LocalizationResources"" WHERE ""ResourceKey"" = '{property.Key}'), -1);
        IF resourceId = -1 THEN 
            INSERT INTO public.""LocalizationResources"" (""ResourceKey"", ""ModificationDate"", ""Author"", ""FromCode"", ""IsModified"", ""IsHidden"") VALUES ('{property.Key}', CAST(NOW() at time zone 'utc' AS timestamp), 'type-scanner', '1', '0', '{Convert.ToInt32(property.IsHidden)}');
            resourceId := LASTVAL();");

                                         // add all translations
                                         foreach (var propertyTranslation in property.Translations)
                                         {
                                             sb.Append($@"
            INSERT INTO public.""LocalizationResourceTranslations"" (""ResourceId"", ""Language"", ""Value"", ""ModificationDate"") VALUES (resourceId, '{propertyTranslation.Culture}', N'{propertyTranslation.Translation.Replace("'", "''")}', CAST(NOW() at time zone 'utc' AS timestamp));");
                                         }

                                         sb.Append(@"
        END IF;
        ");
                                     }

                                     if (existingResource != null)
                                     {
                                         sb.AppendLine($@"UPDATE public.""LocalizationResources"" SET ""FromCode"" = '1', ""IsHidden"" = '{Convert.ToInt32(property.IsHidden)}' where ""Id"" = {existingResource.Id};");

                                         var invariantTranslation = property.Translations.First(t => t.Culture == string.Empty);
                                         sb.AppendLine($@"UPDATE public.""LocalizationResourceTranslations"" SET ""Value"" = N'{invariantTranslation.Translation.Replace("'", "''")}' where ""ResourceId""={existingResource.Id} AND ""Language""='{invariantTranslation.Culture}';");

                                         if (existingResource.IsModified.HasValue && !existingResource.IsModified.Value)
                                         {
                                             foreach (var propertyTranslation in property.Translations)
                                                 AddTranslationScript(existingResource, sb, propertyTranslation);
                                         }
                                     }
                                 }

                                 sb.AppendLine("END $$;");

                                 using (var conn = new NpgsqlConnection(Settings.DbContextConnectionString))
                                 {
                                     var cmd = new NpgsqlCommand(sb.ToString(), conn)
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
            if (existingTranslation == null)
            {
                buffer.AppendLine($@"INSERT INTO public.""LocalizationResourceTranslations"" (""ResourceId"", ""Language"", ""Value"", ""ModificationDate"") VALUES ({existingResource.Id}, '{resource.Culture}', N'{resource.Translation.Replace("'", "''")}',  CAST(NOW() at time zone 'utc' AS timestamp));");
            }
            else if (!existingTranslation.Value.Equals(resource.Translation))
            {
                buffer.AppendLine($@"UPDATE public.""LocalizationResourceTranslations"" SET ""Value"" = N'{resource.Translation.Replace("'", "''")}' WHERE ResourceId={existingResource.Id} and ""Language""='{resource.Culture}';");
            }
        }
    }
}
