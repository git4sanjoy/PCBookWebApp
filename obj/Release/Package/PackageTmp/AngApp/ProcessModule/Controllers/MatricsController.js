
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


        $scope.GetList = function() {
            $http({
                url: "/api/Matrics/GetMatricsList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.MUnitList = data;
                //console.log(data);
            });
        }
        $scope.GetList();

        
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
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
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
                   
               
                    $scope.Unit = {};
                    $scope.submitted = false;
                    $scope.UnitForm.$setPristine();
                    $scope.UnitForm.$setUntouched();
                    $scope.loading = true;
                    $scope.clear();
                   // $route.reload();
                    angular.element('#MatricName').focus();
                    
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
                $scope.clear();
                //alert('Updated');
                $scope.message = "Successfully Unit Updated.";
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
                    $scope.validationErrors.push('Unable to Update Unit.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        };
       

        $scope.remove = function (index, MatricId) {
       
        var msg = confirm("Do you want to delete this data?");
        if (msg == true) {
            $http({
                url: "/api/Matrics/" + MatricId ,
                method: "DELETE",
                headers: authHeaders
               
            }).success(function (data) {
             
                $scope.message = "Data deleted successfully.";
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
               
                $scope.GetList();
                $scope.clear();
              
            }).error(function (data) {
                alert('error occord')
                $scope.message = "Data could not be deleted!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                
            });
        };

    };
        //Clear
        $scope.clear = function () {
           
            //$scope.Matrics = "";
            //$scope.Unit.MatricName = "";
            $scope.submitted = false;
            $scope.Unit = {};
            $scope.addButton = false;
            $scope.UnitForm.$setPristine();
            $scope.UnitForm.$setUntouched();
            $scope.loading = true;
            angular.element('#MatricName').focus();
        };
        
    }])

