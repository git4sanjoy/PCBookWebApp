//var app = angular.module('PCBookWebApp');
//app.controller('UnitRoleController', ['$scope', '$location', '$http', '$timeout', '$filter',
//    function ($scope, $location, $http, $timeout, $filter) {
//        $scope.message = "Unit Role Controller";
//    }])
var app = angular.module('PCBookWebApp');
app.controller('UnitRoleController', ['$scope', '$location', '$http', '$timeout', '$filter', function ($scope, $location, $http, $timeout, $filter) {
    //$scope.Message = "WELCOME TO Unt Controller Angular JS";


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


    

    $scope.UnitRoleList = [];
    $http({
        url: "/api/UnitRoles",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.UnitRoleList = data;
        //console.log(data);
    });
    $scope.checkName = function (data, UnitRoleId) {
        //alert(UnitRoleId);
    };
    $scope.submitUnitRoleForm = function () {
        $scope.submitted = true;
        if ($scope.UnitRoleForm.$valid) {
            var aUnitObj = {
                UnitRoleName: $scope.UnitRole.UnitRoleName,
                ShowRoomId: 0
            };
            $http({
                url: "/api/UnitRoles",
                data: aUnitObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                var aViewObj = {
                    UnitRoleId: data.UnitRoleId,
                    UnitRoleName: $scope.UnitRole.UnitRoleName
                };
                if (data.UnitRoleId > 0) {
                    $scope.message = "Successfully Unit Role Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                }
                else {
                    //$route.reload();

                    $scope.message = "Duplicate Entry...Same value exists in another field.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    $http({
                        url: "/api/UnitRoles",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.UnitRoleList = data;
                        //console.log(data);
                    });
                }
                $scope.UnitRoleList.push(aViewObj);
                alert("Added");
                $scope.UnitRole = {};
                $scope.submitted = false;
                $scope.UnitRoleForm.$setPristine();
                $scope.UnitRoleForm.$setUntouched();
                $scope.loading = true;
                angular.element('#UnitRoleName').focus();
                //$scope.message = "Successfully UnitRole Name Created.";
                //$scope.messageType = "success";
                //$scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.message = 'Unable to save UnitRole' + error.message;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });

        }
        else {
            alert("Please  correct form errors!");
        }
    };
    //Update Unit
    $scope.update = function (data, UnitRoleId) {

        angular.extend(data, { UnitRoleId: UnitRoleId });
        return $http({
            url: '/api/UnitRoles/' + UnitRoleId,
            data: data,
            method: "PUT",
            headers: authHeaders
        }).success(function (data) {
            //alert('Updated');
            $scope.message = "Successfully UnitRole Updated.";
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
                $scope.validationErrors.push('Unable to Update Bank.');
            };
            $scope.messageType = "danger";
            $scope.serverMessage = false;
            $timeout(function () { $scope.serverMessage = true; }, 5000);
        });
    };
    // remove Unit
    $scope.remove = function (index, UnitRoleId) {
        deleteUnitRoleList = confirm('Are you sure you want to delete the unit?');
        if (deleteUnitRoleList) {
            //Your action will goes here
            $http.delete('/api/UnitRoles/' + UnitRoleId)
                .success(function (data) {
                    $scope.UnitRoleList.splice(index, 1);
                })
                .error(function (data) {
                    $scope.error = "An error has occured while deleting! " + data;
                });
            //alert("Deleted successfully!!");
        }
    };
    //Clear
    $scope.clear = function () {
        $scope.UnitRole = {};
        $scope.submitted = false;
        $scope.UnitRoleForm.$setPristine();
        $scope.UnitRoleForm.$setUntouched();
        $scope.loading = true;
        angular.element('#UnitRoleName').focus();
    };
}])

