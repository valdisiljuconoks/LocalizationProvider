View locations in Alloy follows a number of conventions in addition to the default ASP.NET MVC conventions:
* Views for pages and blocks with their own controllers use standard ASP.NET MVC conventions - <controllerName_minus_"controller">/<actionName>.cshtml
* Page types which don't have their own controller are mapped to <pageTypeClassName>/Index.cshtml by DefaultPageController
* Views for block types which don't have their own controllers are found in Shared/Blocks
* Partial views for page types which don't have their own controllers for partial requests are found in Shared/PagePartials