var app = angular.module('PCBookWebApp');
app.controller('DesignController', ['$scope', '$location', '$http', '$timeout', '$filter', 'AddDealService', '$mdDialog',
    function ($scope, $location, $http, $timeout, $filter, AddDealService, $mdDialog) {

        $scope.loading = true;

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";
        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }
        $scope.myInterval = 5000;

        $scope.designList = [];
        $scope.designImages = [];
        //$http({
        //    method: 'Get',
        //    url: '/api/Deals/DealsLists'
        //}).success(function (data, status, headers, config) {
        //    $scope.designList = data;
        //}).error(function (data, status, headers, config) {
        //    $scope.message = 'Unexpected Error';
        //});

        //Add File start.....
        $scope.getTheFiles = function ($files) {

            $scope.imagesrc = [];

            for (var i = 0; i < $files.length; i++) {

                var reader = new FileReader();
                reader.fileName = $files[i].name;

                reader.onload = function (event) {

                    var image = {};
                    image.Name = event.target.fileName;
                    image.Size = (event.total / 1024).toFixed(2);
                    image.Src = event.target.result;
                    $scope.imagesrc.push(image);
                    $scope.$apply();
                }
                reader.readAsDataURL($files[i]);
            }

            $scope.Files = $files;

        };
        // Add File End...
        // Submit Forn data
        $scope.Submit = function () {
            //FILL FormData WITH FILE DETAILS.
            var data = new FormData();

            angular.forEach($scope.Files, function (value, key) {
                data.append(key, value);
            });

            data.append("DealModel", angular.toJson($scope.DealDetail));
            AddDealService.AddDeal(data).then(function (response) {
                $scope.DealDetail.Name = '';
                $scope.DealDetail.Description = '';
                $scope.DealDetail.Quantity = 0;
                $scope.imagesrc = [];

                $scope.designList = [];
                $http({
                    method: 'Get',
                    url: '/api/Deals/DealsLists'
                }).success(function (data, status, headers, config) {
                    $scope.designList = data;
                    $scope.loading = true;
                    $scope.message = "Successfully Gallery Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (data, status, headers, config) {
                    $scope.message = 'Unexpected Error';
                });
                //alert("Added Successfully");
            }, function () {

            });
        };
        $scope.myInterval = 5000;
        $scope.slides = [];

        $scope.saveProduction = function () {
            //console.log($scope.selectedDesign);
            if ($scope.newDeal.FactoryName == '') {
                alert("Please input remarks");
                return false;
            }
            if ($scope.newDeal.Quantity == 0) {
                alert("Please input quantity");
                angular.element('#Quantity').focus();
                return false;
            }
            var fd = $filter('date')($scope.newDeal.DealProductionDate, "yyyy-MM-dd");
            var aDealObj = {
                DealProductionDate: fd,
                Quantity: $scope.newDeal.Quantity,
                DealId: $scope.selectedDesign.Id,
                FactoryName: $scope.newDeal.FactoryName
            };
            //console.log(aDealObj);
            //return false;
            $http({
                url: "/api/DealProductions",
                data: aDealObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                $scope.newDeal = {
                    DealProductionDate: fd,
                    Quantity: 0,
                    FactoryName: 'None'
                };
                //$scope.message = "Successfully Bank Created.";
                //$scope.messageType = "success";
                //$scope.clientMessage = false;
                //$timeout(function () { $scope.clientMessage = true; }, 5000);
                angular.element('#FactoryName').focus();
            }).error(function (error) {
                $scope.message = 'Unable to save Bank' + error.message;
                //$scope.messageType = "warning";
                //$scope.clientMessage = false;
                //$timeout(function () { $scope.clientMessage = true; }, 5000);
            });

        };
        $scope.Clear = function () {
            $scope.DealDetail = {};
            $scope.imagesrc = [];
            $scope.designList = [];
        };

        //$scope.status = '  ';
        //$scope.customFullscreen = false;

        //$scope.showTabDialog = function (ev, aDesign) {
        //    console.log(aDesign);
        //    $scope.aDesignObj = aDesign;
        //    $mdDialog.show({
        //        controller: DialogController,
        //        templateUrl: 'AngApp/ProcessModule/Views/tabDialog.tmpl.html',
        //        parent: angular.element(document.body),
        //        targetEvent: ev,
        //        clickOutsideToClose: true
        //    })
        //    .then(function (answer) {
        //        $scope.status = 'You said the information was "' + answer + '".';
        //    }, function () {
        //        $scope.status = 'You cancelled the dialog.';
        //    });
        //};

        //$scope.showPrerenderedDialog = function (ev) {
        //    $mdDialog.show({
        //        contentElement: '#myDialog',
        //        parent: angular.element(document.body),
        //        targetEvent: ev,
        //        clickOutsideToClose: true
        //    });
        //};

        //function DialogController($scope, $mdDialog) {
        //    $scope.hide = function () {
        //        $mdDialog.hide();
        //    };

        //    $scope.cancel = function () {
        //        $mdDialog.cancel();
        //    };

        //    $scope.answer = function (answer) {
        //        $mdDialog.hide(answer);
        //    };
        //}
    }])