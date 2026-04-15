All language files in this folder are included in the LocalizationService.

The path to this folder is configured in EPiServerFramework.config:

<localization fallbackBehavior="FallbackCulture, MissingMessage, Echo" fallbackCulture="en" xdt:Transform="Insert">
	<providers>
		<add virtualPath="/Resources/Translations"
			name="languageFiles"
			type="EPiServer.Framework.Localization.XmlResources.FileXmlLocalizationProvider" />
	</providers>
</localization>
