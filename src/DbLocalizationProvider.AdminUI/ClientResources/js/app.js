; (function () {
    angular.module('resourceUIApp', ['ui.bootstrap'])
    .controller('resourcesController', ['$scope', '$uibModal', '$http',
            function ($scope, $uibModal, $http) {

                $scope.getTranslation = function (resource, language) {
                    var translation = null;
                    angular.forEach(resource.Value, function (res) {
                        if (res.SourceCulture.Code == language.Code && res.Value.length > 0)
                            translation = res.Value;
                    });

                    return translation;
                };

                var vm = this;

                vm.resources = undefined;
                vm.languages = undefined;
                vm.adminMode = undefined;

                vm.fetch = fetch;

                vm.open = function (resource, lang) {

                    var selectedResource = resource,
                        selectedLanguage = lang;

                    var modalInstance = $uibModal.open({
                        templateUrl: 'popup-content.html',
                        controller: 'ModalInstanceCtrl',
                        size: 'lg',
                        resolve: {
                            resource: function () { return selectedResource; },
                            translation: function() {
                                var translation = $scope.getTranslation(selectedResource, selectedLanguage);
                                return translation == "N/A" ? "" : translation;
                            }
                        }
                    });

                    modalInstance.result.then(
                        function (translation) {
                            $http.post('api/update', { key: selectedResource.Key, language: selectedLanguage.Code, newTranslation: translation })
                            .success(function () {
                                    // TODO: show notification
                                    vm.fetch();
                                });
                        });
                };


                function fetch() {
                    $http.get('api/get')
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


    angular.module('resourceUIApp').controller('ModalInstanceCtrl', function ($scope, $uibModalInstance, resource, translation) {

        $scope.resource = resource;
        $scope.translation = translation;

        $scope.ok = function () {
            $uibModalInstance.close($scope.translation);
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });
})();
