using System;

namespace DbLocalizationProvider.AdminUI.ApiModels
{
    public class CultureApiModel
    {
        public CultureApiModel(string code, string display)
        {
            if(code == null)
                throw new ArgumentNullException(nameof(code));

            if(display == null)
                throw new ArgumentNullException(nameof(display));

            Code = code;
            Display = display;
        }

        public string Code { get; set; }

        public string Display { get; set; }
    }
}
