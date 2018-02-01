var app = angular.module('PCBookWebApp');
app.controller('VoucherTypesController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        //$scope.Message = "404 Not Found!";
        $scope.loading = true;

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 12;
        $scope.currentPage = 1;

        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }


        $scope.users = [];
        $http({
            url: "/api/VoucherTypes/GetVoucherTypeList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
        });

        $scope.saveUser = function (data, id) {
            angular.extend(data, { id: id });

            var productObj = {
                VoucherTypeId: id,
                VoucherTypeName: data.name,
                ShowRoomId: 0
            };


            if (id == 0) {
                $http({
                    url: "/api/VoucherTypes",
                    data: productObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Voucher Type Created.";
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
                        $scope.validationErrors.push('Unable to add Voucher Type.');
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
                    url: '/api/VoucherTypes/' + id,
                    data: productObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Voucher Type Updated.";
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
                        $scope.validationErrors.push('Unable to Update Sub Material.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
        };


        $scope.remove = function (index, MatricId) {
            deleteProduct = confirm('Are you sure you want to delete the Voucher Type?');
            if (deleteProduct) {
                $http({
                    url: "/api/VoucherTypes/" + MatricId,
                    method: "DELETE",
                    //headers: authHeaders
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
        };
        $scope.checkName = function (data, id) {

        };

        $scope.addUser = function () {
            $scope.inserted = {
                id: 0,
                name: ''
            };
            $scope.users.unshift($scope.inserted);
        };
        $scope.sort = function (keyname) {
            $scope.sortKey = keyname;   //set the sortKey to the param passed
            $scope.reverse = !$scope.reverse; //if true make it false and vice versa
        };
}])