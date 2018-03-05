
var app = angular.module('PCBookWebApp');
app.controller('ConversionController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.ConversionObj = [];

        $scope.message = "Conversion Controller";

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

        //**Get Conversion List**
        $scope.GetConversionList = function () {
            $http({
                url: '/api/Conversions/ConversionsList',
                type: 'GET',
                headers: authHeaders
            }).success(function (data) {
                $scope.ConversionObj = data;
            }).error(function (data) {
                $scope.message = "Conversion list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetConversionList();

        //**Save Conversion**
        $scope.Save = function (conversion) {
            console.log(conversion);
            $http({
                url: '/api/Conversions/PostConversion',                
                data: conversion,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {                
                $scope.Cancel();
                $scope.GetConversionList();
                $scope.message = "Conversion data saved successfully.";               
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);               
                }).error(function (data) {                   
                $scope.message = "Conversion data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);              
            });

        };

        $scope.updateData = function (data, id) {
            /// alert(id2);
            //alert(PurchasedProductId);
            var obj = {
                ConversionId: id,
                ConversionName: data.ConversionName,
                ShowRoomId: data.ShowRoomId
            };
            //alert(obj.PurchasedProductId);
            angular.extend(data, { ConversionId: id });
            return $http({
                url: '/api/Conversions/' + id,
                data: obj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Updated.";
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

        $scope.remove = function (index, id) {
            // alert(id)
            userPrompt = confirm('Are you sure you want to delete the Process List?');
            if (userPrompt) {
                $http({
                    url: "/api/Conversions/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.GetConversionList();
                    $scope.ProcessLists.splice(index, 1);
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;

                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting Process List! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }

        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.conversion = '';           
            $('.saveButton').show();
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
        };


    }])