var app = angular.module('PCBookWebApp');
app.controller('navController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        
        $scope.userName = sessionStorage.getItem('userName');
        var accesstoken = sessionStorage.getItem('accessToken');
        $scope.token = accesstoken;

                var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }


        $scope.logout = function () {

            $http({
                url: "/api/Account/Logout",
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                
            }).error(function (error) {
                //alert('Authorize Successfully');
            });
            sessionStorage.removeItem('accessToken');
            window.location.href = '/';
        };

    }])