var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('ProductController', ['$scope', '$location', '$http', '$timeout', '$filter', 'upload',
    function ($scope, $location, $http, $timeout, $filter, upload) {
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

        $scope.product = {
            MultiplyWith: 1,
            Discount:0
        };
        $scope.sort = function (keyname) {
            $scope.sortKey = keyname;   //set the sortKey to the param passed
            $scope.reverse = !$scope.reverse; //if true make it false and vice versa
        }

    $scope.subCategories = [];
    $http({
        url: "/api/SubCategory/GetDropDownList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.subCategories = data;
        //console.log(data);
    });
    $scope.submitProductForm = function () {
            // Set the 'submitted' flag to true
            $scope.submitted = true;
            if ($scope.productForm.$valid) {
                
                var aCountryObj = {
                    SubCategoryId: $scope.product.SubCategoryId.SubCategoryId,
                    ProductName: $scope.product.ProductName,
                    MultiplyWith: $scope.product.MultiplyWith,
                    Rate: $scope.product.Rate,
                    Discount: $scope.product.Discount,
                    ShowRoomId: 0,
                    ProductNameBangla: $scope.product.ProductNameBangla,
                    Image: ""
                };

                $http({
                    url: "/api/Product",
                    data: aCountryObj,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    var file = $scope.myFile;
                    var fileName = "";
                    if (file) {
                        fileName = data.ProductId + '.jpg';
                    }
                    if (fileName) {
                        upload({
                            url: 'Home/UploadProductImage',
                            method: 'POST',
                            data: {
                                aFile: $scope.myFile,
                                ProductId: data.ProductId
                            }
                        })
                        .then(
                        function (response) {
                            console.log(response.data);
                        },
                        function (response) {
                            console.error(response);
                        });
                        //Update Image Name
                        $http({
                            url: '/api/Product/UpdateImageNameToProducts/' + data.ProductId,
                            method: "PUT",
                            headers: authHeaders
                        }).success(function (data) {
                            $('.image-preview').attr("data-content", "").popover('hide');
                            $('.image-preview-filename').val("");
                            $('.image-preview-clear').hide();
                            $('.image-preview-input input:file').val("");
                            $(".image-preview-input-title").text("Browse");
                        }).error(function (error) {

                        });
                        //End Update Image Name
                    }
                    var aViewObj = {
                        id: data.ProductId,
                        name: $scope.product.ProductName,
                        group: $scope.product.SubCategoryId.SubCategoryId,
                        groupName: $scope.product.SubCategoryId.SubCategoryName,
                        MultiplyWith: $scope.product.MultiplyWith,
                        Rate: $scope.product.Rate,
                        Discount: $scope.product.Discount,
                        ProductNameBangla: $scope.product.ProductNameBangla
                    };
                    $scope.productList.push(aViewObj);
                    $scope.product = {};
                    $scope.submitted = false;
                    $scope.productForm.$setPristine();
                    $scope.productForm.$setUntouched();
                    angular.element('#ProductName').focus();
                    $scope.loading = true;
                    $scope.message = "Successfully Product Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (error) {
                    $scope.message = 'Unable to save Product' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });

            }
            else {
                alert("Please  correct form errors!");
            }
        };

        // remove product
        $scope.remove = function (index, ProductId) {

            deleteDepartment = confirm('Are you sure you want to delete the product?');
            if (deleteDepartment) {
                //Your action will goes here
                $http.delete('/api/Product/' + ProductId)
                    .success(function (data) {
                        $scope.productList.splice(index, 1);
                    })
                    .error(function (data) {
                        $scope.error = "An error has occured while deleting! " + data;
                    });
                //alert("Deleted successfully!!");
            }
        };


        $scope.productList = [];
        $http({
            url: "/api/Product/GetProductsList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.productList = data;
            //console.log(data);
        });

        $scope.productGroups = [];
        $scope.loadGroups = function () {
            return $scope.productGroups.length ? null : $http({
                url: "/api/SubCategory/GetDropDownListXedit",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.productGroups = data;
                //console.log(data);
            });
            
        };

        $scope.showGroup = function (user) {
            if (user.group && $scope.productGroups.length) {
                var selected = $filter('filter')($scope.productGroups, { id: user.group });
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
            var file = $scope.myFile;
            var fileName = "";
            if (file) {
                fileName = id + '.jpg';
            }
            var aEditObj = {
                ProductId: id,
                SubCategoryId: data.group,
                ProductName: data.name,
                MultiplyWith: data.MultiplyWith,
                Rate: data.Rate,
                Discount: data.Discount,
                ShowRoomId: 0,
                ProductNameBangla: data.ProductNameBangla,
                Image: fileName
            };

            angular.extend(data, { ProductId: id });
            return $http({
                url: '/api/Product/' + id,
                data: aEditObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {

                if (fileName) {
                    upload({
                        url: 'Home/UploadProductImage',
                        method: 'POST',
                        data: {
                            aFile: $scope.myFile,
                            ProductId: id
                        }
                    })
                    .then(
                    function (response) {
                        console.log(response.data);
                    },
                    function (response) {
                        console.error(response);
                    });
                }
                $('.image-preview').attr("data-content", "").popover('hide');
                $('.image-preview-filename').val("");
                $('.image-preview-clear').hide();
                $('.image-preview-input input:file').val("");
                $(".image-preview-input-title").text("Browse");

                $scope.message = "Successfully Product Updated.";
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
                    $scope.validationErrors.push('Unable to Update Product.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        };
        $scope.cancel = function () {
            $scope.addButton = false;
            $scope.product = {};
            $scope.submitted = false;
            $scope.productForm.$setPristine();
            $scope.productForm.$setUntouched();
            $scope.product = {
                MultiplyWith: 1,
                Discount: 0
            };
            angular.element('#ProductName').focus();
        };
        $scope.doUpload = function () {

            upload({
                url: 'Home/upload',
                method: 'POST',
                data: {
                    aFile: $scope.myFile
                }
            }).then(
                function (response) {
                    console.log(response.data);
                },
                function (response) {
                    console.error(response);
                }
                );

        };

        //$scope.pressHotKey = function () {
        //    //alert(e);
        //    shortcut.add("Ctrl+F12", function () {
        //        alert("Hi there!");
        //    });
        //};
}])