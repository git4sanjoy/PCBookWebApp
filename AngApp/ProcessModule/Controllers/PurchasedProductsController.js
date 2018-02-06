var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('PurchasedProductsController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.Message = "Purchased Products Controller Angular JS";

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

        //**Get Product Type List**
        $scope.GetProductTypeList = function () {
            $http({
                url: "/api/PurchasedProducts/GetProductTypeList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.ProductTypeList = data;
            }).error(function (data) {
                $scope.message = "Product Type list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetProductTypeList();

        //**Get Purchased Produc List**
        $scope.GetPurchasedProductList = function () {
            $http({
                url: "/api/PurchasedProducts/GetPurchasedProductList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data;
            }).error(function (data) {
                $scope.message = "Purchased Product list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetPurchasedProductList();

        //**Save Purchased Product**
        $scope.Save = function (purchasedProduct) {
            $http({
                url: '/api/PurchasedProducts/PostPurchasedProduct',
                data: purchasedProduct,
                method: "POST",
                headers: authHeaders
                //traditional: true,
                //url: '/api/PurchasedProducts/PostPurchasedProduct',
                //method: 'POST',
                //data: JSON.stringify(purchasedProduct),
                //contentType: "application/json",
                //dataType: "json"
            }).success(function (data) {
                $scope.Cancel();                
                $scope.GetProductTypeList();
                $scope.GetPurchasedProductList();               
                if (data == 0) {
                    $scope.message = "Product Name already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Purchased Product data saved successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }          
                //toastr.success("Supplier data saved successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Purchased Product data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });

        };

        //**InLine Update Purchased Product**
        $scope.Update = function (data, purchasedProductId, productTypeId) {
            
            var obj = {
                PurchasedProductId: purchasedProductId,
                ProductTypeId: String(productTypeId),
                PurchasedProductName: data.PurchasedProductName
            };
            //angular.extend(data, { PurchasedProductId: purchasedProductId });
            var id = purchasedProductId;
            alert(id);
            $http({
                url: '/api/PurchasedProducts/' + id,
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
                $scope.GetPurchasedProductList();
                if (data == 0) {
                    $scope.message = "Product Name already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Purchased Product data updated successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }          
                //toastr.success("Data updated successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Data could not be updated!", "Error");
            });

        };

        //**Update Purchased Product**
        $scope.UpdateInput = function (purchasedProduct) {    
            
            var id = purchasedProduct.PurchasedProductId;
            alert(id);
            $http({
                url: '/api/PurchasedProducts/' + id,
                data: purchasedProduct,
                method: "PUT",
                headers: authHeaders               
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetProductTypeList();
                $scope.GetPurchasedProductList();
                if (data == 0) {
                    $scope.message = "Product Name already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Purchased Product data saved successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }          
                //toastr.success("Data updated successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Data could not be updated!", "Error");
            });

        };        

        //**Delete Purchased Product**
        $scope.Delete = function (item) {

            var id = item.PurchasedProductId;
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/PurchasedProducts/" + id,
                    method: "DELETE",
                    headers: authHeaders
                    //traditional: true,
                    //url: "/api/Suppliers/DeleteSupplier",
                    //method: 'POST',
                    //data: JSON.stringify({ id: item.SupplierId }),
                    //contentType: "application/json",
                    //dataType: "json"
                }).success(function (data) {
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.success("Data deleted successfully.", "Success");
                    $scope.Cancel();
                    $scope.GetProductTypeList();
                    $scope.GetPurchasedProductList();
                }).error(function (data) {
                    alert('error occord')
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
            $scope.purchasedProduct = angular.copy(item);
            $('.updateButton').show();
            $('.saveButton').hide();
        };

        //**Cancel Button**
        $scope.Cancel = function () {           
            $scope.purchasedProduct = '';
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
            $('.updateButton').hide();
            $('.saveButton').show();            
        };        

    }])
