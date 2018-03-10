var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('ShowRoomUserController', ['$scope', '$location', '$http', '$timeout', '$filter', function ($scope, $location, $http, $timeout, $filter) {
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
    $scope.usersList = [];
    $http({
        url: "/api/ShowRoomUsers/GetUsersDropDownList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.usersList = data;
        //console.log(data);
    });
    $scope.officerList = [];
    $http({
        url: "/api/ShowRoomUsers/ShowRoomOfficerList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.officerList = data;
        //console.log(data);
    });


    $scope.submitShowroomUserForm = function () {
        // Set the 'submitted' flag to true
        $scope.submitted = true;
        if ($scope.showroomUserForm.$valid) {
            var aInsertObj = {
                Id: $scope.showRoomUser.userListCmb.Id,
                ShowRoomId: $scope.showRoomUser.showRoomCmb.ShowRoomId,
                UserName: $scope.showRoomUser.officerName,
                Address: $scope.showRoomUser.address,
                Phone: $scope.showRoomUser.phone,
                Email: $scope.showRoomUser.email
            };
            $http.post('/api/ShowRoomUsers', aInsertObj).success(function (data) {
                //var aViewObj = {
                //    ShowRoomUserId: data.ShowRoomUserId,
                //    ShowRoomName: $scope.showRoomUser.userListCmb.ShowRoomName,
                //    ShowRoomId: $scope.showRoomUser.userListCmb.ShowRoomId,
                //    UserName: $scope.showRoomUser.userListCmb.UserName,
                //    Id: $scope.showRoomUser.userListCmb.Id,
                //    officerName: $scope.showRoomUser.officerName,
                //    address: $scope.showRoomUser.address,
                //    phone: $scope.showRoomUser.phone,
                //    email: $scope.showRoomUser.email,
                //};
                //$scope.officerList.push(aViewObj);
                $http.get('/api/ShowRoomUsers/ShowRoomOfficerList').success(function (data) {
                    $scope.officerList = data;
                });
                alert("Added");
                $scope.showRoomUser = {};
                $scope.submitted = false;
                $scope.showroomUserForm.$setPristine();
                $scope.showroomUserForm.$setUntouched();
                $scope.loading = true;
                angular.element('#userListCmb').focus();
            }).error(function (error) {
                $scope.message = 'Unable to add Show Room User data: ' + error.message;
                console.log($scope.message);
            });
        }
        else {
            alert("Please  correct form errors!");
        }
    };

    $scope.delete = function (index, aObj) {

        deleteObj = confirm('Are you sure you want to delete the OU User?');
        if (deleteObj) {
            var Id = aObj.ShowRoomUserId;
            $http({
                url: "/api/ShowRoomUsers/" + Id,
                method: "DELETE",
                headers: authHeaders
            }).success(function (data) {
                $scope.officerList.splice(index, 1);
                $scope.message = 'OU User Deleted Successfull!';
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.loginAlertMessage = true; }, 3000);
                $scope.loading = false;
                alert('OU User Deleted Successfull!');
            }).error(function (data) {
                $scope.error = "An Error has occured while Deleteding OU User! " + data;
                $scope.loading = true;
            });
        }
    };


    $scope.clear = function () {
        $scope.addButton = false;
        $scope.updateButton = true;
        $scope.showRoomUser = {};
        $scope.submitted = false;
        $scope.showroomUserForm.$setPristine();
        $scope.showroomUserForm.$setUntouched();
        angular.element('#userListCmb').focus();
    };

}])