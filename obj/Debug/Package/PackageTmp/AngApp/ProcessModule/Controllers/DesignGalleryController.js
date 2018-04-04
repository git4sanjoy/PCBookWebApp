var app = angular.module('PCBookWebApp');
app.controller('DesignGalleryController', ['$scope', '$location', '$http', '$timeout', '$filter', 'AddDealService', '$mdDialog',
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
        $scope.newDeal = {};
        // For 3 DatePicker
        $scope.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.opened = true;
        };

        $scope.fromDatePickerIsOpen = false;
        $scope.FromDatePickerOpen = function () {
            this.fromDatePickerIsOpen = true;
            $scope.toDatePickerIsOpenPickerIsOpen = false;
        };
        $scope.toDatePickerIsOpen = false;
        $scope.ToDatePickerOpen = function () {
            this.toDatePickerIsOpen = true;
            $scope.fromDatePickerIsOpen = false;
        };
        //End DatePicker
        $scope.newDeal = {
            DealProductionDate: new Date(),
            Quantity: 0,
            FactoryName: 'None'
        };

        $scope.myInterval = 5000;

        $scope.designList = [];
        $scope.designImages = [];
        $http({
            method: 'Get',
            url: '/api/Deals/DealsLists'
        }).success(function (data, status, headers, config) {
            $scope.designList = data;
        }).error(function (data, status, headers, config) {
            $scope.message = 'Unexpected Error';
        });

        $scope.slides = [];
        $scope.showGallery = function (a) {
            $scope.showModal = true;
            //$scope.designImages = '';
            //$scope.designName = '';
            //$scope.designDescription = '';
            //console.log(aDesign);
            //$scope.designName = a.Name;
            //$scope.designDescription = a.Description;
            //$scope.designImages = a.Deal_Images;
            //console.log($scope.designImages);
        };
        $scope.showProduction = function (aDesign) {
            $scope.showModal = true;
            $scope.selectedDesign = aDesign;
        };
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
                $scope.message = "Successfully Production Created.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                angular.element('#FactoryName').focus();
            }).error(function (error) {
                $scope.message = 'Unable to save Bank' + error.message;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });

        };

        $scope.deleteProduction = function (aProduction) {
            console.log(aProduction.DealProductionId);

            var id = aProduction.DealProductionId;
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/DealProductions/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.success("Data deleted successfully.", "Success");
                }).error(function (data) {
                    alert('error occord')
                    $scope.message = "Data could not be deleted!";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.error("Data could not be deleted!", "Failed!");
                });
            };

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