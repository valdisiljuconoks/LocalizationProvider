namespace TechFellow.LocalizationProvider.MigrationTool
{
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
