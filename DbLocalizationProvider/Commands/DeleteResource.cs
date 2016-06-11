using System;
using System.Linq;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Commands
{
    public class DeleteResource
    {
        public class Command : ICommand
        {
            public Command(string key)
            {
                Key = key;
            }

            public string Key { get; }
        }

        public class Handler : ICommandHandler<Command>
        {
            public void Execute(Command command)
            {
                if(string.IsNullOrEmpty(command.Key))
                {
                    throw new ArgumentNullException(nameof(command.Key));
                }

                using (var db = new LanguageEntities())
                {
                    var existingResource = db.LocalizationResources.FirstOrDefault(r => r.ResourceKey == command.Key);

                    if(existingResource == null)
                    {
                        return;
                    }

                    if(existingResource.FromCode)
                    {
                        throw new InvalidOperationException($"Cannot delete resource `{command.Key}` that is synced with code");
                    }

                    db.LocalizationResources.Remove(existingResource);
                    db.SaveChanges();
                }

                ConfigurationContext.Current.CacheManager.Remove(CacheKeyHelper.BuildKey(command.Key));
            }
        }
    }
}
