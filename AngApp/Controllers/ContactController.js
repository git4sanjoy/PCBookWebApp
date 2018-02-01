var app = angular.module('PCBookWebApp');
app.controller('ContactController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "ContactController";
    }])