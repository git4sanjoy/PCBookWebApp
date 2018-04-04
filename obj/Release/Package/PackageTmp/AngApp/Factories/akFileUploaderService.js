
var app = angular.module('PCBookWebApp');
app.factory('akFileUploaderService', ['$http','$q', function ($http, $q) {
    var accesstoken = sessionStorage.getItem('accessToken');
    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }

    //var getModelAsFormData = function (data) {
    //    var dataAsFormData = new FormData();
    //    angular.forEach(data, function (value, key) {
    //        dataAsFormData.append(key, value);
    //    });
    //    return dataAsFormData;
    //};
    var getModelAsFormData = function (data) {
        var dataAsFormData = new FormData();
        angular.forEach(data, function (value, key) {
            if (key == "attachment") {
                console.log(value);
                for (var i = 0; i < value.length; i++) {
                    dataAsFormData.append(value[i].name, value[i]);
                }
            } else {
                dataAsFormData.append(key, value);
            }
        });
        return dataAsFormData;
    };

    var saveModel = function (data, url) {
        var deferred = $q.defer();
        $http({
            url: url,
            method: "POST",
            data: getModelAsFormData(data),
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).success(function (result) {
            deferred.resolve(result);
        }).error(function (result, status) {
            deferred.reject(status);
        });
        return deferred.promise;
    };

    return {
        saveModel: saveModel
    }

}])

