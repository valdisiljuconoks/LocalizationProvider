// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Commands
{
    /// <summary>
    /// Implementation for creating or updating existing translation
    /// </summary>
    public class CreateOrUpdateTranslationHandler : ICommandHandler<CreateOrUpdateTranslation.Command>
    {
        private readonly ConfigurationContext _configurationContext;
        private readonly IResourceRepository _repository;

        /// <summary>
        /// Creates new instance of the class.
        /// </summary>
        /// <param name="configurationContext">Configuration settings.</param>
        /// <param name="repository">Resource repository</param>
        public CreateOrUpdateTranslationHandler(ConfigurationContext configurationContext, IResourceRepository repository)
        {
            _configurationContext = configurationContext;
            _repository = repository;
        }

        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        public void Execute(CreateOrUpdateTranslation.Command command)
        {
            var resource = _repository.GetByKey(command.Key);
            var now = DateTime.UtcNow;

            if (resource == null)
            {
                return;
            }

            var translation = resource.Translations.FindByLanguage(command.Language);

            if (translation == null)
            {
                var newTranslation = new LocalizationResourceTranslation
                {
                    Value = command.Translation,
                    Language = command.Language.Name,
                    ResourceId = resource.Id,
                    ModificationDate = now
                };

                _repository.AddTranslation(resource, newTranslation);
            }
            else
            {
                translation.Value = command.Translation;
                translation.ModificationDate = now;
                _repository.UpdateTranslation(resource, translation);
            }

            resource.ModificationDate = now;
            resource.IsModified = true;

            _repository.UpdateResource(resource);

            _configurationContext.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
