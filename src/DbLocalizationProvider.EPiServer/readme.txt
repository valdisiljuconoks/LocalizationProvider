DbLocalizationProvider


Installation (with EPiServer.Framework.config file)
===================================================


Current version of DbLocalizationProvider NuGet package does not support proper configuration file transformations
if external file is used as configuration source for EPiServer Framework configuration section.
That's why default web.config file may be corruped after NuGet package installation. PLEASE review web.config file




Localization Provider Order
===========================

After installation of this package, please check your EPiServer.Framework configuration section (<episerver.framework>).
DbLocalizationProvider is not added as first provider in <localization> element <providers> section.

So it maylook like this at the end (together with Xml resource language file provider):

<configuration>
  <episerver.framework>
    <localization>
      <providers>
        <add name="db" type="DbLocalizationProvider.EPiServer.DatabaseLocalizationProvider, DbLocalizationProvider.EPiServer" />
        <add name="languageFiles" virtualPath="~/lang"
             type="EPiServer.Framework.Localization.XmlResources.FileXmlLocalizationProvider, EPiServer.Framework" />
      </providers>
    </localization>
  </episerver.framework>
</configuration>



==
Valdis Iljuconoks (aka Tech Fellow)

.. Greetings from Riga (Latvia)