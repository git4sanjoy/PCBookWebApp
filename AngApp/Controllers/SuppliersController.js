var app = angular.module('PCBookWebApp');

app.controller('SuppliersController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {

        $scope.loading = true;

        $scope.class = "overlay";
        $scope.changeClass = function () {
            if ($scope.class === "overlay")
                $scope.class = "";
            else
                $scope.class = "overlay";
        };


        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 200;
        $scope.currentPage = 1;


        var accesstoken = sessionStorage.getItem('accessToken');

        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        $scope.users = [];
        $http({
            url: "/api/Suppliers/GetSupplierList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            $scope.class = "";
            $scope.loading = false;
            //console.log(data);
        });



        $scope.checkName = function (data, id) {
            //if (id === 2 && data !== 'awesome') {
            //    return "Username 2 should be `awesome`";
            //}
        };

        $scope.saveUser = function (data, id) {
            angular.extend(data, { id: id });

            var productObj = {
                SupplierId: id,
                SupplierName: data.name,
                SupplierAddress: data.SupplierAddress,
                SupplierEmail: data.SupplierEmail,
                SupplierPhone: data.SupplierPhone
            };

            if (id == 0) {
                $http({
                    url: "api/Suppliers",
                    data: productObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Suppliers Created.";
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
                        $scope.validationErrors.push('Unable to Update Suppliers.');
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
                    url: '/api/Suppliers/' + id,
                    data: productObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Suppliers Updated.";
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
                        $scope.validationErrors.push('Unable to Update Suppliers.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
        };


        $scope.remove = function (index, SupplierId) {
            deleteProduct = confirm('Are you sure you want to delete the Suppliers?');
            if (deleteProduct) {
                $http({
                    url: "/api/Suppliers/" + SupplierId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users.splice(index, 1);
                    $scope.message = "Successfully Supplier Deleted.";
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


        // add user
        $scope.addUser = function () {
            $scope.inserted = {
                id: 0,
                name: '',
                status: null,
                group: null
            };
            //$scope.users.push($scope.inserted);
            $scope.users.unshift($scope.inserted);
        };



    }]);