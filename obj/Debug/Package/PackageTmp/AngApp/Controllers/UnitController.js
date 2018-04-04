var app = angular.module('PCBookWebApp');

app.controller('UnitsController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        
        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";
        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }
        $scope.users = [];
        $http({
            url: "/api/Units/GetUnitList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
        });


        $scope.groups = [];
        $scope.loadGroups = function () {
            return $scope.groups.length ? null : $http({
                url: "/api/Projects/GetDropDownListXedit",
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
            //$scope.user not updated yet
            angular.extend(data, { id: id });

            var productObj = {
                UnitId:id,
                ProjectId: data.group,
                UnitName: data.name,
                Address: data.Address,
                Email: data.Email,
                Phone: data.Phone,
                Website: data.Website
            };

            if (id == 0) {
                $http({
                    url: "/api/Units/PostUnit",
                    data: productObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $http({
                        url: "/api/Units/GetUnitList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.users = data;
                    });
                    $scope.message = "Successfully Unit Created.";
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
                        $scope.validationErrors.push('Unable to add Unit.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });

            } else {
                //$http.put('/api/Departments/' + id, productObj);
                //alert('Updated');
                $http({
                    url: '/api/Units/' + id,
                    data: productObj,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
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
                        $scope.validationErrors.push('Unable to Update project.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
        };


        $scope.remove = function (index, UnitId) {
            deleteProduct = confirm('Are you sure you want to delete the Unit?');
            if (deleteProduct) {
                $http({
                    url: "/api/Units/" + UnitId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users.splice(index, 1);
                    $scope.message = "Successfully Unit Deleted.";
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
        $scope.checkName = function (data, id) {
            //if (id === 2 && data !== 'awesome') {
            //    return "Username 2 should be `awesome`";
            //}
        };

        //// remove user
        //$scope.removeUser = function (index) {
        //    $scope.users.splice(index, 1);
        //};

        // add user
        $scope.addUser = function () {
            $scope.inserted = {
                //id: $scope.users.length + 1,
                id: 0,
                name: '',
                group: null
            };
            $scope.users.push($scope.inserted);
        };


}]);
