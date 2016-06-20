; (function () {
    angular.module('resourceUIApp', [])
    .controller('resourcesController', ['$scope', '$http',
            function ($scope, $http) {

                $scope.getTranslation = function (resource, language) {
                    var translation = "";
                    angular.forEach(resource.Value, function (res) {
                        if (res.SourceCulture.Code == language.Code)
                            translation = res.Value;
                    });

                    return translation;
                };

                var vm = this;

                vm.resources = undefined;
                vm.languages = undefined;
                vm.adminMode = undefined;

                vm.fetch = fetch;

                function fetch() {
                    $http.get('api/')
                        .success(function (data) {
                            try {
                                var response = angular.fromJson(data);
                                vm.resources = response.Resources;
                                vm.languages = response.Languages;
                                vm.adminMode = response.AdminMode;
                            } catch (e) {
                                // error may occur when service returns html for login page instead of json (unauthorized access, session expired, etc)
                                alert(e);
                            }
                        });
                }
            }
    ]);
})();

//$(function () {
//    $('.localization a').editable({
//        url: '@Url.Action("Update")',
//        type: 'textarea',
//        placement: 'top',
//        mode: 'popup',
//        title: '@Html.Translate(() => Resources.TranslationPopupHeader)',
//        emptytext: '@Html.Translate(() => Resources.Empty)'
//    });

//    $('#resourceList').on('submit', '.delete-form', function (e) {
//        e.preventDefault();

//        var $form = $(this);
//        var pk = $(this).find('input[name=pk]').val();
//        if (confirm('@Html.Translate(() => Resources.DeleteConfirm) `' + pk + '`?')) {
//            $.ajax({ url: $form.attr('action'), method: 'post', data: $form.serialize() });
//            $form.closest('.resource').remove();
//        }
//    });

//    var $filterForm = $('#resourceFilterForm'),
//        $filterInput = $filterForm.find('.form-control:first-child'),
//        $resourceList = $('#resourceList'),
//        $resourceItems = $resourceList.find('.resource'),
//        $showEmpty = $('#showEmptyResources');

//    function filter($item, query) {
//        if ($item.text().search(new RegExp(query, 'i')) > -1) {
//            $item.removeClass('hidden');
//        } else {
//            $item.addClass('hidden');
//        }
//    }

//    function filterEmpty($item) {
//        if ($item.find('.editable-empty').length == 0) {
//            $item.addClass('hidden');
//        }
//    }

//    function runFilter(query) {
//        // clear state
//        $resourceItems.removeClass('hidden');
//        $resourceItems.each(function () { filter($(this), query); });

//        if ($showEmpty.prop('checked')) {
//            // if show only empty - filter empty ones as well
//            $resourceItems.not('.hidden').each(function () { filterEmpty($(this)); });
//        }
//    }

//    $showEmpty.change(function () {
//        runFilter($filterInput.val());
//    });

//    var t;
//    $filterInput.on('input', function () {
//        clearTimeout(t);
//        t = setTimeout(function () { runFilter($filterInput.val()); }, 500);
//    });

//    $filterForm.on('submit', function (e) {
//        e.preventDefault();
//        clearTimeout(t);
//        runFilter($filterInput.val());
//    });

//    $('#newResource').on('click', function () {
//        $('.new-resource-form').removeClass('hidden');
//        $('#resourceKey').focus();
//    });

//    $('#cancelNewResource').on('click', function () {
//        $('.new-resource-form').addClass('hidden');
//    });

//    $('#saveResource').on('click', function () {
//        var $form = $('.new-resource-form'),
//            $resourceKey = $form.find('#resourceKey').val();

//        if ($resourceKey.length == 0) {
//            alert('Fill resource key');
//            return;
//        }

//        $.ajax({
//            url: '@Url.Action("Create")',
//            method: 'POST',
//            data: 'pk=' + $resourceKey
//        }).success(function () {
//            var $translations = $form.find('.resource-translation');

//            var requests = [];

//            $.map($translations, function (el) {
//                var $el = $(el);
//                requests.push($.ajax({
//                    url: '@Url.Action("Update")',
//                    method: 'POST',
//                    data: 'pk=' + $resourceKey + '&name=' + el.id + '&value=' + $el.val()
//                }));
//            });

//            $.when(requests).then(function () {
//                setTimeout(function () {
//                    location.reload();
//                }, 1000);
//            });
//        }).error(function (e) {
//            alert('Error: ' + e.Message);
//        });
//    });
//});
