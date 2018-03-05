var app = angular.module('PCBookWebApp');
app.controller('ZoneManagerController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "ZoneManagerController";
        $scope.loading = true;

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 20;
        $scope.currentPage = 1;

        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        $scope.zoneManagerList = [];
        $http({
            method: 'GET',
            url: '/api/ZoneManagers/ZoneManagerList',
            contentType: "application/json; charset=utf-8",
            headers: authHeaders,
            dataType: 'json'
        }).success(function (data) {
            $scope.zoneManagerList = data;
        });

        $scope.checkName = function (data, ZoneManagerId) {
            //alert(ZoneManagerId);
        };
        $scope.submitZoneManagerForm = function () {
            $scope.submitted = true;
            if ($scope.zoneManagerForm.$valid) {
                var aObj = {
                    ZoneManagerName: $scope.zoneManager.ZoneManagerName,
                    Address: $scope.zoneManager.Address,
                    Phone: $scope.zoneManager.Phone,
                    Email: $scope.zoneManager.Email,
                    ShowRoomId: 0
                };

                $http({
                    url: "/api/ZoneManagers",
                    data: aObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    var aViewObj = {
                        ZoneManagerId: data.ZoneManagerId,
                        ZoneManagerName: $scope.zoneManager.ZoneManagerName,
                        Address: $scope.zoneManager.Address,
                        Phone: $scope.zoneManager.Phone,
                        Email: $scope.zoneManager.Email
                    };
                    $scope.zoneManagerList.push(aViewObj);
                    $scope.zoneManager = {};
                    $scope.submitted = false;
                    $scope.zoneManagerForm.$setPristine();
                    $scope.zoneManagerForm.$setUntouched();
                    $scope.loading = true;
                    angular.element('#ZoneManagerName').focus();
                    $scope.message = "Successfully Zone Manager Saved.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Sales Officer' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
            else {
                alert("Please  correct form errors!");
            }
        };
        //Update zoneManager
        $scope.update = function (data, ZoneManagerId) {
            angular.extend(data, { ZoneManagerId: ZoneManagerId });
            return $http({
                url: '/api/ZoneManagers/' + ZoneManagerId,
                data: data,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {

                $scope.message = "Successfully Zone Manager info Updated.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to Update Zone Manager.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        };

        // remove zoneManager
        $scope.remove = function (index, ZoneManagerId) {
            deleteDepartment = confirm('Are you sure you want to delete the Zone Manager?');
            if (deleteDepartment) {
                //Your action will goes here
                $http.delete('/api/ZoneManagers/' + ZoneManagerId)
                    .success(function (data) {
                        $scope.zoneManagerList.splice(index, 1);
                    })
                    .error(function (data) {
                        $scope.error = "An error has occured while deleting! " + data;
                    });
                //alert("Deleted successfully!!");
            }
        };
        //Clear
        $scope.clear = function () {
            $scope.zoneManager = {};
            $scope.submitted = false;
            $scope.zoneManagerForm.$setPristine();
            $scope.zoneManagerForm.$setUntouched();
            $scope.loading = true;
            angular.element('#ZoneManagerName').focus();
        };
    }])