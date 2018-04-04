var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('ConversionController', ['$scope', '$location', '$http', '$timeout', '$filter',
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

        $scope.conversions = [];
        loadData();
        function loadData()
        {
            $http({
                url: "api/Conversions/ConversionsList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.conversions = data;
                $scope.class = "";
                $scope.loading = false;
            });
        }
        

        $scope.matricsList = [];
        $http({
            url: "/api/Conversions/MatricsListXEdit",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.matricsList = data;
        });

        $scope.displayMatricName1 = function (conversionRow) {
            var selected = [];
            if (conversionRow.MatricId1) {
                selected = $filter('filter')($scope.matricsList, { value: conversionRow.MatricId1 });
            }
            return selected.length ? selected[0].text : 'Not set';
        };
        $scope.displayMatricName2 = function (conversionRow) {
            var selected = [];
            if (conversionRow.MatricId2) {
                selected = $filter('filter')($scope.matricsList, { value: conversionRow.MatricId2 });
            }
            return selected.length ? selected[0].text : 'Not set';
        };
        $scope.saveConversion = function (data, id) {
            var aObj = {
                ConversionId: id,
                ConversionName: data.ConversionName,
                MatricId1: data.MatricId1,
                MatricId2: data.MatricId2
            };
            if (id == 0) {
                $http({
                    url: "/api/Conversions",
                    data: aObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Conversion Created.";
                    loadData();
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
                    $.each($scope.conversions, function (i) {
                        if ($scope.conversions[i].id === 0) {
                            $scope.conversions.splice(i, 1);
                            return false;
                        }
                    });
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            } else {
                $http({
                    url: '/api/Conversions/' + id,
                    data: aObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Conversion Updated.";
                    loadData();
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
                        $scope.validationErrors.push('Unable to Update Conversion.');
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
                    url: "/api/Conversions/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.conversions.splice(index, 1);
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting! " + data;
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
            $.each($scope.conversions, function (i) {
                if ($scope.conversions[i].id === 0) {
                    $scope.conversions.splice(i, 1);
                    return false;
                }
            });
        };
        
        $scope.addConversion = function () {
            $scope.inserted = {
                id: 0,
                name: '',
                MatricId1: null,
                MatricId2:null,
                group: null
            };
            $scope.conversions.unshift($scope.inserted);
        };
    }])
