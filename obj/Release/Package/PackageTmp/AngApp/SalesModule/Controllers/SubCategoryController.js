var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
var app = angular.module('PCBookWebApp');
app.controller('SubCategoryController', ['$scope', '$location', '$http', '$timeout', '$filter',
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


        $scope.mainCategoryList = [];
        $http({
            url: "/api/MainCategory/GetDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.mainCategoryList = data;
            //console.log(data);
        });

        $scope.submitSubCategoryForm = function () {
            // Set the 'submitted' flag to true
            $scope.submitted = true;
            if ($scope.subCategoryForm.$valid) {
                var aCountryObj = {
                    MainCategoryId: $scope.subCategory.MainCategoryId.MainCategoryId,
                    SubCategoryName: $scope.subCategory.SubCategoryName
                };

                $http({
                    url: "/api/SubCategory",
                    data: aCountryObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    var aViewObj = {
                        id: data.SubCategoryId,
                        name: $scope.subCategory.SubCategoryName,
                        group: $scope.subCategory.MainCategoryId.MainCategoryId,
                        groupName: $scope.subCategory.MainCategoryId.MainCategoryName
                    };
                    $scope.subCategories.push(aViewObj);
                    alert("Added");
                    $scope.subCategory = {};
                    $scope.submitted = false;
                    $scope.subCategoryForm.$setPristine();
                    $scope.subCategoryForm.$setUntouched();
                    $scope.loading = true;
                    angular.element('#SubCategoryName').focus();
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

        // remove subCategory
        $scope.remove = function (index, SubCategoryId) {

            deleteDepartment = confirm('Are you sure you want to delete the subCategory?');
            if (deleteDepartment) {
                //Your action will goes here
                $http.delete('/api/SubCategory/' + SubCategoryId)
                    .success(function (data) {
                        $scope.subCategories.splice(index, 1);
                    })
                    .error(function (data) {
                        $scope.error = "An error has occured while deleting! " + data;
                    });
                //alert("Deleted successfully!!");
            }
        };


        $scope.subCategories = [];
        $http({
            url: "/api/SubCategory/GetSubCategoryList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.subCategories = data;
            //console.log(data);
        });
        $scope.mcgroups = [];
        $scope.loadGroups = function () {
            return $scope.mcgroups.length ? null : $http({
                url: "/api/MainCategory/GetDropDownListXedit",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.mcgroups = data;
                //console.log(data);
            });
        };

        $scope.showGroup = function (user) {
            if (user.group && $scope.mcgroups.length) {
                var selected = $filter('filter')($scope.mcgroups, { id: user.group });
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
                SubCategoryId: id,
                MainCategoryId: data.group,
                SubCategoryName: data.name
            };

            angular.extend(data, { SubCategoryId: id });
            //return $http.put('/api/SubCategory/' + id, aEditObj);
            return $http({
                url: '/api/SubCategory/' + id,
                data: aEditObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Sub Category Updated.";
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
                    $scope.validationErrors.push('Unable to Update Sub Category.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        };
        $scope.cancel = function () {
            $scope.addButton = false;
            $scope.subCategory = {};
            $scope.submitted = false;
            $scope.subCategoryForm.$setPristine();
            $scope.subCategoryForm.$setUntouched();
            angular.element('#SubCategoryName').focus();
        };
}])