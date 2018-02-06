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


        $scope.unitRoles = [];
        $http({
            url: "/api/UnitRoles",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.unitRoles= data;
            //console.log(data);
            });
        $scope.ProcessLists = [];
        $http({
            url: "/api/ProcessLists/GetProcessList",
            method: "GET", 
            headers: authHeaders
        }).success(function (data) {
            $scope.ProcessLists = data;
            //console.log(data);
        });
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
                    $scope.ProcessLists.push(aViewObj);
                    
                    alert("Added");
                   
                    $scope.submitted = false;
                    $scope.ProcessListForm.$setPristine();
                    $scope.ProcessListForm.$setUntouched();
                    $scope.loading = true;
                    $scope.message = "Successfully Process List Created.";
                   
                    $scope.messageType = "success";
                    $route.reload();
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    angular.element('#ProcessList').focus();

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

            userPrompt = confirm('Are you sure you want to delete the Process List?');
            if (userPrompt) {
                $http({
                    url: "/api/ProcessLists/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.ProcessLists.splice(index, 1);
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting Process List! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }

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
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Process List Updated.";
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

        };

     
        $scope.cancel = function () {
            $scope.addButton = false;
            $scope.ProcessList = {};
            $scope.submitted = false;
            $scope.ProcessListForm.$setPristine();
            $scope.ProcessListForm.$setUntouched();
            angular.element('#ProcessList').focus();
        };
    }])