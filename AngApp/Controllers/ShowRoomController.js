var app = angular.module('PCBookWebApp');
app.controller('ShowRoomController', ['$scope', '$location', '$http', '$timeout', '$filter',
function ($scope, $location, $http, $timeout, $filter) {
        
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


        $scope.unitList = [];
        $http({
            url: "/api/Units/GetDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.unitList = data;
            //console.log(data);
        });

        $scope.users = [];
        $http({
            url: "/api/ShowRooms/GetImportProductsList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            //console.log(data);
        });

        $scope.submitShowRoomForm = function () {
            // Set the 'submitted' flag to true
            $scope.submitted = true;
            if ($scope.showRoomForm.$valid) {
                var aCountryObj = {
                    UnitId: $scope.showRoom.UnitId.UnitId,
                    ShowRoomName: $scope.showRoom.ShowRoomName,
                    ShowRoomNameBangla: $scope.showRoom.ShowRoomNameBangla
                };
                $http({
                    url: "/api/ShowRooms",
                    data: aCountryObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    var aViewObj = {
                        id: data.ShowRoomId,
                        name: $scope.showRoom.ShowRoomName,
                        group: $scope.showRoom.UnitId.UnitId,
                        groupName: $scope.showRoom.UnitId.UnitName,
                        ShowRoomNameBangla: $scope.showRoom.ShowRoomNameBangla
                    };
                    $scope.users.push(aViewObj);
                    //alert("Added");
                    $scope.showRoom = {};
                    $scope.submitted = false;
                    $scope.showRoomForm.$setPristine();
                    $scope.showRoomForm.$setUntouched();
                    $scope.loading = true;
                    $scope.message = "Successfully Show Room Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    angular.element('#ShowRoomName').focus();

                }).error(function (error) {
                    $scope.validationErrors = [];
                    if (error.ModelState && angular.isObject(error.ModelState)) {
                        for (var key in error.ModelState) {
                            $scope.validationErrors.push(error.ModelState[key][0]);
                        }
                    } else {
                        $scope.validationErrors.push('Unable to add Ledger.');
                    };
                    //$.each($scope.users, function (i) {
                    //    if ($scope.users[i].id === 0) {
                    //        $scope.users.splice(i, 1);
                    //        return false;
                    //    }
                    //});
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
            }
            else {
                alert("Please  correct form errors!");
            }
        };

        // remove showRoom
        $scope.remove = function (index, ShowRoomId) {

            userPrompt = confirm('Are you sure you want to delete the showRoom?');
            if (userPrompt) {
                $http({
                    url: "/api/ShowRooms/" + ShowRoomId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.users.splice(index, 1);
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting show room! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }

        };


        $scope.groups = [];
        $scope.loadGroups = function () {
            return $scope.groups.length ? null :
                $http({
                    url: "/api/Units/GetDropDownListXedit",
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


        //$scope.checkName = function (data, id) {
        //    if (id === 2 && data !== 'awesome') {
        //        return "Username 2 should be `awesome`";
        //    }
        //};


        $scope.saveUser = function (data, id) {
            var aEditObj = {
                ShowRoomId: id,
                UnitId: data.group,
                ShowRoomName: data.name
            };

            angular.extend(data, { ShowRoomId: id });
            //return $http.put('/api/ShowRooms/' + id, aEditObj);

            return $http({
                url: '/api/ShowRooms/' + id,
                data: aEditObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Show room Updated.";
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
            $scope.showRoom = {};
            $scope.submitted = false;
            $scope.showRoomForm.$setPristine();
            $scope.showRoomForm.$setUntouched();
            angular.element('#ShowRoomName').focus();
        };
}])