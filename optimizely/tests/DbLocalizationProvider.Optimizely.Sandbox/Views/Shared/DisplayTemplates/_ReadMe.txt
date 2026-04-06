The views in this folder are used when rendering properties using Html.DisplayFor and Html.PropertyFor. 
Display templates are selected based on the type name of the property and, optionally, by UIHint and DataType attributes added to the property.
Note that the CMS adds a number of view templates which do not exist in this folder but found through a view engine which the CMS adds at start up.
Those view templates can be found in  <cms_install_dir>\Application\Util\Views\Shared\DisplayTemplates. Views in this folder takes precedence meaning
that we can override those templates, which is currently done for content areas.