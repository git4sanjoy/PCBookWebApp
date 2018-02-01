var app = angular.module('PCBookWebApp');

app.controller('UnitUsersController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";
        var accesstoken = sessionStorage.getItem('accessToken');

        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        $scope.userUnitList = [];
        $http({
            url: "/api/UnitUsers/GetUnitUsersList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.userUnitList = data;
            //console.log(data);
        });

        $scope.userList = [];
        $http({
            url: "/api/UnitUsers/GetUsersXeditDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.userList = data;
            //console.log(data);
        });

        $scope.unitList = [];
        $http({
            url: "/api/UnitUsers/GetUnitsXeditDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.unitList = data;
            //console.log(data);
        });

        $scope.submitUserUnitsForm = function () {
            $scope.submitted = true;
            if ($scope.userUnitsForm.$valid) {
                var aUnitUserObj = {
                    UnitUserId: 0,
                    Id: $scope.userListDdl.id,
                    UnitId: $scope.unitListDdl.value
                };

                $http({
                    url: "/api/UnitUsers",
                    data: aUnitUserObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.userUnitList = [];
                    $http({
                        url: "/api/UnitUsers/GetUnitUsersList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.userUnitList = data;
                    });

                    //$scope.userUnit = {};
                    $scope.submitted = false;
                    $scope.userUnitsForm.$setPristine();
                    $scope.userUnitsForm.$setUntouched();

                    $scope.message = "Successfully Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    angular.element('#userListDdl').focus();
                }).error(function (error) {
                    $scope.validationErrors = [];
                    if (error.ModelState && angular.isObject(error.ModelState)) {
                        for (var key in error.ModelState) {
                            $scope.validationErrors.push(error.ModelState[key][0]);
                        }
                    } else {
                        $scope.validationErrors.push('Unable to add Check User.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });

            }
            else {
                alert("Please  correct form errors!");
            }
        };
        $scope.remove = function (UserUnitId) {
            deleteProduct = confirm('Are you sure you want to delete the Unit User?');
            if (deleteProduct) {

                $http({
                    url: "/api/UnitUsers/" + UserUnitId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.userUnitList = [];
                    $http({
                        url: "/api/UnitUsers/GetUnitUsersList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.userUnitList = data;
                    });
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
        };

    }]);
