var app = angular.module('PCBookWebApp');
app.controller('UnitManagersController', ['$scope', '$location', '$http', '$timeout', '$filter',
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

        $scope.unitList = [];
        $http({
            url: "/api/UnitManagers/UnitsDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.unitList = data;
            //console.log(data);
        });
        $scope.usersList = [];
        $http({
            url: "/api/ShowRoomUsers/GetUsersDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.usersList = data;
            //console.log(data);
        });
        $scope.unitManagerList = [];
        $http({
            url: "/api/UnitManagers/UnitManagerList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.unitManagerList = data;
            //console.log(data);
        });
        $scope.submitUnitManagerForm = function () {
            $scope.submitted = true;
            if ($scope.unitManagerForm.$valid) {
                var aInsertObj = {
                    Id: $scope.unitManager.userCmb.Id,
                    UnitId: $scope.unitManager.unitCmb.UnitId,
                    UnitManagerName: $scope.unitManager.officerName,
                    Address: $scope.unitManager.address,
                    Phone: $scope.unitManager.phone,
                    Email: $scope.unitManager.email
                };
                //console.log(aPayment);
                $http({
                    url: "/api/UnitManagers",
                    data: aInsertObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.unitManagerList = [];
                    $http({
                        url: "/api/UnitManagers/UnitManagerList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.unitManagerList = data;
                        //console.log(data);
                    });
                }).error(function (error) {
                    $scope.message = 'Unable to save Unit Manager' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
            else {
                alert("Please  correct form errors!");
            }
        };

        $scope.delete = function (index, aObj) {
           
            deleteObj = confirm('Are you sure you want to delete the upazilla?');
            if (deleteObj) {
                var Id = aObj.UnitManagerId;
                $http({
                    url: "/api/UnitManagers/" + Id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.unitManagerList.splice(index, 1);
                    $scope.message = 'Payment Deleted Successfull!';
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.loginAlertMessage = true; }, 3000);
                    $scope.loading = false;
                    alert('Payment Deleted Successfull!');
                }).error(function (data) {
                    $scope.error = "An Error has occured while Deleteding Payment! " + data;
                    $scope.loading = true;
                });
            }
        };

    }])