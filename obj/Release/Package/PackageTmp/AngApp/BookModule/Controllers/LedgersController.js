var app = angular.module('PCBookWebApp');
app.controller('LedgersController', ['$scope', '$location', '$http', '$timeout', '$filter', function ($scope, $location, $http, $timeout, $filter) {
    //$scope.Message = "404 Not Found!";
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
        url: "/api/Ledgers/GetLedgerList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.users = data;
    });


    $scope.groups = [];
    $scope.loadGroups = function () {
        return $scope.groups.length ? null : $http({
            url: "/api/Ledgers/GetGroupListXEdit",
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

        var aLedgerObj = {
            LedgerId: id,
            GroupId: data.group,
            LedgerName: data.name,
            ShowRoomId: 0,
            BookId: 0,
            TrialBalanceId: 0,
            TrialBalance: false,
            Provision: false
        };

        if (id == 0) {
            $http({
                url: "/api/Ledgers",
                data: aLedgerObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Ledger Created.";
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
                url: '/api/Ledgers/' + id,
                data: aLedgerObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Ledger Updated.";
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
        deleteProduct = confirm('Are you sure you want to delete the Ledger?');
        if (deleteProduct) {
            $http({
                url: "/api/Ledgers/" + id,
                method: "DELETE",
                headers: authHeaders
            }).success(function (data) {
                $scope.users.splice(index, 1);
                $scope.message = "Successfully Deleted.";
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "An error has occured while deleting ledger! " + data;
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