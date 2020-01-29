// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public class CreateOrUpdateTranslationHandler : ICommandHandler<CreateOrUpdateTranslation.Command>
    {
        public void Execute(CreateOrUpdateTranslation.Command command)
        {
            var repository = new ResourceRepository();
            var resource = repository.GetByKey(command.Key);

            if(resource == null) return;

            var translation = resource.Translations.FirstOrDefault(t => t.Language == command.Language.Name);
            if(translation == null)
            {
                var newTranslation = new LocalizationResourceTranslation
                {
                    Value = command.Translation,
                    Language = command.Language.Name,
                    ResourceId = resource.Id
                };

                repository.AddTranslation(resource, newTranslation);
            }
            else
            {
                translation.Value = command.Translation;
                repository.UpdateTranslation(resource, translation);
            }

            resource.ModificationDate = DateTime.UtcNow;
            resource.IsModified = true;

            repository.UpdateResource(resource);

            ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
