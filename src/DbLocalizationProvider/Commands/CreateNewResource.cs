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
    }
}
