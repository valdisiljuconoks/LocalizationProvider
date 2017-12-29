# DbLocalizationProvider
===================================================

Might be annoying to see this file all the time (even after minor version upgrade), but it's worth reading sometimes.


## .Net Standard (2.0)
===================================================

Starting from version 4.0 core packages now targets .Net Standard 2.0 version.
If you are using Episerver v11 - this is no new for you (as Episerver v11 requires .Net Standard).


## Installation (with EPiServer.Framework.config file)
===================================================

This version of DbLocalizationProvider NuGet package does not support proper configuration file transformations
if external file is used as configuration source for EPiServer Framework configuration section. And don't think
it ever will support proper transformation as last time I saw EPiServer.Framework.config file was in late 9.x days.
That's why default web.config file might be corruped after NuGet package installation. *PLEASE* review web.config file.


## Localization Provider Order
===================================================

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
Valdis Iljuconoks (aka Tech Fellow, https://tech-fellow.net)

.. Greetings from Riga (Latvia)
