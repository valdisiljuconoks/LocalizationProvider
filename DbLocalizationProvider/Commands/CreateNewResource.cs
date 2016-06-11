using System;
using System.Linq;

namespace DbLocalizationProvider.Commands
{
    public class CreateNewResource
    {
        public class Command : ICommand
        {
            public Command(string key, string userName, bool fromCode = true)
            {
                Key = key;
                UserName = userName;
                FromCode = fromCode;
            }

            public string Key { get; }

            public string UserName { get; }

            public bool FromCode { get; }
        }

        public class Handler : ICommandHandler<Command>
        {
            public void Execute(Command command)
            {
                if(string.IsNullOrEmpty(command.Key))
                    throw new ArgumentNullException(nameof(command.Key));

                using (var db = new LanguageEntities())
                {
                    var existingResource = db.LocalizationResources.FirstOrDefault(r => r.ResourceKey == command.Key);
                    if(existingResource != null)
                    {
                        throw new InvalidOperationException($"Resource with key `{command.Key}` already exists");
                    }

                    db.LocalizationResources.Add(new LocalizationResource(command.Key)
                                                 {
                                                     ModificationDate = DateTime.UtcNow,
                                                     FromCode = command.FromCode,
                                                     IsModified = false,
                                                     Author = command.UserName
                                                 });

                    db.SaveChanges();
                }
            }
        }
    }
}
