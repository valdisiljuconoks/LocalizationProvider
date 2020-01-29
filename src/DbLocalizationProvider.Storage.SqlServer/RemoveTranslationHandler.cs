// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;
using DbLocalizationProvider.Commands;

namespace DbLocalizationProvider.Storage.SqlServer
{
    public class RemoveTranslationHandler : ICommandHandler<RemoveTranslation.Command>
    {
        public void Execute(RemoveTranslation.Command command)
        {
            var repository = new ResourceRepository();
            var resource = repository.GetByKey(command.Key);

            if (resource == null) return;
            if (!resource.IsModified.HasValue || !resource.IsModified.Value)
                throw new InvalidOperationException(
                    $"Cannot delete translation for not modified resource (key: `{command.Key}`");

            var t = resource.Translations.FirstOrDefault(_ => _.Language == command.Language.Name);
            if (t != null) repository.DeleteTranslation(resource, t);

            ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
        }
    }
}
