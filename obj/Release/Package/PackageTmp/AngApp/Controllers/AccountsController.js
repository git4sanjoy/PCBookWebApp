var app = angular.module('PCBookWebApp');

app.controller('AccountsController', ['$scope', '$location', '$http', '$timeout', '$filter',
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


        $scope.accountInfo = [];
        $http({
            url: "/api/Account/UserInfo",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.accountInfo = data;
            //console.log(data);
        });




}]);
