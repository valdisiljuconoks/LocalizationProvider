<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<DbLocalizationProvider.AdminUI.LocalizationResourceViewModel>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="EPiServer.Framework.Web.Mvc.Html"%>
<%@ Import Namespace="EPiServer.Framework.Web.Resources"%>
<%@ Import Namespace="EPiServer.Shell" %>
<%@ Import Namespace="EPiServer.Shell.Navigation" %>
<%@ Import Namespace="EPiServer" %>
<%@ Import Namespace=" EPiServer.Shell.Web.Mvc.Html"%>
<%@ Assembly Name="EPiServer.Shell.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Localization Resources</title>

    <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css" rel="stylesheet" />
    <link href="//cdnjs.cloudflare.com/ajax/libs/x-editable/1.5.0/bootstrap3-editable/css/bootstrap-editable.css" rel="stylesheet" />

    <%= Page.ClientResources("ShellCore") %>
    <%= Page.ClientResources("ShellWidgets") %>
    <%= Page.ClientResources("ShellCoreLightTheme") %>
    <%= Page.ClientResources("ShellWidgetsLightTheme")%>
    <%= Page.ClientResources("Navigation") %>
    <%= Page.ClientResources("DijitWidgets", new[] { ClientResourceType.Style })%>
    <%= Html.CssLink(UriSupport.ResolveUrlFromUIBySettings("App_Themes/Default/Styles/ToolButton.css")) %>
    <%= Html.CssLink(Paths.ToClientResource("CMS", "ClientResources/Epi/Base/CMS.css"))%>
    
    <%= Html.ShellAsyncInitializationScript() %>
    
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUtilBySettings("javascript/episerverscriptmanager.js"))%>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/system.js")) %>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/dialog.js")) %>
    <%= Html.ScriptResource(UriSupport.ResolveUrlFromUIBySettings("javascript/system.aspx")) %>

    <script src="//code.jquery.com/jquery-2.0.3.min.js"></script>
    <script src="//netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/x-editable/1.5.0/bootstrap3-editable/js/bootstrap-editable.min.js"></script>

    <style type="text/css">
        body {
            font-size: 1.2em;
        }

         table.table > tbody > tr > td {
             height: 30px;
             vertical-align: middle;
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

        a.editable-empty {
            color: red;
        }
    </style>
</head>
<body>
    <% if (Model.ShowMenu)
       {
           %><%= Html.GlobalMenu() %><%
       } %>
    <div class="epi-contentContainer epi-padding">
        <div class="epi-contentArea epi-paddingHorizontal">
            <h1 class="EP-prefix">Localization Resources</h1>
            <div class="epi-paddingVertical">

                <form action="<%= Url.Action("UpdateLanguages") %>" method="post">
                    <div class="available-languages"><a data-toggle="collapse" href="#collapseExample" aria-expanded="false" aria-controls="collapseExample" class="available-languages-toggle">Available Languages</a></div>
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
                                <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Save" type="submit" id="saveLanguages" value="Save" title="Save" /></span>
                        </div>
                    </div>
                </form>

                <form action="<%= Url.Action("ExportResources") %>" method="get" id="exportForm"></form>
                <form action="<%= Url.Action("ImportResources") %>" method="get" id="importLinkForm">
                    <input type="hidden" name="showMenu" value="<%= Model.ShowMenu  %>"/>
                </form>
                <div class="epi-buttonContainer">
                    <span class="epi-cmsButton">
                        <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Export" type="submit" id="exportResources" value="Export" title="Export" onclick="$('#exportForm').submit();" /></span>
                    <span class="epi-cmsButton">
                        <input class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Import" type="submit" id="importResources" value="Import" title="Import" onclick="$('#importLinkForm').submit();" /></span>
                </div>

                <form id="resourceFilterForm">
                    <div class="form-group">
                        <div class="input-group">
                            <input type="search" value="" class="form-control" placeholder="Enter Search Query" />
                            <span class="input-group-btn">
                                <button class="btn btn-default" type="submit">
                                    <span class="glyphicon glyphicon-search"></span>
                                    <span class="sr-only">Search</span>
                                </button>
                            </span>
                        </div>
                    </div>
                </form>


                <%--<form>
                <div class="form-group">
                    <div class="input-group">
                        <a class="btn btn-blue btn-primary" href="#" id="newResource">+ Create new resource</a>
                    </div>
                </div>
            </form>--%>

                <table class="table table-bordered table-striped" id="resourceList" style="clear: both">
                    <thead>
                        <tr>
                            <th>Resource Key</th>
                            <% foreach (var language in Model.SelectedLanguages)
                                { %>
                            <th><%= language.EnglishName %></th>
                            <% } %>
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
                                    <input class="form-control" id="resourceKey" placeholder="Resource Key" style="width: 50%" />
                                </div>
                            </td>
                            <% foreach (var language in Model.SelectedLanguages)
                                { %>
                            <td>
                                <input class="form-control resource-translation" id="<%= language %>" />
                            </td>
                            <% } %>
                        </tr>

                        <% foreach (var resource in Model.Resources)
                            { %>
                        <tr class="localization resource">
                            <td><%= resource.Key %></td>
                            <% foreach (var localizedResource in Model.Resources.Where(r => r.Key == resource.Key))
                                {
                                    foreach (var language in Model.SelectedLanguages)
                                    {
                                        var z = localizedResource.Value.FirstOrDefault(l => l.SourceCulture.Name == language.Name);
                                        if (z != null)
                                        { %>
                            <td>
                                <a href="#" id="<%= language.Name %>" data-type="text" data-pk="<%= resource.Key %>" data-title="Enter translation"><%= z.Value %></a>
                            </td>
                            <% }
                                    }
                                } %>
                        </tr>
                        <% } %>
                    </tbody>
                </table>


                <script type="text/javascript">
                    $(function () {
                        $.fn.editable.defaults.mode = 'popup';
                        $('.localization a').editable({
                            url: '<%= Url.Action("Update") %>'
                    });

                    var $filterForm = $('#resourceFilterForm'),
                        $filterInput = $filterForm.find('.form-control:first-child'),
                        $resourceList = $('#resourceList'),
                        $resourceItem = $resourceList.find('.resource');

                    function runFilter() {
                        var query = $filterInput.val();

                        if (query.length === 0) {
                            $resourceItem.removeClass('hidden');
                            return;
                        }

                        $resourceItem.each(function () {
                            var $item = $(this);
                            if ($item.text().search(new RegExp(query, 'i')) > -1) {
                                $item.removeClass('hidden');
                            } else {
                                $item.addClass('hidden');
                            }
                        });
                    }

                    var t;
                    $filterInput.on('input', function () {
                        clearTimeout(t);
                        t = setTimeout(runFilter, 500);
                    });
                    $filterForm.on('submit', function (e) {
                        e.preventDefault();
                        clearTimeout(t);
                        runFilter();
                    });

                    $('#newResource').on('click', function () {
                        $('.new-resource-form').removeClass('hidden');
                        $('#resourceKey').focus();
                    });

                    $('#cancelNewResource').on('click', function () {
                        $('.new-resource-form').addClass('hidden');
                    });

                    $('#saveResource').on('click', function () {
                        var $form = $('.new-resource-form'),
                            $resourceKey = $form.find('#resourceKey').val();

                        if ($resourceKey.length == 0) {
                            alert('Fill resource key');
                            return;
                        }

                        $.ajax({
                            url: '@Url.Action("Create")',
                            method: 'POST',
                            data: 'pk=' + $resourceKey
                        }).success(function () {
                            var $translations = $form.find('.resource-translation');

                            var requests = [];

                            $.map($translations, function (el) {
                                var $el = $(el);
                                requests.push($.ajax({
                                    url: '<%= Url.Action("Update") %>',
                                    method: 'POST',
                                    data: 'pk=' + $resourceKey + '&name=' + el.id + '&value=' + $el.val()
                                }));
                            });

                            $.when(requests).then(function () {
                                setTimeout(function () {
                                    location.reload();
                                }, 1000);
                            });
                        });
                    });
                })
                </script>

            </div>
        </div>
    </div>
</body>
</html>
