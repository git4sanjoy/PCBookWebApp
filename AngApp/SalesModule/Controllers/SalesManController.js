var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('SalesManController', ['$scope', '$location', '$http', '$timeout', '$filter', function ($scope, $location, $http, $timeout, $filter) {
    $scope.Message = "Sales Man Controller Angular JS";

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



    $scope.salesManList = [];
    $http({
        method: 'GET',
        url: '/api/SalesMen/ShowRoomSalesMenList',
        contentType: "application/json; charset=utf-8",
        headers: authHeaders,
        dataType: 'json'
    }).success(function (data) {
        $scope.salesManList = data;
    });

    $scope.checkName = function (data, SalesManId) {
        //alert(SalesManId);
    };
    $scope.submitSalesManForm = function () {
        $scope.submitted = true;
        if ($scope.salesManForm.$valid) {
            var aUnitObj = {
                SalesManName: $scope.salesMan.SalesManName,
                Address: $scope.salesMan.Address,
                Phone: $scope.salesMan.Phone,
                Email: $scope.salesMan.Email,
                ShowRoomId: 0
            };

            $http({
                url: "/api/SalesMen",
                data: aUnitObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                var aViewObj = {
                    SalesManId: data.SalesManId,
                    SalesManName: $scope.salesMan.SalesManName,
                    Address: $scope.salesMan.Address,
                    Phone: $scope.salesMan.Phone,
                    Email: $scope.salesMan.Email
                };
                $scope.salesManList.push(aViewObj);
                $scope.salesMan = {};
                $scope.submitted = false;
                $scope.salesManForm.$setPristine();
                $scope.salesManForm.$setUntouched();
                $scope.loading = true;
                angular.element('#SalesManName').focus();
                $scope.message = "Successfully Sales Officer Saved.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.message = 'Unable to save Sales Officer' + error.message;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }
        else {
            alert("Please  correct form errors!");
        }
    };
    //Update salesMan
    $scope.update = function (data, SalesManId, ShowRoomId) {
        angular.extend(data, { SalesManId: SalesManId, ShowRoomId: ShowRoomId});
        return $http({
            url: '/api/SalesMen/' + SalesManId,
            data: data,
            method: "PUT",
            headers: authHeaders
        }).success(function (data) {

            $scope.message = "Successfully Primary Updated.";
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
                $scope.validationErrors.push('Unable to Update Primary.');
            };
            $scope.messageType = "danger";
            $scope.serverMessage = false;
            $timeout(function () { $scope.serverMessage = true; }, 5000);
        });
    };
    
    // remove salesMan
    $scope.remove = function (index, SalesManId) {
        deleteDepartment = confirm('Are you sure you want to delete the Sales Man?');
        if (deleteDepartment) {
            //Your action will goes here
            $http.delete('/api/SalesMen/' + SalesManId)
                .success(function (data) {
                    $scope.salesManList.splice(index, 1);
                })
                .error(function (data) {
                    $scope.error = "An error has occured while deleting! " + data;
                });
            //alert("Deleted successfully!!");
        }
    };
    //Clear
    $scope.clear = function () {
        $scope.salesMan = {};
        $scope.submitted = false;
        $scope.salesManForm.$setPristine();
        $scope.salesManForm.$setUntouched();
        $scope.loading = true;
        angular.element('#SalesManName').focus();
    };
}])