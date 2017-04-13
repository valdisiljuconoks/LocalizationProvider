# Administration UI (EPiServer)

Administration UI is available form top menu under `CMS > Localization`.

## Export & Import - View Diff

Once upon a time I got sick of merging and importing new resources from staging environment (with adjusted translations by editors) into production database where on the other hand editors already made changes to existing resources. Up till now import process only supported *partial import* or *full import*. Partial import was dealing with only new resources (those ones that were not in target database), however full import - on the other hand - was used to flush everything and import stuff from exported file. Kinda worked, but it was v0.1 of this export/import component. Pretty green.

Updates to this story was one of the main focus for this upcoming version. Eventually if you are importing resources from other environment, now your import experience might look like this:

![](adminui-import.png)

To share the code would be overkill here in this post. Better sit back and relax watching import in action. Enjoy!

<iframe src="https://player.vimeo.com/video/205294678" width="800" height="520" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
<p><a href="https://vimeo.com/205294678">DbLocalizationProvider - Export_Import</a> from <a href="https://vimeo.com/user49426707">Valdis Iljuconoks</a> on <a href="https://vimeo.com">Vimeo</a>.</p>
