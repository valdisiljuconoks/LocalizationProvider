using System;

namespace DbLocalizationProvider.Queries
{
    public static class IQueryExtensions
    {
        public static TResult Execute<TResult>(this IQuery<TResult> query)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            var handler = ConfigurationContext.Current.TypeFactory.GetQueryHandler(query);
            return handler.Execute(query);
        }
    }
}
