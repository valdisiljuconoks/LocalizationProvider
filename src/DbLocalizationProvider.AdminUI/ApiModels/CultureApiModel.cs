using System;

namespace DbLocalizationProvider.AdminUI.ApiModels
{
    public class CultureApiModel
    {
        public CultureApiModel(string code, string display)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Display = display ?? throw new ArgumentNullException(nameof(display));
        }

        public string Code { get; set; }

        public string Display { get; set; }
    }
}
