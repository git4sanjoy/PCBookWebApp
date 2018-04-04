var app = angular.module('PCBookWebApp');

app.controller('GroupsController', ['$scope', '$location', '$http', '$timeout', '$filter',
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

        $scope.pageSize = 12;
        $scope.currentPage = 1;
        $scope.primaryListCmbDiv = false;

        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }
        $scope.group = {};

        $scope.groupList = [];
        $http({
            url: "/api/Groups/GetTypeHeadList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.groupList = data;
        });
        $scope.changeSelectGroup = function (item) {
            $scope.group.groupListCmb = item;
        };

        $scope.primaryAccList = [];
        $http({
            url: "/api/Primaries/GetPrimaryDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.primaryAccList = data;
        });

        $scope.groupsList = [];
        $http({
            url: "/api/Groups/GetGroupList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.groupsList = data;
            //$scope.class = "";
            //$scope.loading = false;
            //console.log(data);
        });


        $scope.checkUnderGroup = function () {
            var groupname = $scope.group.groupListCmb.GroupName;
            if (groupname == "Primary") {
                $scope.primaryListCmbDiv = true;
            } else {
                $scope.primaryListCmbDiv = false;
            }
        }

        $scope.submitGroupForm = function () {
            var groupName = null;
            var parentId = null;
            var primaryId = null;
            var primaryName = "";
            var underGroup = "";

            if ($scope.group.groupName) {
                groupName = $scope.group.groupName
            } else {
                alert("Please input group name");
                angular.element('#groupName').focus();
            }
            if ($scope.group.groupListCmb){
                parentId = $scope.group.groupListCmb.GroupIdStr
                underGroup = $scope.group.groupListCmb.GroupName
            } else {
                alert("Please select Under group");
                angular.element('#groupListCmb').focus();
            }
            if ($scope.group.primaryGroupCmb) {
                primaryId = $scope.group.primaryGroupCmb.PrimaryId;
                primaryName = $scope.group.primaryGroupCmb.PrimaryName;
            }

            var aGroup = {
                GroupName: groupName,
                ParentId: parentId,
                PrimaryId: primaryId,
                GroupIdStr: "",
                Active: false,
                ShowRoomId: 0
            };

            $http({
                headers: authHeaders,
                method: 'POST',
                url: '/api/Groups',
                data: aGroup
            }).success(function (data, status, headers, config) {
                pushAGroup = {
                    id: data.GroupId,
                    name: groupName,
                    UnderGroup: underGroup,
                    PrimaryName: primaryName
                };
                $scope.groupsList.push(pushAGroup);
                $scope.submitted = false;
                $scope.group = {};
                $scope.groupForm.$setPristine();
                $scope.groupForm.$setUntouched();     
                $scope.primaryListCmbDiv = false;
                $http({
                    url: "/api/Groups/GetTypeHeadList",
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.groupList = data;
                });

                $scope.message = "Successfully Group Saved.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                angular.element('#groupName').focus();
            }).error(function (error) {
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to Update group.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        }

        $scope.saveUser = function (data, id, ) {
            angular.extend(data, { id: id });

            var groupObj = {
                GroupId: id,
                GroupName: data.name,
                ShowRoomId:0
            };

            $http({
                url: '/api/Groups/' + id,
                data: groupObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Group Name Updated.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $http({
                    url: "/api/Groups/GetTypeHeadList",
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.groupList = data;
                });
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to Update Group Name.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });

        };
        $scope.sort = function (keyname) {
            $scope.sortKey = keyname;   //set the sortKey to the param passed
            $scope.reverse = !$scope.reverse; //if true make it false and vice versa
        }
        //$scope.checkName = function (data, id) {
        //    if (id === 1 && data !== 'Primary') {
        //        return "Non editable item `Primary`";
        //    }
        //};

}]);
