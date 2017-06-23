namespace DbLocalizationProvider.Queries
{
    public class DetermineDefaultCulture
    {
        public class Query : IQuery<string> { }

        public class Handler : IQueryHandler<Query, string>
        {
            public string Execute(Query query)
            {
                return ConfigurationContext.Current.DefaultResourceCulture != null
                           ? ConfigurationContext.Current.DefaultResourceCulture.Name
                           : "en";
            }
        }
    }
}
