var app = angular.module('PCBookWebApp');
app.controller('homeController', ['$scope', '$location', '$http', '$timeout', '$filter',
function ($scope, $location, $http, $timeout, $filter) {
    $scope.message = "PC App. V-1.0.1";
}])