using System.Diagnostics;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    [DebuggerDisplay("Culture = {CultureName}, Value = {Translation}")]
    public class ResourceTranslation
    {
        public ResourceTranslation(string cultureId, string cultureName, string translation)
        {
            CultureId = cultureId;
            CultureName = cultureName;
            Translation = translation;
        }

        public string CultureId { get; private set; }
        public string CultureName { get; private set; }
        public string Translation { get; private set; }
    }
}
