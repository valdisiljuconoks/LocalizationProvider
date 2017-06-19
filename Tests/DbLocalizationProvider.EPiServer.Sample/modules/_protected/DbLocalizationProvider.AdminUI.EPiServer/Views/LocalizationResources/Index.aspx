<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<DbLocalizationProvider.AdminUI.LocalizationResourceViewModel>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.Framework.Web.Mvc.Html"%>
<%@ Import Namespace="EPiServer.Framework.Web.Resources"%>
<%@ Import Namespace="EPiServer.Shell" %>
<%@ Import Namespace="EPiServer.Shell.Navigation" %>
<%@ Import Namespace="EPiServer" %>
<%@ Import Namespace=" EPiServer.Shell.Web.Mvc.Html"%>
<%@ Import Namespace=" DbLocalizationProvider"%>
<%@ Import Namespace="DbLocalizationProvider.AdminUI" %>
<%@ Assembly Name="EPiServer.Shell.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= Html.Translate(() => Resources.Header) %></title>

    <%= Html.CssLink(Paths.ToClientResource(typeof(LocalizationResourceViewModel), "ClientResources/bootstrap.min.css"))%>
    <%= Html.CssLink(Paths.ToClientResource(typeof(LocalizationResourceViewModel), "ClientResources/bootstrap-editable.css"))%>

    <%= Page.ClientResources("ShellCore") %>
    <%= Page.ClientResources("ShellWidgets") %>
    <%= Page.ClientResources("ShellCoreLightTheme") %>
    <%= Page.ClientResources("ShellWidgetsLightTheme")%>
    <%= Page.ClientResources("Navigation") %>
    <%= Page.ClientResources("DijitWidgets", new[] { ClientResourceType.Style })%>

    <%= Html.CssLink(UriSupport.ResolveUrlFromUIBySettings("App_Themes/Default/Styles/ToolButton.css")) %>
    <%--<%= Html.CssLink(Paths.ToClientResource("CMS", "ClientResources/Epi/Base/CMS.css"))%>--%>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUtilBySettings("javascript/episerverscriptmanager.js"))%>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/system.js")) %>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/dialog.js")) %>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/system.aspx")) %>

    <%= Html.ScriptResource(Paths.ToClientResource(typeof(LocalizationResourceViewModel), "ClientResources/jquery-2.0.3.min.js"))%>
    <%= Html.ScriptResource(Paths.ToClientResource(typeof(LocalizationResourceViewModel), "ClientResources/bootstrap.min.js"))%>
    <%= Html.ScriptResource(Paths.ToClientResource(typeof(LocalizationResourceViewModel), "ClientResources/bootstrap-editable.min.js"))%>
    <%= Html.ScriptResource(Paths.ToClientResource(typeof(LocalizationResourceViewModel), "ClientResources/jquery.tablesorter.min.js"))%>

    <style type="text/css">
        body {
            font-size: 1.2em;
        }

         table.table > tbody > tr > td {
             height: 30px;
             vertical-align: middle;
         }

         table.table-sorter .header {
             cursor: pointer;
         }

        table.table-sorter thead tr .headerSortDown, table.table-sorter thead tr .headerSortUp {
            background: #bebebe url("../../shell/Resources/Gradients.png") repeat-x scroll left -200px;
            _background: #949494 none;
            color: #ffffff;
            text-shadow: none;
        }

        table.table-sorter thead tr .sortable:after {
            position: relative;
            left: 2px;
            border: 8px solid transparent;
        }

        table.table-sorter thead tr .sortable:after {
            content: '\25ca';
        }

        table.table-sorter thead tr .headerSortDown:after {
            content: '\25b2';
        }

        table.table-sorter thead tr .headerSortUp:after {
            content: '\25bc';
        }

        .headerSortDown,
        .headerSortUp{
            padding-right: 10px;
        }

         .search-input {
             width: 100%;
         }

        .glyphicon {
            font-size: 2rem;
        }

        .epi-contentContainer {
            max-width: 100%;
        }

        label {
            font-weight: normal;
            margin-top: 5px;
        }

        input[type="radio"], input[type="checkbox"] {
            margin: 0;
        }

        .available-languages {
            margin-bottom: 15px;
        }

        .available-languages-toggle {
            text-decoration: underline;
        }

        a.editable-empty, a.editable-empty:visited {
            color: red;
        }

        a.editable-empty.editable-click, a.editable-click:hover {
            border-bottom-color: red;
        }

        .EP-systemMessage {
            display: block;
            border: solid 1px #878787;
            background-color: #fffdbd;
            padding: 0.3em;
            margin-top: 0.5em;
            margin-bottom: 0.5em;
        }
    </style>
</head>
<body>
    <% if (Model.ShowMenu)
       {
    %><%= Html.GlobalMenu(string.Empty, "/global/cms/localization") %><%
       } %>
    <div class="epi-contentContainer epi-padding">
        <div class="epi-contentArea epi-paddingHorizontal">
            <h1 class="EP-prefix"><%= Html.Translate(() => Resources.Header) %></h1>
            <div class="epi-paddingVertical">
                <% if (!string.IsNullOrEmpty(ViewData["LocalizationProvider_Message"] as string))
                   {
                %>
                <div class="EP-systemMessage">
                    <%= ViewData["LocalizationProvider_Message"] %>
                    <%= Html.ValidationSummary() %>
                </div>
                <%
                   } %>
                <form action="<%= Url.Action("UpdateLanguages") %>" method="post">
                    <div class="available-languages"><a data-toggle="collapse" href="#collapseExample" aria-expanded="false" aria-controls="collapseExample" class="available-languages-toggle"><%= Html.Translate(() => Resources.AvailableLanguages) %></a></div>
                    <div class="collapse" id="collapseExample">
                        <% foreach (var language in Model.Languages)
                           {
                               var isSelected = Model.SelectedLanguages.FirstOrDefault(l => language.Equals(l)) != null;
                        %>
                        <div>
                            <label>
                                <input type="checkbox" <%= isSelected ? "checked" : string.Empty %> name="languages" value="<%= language.Name %>" /><%= language.EnglishName %></label>
                        </div>
                        <% } %>
                        <div class="epi-buttonContainer">
                            <span class="epi-cmsButton">
                                <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Save" type="submit" id="saveLanguages" value="<%= Html.Translate(() => Resources.Save) %>" title="<%= Html.Translate(() => Resources.Save) %>" /></span>
                        </div>
                    </div>
                </form>

                <form action="<%= Url.Action("ExportResources") %>" method="get" id="exportForm"></form>
                <form action="<%= Url.Action("ImportResources") %>" method="get" id="importLinkForm">
                    <input type="hidden" name="showMenu" value="<%= Model.ShowMenu %>"/>
                </form>
                <div class="epi-buttonContainer">
                    <span class="epi-cmsButton">
                        <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Export" type="submit" id="exportResources" value="<%= Html.Translate(() => Resources.Export) %>" title="<%= Html.Translate(() => Resources.Export) %>" onclick="$('#exportForm').submit();" /></span>

                    <% if (Model.AdminMode)
                       {
                    %>
                        <span class="epi-cmsButton">
                            <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Import" type="submit" id="importResources" value="<%= Html.Translate(() => Resources.Import) %>" title="<%= Html.Translate(() => Resources.Import) %>" onclick="$('#importLinkForm').submit();" /></span>
                    <%
                       } %>
                </div>

                <form id="resourceFilterForm">
                    <div class="form-group">
                        <input type="search" value="" class="form-control search-input" placeholder="<%= Html.Translate(() => Resources.SearchPlaceholder) %>" />
                    </div>
                </form>

                <div class="epi-buttonContainer">
                <% if (Model.AdminMode)
                   { %>
                    <span class="epi-cmsButton">
                    <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-NewFile" type="submit" id="newResource" value="<%= Html.Translate(() => Resources.New) %>" title="<%= Html.Translate(() => Resources.New) %>"/></span>
                <% } %>
                    <span>
                        <input type="checkbox" name="showEmptyResources" id="showEmptyResources"/>
                        <label for="showEmptyResources"><%= Html.Translate(() => Resources.ShowEmpty) %></label>
                    </span>
                    <span>
                        <input type="checkbox" name="showHiddenResources" id="showHiddenResources"/>
                        <label for="showHiddenResources"><%= Html.Translate(() => Resources.ShowHidden) %></label>
                    </span>
                </div>
                <table class="table table-bordered table-striped table-sorter" id="resourceList" style="clear: both">
                    <thead>
                        <tr>
                            <th class="sortable"><%= Html.Translate(() => Resources.KeyColumn) %></th>
                            <% foreach (var language in Model.SelectedLanguages)
                               { %>
                            <th class="sortable"><%= language.EnglishName %></th>
                            <% } %>
                            <% if (Model.AdminMode)
                               {
                            %><th><%= Html.Translate(() => Resources.DeleteColumn) %></th><%
                               }
                               else
                               {
                            %><th class="sortable"><%= Html.Translate(() => Resources.FromCodeColumn) %></th><% } %>
                        </tr>
                    </thead>
                    <tbody>
                        <tr class="hidden new-resource-form">
                            <td>
                                <div class="form-inline">
                                    <button class="btn btn-default btn-primary" id="saveResource">
                                        <span href="#" class="glyphicon glyphicon-ok"></span>
                                    </button>
                                    <button class="btn" id="cancelNewResource">
                                        <span href="#" class="glyphicon glyphicon-remove"></span>
                                    </button>
                                    <input class="form-control" id="resourceKey" placeholder="<%= Html.Translate(() => Resources.KeyColumn) %>" style="width: 50%" />
                                </div>
                            </td>
                            <% foreach (var language in Model.SelectedLanguages)
                               { %>
                            <td>
                                <input class="form-control resource-translation" id="<%= language %>" />
                            </td>
                            <% } %>
                            <% if (Model.AdminMode) { %><td></td><% } %>
                        </tr>

                        <% foreach (var resource in Model.Resources)
                            { %>
                        <tr class="localization resource <%= resource.IsHidden ? "hidden-resource hidden" : "" %>">
                            <td><span title="<%= resource.Key %>"><%= resource.DisplayKey %></span></td>
                            <% foreach (var localizedResource in Model.Resources.Where(r => r.Key == resource.Key))
                                {
                                    foreach (var language in Model.SelectedLanguages)
                                    {
                                        var z = localizedResource.Value.FirstOrDefault(l => l.SourceCulture.Name == language.Name);
                                        if (z != null)
                                        { %>
                            <td>
                                <a href="#" id="<%= language.Name %>" data-pk="<%: resource.Key %>"><%: z.Value %></a>
                            </td>
                            <%
                                        }
                                        else
                                        { %>
                            <td>
                                <a href="#" id="<%= language.Name %>" data-pk="<%: resource.Key %>"></a>
                            </td>
                                        <% }
                                    }
                                } %>
                            <% if (Model.AdminMode)
                                {
                               %><td>
                                    <form action="<%= Url.Action("Delete") %>" method="post" class="delete-form">
                                        <input type="hidden" name="pk" value="<%: resource.Key %>"/>
                                        <input type="hidden" name="returnUrl" value="<%= Model.ShowMenu ? Url.Action("Main") : Url.Action("Index") %>" />
                                        <% if (resource.AllowDelete) { %>
                                        <span class="epi-cmsButton">
                                        <%}
                                           else
                                           {
                                        %>
                                            <span class="epi-cmsButtondisabled"><%
                                           } %>
                                        <% if (resource.AllowDelete)
                                           { %>
                                            <input class="epi-cmsButton-tools epi-cmsButton-Delete" type="submit" id="deleteResource" value="" />
                                        <% } %><%
                                           else
                                           { %>
                                            <input class="epi-cmsButton-tools epi-cmsButton-Delete" type="submit" id="deleteResource" value="" disabled="disabled"/>
                                        <% } %>
                                        </span>
                                    </form>
                                </td><%
                                } else { %>
                                <td><%= !resource.AllowDelete %></td>
                                <% } %>
                        </tr>
                        <% } %>
                    </tbody>
                </table>

                <script type="text/javascript">
                    $(function() {
                        $('.localization a').editable({
                            url: '<%= Url.Action("Update") %>',
                            type: 'textarea',
                            placement: 'top',
                            mode: 'popup',
                            title: '<%= Html.Translate(() => Resources.TranslationPopupHeader) %>',
                            emptytext: '<%= Html.Translate(() => Resources.Empty) %>'
                        });

                        $('#resourceList').on('submit', '.delete-form', function (e) {
                            e.preventDefault();

                            var $form = $(this);
                            var pk = $(this).find('input[name=pk]').val();
                            if (confirm('<%= Html.Translate(() => Resources.DeleteConfirm) %> `' + pk + '`?')) {
                                $.ajax({ url: $form.attr('action'), method: 'post', data: $form.serialize() });
                                $form.closest('.resource').remove();
                            }
                        });

                        var $filterForm = $('#resourceFilterForm'),
                            $filterInput = $filterForm.find('.form-control:first-child'),
                            $resourceList = $('#resourceList'),
                            $resourceItems = $resourceList.find('.resource'),
                            $showEmpty = $('#showEmptyResources'),
                            $showHidden = $('#showHiddenResources');

                        $resourceList.tablesorter();

                        function filter($item, query) {
                            if ($item.html().search(new RegExp(query, 'i')) > -1) {
                                $item.removeClass('hidden');
                            } else {
                                $item.addClass('hidden');
                            }
                        }

                        function filterEmpty($item) {
                            if ($item.find('.editable-empty').length == 0) {
                                $item.addClass('hidden');
                            }
                        }

                        function runFilter(query) {
                            // clear state
                            $resourceItems.removeClass('hidden');
                            $resourceItems.each(function() { filter($(this), query); });

                            if ($showEmpty.prop('checked')) {
                                // if show only empty - filter empty ones as well
                                $resourceItems.not('.hidden').each(function() { filterEmpty($(this)); });
                            }

                            if (!$showHidden.prop('checked')) {
                                $resourceItems.filter('.hidden-resource').each(function () { $(this).addClass('hidden'); });
                            }
                        }

                        $showEmpty.change(function () {
                            runFilter($filterInput.val());
                        });

                        $showHidden.change(function () {
                            runFilter($filterInput.val());
                        });

                        var t;
                        $filterInput.on('input', function() {
                            clearTimeout(t);
                            t = setTimeout(function() { runFilter($filterInput.val()); }, 500);
                        });

                        $filterForm.on('submit', function (e) {
                            e.preventDefault();
                            clearTimeout(t);
                            runFilter($filterInput.val());
                        });

                        $('#newResource').on('click', function() {
                            $('.new-resource-form').removeClass('hidden');
                            $('#resourceKey').focus();
                        });

                        $('#cancelNewResource').on('click', function() {
                            $('.new-resource-form').addClass('hidden');
                        });

                        $('#saveResource').on('click', function() {
                            var $form = $('.new-resource-form'),
                                $resourceKey = $form.find('#resourceKey').val();

                            if ($resourceKey.length == 0) {
                                alert('Fill resource key');
                                return;
                            }

                            $.ajax({
                                url: '<%= Url.Action("Create") %>',
                                method: 'POST',
                                data: 'pk=' + $resourceKey
                            }).success(function() {
                                var $translations = $form.find('.resource-translation');

                                var requests = [];

                                $.map($translations, function(el) {
                                    var $el = $(el);
                                    requests.push($.ajax({
                                        url: '<%= Url.Action("Update") %>',
                                        method: 'POST',
                                        data: 'pk=' + $resourceKey + '&name=' + el.id + '&value=' + $el.val()
                                    }));
                                });

                                $.when(requests).then(function() {
                                    setTimeout(function() {
                                        location.reload();
                                    }, 1000);
                                });
                            }).error(function(e) {
                                alert('Error: ' + e.Message);
                            });
                        });
                    });
                </script>
            </div>
        </div>
    </div>
</body>
</html>
