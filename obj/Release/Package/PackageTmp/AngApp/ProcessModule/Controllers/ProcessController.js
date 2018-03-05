
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
        $scope.data = {
            cb1: false
        };


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
        $scope.fromDatePickerIsOpen = false;
        $scope.FromDatePickerOpen = function () {
            this.fromDatePickerIsOpen = true;
            $scope.toDatePickerIsOpenPickerIsOpen = false;
        };
        $scope.toDatePickerIsOpen = false;
        $scope.ToDatePickerOpen = function () {
            this.toDatePickerIsOpen = true;
            $scope.fromDatePickerIsOpen = false;
        };
        $scope.processSearch = {};
        $scope.processSearch.FromDate = new Date();
        $scope.processSearch.ToDate = new Date();
        //***End DatePicker***

        //**Change Select Product Name**
        $scope.changeSelectProductName = function (item) {
            $scope.process.PurchasedProductId = item;
        };

        //**Get All List**
        $scope.GetAllList = function () {
            $http({
                url: "/api/Processes/GetAllList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data.list;
                $scope.ProsessList = data.prosessList;
                $scope.prosessListAll = data.prosessListAll;
                //$scope.PurchasedProductList = data.purchasedProductList;
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
        $scope.Save = function (process, newID) {

            if (newID != null && process.ProcessListId == newID.ProcessListId) {

                alert("Current Process and Next Process can not be Same!!!");

            } else {

                var date = $filter('date')($scope.process.ProcessDate, "yyyy-MM-dd");

                //console.log(process);
                //return false;

                var obj1 = {
                    PurchasedProductId: $scope.process.PurchasedProductId.PurchasedProductId,
                    ConversionId: process.ConversionId,
                    ProcessDate: date,
                    ProcesseLocationId: process.ProcesseLocationId,
                    ProcessListId: process.ProcessListId,
                    LotNo: process.LotNo,
                    DeliveryQuantity: process.DeliveryQuantity,
                    ReceiveQuantity: 0,
                    SE: 0,
                    Rate: 0,
                    Amount: 0,
                    Discount: 0
                };               
                var obj2 = {
                    PurchasedProductId: $scope.process.PurchasedProductId.PurchasedProductId,
                    ConversionId: process.ConversionId,
                    ProcessDate: date,
                    ProcesseLocationId: process.ProcesseLocationId,
                    ProcessListId: newID.ProcessListId,
                    LotNo: process.LotNo,
                    ReceiveQuantity: process.DeliveryQuantity,
                    DeliveryQuantity: 0,
                    SE: 0,
                    Rate: 0,
                    Amount: 0,
                    Discount: 0
                };

                if (newID.ProcessListName == 'Short/Excess') {
                    process.ProcessDate = date;
                    process.PurchasedProductId = process.PurchasedProductId.PurchasedProductId;
                    process.LotNo = process.LotNo;
                    process.SE = process.DeliveryQuantity;
                    process.DeliveryQuantity = 0;
                    process.ReceiveQuantity = 0;
                    process.Rate = 0;
                    process.Amount = 0;
                    process.Discount = 0;
                };
                if (newID.ProcessListName == 'Finished') {
                    process.ProcessDate = date;
                    process.PurchasedProductId = process.PurchasedProductId.PurchasedProductId;
                    process.LotNo = process.LotNo;
                    process.DeliveryQuantity = process.DeliveryQuantity;
                    process.SE = 0;
                    process.ReceiveQuantity = 0;
                    process.Rate = 0;
                    process.Amount = 0;
                    process.Discount = 0;
                };

                if (newID.ProcessListName == 'Short/Excess' || newID.ProcessListName == 'Finished') {
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
                    }).error(function (data) {
                        $scope.message = "Process data saving attempt failed!";
                        $scope.messageType = "warning";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                        //toastr.error("Supplier data saving attempt failed!", "Error");
                    });
                }
                else {
                    $http({
                        url: '/api/Processes/PostProcess',
                        data: obj1,
                        method: "POST",
                        headers: authHeaders
                    }).success(function (data) {

                        $http({
                            url: '/api/Processes/PostProcess',
                            data: obj2,
                            method: "POST",
                            headers: authHeaders
                        }).success(function (data) {
                        }).error(function (data) {
                        });

                        $scope.message = "Process data saved successfully.";
                        $scope.messageType = "success";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                        $scope.GetAllList();
                        $scope.Cancel();
                    }).error(function (data) {
                        $scope.message = "Process data saving attempt failed!";
                        $scope.messageType = "warning";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                        //toastr.error("Supplier data saving attempt failed!", "Error");
                    });
                }
            }


        };

        //**Update Process**
        $scope.Update = function (process) {
            var date = $filter('date')($scope.process.ProcessDate, "yyyy-MM-dd");
            process.ProcessDate = date;

            var processObj = {
                ProcessId: process.ProcessId,
                PurchasedProductId: $scope.process.PurchasedProductId.PurchasedProductId,
                ConversionId: process.ConversionId,
                ProcessDate: date,
                ProcesseLocationId: process.ProcesseLocationId,
                ProcessListId: process.ProcessListId,
                LotNo: process.LotNo,
                DeliveryQuantity: process.DeliveryQuantity,
                ReceiveQuantity: process.ReceiveQuantity,
                SE: process.SE,
                Rate: 0,
                Amount: 0,
                Discount: 0
            };

            $http({
                url: '/api/Processes/' + process.ProcessId,
                data: processObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetAllList();
                $scope.message = "Process data updated successfully.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
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
                    $scope.messageType = "danger";
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
            //$scope.process.PurchasedProductId = angular.copy(item.PurchasedProductName);
            $scope.process.PurchasedProductId = { PurchasedProductName: item.PurchasedProductName, PurchasedProductId: item.PurchasedProductId};
            $scope.editMode = true;
        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.process = '';
            $scope.newID = '';
            $scope.ProcessList = '';
            $scope.ProcessListForEdit = '';
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
            $scope.editMode = false;
            $scope.data = {
                cb1: false
            };
        };

        //**Search Form Clear Button**
        $scope.searchFormClear = function () {
            $scope.processSearch = '';
            $scope.searchForm.$setPristine();
            $scope.searchForm.$setUntouched();
        };

        $scope.changeSelectFactoryName = function (item) {
            $scope.process.LotNo = item;
        };

        //**Clear Field**
        $scope.ClearFild = function () {
            $scope.process.ProcessListId = '';
            $scope.process.LotNo = '';
            $scope.process.DeliveryQuantity = '';
            $scope.process.PurchasedProductId = '';
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
        };

        //**Get Lot No Type Head List**
        $scope.GetLotNoTypeHeadList = function (processeLocationId, processListId) {
            if (processListId != null && processeLocationId != null) {
                $http({
                    url: '/api/Processes/LotNoList/' + processeLocationId + '/' + processListId,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.lotList = data;
                }).error(function (data) {
                    //alert("Attempt failed!");
                    $scope.Cancel();
                });
            }
        };

        //**Get Process Data**
        $scope.GetProcessData = function (processeLocationId, processListId, lotNo) {
            if (processListId != null && lotNo.LotNo != null && processeLocationId != null) {

                $http({
                    url: '/api/Processes/GetProcessData/' + processeLocationId + '/' + processListId + '/' + lotNo.LotNo,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {                  
                   
                    if (data.length > 0) {
                        $scope.PurchasedProductList = data;
                        if (data.length == 1) {                          
                            $scope.process = angular.copy(data[0]);
                            //$scope.process.PurchasedProductId.PurchasedProductId = data[0].PurchasedProductId;
                            $scope.process.PurchasedProductId = { PurchasedProductId: data[0].PurchasedProductId, PurchasedProductName: data[0].PurchasedProductName };
                            $scope.process.DeliveryQuantity = angular.copy(data[0].ReceiveQuantity);
                            $scope.ProcessList = '';
                        } else {                          
                            $scope.ProcessList = data;
                            $scope.getTotal = function () {
                                var total = 0;
                                for (var i = 0; i < $scope.ProcessList.length; i++) {
                                    var product = $scope.ProcessList[i];
                                    total += product.ReceiveQuantity;
                                }
                                return total;
                            }
                            //$scope.process = '';
                            //$scope.entryForm.$setPristine();
                            //$scope.entryForm.$setUntouched();
                        }
                    }
                    else {
                        $scope.ProcessList = '';
                        alert("No data found for this Porcess and Lot no.!");
                    }
                }).error(function (data) {
                    //alert("Attempt failed!");
                    //$scope.Cancel();
                });
            }
            else {
                //alert("Please fill up Porcess Name and Lot No. fields first.");
                //$scope.Cancel();
            }
        };

        //**Get Search**
        $scope.GetSearch = function (processSearch) {
            var fdate = $filter('date')($scope.processSearch.FromDate, "yyyy-MM-dd");
            var tdate = $filter('date')($scope.processSearch.ToDate, "yyyy-MM-dd");
            var fromdate = fdate;
            var todate = tdate;
            var processeLocationId = 0;
            var processListId = 0;
            if ($scope.processSearch.ProcesseLocation != null) {
                processeLocationId = $scope.processSearch.ProcesseLocation;
            };
            if ($scope.processSearch.ProcessList != null) {
                processListId = $scope.processSearch.ProcessList;
            };

            $http({
                url: 'api/Processes/GetSearch/' + fromdate + '/' + todate + '/' + processeLocationId + '/' + processListId,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data;               
            }).error(function (data) {
                $scope.message = "Process  list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);

            });
        }

        //**Get Process Data For Edit**
        $scope.GetProcessDataForEdit = function (item) {

            var processDate = $filter('date')(item.ProcessDate, "yyyy-MM-dd");            
                $http({
                    url: '/api/Processes/GetProcessDataForEdit/' + processDate + '/' + item.ProcesseLocationId + '/' + item.ProcessListId + '/' + item.LotNo,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    if (data.length > 0) {
                        $scope.PurchasedProductList = data;
                        if (data.length == 1) {
                            $scope.process = angular.copy(data[0]);                            
                            $scope.process.PurchasedProductId = { PurchasedProductId: data[0].PurchasedProductId, PurchasedProductName: data[0].PurchasedProductName };                           
                            $scope.ProcessListForEdit = '';
                            $scope.editMode = true;
                            $scope.data = {
                                cb1: true
                            };
                        } else {
                            $scope.ProcessListForEdit = data;  
                            $scope.editMode = true;
                            $scope.data = {
                                cb1: true
                            };
                        }
                    }
                    else {
                        $scope.ProcessListForEdit = '';
                        alert("No data found for Edit.!");
                    }
                }).error(function (data) {
                    //alert("Attempt failed!");
                    //$scope.Cancel();
                });
        };

        //**Get Quantity**
        $scope.GetReceiveQuantity = function (processeLocationId, ProcesseLocationId, lotNo, productId, deliveryQuantity) {
            if (ProcesseLocationId != null && lotNo.LotNo != null && processeLocationId != null) {
                $http({
                    url: '/api/Processes/GetReceiveQuantity/' + processeLocationId + '/' + ProcesseLocationId + '/' + lotNo.LotNo + '/' + productId.PurchasedProductId + '/' + deliveryQuantity,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {

                }).error(function (data) {
                    //alert("Attempt failed!");
                    //$scope.Cancel();
                });
            }

        };


    }])
