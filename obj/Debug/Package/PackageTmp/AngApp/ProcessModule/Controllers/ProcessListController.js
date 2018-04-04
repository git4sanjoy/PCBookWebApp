var app = angular.module('PCBookWebApp');
app.controller('ProcessListController', ['$scope', '$location', '$http', '$timeout', '$filter', '$route',
    function ($scope, $location, $http, $timeout, $filter, $route) {
        $scope.message = "Process List Controller";

        $scope.loading = true;

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 20;
        $scope.currentPage = 1;

        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        //$scope.unitRoles = [];
        $scope.GetunitRoles = function () {
            $http({
                url: "/api/UnitRoles/GetUnitRolesList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.unitRoles = data;
                //console.log(data);
            });
        }
        $scope.GetunitRoles();
        $scope.ProcessLists = [];

        $scope.GetList = function () {
            $http({
                url: "/api/ProcessLists/GetProcessList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.ProcessLists = data;
                //console.log(data);
            });
        }
        $scope.GetList();
        $scope.submitProcessListForm = function () {
            // Set the 'submitted' flag to true
            $scope.submitted = true;
            if ($scope.ProcessListForm.$valid) {
                var ProcessListObj = {

                    UnitRoleId: $scope.ProcessLists.UnitRoleId.UnitRoleId,
                    ProcessListName: $scope.ProcessListName,

                };
                $http({
                    url: "/api/ProcessLists",
                    data: ProcessListObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    var aViewObj = {

                        id: data.ProcessListId,
                        ProcessListName: $scope.ProcessListName,
                        UnitRoleName: $scope.ProcessLists.UnitRoleId.UnitRoleName,

                    };

                    if (data.ProcessListId > 0) {
                        $scope.message = "Successfully Process Created.";
                        $scope.messageType = "success";
                        $scope.clientMessage = false;
                        $scope.ProcessListName = '';
                        $scope.ProcessLists.UnitRoleId = '';


                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                    }
                    else {
                        //$route.reload();

                        $scope.message = "Duplicate Entry...Same value exists in another field.";
                        $scope.messageType = "warning";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                        $http({
                            url: "/api/ProcessLists/GetProcessList",
                            method: "GET",
                            headers: authHeaders
                        }).success(function (data) {
                            $scope.ProcessLists = data;
                            //console.log(data);
                        });
                    }

                    $scope.ProcessLists.push(aViewObj);

                    $scope.GetunitRoles();
                    $scope.GetList();
                    $scope.submitted = false;
                    $scope.ProcessListForm.$setPristine();
                    $scope.ProcessListForm.$setUntouched();
                    $scope.loading = true;

                    //$route.reload();
                    $scope.clientMessage = false;
                    angular.element('#ProcessList').focus();
                    $timeout(function () { $scope.clientMessage = true; }, 5000);


                }).error(function (error) {
                    $scope.validationErrors = [];
                    if (error.ModelState && angular.isObject(error.ModelState)) {
                        for (var key in error.ModelState) {
                            $scope.validationErrors.push(error.ModelState[key][0]);
                        }
                    } else {
                        $scope.validationErrors.push('Unable to add Abc.');
                    };

                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
            else {
                alert("Please  correct form errors!");
            }
        };

        // remove 
        $scope.remove = function (index, id) {

            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/ProcessLists/" + id,
                    method: "DELETE",
                    headers: authHeaders

                }).success(function (data) {
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    $scope.GetunitRoles();
                    $scope.GetList();
                    $scope.Cancel();

                }).error(function (data) {
                    alert('error occord')
                    $scope.message = "Data could not be deleted!";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);

                });
            };

        };

        $scope.Update = function (data, id, id2) {
            //alert(id2);
            var aEditObj = {
                ProcessListId: id,
                UnitRoleId: String(id2),
                ProcessListName: data.ProcessListName

            };
            angular.extend(data, { ProcessListId: id });

            return $http({
                url: '/api/ProcessLists/' + id,
                data: aEditObj,
                method: "PUT",
                //headers: authHeaders
            }).success(function (data) {

                $scope.GetunitRoles();
                $scope.GetList();

                if (data == 0) {
                    $scope.message = "Process Name already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Successfully Process List Updated.";
                    $scope.messageType = "info";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }

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

        };



        $scope.cancel = function () {
            $scope.ProcessLists = '';
            $scope.ProcessListName = '';
            $scope.addButton = false;
            $scope.ProcessList = {};
            $scope.submitted = false;
            $scope.ProcessListForm.$setPristine();
            $scope.ProcessListForm.$setUntouched();
            angular.element('#ProcessList').focus();
        };
    }])