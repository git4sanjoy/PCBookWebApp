var app = angular.module('PCBookWebApp');
app.controller('PurchaseController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "Purchase Controller";
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

        $('.updateButton').hide();


        //***For DatePicker***   
        $scope.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.opened = true;
        };

        $scope.purchaseDatePickerOpen = false;

        $scope.PurchaseDatePickerOpen = function () {
            this.purchaseDatePickerOpen = true;
        };
        //***End DatePicker***

        //**Get Purchases List**
        $scope.GetPurchasesList = function () {
            $http({
                url: "api/Purchases/GetPurchasesList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data.list;
                //$scope.ProsessList = data.prosessList;
                $scope.PurchasedProductList = data.purchasedProductList;
                $scope.SupplierList = data.supplierList;

                $scope.ShowRoomList = data.showRoomList;
            }).error(function (data) {
                $scope.message = "purchase  list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetPurchasesList();



        //**Save Purchased Product**
        $scope.Save = function (purchase) {
            var date = $filter('date')($scope.purchase.PurchaseDate, "yyyy-MM-dd");
            purchase.PurchaseDate = date;
            console.log(purchase);
            $http({
                traditional: true,
                url: '/api/Purchases/purchase',
                method: 'POST',
                data: JSON.stringify(purchase),
                contentType: "application/json",
                dataType: "json"
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetPurchasesList();
                $scope.message = "purchase data saved successfully.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "purchase  data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });

        };
        $scope.Update = function (purchase) {
            var date = $filter('date')($scope.purchase.PurchaseDate, "yyyy-MM-dd");
            purchase.PurchaseDate = date;

            var id = purchase.PurchaseId;

            $http({
                url: '/api/Purchases/' + id,
                data: purchase,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetPurchasesList();
                $scope.message = "Purchases data updated successfully.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //if (data == 0) {
                //    $scope.message = "Product Name already Exist.";
                //    $scope.messageType = "warning";
                //    $scope.clientMessage = false;
                //    $timeout(function () { $scope.clientMessage = true; }, 5000);
                //}
                //else {
                //    $scope.message = "Purchased Product data saved successfully.";
                //    $scope.messageType = "success";
                //    $scope.clientMessage = false;
                //    $timeout(function () { $scope.clientMessage = true; }, 5000);
                //}
                //toastr.success("Data updated successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Data could not be updated!", "Error");
            });

        };

        //**Delete Process**r
        $scope.Delete = function (item) {

            var id = item.PurchaseId;
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/Purchases/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.success("Data deleted successfully.", "Success");
                    $scope.Cancel();
                    $scope.GetPurchasesList();
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
            $scope.purchase = angular.copy(item);
            $('.updateButton').show();
            $('.saveButton').hide();
        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.purchase = '';
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
            $('.updateButton').hide();
            $('.saveButton').show();
        };

    }])
