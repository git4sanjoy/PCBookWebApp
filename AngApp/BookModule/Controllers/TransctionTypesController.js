var app = angular.module('PCBookWebApp');

app.controller('TransctionTypesController', ['$scope', '$location', '$http', '$timeout', '$filter',
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
        $scope.users = [];
        $http({
            url: "/api/TransctionTypes/GetTransctionTypeList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            //console.log(data);
        });
        $scope.saveUser = function (data, id) {
            //$scope.user not updated yet
            angular.extend(data, { id: id });

            var productObj = {
                TransctionTypeId: id,
                TransctionTypeName: data.name,
            };

            if (id == 0) {
                $http({
                    url: "/api/TransctionTypes",
                    data: productObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users = [];
                    $http({
                        url: "/api/TransctionTypes/GetTransctionTypeList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.users = data;
                        //console.log(data);
                    });
                    $scope.message = "Successfully Transction Type Saved.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Matric' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            } else {
                $http({
                    url: '/api/TransctionTypes/' + id,
                    data: productObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Transction Type Updated.";
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
                        $scope.validationErrors.push('Unable to Update Transction Type.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
        };


        $scope.remove = function (index, TransctionTypeId) {
            deleteProduct = confirm('Are you sure you want to delete the Transction Type?');
            if (deleteProduct) {
                $http({
                    url: "/api/TransctionTypes/" + TransctionTypeId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users.splice(index, 1);
                    $scope.message = "Successfully Transction Type Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    alert("Deleted successfully!!");
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
        };

        // add user
        $scope.addUser = function () {
            $scope.inserted = {
                //id: $scope.users.length + 1,
                id: 0,
                name: ''
            };
            $scope.users.push($scope.inserted);
        };

    }]);
