using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.EPiServer.Queries;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.EPiServer
{
    public class DatabaseLocalizationProvider : global::EPiServer.Framework.Localization.LocalizationProvider
    {
        public override IEnumerable<CultureInfo> AvailableLanguages
        {
            get
            {
                var q = new AvailableLanguages.Query();
                return q.Execute();
            }
        }

        public override string GetString(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            // we need to call handler directly here
            // if we would dispatch query and ask registered handler to execute
            // we would end up in stack-overflow as in EPiServer context
            // the same database localization provider is registered as the query handler

            // TODO: this seems to be odd, no point for decorators. needs to be reviewed
            IQueryHandler<GetTranslation.Query, string> originalHandler = new GetTranslation.Handler();

            if(ConfigurationContext.Current.DiagnosticsEnabled)
                originalHandler = new EPiServerGetTranslation.HandlerWithLogging(new GetTranslation.Handler());

            return originalHandler.Execute(new GetTranslation.Query(originalKey, culture, false));
        }

        public override IEnumerable<global::EPiServer.Framework.Localization.ResourceItem> GetAllStrings(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            var q = new GetAllTranslations.Query(originalKey, culture);

            return q.Execute()
                    .Select(r => new global::EPiServer.Framework.Localization.ResourceItem(r.Key, r.Value, r.SourceCulture));
        }
    }
}
