var app = angular.module('PCBookWebApp');

app.controller('RolesController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {

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

        $scope.showRoomList = [];
        $http({
            url: "/api/showRooms/GetAllShowRoom",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.showRoomList = data;
            //console.log(data);
        });
        $scope.roles = [];
        $http({
            url: "/api/Roles",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.roles = data;
            //console.log(data);
        });


        $scope.userList = [];
        $http({
            url: "/api/Roles/GetUsersDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.userList = data;
            //console.log(data);
        });


        $scope.users = [];
        $http({
            url: "/api/Roles/GetRolesList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            //console.log(data);
        });



        $scope.userRoles = [];
        $http({
            url: "/api/Roles/GetUserRoleList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.userRoles = data;
            //console.log(data);
        });


        $scope.submitRoleForm = function () {
            $scope.submitted = true;
            if ($scope.roleForm.$valid) {
                var aUserRoleObj = {
                    UserName: $scope.userListDdl.UserName,
                    RoleName: $scope.roleListDdl.Name
                };
                $http({
                    url: "/api/Roles/PostUserRole",
                    data: aUserRoleObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.userRoles = [];
                    $http({
                        url: "/api/Roles/GetUserRoleList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.userRoles = data;
                    });
                    $scope.message = "Successfully Role Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Role' + error.status;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });

            }
            else {
                //alert("Please  correct form errors!");
            }
        };
        $scope.saveUser = function (data, id) {
            //$scope.user not updated yet
            angular.extend(data, { id: id });

            var productObj = {
                Id: id,
                Name: data.name,
            };

            if (id == 0) {

                $http({
                    url: "/api/Roles/PostRole",
                    data: productObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users = [];
                    $http({
                        url: "/api/Roles/GetRolesList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.users = data;
                        //console.log(data);
                    });
                    $scope.message = "Successfully Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Role' + error.status;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });

            } else {
                var oParam = { "Name": "","Id": "" };
                oParam.Name = productObj.Name;
                oParam.Id = productObj.Id;
                //$http.put('/api/Roles/PutRole/' + id, oParam);
                
                $.ajax({
                    type: 'POST',
                    url: '../Role/EditRole',
                    data: JSON.stringify(oParam),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: false,
                    success: function () {
                        $http.get('/api/Roles/GetRolesList').success(function (data) {
                            $scope.users = data;
                        });
                        alert('Updated');
                    },
                    error: function () {
                        alert('Error');
                    }
                });
                
            }
        };


        $scope.remove = function (index, RoleName) {
            deleteProduct = confirm('Are you sure you want to delete the Role?');
            if (deleteProduct) {
                $http({
                    url: "/api/Roles/DeleteRole/" + RoleName,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users.splice(index, 1);
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //console.log(data);
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });

            }
        };

        $scope.removeUserRole = function (UserName, RoleName) {
            //console.log(UserName +"-"+ RoleName);
            deleteUserRole = confirm('Are you sure you want to delete the User Role?');
            if (deleteUserRole) {

                $http({
                    url: '/api/Roles/DeleteUserRole/'+UserName+'/'+RoleName,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.userRoles = [];
                    $http({
                        url: "/api/Roles/GetUserRoleList",
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        $scope.userRoles = data;
                    });
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //console.log(data);
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
                //id: $scope.users.length + 1,
                id: 0,
                name: ''
            };
            $scope.users.push($scope.inserted);
        };

    }]);
