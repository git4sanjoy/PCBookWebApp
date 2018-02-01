var app = angular.module('PCBookWebApp');
app.controller('UpazilaController', ['$scope', '$location', '$http', '$timeout', '$filter',
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

        // Any function returning a promise object can be used to load values asynchronously
        $scope.getLocation = function (val) {
            return $http.get('http://maps.googleapis.com/maps/api/geocode/json', {
                params: {
                    address: val,
                    sensor: false
                }
            }).then(function (response) {
                return response.data.results.map(function (item) {
                    return item.formatted_address;
                });
            });
        };

        $scope.districtList = [];
        $http({
            url: "/api/District/GetDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.districtList = data;
            //console.log(data);
        });
        $scope.submitUpazilaForm = function () {
            // Set the 'submitted' flag to true
            $scope.submitted = true;
            if ($scope.upazilaForm.$valid) {
                var aCountryObj = {
                    DistrictId: $scope.upazila.DistrictId.DistrictId,
                    UpazilaName: $scope.upazila.UpazilaName,
                    UpazilaNameBangla: $scope.upazila.UpazilaNameBangla
                };
               
                $http({
                    url: "/api/Upazila",
                    data: aCountryObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    var aViewObj = {
                        id: data.UpazilaId,
                        name: $scope.upazila.UpazilaName,
                        group: $scope.upazila.DistrictId.DistrictId,
                        groupName: $scope.upazila.DistrictId.DistrictName,
                        UpazilaNameBangla: $scope.upazila.UpazilaNameBangla
                    };
                    $scope.users.push(aViewObj);
                    //alert("Added");
                    $scope.upazila = {};
                    $scope.submitted = false;
                    $scope.upazilaForm.$setPristine();
                    $scope.upazilaForm.$setUntouched();
                    $scope.loading = true;
                    angular.element('#UpazilaName').focus();
                    $scope.message = "Successfully Bank Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Bank' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
            else {
                alert("Please  correct form errors!");
            }
        };

        // remove showRoom
        $scope.remove = function (index, UpazilaId) {

            deleteDepartment = confirm('Are you sure you want to delete the upazilla?');
            if (deleteDepartment) {
                //Your action will goes here
                $http.delete('/api/Upazila/' + UpazilaId)
                    .success(function (data) {
                        $scope.users.splice(index, 1);
                    })
                    .error(function (data) {
                        $scope.error = "An error has occured while deleting! " + data;
                    });
                //alert("Deleted successfully!!");
            }
        };


        $scope.users = [];
        $http({
            url: "/api/Upazila/GetDistrictList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.users = data;
            //console.log(data);
        });
        $scope.districts = [];
        $scope.loadGroups = function () {
            return $scope.districts.length ? null : $http({
                url: "/api/District/GetDropDownListXedit",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.districts = data;
            });
            
        };

        $scope.showGroup = function (user) {
            if (user.group && $scope.districts.length) {
                var selected = $filter('filter')($scope.districts, { id: user.group });
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


        $scope.update = function (data, id) {
            var aEditObj = {
                UpazilaId: id,
                DistrictId: data.group,
                UpazilaName: data.name,
                UpazilaNameBangla: data.UpazilaNameBangla
            };

            angular.extend(data, { UpazilaId: id });

            return $http({
                url: '/api/Upazila/' + id,
                data: aEditObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                //alert('Updated');
                $scope.message = "Successfully Upazilla Updated.";
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

        $scope.cancel = function () {
            $scope.addButton = false;
            $scope.upazila = {};
            $scope.submitted = false;
            $scope.upazilaForm.$setPristine();
            $scope.upazilaForm.$setUntouched();
            angular.element('#UpazilaName').focus();
        };
}])