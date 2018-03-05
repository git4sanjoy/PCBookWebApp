var app = angular.module('PCBookWebApp');

app.controller('BanksController', ['$scope', '$location', '$http', '$timeout', '$filter',
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
            url: "/api/Banks/GetBanksList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            //console.log(data);
        });

        $scope.saveUser = function (data, id) {
            //$scope.user not updated yet
            angular.extend(data, { id: id });

            var bank = {
                BankId: id,
                BankName: data.name,
                Address: data.Address,
                Email: data.Email,
                Phone: data.Phone,
                Website: data.Website
            };

            if (id == 0) {
                $http({
                    url: "/api/Banks/PostBank",
                    data: bank,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Bank Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Bank' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });

            } else {
                $http({
                    url: '/api/Banks/' + id,
                    data: bank,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    //alert('Updated');
                    $scope.message = "Successfully Bank Updated.";
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
                        $scope.validationErrors.push('Unable to Update Bank.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
        };


        $scope.remove = function (index, ProjectId) {
            deleteProduct = confirm('Are you sure you want to delete the project?');
            if (deleteProduct) {

                $http({
                    url: "/api/Banks/" + ProjectId,
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
                Address: '',
                Email: '',
                Phone: '',
                Website: ''
            };
            $scope.users.unshift($scope.inserted);
        };

    }]);
