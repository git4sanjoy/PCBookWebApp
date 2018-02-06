//var app = angular.module('PCBookWebApp');
//app.controller('MUnitController', ['$scope', '$location', '$http', '$timeout', '$filter',
//    function ($scope, $location, $http, $timeout, $filter) {
//        $scope.message = "MUnit Controller";
//    }])
var app = angular.module('PCBookWebApp');
app.controller('MatricsController', ['$scope', '$location', '$http', '$timeout', '$filter', '$route',
    function ($scope, $location, $http, $timeout, $filter,$route) {

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




        //$scope.MUnitList = [];
        $http({
            url: "/api/Matrics",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.MUnitList = data;
            //console.log(data);
        });
        $scope.checkName = function (data, MatricId) {
            //alert(Unit);
        };
        $scope.submitUnitForm = function () {
            $scope.submitted = true;
            if ($scope.UnitForm.$valid) {
                var aUnitObj = {
                    MatricName: $scope.Unit.MatricName
                };
                $http({
                    url: "/api/Matrics",
                    data: aUnitObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    var aViewObj = {
                        MatricId: data.MatricId,
                        MatricName: $scope.Unit.MatricName
                    };
                    if (data.MatricId > 0) {
                        $scope.message = "Successfully Unit Name Created.";
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
                            url: "/api/Matrics",
                            method: "GET",
                            headers: authHeaders
                        }).success(function (data) {
                            $scope.MUnitList = data;
                            //console.log(data);
                        });
                    }
                    $scope.MUnitList.push(aViewObj);
                   
                    alert("Added");
                    $scope.Unit = {};
                    $scope.submitted = false;
                    $scope.UnitForm.$setPristine();
                    $scope.UnitForm.$setUntouched();
                    $scope.loading = true;
                   // $route.reload();
                    angular.element('#MatricName').focus();
                    //$scope.message = data;
                    //if (data.MatricId > 0) {
                    //    $scope.message = "Successfully Unit Name Created.";
                    //    $scope.messageType = "success";
                    //    $scope.clientMessage = false;
                    //}
                    //else {
                    //    $scope.message = "Duplicate Unit Available.";
                    //}
                   // $route.reload();
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Unit' + error.message;
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
        $scope.update = function (data, MatricId) {

            angular.extend(data, { MatricId: MatricId });
            return $http({
                url: '/api/Matrics/' + MatricId,
                data: data,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                //alert('Updated');
                $scope.message = "Successfully Unit Updated.";
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
                    $scope.validationErrors.push('Unable to Update Unit.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        };
        // remove Unit
        $scope.remove = function (index, MatricId) {
            deleteUnitList = confirm('Are you sure you want to delete the unit?');
            if (deleteUnitList) {
                //Your action will goes here
                $http.delete('/api/Matrics/' + MatricId)
                    .success(function (data) {
                        $scope.MUnitList.splice(index, 1);
                    })
                    .error(function (data) {
                        $scope.error = "An error has occured while deleting! " + data;
                    });
                //alert("Deleted successfully!!");
            }
        };
        //Clear
        $scope.clear = function () {
            $scope.Unit = {};
            $scope.submitted = false;
            $scope.UnitForm.$setPristine();
            $scope.UnitForm.$setUntouched();
            $scope.loading = true;
            angular.element('#MatricName').focus();
        };
    }])

