var app = angular.module('PCBookWebApp');

app.controller('PrimariesController', ['$scope', '$location', '$http', '$timeout', '$filter',
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
            url: "/api/Primaries/GetPrimaryList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            //console.log(data);
        });

        $scope.saveUser = function (data, id) {
            //$scope.user not updated yet
            angular.extend(data, { id: id });

            var project = {
                PrimaryId:id,
                PrimaryName: data.name
            };

            if (id == 0) {
                $http({
                    url: "/api/Primaries/PostProject",
                    data: project,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $http({
                        url: "/api/Primaries/GetPrimaryList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.users = data;
                        //console.log(data);
                    });
                    $scope.message = "Successfully Primary Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Primary' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });

            } else {
                $http({
                    url: '/api/Primaries/' + id,
                    data: project,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    //alert('Updated');
                    $scope.message = "Successfully Primary Updated.";
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
                        $scope.validationErrors.push('Unable to Update Primary.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
        };


        $scope.remove = function (index, ProjectId) {
            deleteProduct = confirm('Are you sure you want to delete the primary?');
            if (deleteProduct) {

                $http({
                    url: "/api/Primaries/" + ProjectId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users.splice(index, 1);
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //alert("Deleted successfully!!");
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
                name: '',
            };
            $scope.users.push($scope.inserted);
        };

}]);
