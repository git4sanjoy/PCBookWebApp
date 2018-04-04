
var app = angular.module('PCBookWebApp');
app.controller('ConversionDetailController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.ConversionDetailObj = [];
        var accesstoken = sessionStorage.getItem('accessToken');
        $scope.message = "Conversion Controller";

        $scope.loading = true;

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 20;
        $scope.currentPage = 1;

        var authHeaders = {};

        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }       

        //**Get Purchase Product List**
        $scope.GetPurchaseProductList = function () {
            $http({
                url: "/api/ConversionDetails/GetPurchaseProductList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.PurchaseProductList = data.list;
                $scope.ConversionList = data.list2;
            }).error(function (data) {
                $scope.message = "Purchase Product list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetPurchaseProductList();

        //**Get Conversion Detail List**
        $scope.GetConversionDetailtList = function () {
            $http({
                url: "/api/ConversionDetails/",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.ConversionDetailObj = data;
            }).error(function (data) {
                $scope.message = "Conversion Detail list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetConversionDetailtList();

        //**Save Conversion Detail**
        $scope.Save = function (conversionDetail) {
            $http({
                url: '/api/ConversionDetails/',
                data: conversionDetail,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {                
                if (data == 0) {
                    $scope.message = "This Convertion Details already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Successfully saved.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);   
                }  
                $scope.Cancel();
                $scope.GetConversionDetailtList();
                //$scope.GetPurchasedProductList();
              
                           
            }).error(function (data) {
                $scope.message = "Conversion Detail data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });

        };

        //**Update Conversion Detail**
        $scope.updateData = function (data, id, id1, id2) {
            //alert(id);
            //alert(id1);
            //alert(id2);

            var obj = {
                ConversionDetailsId: id,
                ConversionId: String(id1),
                PurchaseProductId: String(id2),
                Quantity: data.Quantity

            };
            //alert(obj.PurchasedProductId);
            angular.extend(data, { ConversionDetailsId: id });
            return $http({
                url: '/api/ConversionDetails/' + id,
                data: obj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Updated.";
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
        }

        //**Delete Purchased Product**
        $scope.remove = function (item) {

            var id = item.ConversionDetailsId;
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: '/api/ConversionDetails/'  + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.success("Data deleted successfully.", "Success");
                    $scope.Cancel();
                    $scope.GetConversionDetailtList();
                    //$scope.GetPurchasedProductList();
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

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.conversionDetail = '';
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
            $('.updateButton').hide();
            $('.saveButton').show();
        };     

        
    }])