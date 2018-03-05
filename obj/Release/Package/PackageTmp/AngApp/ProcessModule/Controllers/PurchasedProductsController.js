var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('PurchasedProductsController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.Message = "Purchased Products Controller Angular JS";

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
            url: "/api/PurchasedProducts/PurchasedProductsList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            $scope.class = "";
            $scope.loading = false;
            //console.log(data);
        });

        $scope.statuses = [];
        $http({
            url: "/api/PurchasedProducts/MatricsListXEdit",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.statuses = data;
        });

        $scope.groups = [];
        $scope.loadGroups = function () {
            return $scope.groups.length ? null : $http({
                url: "/api/PurchasedProducts/ProductTypeListXEdit",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.groups = data;
            });
        };


        $scope.showGroup = function (user) {
            if (user.group && $scope.groups.length) {
                var selected = $filter('filter')($scope.groups, { id: user.group });
                return selected.length ? selected[0].text : 'Not set';
            } else {
                return user.groupName || 'Not set';
            }
        };

        $scope.showStatus = function (user) {
            var selected = [];
            if (user.status) {
                selected = $filter('filter')($scope.statuses, { value: user.status });
            }
            return selected.length ? selected[0].text : 'Not set';
        };

        $scope.saveUser = function (data, id) {
            angular.extend(data, { id: id });

            var aObj = {
                PurchasedProductId: id,
                ProductTypeId: data.group,
                MatricId: data.status,
                PurchasedProductName: data.name,
                ShowRoomId:0
            };

            if (id == 0) {
                $http({
                    url: "/api/PurchasedProducts",
                    data: aObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Product Created.";
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
                        $scope.validationErrors.push('Unable to Update Prodect.');
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
                    url: '/api/PurchasedProducts/' + id,
                    data: aObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Product Updated.";
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
            deleteProduct = confirm('Are you sure you want to delete the Product?');
            if (deleteProduct && id > 0) {
                $http({
                    url: "/api/PurchasedProducts/" + id,
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


        // add Product
        $scope.addProduct = function () {
            $scope.inserted = {
                id: 0,
                name: '',
                status: null,
                group: null
            };
            //$scope.users.push($scope.inserted);
            $scope.users.unshift($scope.inserted);
        };


    }])
