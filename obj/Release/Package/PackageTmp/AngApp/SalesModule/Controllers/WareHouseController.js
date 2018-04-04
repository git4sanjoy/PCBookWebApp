var app = angular.module('PCBookWebApp');
app.controller('WareHouseController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {

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
        $scope.users = [];
        $http({
            url: "/api/WareHouses/WareHouseList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            $scope.class = "";
            $scope.loading = false;
            //console.log(data);
        });

        $scope.saveOrUpdate = function (data, id) {
            angular.extend(data, { id: id });

            var aObj = {
                WareHouseId: id,
                WareHouseName: data.WareHouseName,
                WareHouseLocation: data.WareHouseLocation
            };

            if (id == 0) {
                $http({
                    url: "/api/WareHouses",
                    data: aObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Division Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.validationErrors = [];
                    if (error.ModelState && angular.isObject(error.ModelState)) {
                        for (var key in error.ModelState) {
                            $scope.validationErrors.push(error.ModelState[key][0]);
                        }
                    } else {
                        $scope.validationErrors.push('Unable to Update Sale Zone.');
                    };
                    $.each($scope.users, function (i) {
                        if ($scope.users[i].id === 0) {
                            $scope.users.splice(i, 1);
                            return false;
                        }
                    });
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            } else {
                $http({
                    url: '/api/WareHouses/' + id,
                    data: aObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Division Updated.";
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
                        $scope.validationErrors.push('Unable to Update Prodect.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
        };


        $scope.remove = function (index, id) {
            deleteProduct = confirm('Are you sure you want to delete the Sale Zone?');
            if (deleteProduct && id > 0) {
                $http({
                    url: "/api/WareHouses/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users.splice(index, 1);
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
            $.each($scope.users, function (i) {
                if ($scope.users[i].id === 0) {
                    $scope.users.splice(i, 1);
                    return false;
                }
            });
        };


        // add 
        $scope.add = function () {
            $scope.inserted = {
                id: 0,
                WareHouseName: '',
                WareHouseLocation: null
            };
            $scope.users.unshift($scope.inserted);
        };
    }])