var app = angular.module('PCBookWebApp');
app.controller('CostCenterController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
    //$scope.Message = "Cost Center Controller!";
    $scope.loading = true;

    $scope.clientMessage = true;
    $scope.serverMessage = true;
    $scope.messageType = "";
    $scope.message = "";

    $scope.pageSize = 12;
    $scope.currentPage = 1;

    var accesstoken = sessionStorage.getItem('accessToken');
    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }


    $scope.users = [];
    $http({
        url: "/api/CostCenters/GetCostCentersList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.users = data;
    });


    $scope.groups = [];
    $scope.loadGroups = function () {
        return $scope.groups.length ? null : $http({
            url: "/api/Ledgers/LedgersXEditList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.groups = data;
        });
    };



    $scope.showGroup = function (user) {
        if (user.group && $scope.groups.length) {
            var selected = $filter('filter')($scope.groups, { id: user.group });
            return selected.length ? selected[0].text : 'Not set';
        } else {
            return user.groupName || 'Not set';
        }
    };



    $scope.saveUser = function (data, id) {
        angular.extend(data, { id: id });

        var aObj = {
            CostCenterId: id,
            LedgerId: data.group,
            CostCenterName: data.name,
            ShowRoomId: 0
        };

        if (id == 0) {
            $http({
                url: "/api/CostCenters",
                data: aObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Cost Center Created.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to add Ledger.');
                };
                $.each($scope.users, function (i) {
                    if ($scope.users[i].id === 0) {
                        $scope.users.splice(i, 1);
                        return false;
                    }
                });
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });

        } else {

            $http({
                url: '/api/CostCenters/' + id,
                data: aObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Cost Center Updated.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to Update Sub Material.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        }
    };


    $scope.remove = function (index, id) {
        deleteProduct = confirm('Are you sure you want to delete the Cost Center?');
        if (deleteProduct) {
            $http({
                url: "/api/CostCenters/" + id,
                method: "DELETE",
                headers: authHeaders
            }).success(function (data) {
                $scope.users.splice(index, 1);
                $scope.message = "Successfully Cost Center Deleted.";
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "An error has occured while deleting Cost Center! " + data;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }
    };
    $scope.checkName = function (data, id) {

    };

    $scope.addUser = function () {
        $scope.inserted = {
            id: 0,
            name: '',
            group: null
        };
        $scope.users.unshift($scope.inserted);
    };
    $scope.sort = function (keyname) {
        $scope.sortKey = keyname;   //set the sortKey to the param passed
        $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    };

}])