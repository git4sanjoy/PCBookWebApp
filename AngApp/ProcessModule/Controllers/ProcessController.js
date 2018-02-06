
var app = angular.module('PCBookWebApp');
app.controller('ProcessController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "PC App. V-1.0.1";

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

        //***For Process DatePicker***        
        $scope.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.opened = true;
        };
        $scope.processDatePickerIsOpen = false;
        $scope.ProcessDatePickerOpen = function () {
            this.processDatePickerIsOpen = true;
        };
        //***End DatePicker***

        //**Get All List**
        $scope.GetAllList = function () {
            $http({
                url: "/api/Processes/GetAllList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data.list;
                $scope.ProsessList = data.prosessList;
                $scope.PurchasedProductList = data.purchasedProductList;
                $scope.ProcesseLocationList = data.processeLocationList;
                $scope.ConversionList = data.conversionList;
                $scope.ShowRoomList = data.showRoomList;
            }).error(function (data) {
                $scope.message = "Process list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetAllList();

        //**Save Process**
        $scope.Save = function (process) {
            var date = $filter('date')($scope.process.ProcessDate, "yyyy-MM-dd");            
            process.ProcessDate = date;
            $http({
                url: '/api/Processes/PostProcess',
                data: process,
                method: "POST",
                headers: authHeaders                         
            }).success(function (data) {               
                $scope.message = "Process data saved successfully.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                $scope.GetAllList();
                $scope.Cancel(); 
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
                //toastr.success("Supplier data saved successfully.", "Success");
            }).error(function (data) {
                $scope.message = "Purchased Product data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);              
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });

        };

        //**Update Process**
        $scope.Update = function (process) {
            var date = $filter('date')($scope.process.ProcessDate, "yyyy-MM-dd");
            process.ProcessDate = date;
            var id = process.ProcessId;
           
            $http({
                url: '/api/Processes/' + id,
                data: process,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetAllList();
                $scope.message = "Process data updated successfully.";
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

            var id = item.ProcessId;
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/Processes/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.success("Data deleted successfully.", "Success");
                    $scope.Cancel();
                    $scope.GetAllList();
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
            $scope.process = angular.copy(item);
            $('.updateButton').show();
            $('.saveButton').hide();
        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.process = '';
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
            $('.updateButton').hide();
            $('.saveButton').show();
        };

    }])
