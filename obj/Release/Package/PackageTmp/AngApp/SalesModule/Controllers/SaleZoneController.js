var app = angular.module('PCBookWebApp');
app.controller('SaleZoneController', ['$scope', '$location', '$http', '$timeout', '$filter',
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
            url: "/api/SaleZones/SaleZonesList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            $scope.class = "";
            $scope.loading = false;
            //console.log(data);
        });


        $scope.groups = [];
        $scope.loadGroups = function () {
            return $scope.groups.length ? null : $http({
                url: "/api/SaleZones/ZoneManagersListXEdit",
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



        $scope.saveOrUpdateZone = function (data, id) {
            angular.extend(data, { id: id });

            var aObj = {
                SaleZoneId: id,
                ZoneManagerId: data.group,
                SaleZoneName: data.name,
                SaleZoneDescription: data.SaleZoneDescription
            };

            if (id == 0) {
                $http({
                    url: "/api/SaleZones",
                    data: aObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Sale Zone Created.";
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
                    url: '/api/SaleZones/' + id,
                    data: aObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Sale Zone Updated.";
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
                    url: "/api/SaleZones/" + id,
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
        $scope.addZone = function () {
            $scope.inserted = {
                id: 0,
                name: '',
                group: null
            };
            $scope.users.unshift($scope.inserted);
        };

    }])