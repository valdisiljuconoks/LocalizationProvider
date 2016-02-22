DbLocalizationProvider


Installation
============


After installation of this package, please check your EPiServer.Framework configuration section (<episerver.framework>).
DbLocalizationProvider is not added as first provider in <localization> element <providers> section.

So it maylook like this at the end (together with Xml resource language file provider):

<configuration>
  <episerver.framework>
    <localization>
      <providers>
        <add name="db" type="DbLocalizationProvider.DatabaseLocalizationProvider, DbLocalizationProvider" />
        <add virtualPath="~/lang" name="languageFiles" type="EPiServer.Framework.Localization.XmlResources.FileXmlLocalizationProvider, EPiServer.Framework" />
      </providers>
    </localization>
  </episerver.framework>
</configuration>

