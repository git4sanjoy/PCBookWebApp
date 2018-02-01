var app = angular.module('PCBookWebApp');
app.controller('UnauthorizedController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "Error Controller JS";


    }])