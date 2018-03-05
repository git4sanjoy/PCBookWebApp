
var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('ProductTypesController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.Message = "Product Types Controller Angular JS";

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

        //***For Hiding Update Button in page load***
        $('.updateButton').hide();

        $scope.List = [];
        //**Get Product Type List**
        $scope.GetProductTypeList = function () {
            $http({
                url: "/api/ProductTypes/GetProductTypeList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data;
            }).error(function (data) {
                $scope.message = "Product Type list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetProductTypeList();

        //**Save ProductType**
        $scope.Save = function (productType) {
            $http({
                url: '/api/ProductTypes/PostProductType',
                data: productType,
                method: "POST",
                headers: authHeaders
                //traditional: true,
                //url: '/api/ProductTypes/PostProductType',
                //method: 'POST',
                //data: JSON.stringify(productType),
                //contentType: "application/json",
                //dataType: "json"
            }).success(function (data) {               
                $scope.Cancel();
                $scope.GetProductTypeList();
                if (data == 0) {
                    $scope.message = "This Product Type already Exist.";                   
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Product Type data saved successfully.";
                    //$scope.message = "Product Type data saved successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                //toastr.success("Supplier data saved successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Product Type data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });

        };

        //**InLine Update ProductType**
        $scope.Update = function (data, productTypeId) {

            var obj = {
                ProductTypeId: productTypeId,               
                ProductTypeName: data.ProductTypeName
            };
            var id = productTypeId;
            $http({
                url: '/api/ProductTypes/' + id,
                data: obj,
                method: "PUT",
                headers: authHeaders
                //traditional: true,
                //url: '/api/Suppliers/' + id,
                //method: 'PUT',
                //data: JSON.stringify(supplier),
                //contentType: "application/json",
                //dataType: "json"
            }).success(function (data) {               
                $scope.Cancel();
                $scope.GetProductTypeList();
                if (data == 0) {
                    $scope.message = "This Product Type already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Product Type data updated successfully.";
                    //$scope.message = "Product Type data saved successfully.";
                    $scope.messageType = "info";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                };
                //toastr.success("Data updated successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Data could not be updated!", "Error");
            });

        };

        //**Update ProductType**
        $scope.UpdateInput = function (productType) {

            var id = productType.ProductTypeId;
            $http({
                url: '/api/ProductTypes/' + id,
                data: productType,
                method: "PUT",
                headers: authHeaders
                //traditional: true,
                //url: '/api/Suppliers/' + id,
                //method: 'PUT',
                //data: JSON.stringify(supplier),
                //contentType: "application/json",
                //dataType: "json"
            }).success(function (data) {
                $scope.entryForm.$setUntouched();
                $scope.Cancel();
                $scope.GetProductTypeList();
                if (data == 0) {
                    $scope.message = "This Product Type already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Product Type data updated successfully.";
                    //$scope.message = "Product Type data saved successfully.";
                    $scope.messageType = "info";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                };
                //toastr.success("Data updated successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Data could not be updated!", "Error");
            });

        };

        //**Delete ProductType**
        $scope.Delete = function (item) {

            var id = item.ProductTypeId;
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/ProductTypes/" + id,
                    method: "DELETE",
                    headers: authHeaders
                    //traditional: true,
                    //url: "/api/Suppliers/DeleteSupplier",
                    //method: 'POST',
                    //data: JSON.stringify({ id: item.SupplierId }),
                    //contentType: "application/json",
                    //dataType: "json"
                }).success(function (data) {
                    $scope.Cancel();
                    $scope.GetProductTypeList();
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.success("Data deleted successfully.", "Success");                    
                }).error(function (data) {
                    $scope.message = "Data could not be deleted!";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.error("Data could not be deleted!", "Failed!");
                });
            };

        };

        //**Edit Button**
        $scope.Edit = function (item) {
            $scope.productType = angular.copy(item);
            $('.updateButton').show();
            $('.saveButton').hide();
        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.productType = '';
            $('.updateButton').hide();
            $('.saveButton').show();
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
        };

        //** Start Check Duplicate No **//
        $scope.ChkDuplicateNo = function () {
            var name = $scope.productType.ProductTypeName;
            alert(name);
            $http({
                method: "post",
                url: '/api/ProductTypes/ChkDuplicateNo',
                data: name,
                dataType: "json",
                contentType: "application/json",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = data;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "ERROR!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }   
         //** End Check Duplicate No **//
    }])
