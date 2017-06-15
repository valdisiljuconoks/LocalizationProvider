using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Commands
{
    public class CreateOrUpdateTranslation
    {
        public class Command : ICommand
        {
            public Command(string key, CultureInfo language, string translation)
            {
                Key = key;
                Language = language;
                Translation = translation;
            }

            public string Key { get; }

            public CultureInfo Language { get; }

            public string Translation { get; }
        }

        public class Handler : ICommandHandler<Command>
        {
            public void Execute(Command command)
            {
                using (var db = new LanguageEntities())
                {
                    var resource = db.LocalizationResources.Include(r => r.Translations).FirstOrDefault(r => r.ResourceKey == command.Key);

                    if(resource == null)
                    {
                        // TODO: return some status response obj
                        return;
                    }

                    var translation = resource.Translations.FirstOrDefault(t => t.Language == command.Language.Name);

                    if(translation != null)
                    {
                        // update existing translation
                        translation.Value = command.Translation;
                    }
                    else
                    {
                        var newTranslation = new LocalizationResourceTranslation
                                             {
                                                 Value = command.Translation,
                                                 Language = command.Language.Name,
                                                 ResourceId = resource.Id
                                             };

                        db.LocalizationResourceTranslations.Add(newTranslation);
                    }

                    resource.ModificationDate = DateTime.UtcNow;
                    resource.IsModified = true;
                    db.SaveChanges();
                }

                ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
            }
        }
    }
}
