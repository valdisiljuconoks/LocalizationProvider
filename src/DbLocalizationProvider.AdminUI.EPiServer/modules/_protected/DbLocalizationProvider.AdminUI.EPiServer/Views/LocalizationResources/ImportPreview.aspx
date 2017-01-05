<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<DbLocalizationProvider.AdminUI.PreviewImportResourcesViewModel>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.Framework.Web.Mvc.Html"%>
<%@ Import Namespace="EPiServer.Framework.Web.Resources"%>
<%@ Import Namespace="EPiServer.Shell" %>
<%@ Import Namespace="EPiServer.Shell.Navigation" %>
<%@ Import Namespace="EPiServer" %>
<%@ Import Namespace=" EPiServer.Shell.Web.Mvc.Html"%>
<%@ Assembly Name="EPiServer.Shell.UI" %>
<%@ Import Namespace=" DbLocalizationProvider"%>
<%@ Import Namespace="DbLocalizationProvider.AdminUI" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Localization Resources</title>

    <%= Page.ClientResources("ShellCore") %>
    <%= Page.ClientResources("ShellWidgets") %>
    <%= Page.ClientResources("ShellCoreLightTheme") %>
    <%= Page.ClientResources("ShellWidgetsLightTheme")%>
    <%= Page.ClientResources("Navigation") %>
    <%= Page.ClientResources("DijitWidgets", new[] { ClientResourceType.Style })%>

    <%= Html.CssLink(UriSupport.ResolveUrlFromUIBySettings("App_Themes/Default/Styles/ToolButton.css")) %>
    <%= Html.CssLink(Paths.ToClientResource("CMS", "ClientResources/Epi/Base/CMS.css"))%>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUtilBySettings("javascript/episerverscriptmanager.js"))%>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/system.js")) %>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/dialog.js")) %>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/system.aspx")) %>

    <style type="text/css">
        .EP-systemMessage {
            display: block;
            border: solid 1px #878787;
            background-color: #fffdbd;
            padding: 0.3em;
            margin-top: 0.5em;
            margin-bottom: 0.5em;
        }
    </style>

    <%= Html.ScriptResource(Paths.ToClientResource(typeof(PreviewImportResourcesViewModel), "ClientResources/jquery-2.0.3.min.js"))%>
    <%= Html.ScriptResource(Paths.ToClientResource(typeof(PreviewImportResourcesViewModel), "ClientResources/bootstrap.min.js"))%>
    <%= Html.ScriptResource(Paths.ToClientResource(typeof(PreviewImportResourcesViewModel), "ClientResources/bootstrap-editable.min.js"))%>

</head>
<body>
    <% if (Model.ShowMenu)
       {
           %><%= Html.GlobalMenu(string.Empty, "/global/cms/localization") %><%
       } %>
    <div class="epi-contentContainer epi-padding">
        <div class="epi-contentArea epi-paddingHorizontal">
            <h1 class="EP-prefix">Import Localization Resources (PREVIEW)</h1>
            <form id="backForm" action="<%= Model.ShowMenu ? Url.Action("Main") : Url.Action("Index") %>" method="get"></form>
            <div class="epi-paddingVertical">
                <% if (!string.IsNullOrEmpty(ViewData["LocalizationProvider_ImportResult"] as string))
                   {
                       %>
                <div class="EP-systemMessage">
                    <%= ViewData["LocalizationProvider_ImportResult"] %>
                    <%= Html.ValidationSummary() %>
                </div>
                <%
                   } %>
                <form action="<%= Url.Action("CommitImportResources") %>" method="post" enctype="multipart/form-data" id="importForm">
                    <input type="hidden" name="showMenu" value="<%= Model.ShowMenu %>"/>
                    <p class="EP-systemInfo">Please review your pending changes</p>
                    <div class="epi-formArea">
                        <table>
                            <tr>
                                <th>Choose</th>
                                <th>Operation</th>
                                <th>Key</th>
                                <th>Importing</th>
                                <th>Existing</th>
                            </tr>
                                <% foreach(var change in Model.Changes)
                                   { %>
                            <tr>
                                <td><input type="checkbox" name="change.ChangeType"/></td>
                                <td><%= change.ChangeType %></td>
                                <td><%= change.Importingresource.ResourceKey %></td>
                                <td><%= change.Importingresource.Translations.First().Value %></td>
                                <td><%= change.ExistingResource.Translations.First().Value %></td>
                            </tr>
                                <% } %>
                        </table>
                    </div>
                    <div class="epi-buttonContainer">
                        <span class="epi-cmsButton">
                            <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Import" type="submit" id="importResources" value="Import" title="Import" />
                        </span>
                        <span class="epi-cmsButton">
                            <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Undo" type="button" id="back" value="Back" title="Back" onclick="$('#backForm').submit();" />
                        </span>
                    </div>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
