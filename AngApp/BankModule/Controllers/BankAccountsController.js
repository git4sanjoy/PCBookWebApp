var app = angular.module('PCBookWebApp');

app.controller('BankAccountsController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.loading = true;

        $scope.class = "overlay";
        $scope.changeClass = function () {
            if ($scope.class === "overlay")
                $scope.class = "";
            else
                $scope.class = "overlay";
        };


        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 200;
        $scope.currentPage = 1;


        var accesstoken = sessionStorage.getItem('accessToken');

        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        $scope.users = [];
        $http({
            url: "/api/BankAccounts/GetBankAccountsList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            $scope.class = "";
            $scope.loading = false;
            //console.log(data);
        });

        $scope.statuses = [];
        $http({
            url: "/api/BankAccounts/GetBanksXEditList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.statuses = data;
        });

        $scope.groups = [];
        $scope.loadGroups = function () {
            return $scope.groups.length ? null : $http({
                url: "/api/BankAccounts/GetGroupXEditList",
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

        $scope.showStatus = function (user) {
            var selected = [];
            if (user.status) {
                selected = $filter('filter')($scope.statuses, { value: user.status });
            }
            return selected.length ? selected[0].text : 'Not set';
        };

        $scope.checkName = function (data, id) {
            //if (id === 2 && data !== 'awesome') {
            //    return "Username 2 should be `awesome`";
            //}
        };

        $scope.saveUser = function (data, id) {
            angular.extend(data, { id: id});
            var bankId = data.status;
            var groupId = data.group;
            var name = data.name;

            var aLedgerObj = {
                LedgerId: 0,
                GroupId: groupId,
                LedgerName: name,
                ShowRoomId: 0,
                BookId: 0,
                TrialBalanceId: 0,
                TrialBalance: false,
                Provision: false
            };


            if (id == 0) {
                var newLedgerId = 0;
                $http({
                    url: "/api/Ledgers",
                    data: aLedgerObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    console.log(data.LedgerId);
                    var newLedgerId = data.LedgerId;
                    var bankAccountObj = {
                        BankAccountId: id,
                        GroupId: groupId,
                        BankId: bankId,
                        BankAccountNumber: name,
                        LedgerId: newLedgerId,
                        ShowRoomId: 0
                    };
                    $http({
                        url: "api/BankAccounts/PostBankAccount",
                        data: bankAccountObj,
                        method: "POST",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.message = "Successfully Bank Account Created.";
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
                            $scope.validationErrors.push('Unable to Add Bank Account.');
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
                }).error(function (error) {

                });
                
                

            } else {
                var bankAccountObj = {
                    BankAccountId: id,
                    GroupId: groupId,
                    BankId: bankId,
                    BankAccountNumber: name,
                    LedgerId: 0,
                    ShowRoomId: 0
                };
                $http({
                    url: '/api/BankAccounts/' + id,
                    data: bankAccountObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Successfully Bank Account Updated.";
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
                        $scope.validationErrors.push('Unable to Update Bank Account.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
        };


        $scope.remove = function (index, id) {
            deleteProduct = confirm('Are you sure you want to delete the Bank Account?');
            if (deleteProduct) {
                $http({
                    url: "/api/BankAccounts/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users.splice(index, 1);
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
        };


        // add user
        $scope.addUser = function () {
            $scope.inserted = {
                id: 0,
                name: '',
                status: null,
                group: null,
                rate: 0
            };
            //$scope.users.push($scope.inserted);
            $scope.users.unshift($scope.inserted);
        };

    }]);
