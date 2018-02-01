var app = angular.module('PCBookWebApp');
app.controller('AboutController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "AboutController";
    }])