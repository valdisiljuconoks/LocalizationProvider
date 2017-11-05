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
    }
}
