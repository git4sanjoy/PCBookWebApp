var app = angular.module('PCBookWebApp');
//Start Light Weight Controllers
app.controller('SuppliersController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.Message = "Supplier Controller Angular JS";

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

        //**Get Supplier List**
        $scope.GetSupplierList = function () {
            $http({
                url: "/api/Suppliers/GetSupplierList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data;
            }).error(function (data) {
                $scope.message = "Supplier list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetSupplierList();

        //***Save Supplier***
        $scope.Save = function (supplier) {
            $http({
                url: '/api/Suppliers/PostSupplier',
                data: supplier,
                method: "POST",
                headers: authHeaders
                //traditional: true,
                //url: '/api/Suppliers/PostSupplier',
                //method: 'POST',
                //data: JSON.stringify(supplier),
                //contentType: "application/json",
                //dataType: "json"
            }).success(function (data) {
                $scope.entryForm.$setUntouched();
                $scope.Cancel();
                $scope.GetSupplierList();
                if (data == 0) {
                    $scope.message = "Supplier Name already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Supplier data saved successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                //toastr.success("Supplier data saved successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Supplier data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });

        };

        //**Inline Update Supplier**
        $scope.Update = function (data, supplierId) {

            var obj = {
                SupplierId: supplierId,
                SupplierName: data.SupplierName,
                Address: data.Address,
                Email: data.Email,
                Phone: data.Phone
            };
            var id = supplierId;
            $http({
                url: '/api/Suppliers/' + id,
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
                $scope.entryForm.$setUntouched();
                $scope.Cancel();
                $scope.GetSupplierList();
                if (data == 0) {
                    $scope.message = "Supplier Name already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Supplier data updated successfully.";
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

        //****Update Supplier****
        $scope.UpdateInput = function (supplier) {

            var id = supplier.SupplierId;
            $http({
                url: '/api/Suppliers/' + id,
                data: supplier,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.entryForm.$setUntouched();
                $scope.Cancel();
                $scope.GetSupplierList();
                if (data == 0) {
                    $scope.message = "Supplier Name already Exist.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
                else {
                    $scope.message = "Supplier data updated successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Data could not be updated!", "Error");
            });

        };

        //**Delete Supplier**
        $scope.Delete = function (item) {

            var id = item.SupplierId;
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/Suppliers/" + id,
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
                    $scope.GetSupplierList();
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
            $scope.supplier = angular.copy(item);
            $('.updateButton').show();
            $('.saveButton').hide();
        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.supplier = '';
            $('.updateButton').hide();
            $('.saveButton').show();
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
        };

    }])
