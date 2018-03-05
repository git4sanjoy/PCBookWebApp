var app = angular.module('PCBookWebApp');
app.controller('DistrictController', ['$scope', '$location', '$http', '$timeout', '$filter','$q',
    function ($scope, $location, $http, $timeout, $filter, $q) {
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

    $scope.saleZoneList = [];
    $http({
        url: "/api/SaleZones/SalesZoneDeropDownList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.saleZoneList = data;
        //console.log(data);
    });

    //$scope.districtList = [];
    //$http({
    //    url: "/api/District",
    //    method: "GET",
    //    headers: authHeaders
    //}).success(function (data) {
    //    $scope.districtList = data;
    //    //console.log(data);
    //});
    //$scope.checkName = function (data, DistrictId) {
    //    //alert(DistrictId);
    //};
    //$scope.submitDistrictForm = function () {
    //    $scope.submitted = true;
    //    if ($scope.districtForm.$valid) {
    //        var aUnitObj = {
    //            DistrictName: $scope.district.DistrictName,
    //            DistrictNameBangla: $scope.district.DistrictNameBangla,
    //            SaleZoneId: $scope.SaleZoneName.SaleZoneId
    //        };
    //        $http({
    //            url: "/api/District",
    //            data: aUnitObj,
    //            method: "POST",
    //            headers: authHeaders
    //        }).success(function (data) {
    //            var aViewObj = {
    //                DistrictId: data.DistrictId,
    //                DistrictName: $scope.district.DistrictName,
    //                DistrictNameBangla: $scope.district.DistrictNameBangla
    //            };
    //            $scope.districtList.push(aViewObj);
    //            alert("Added");
    //            $scope.district = {};
    //            $scope.submitted = false;
    //            $scope.districtForm.$setPristine();
    //            $scope.districtForm.$setUntouched();
    //            $scope.loading = true;
    //            angular.element('#DistrictName').focus();
    //            $scope.message = "Successfully Bank Created.";
    //            $scope.messageType = "success";
    //            $scope.clientMessage = false;
    //            $timeout(function () { $scope.clientMessage = true; }, 5000);
    //        }).error(function (error) {
    //            $scope.message = 'Unable to save Bank' + error.message;
    //            $scope.messageType = "warning";
    //            $scope.clientMessage = false;
    //            $timeout(function () { $scope.clientMessage = true; }, 5000);
    //        });

    //    }
    //    else {
    //        alert("Please  correct form errors!");
    //    }
    //};
    ////Update Unit
    //$scope.update = function (data, DistrictId) {

    //    angular.extend(data, { DistrictId: DistrictId });
    //    return $http({
    //        url: '/api/District/' + DistrictId,
    //        data: data,
    //        method: "PUT",
    //        headers: authHeaders
    //    }).success(function (data) {
    //        //alert('Updated');
    //        $scope.message = "Successfully Bank Updated.";
    //        $scope.messageType = "success";
    //        $scope.clientMessage = false;
    //        $timeout(function () { $scope.clientMessage = true; }, 5000);
    //    }).error(function (error) {
    //        $scope.validationErrors = [];
    //        if (error.ModelState && angular.isObject(error.ModelState)) {
    //            for (var key in error.ModelState) {
    //                $scope.validationErrors.push(error.ModelState[key][0]);
    //            }
    //        } else {
    //            $scope.validationErrors.push('Unable to Update Bank.');
    //        };
    //        $scope.messageType = "danger";
    //        $scope.serverMessage = false;
    //        $timeout(function () { $scope.serverMessage = true; }, 5000);
    //    });
    //};
    //// remove Unit
    //$scope.remove = function (index, DistrictId) {
    //    deleteDepartment = confirm('Are you sure you want to delete the unit?');
    //    if (deleteDepartment) {
    //        //Your action will goes here
    //        $http.delete('/api/District/' + DistrictId)
    //            .success(function (data) {
    //                $scope.districtList.splice(index, 1);
    //            })
    //            .error(function (data) {
    //                $scope.error = "An error has occured while deleting! " + data;
    //            });
    //        //alert("Deleted successfully!!");
    //    }
    //};
    ////Clear
    //$scope.clear = function () {
    //    $scope.district = {};
    //    $scope.submitted = false;
    //    $scope.districtForm.$setPristine();
    //    $scope.districtForm.$setUntouched();
    //    $scope.loading = true;
    //    angular.element('#DistrictName').focus();
    //};
    $scope.users = [];
    $http({
        url: "/api/District/DistrictListForColumnEdit",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.users = data;
        //console.log(data);
    });
    //$scope.users = [
    //    { id: 1, name: 'awesome user1', status: 2, groupName: null},
    //    { id: 2, name: 'awesome user2', status: undefined, groupName: 'vip' },
    //    { id: 3, name: 'awesome user3', status: 2, groupName: 'vip'}
    //];

    //$scope.statuses = [
    //    { value: 1, text: 'status1' },
    //    { value: 2, text: 'status2' },
    //    { value: 3, text: 'status3' },
    //    { value: 4, text: 'status4' }
    //];
    $scope.statuses = [];
    $http({
        url: "/api/District/ZoneListXEdit",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.statuses = data;
        //console.log(data);
    });


    $scope.showStatus = function (user) {
        var selected = [];
        if (user.status) {
            selected = $filter('filter')($scope.statuses, { value: user.status });
        }
        return selected.length ? selected[0].text : 'Not set';
    };

    //$scope.checkName = function (data) {
    //    if (data !== 'awesome') {
    //        return "Username should be `awesome`";
    //    }
    //};


    $scope.saveColumn = function (column) {
        var results = [];
        angular.forEach($scope.users, function (user) { 
            var districtObj = { column: column, value: user[column], id: user.id };
            //console.log(districtObj);
            results.push($http({ data: districtObj, url: '/api/District/DistrictUpdate/' + user.id + '/' + column + '/' + user[column], method: "POST", headers: authHeaders }));
        })
        return $q.all(results);
    };
}])

