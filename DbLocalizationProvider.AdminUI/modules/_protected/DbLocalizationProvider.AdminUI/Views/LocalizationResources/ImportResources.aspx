<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Assembly Name="EPiServer.Shell.UI" %>
<%@ Import Namespace="EPiServer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Localization Resources</title>

    <%= Page.ClientResources("ShellCore") %>
    <%= Page.ClientResources("ShellCoreLightTheme") %>
    <%= Page.ClientResources("Navigation") %>
    <%= Html.CssLink(UriSupport.ResolveUrlFromUIBySettings("App_Themes/Default/Styles/ToolButton.css")) %>

    <script src="//code.jquery.com/jquery-2.0.3.min.js"></script>
    <script src="//netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/x-editable/1.5.0/bootstrap3-editable/js/bootstrap-editable.min.js"></script>
    
    <style>
        label {
            font-weight: normal;

        }
    </style>
</head>
<body>
    <div class="epi-contentContainer epi-padding">
        <div class="epi-contentArea epi-paddingHorizontal">
            <h1 class="EP-prefix">Import Localization Resources</h1>
            <div class="epi-paddingVertical">
                <form action="<%= Url.Action("ImportResources") %>" method="post" enctype="multipart/form-data" id="importForm">
                    <div class="epi-formArea epi-paddingVertical-small">
                        <div class="epi-size25">
                            <input type="checkbox" id="importOnlyNewContent" name="importOnlyNewContent"/>
                            <label for="importOnlyNewContent">Import only new content</label>
                        </div>
                    </div>
                    <div class="epi-buttonContainer">
                        <span class="epi-cmsButton">
                            <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Import" type="submit" id="importResources" value="Import" title="Import" />
                        </span>
                        <span class="epi-cmsButton">
                            <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-File" type="submit" id="importTestResources" value="Test Import with Log" title="Test Import with Log" />
                        </span>
                    </div>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
