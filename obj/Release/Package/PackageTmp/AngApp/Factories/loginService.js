
var app = angular.module('PCBookWebApp');
app.factory('loginService', ['$http', function ($http) {
    var accesstoken = sessionStorage.getItem('accessToken');
    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }


    var fac = {};
    fac.login = function (userlogin) {
        var resp = $http({
            url: "/TOKEN",
            method: "POST",
            data: $.param({ grant_type: 'password', username: userlogin.username, password: userlogin.password }),
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        });
        return resp;
        
    }
    fac.register = function (userInfo) {
        var resp = $http({
            url: "/api/Account/Register",
            method: "POST",
            data: userInfo,
            headers: authHeaders
        });
        return resp;

    }
    fac.logout = function () {
        userService.CurrentUser = null;
        userService.SetCurrentUser(userService.CurrentUser);
        $http({
            url: "/api/Account/Logout",
            method: "POST",
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        });
    }
    return fac;
}])