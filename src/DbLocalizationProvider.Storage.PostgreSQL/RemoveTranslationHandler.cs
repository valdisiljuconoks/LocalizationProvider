// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.PostgreSql
{
    /// <summary>
    /// Removes single translation
    /// </summary>
    public class RemoveTranslationHandler : ICommandHandler<RemoveTranslation.Command>
    {
        /// <summary>
        /// Handles the command. Actual instance of the command being executed is passed-in as argument
        /// </summary>
        /// <param name="command">Actual command instance being executed</param>
        /// <exception cref="InvalidOperationException">Cannot delete translation for not modified resource (key: `{command.Key}`</exception>
        public void Execute(RemoveTranslation.Command command)
        {
            var repository = new ResourceRepository();
            var resource = repository.GetByKey(command.Key);

            if (resource == null)
            {
                return;
            }

            if (!resource.IsModified.HasValue || !resource.IsModified.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot delete translation for not modified resource (key: `{command.Key}`");
            }

            var t = resource.Translations.FirstOrDefault(_ => _.Language == command.Language.Name);
            if (t != null)
            {
                repository.DeleteTranslation(resource, t);
            }

            ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
