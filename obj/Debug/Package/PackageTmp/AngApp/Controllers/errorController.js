var app = angular.module('PCBookWebApp');
app.controller('errorController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "Oops! You are not authorized to use this page.";


}])