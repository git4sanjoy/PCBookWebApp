var app = angular.module('PCBookWebApp');
app.service('dataservice', function ($http) {
    this.get = function () {
        
        var accesstoken = sessionStorage.getItem('accessToken');

        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        var response = $http({
            url: "/api/Values",
            method: "GET",
            headers: authHeaders
        });
        return response;
    };
});