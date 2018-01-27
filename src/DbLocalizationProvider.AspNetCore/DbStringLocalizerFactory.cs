using System;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public class DbStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            return new DbStringLocalizer();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new DbStringLocalizer();
        }
    }
}
