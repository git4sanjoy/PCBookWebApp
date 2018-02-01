var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('MainCategoryController', ['$scope', '$location', '$http', '$timeout', '$filter', function ($scope, $location, $http, $timeout, $filter) {

    $scope.clientMessage = true;
    $scope.serverMessage = true;
    $scope.messageType = "";
    $scope.message = "";

    var accesstoken = sessionStorage.getItem('accessToken');
    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }


    $scope.mainCategoryList = [];
    $http({
        url: "/api/MainCategory",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.mainCategoryList = data;
        //console.log(data);
    });

    $scope.checkName = function (data, MainCategoryId) {
        //alert(MainCategoryId);
    };
    $scope.submitMainCategoryForm = function () {
        $scope.submitted = true;
        if ($scope.mainCategoryForm.$valid) {
            var aUnitObj = {
                MainCategoryName: $scope.mainCategory.MainCategoryName
            };
            $http({
                url: "/api/MainCategory",
                data: aUnitObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                var aViewObj = {
                    MainCategoryId: data.MainCategoryId,
                    MainCategoryName:$scope.mainCategory.MainCategoryName
                };
                $scope.mainCategoryList.push(aViewObj);
                $scope.mainCategory = {};
                $scope.submitted = false;
                $scope.mainCategoryForm.$setPristine();
                $scope.mainCategoryForm.$setUntouched();
                $scope.loading = true;
                angular.element('#MainCategoryName').focus();
                $scope.message = "Successfully Main Category Created.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.message = 'Unable to save Main Category' + error.message;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }
        else {
            alert("Please  correct form errors!");
        }
    };
    //Update mainCategory
    $scope.update = function (data, MainCategoryId) {
        angular.extend(data, { MainCategoryId: MainCategoryId });
        return $http({
            url: '/api/MainCategory/' + MainCategoryId,
            data: data,
            method: "PUT",
            headers: authHeaders
        }).success(function (data) {
            //alert('Updated');
            $scope.message = "Successfully Main Category Updated.";
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
                $scope.validationErrors.push('Unable to Update Main Category.');
            };
            $scope.messageType = "danger";
            $scope.serverMessage = false;
            $timeout(function () { $scope.serverMessage = true; }, 5000);
        });

    };
    // remove mainCategory
    $scope.remove = function (index, MainCategoryId) {
        deleteDepartment = confirm('Are you sure you want to delete the Main Category?');
        if (deleteDepartment) {
            //Your action will goes here
            $http.delete('/api/MainCategory/' + MainCategoryId)
                .success(function (data) {
                    $scope.mainCategoryList.splice(index, 1);
                })
                .error(function (data) {
                    $scope.error = "An error has occured while deleting! " + data;
                });
            //alert("Deleted successfully!!");
        }
    };
    //Clear
    $scope.clear = function () {
        $scope.mainCategory = {};
        $scope.submitted = false;
        $scope.mainCategoryForm.$setPristine();
        $scope.mainCategoryForm.$setUntouched();
        $scope.loading = true;
        angular.element('#MainCategoryName').focus();
    };
}])