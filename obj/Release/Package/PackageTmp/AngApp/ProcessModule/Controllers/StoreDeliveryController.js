var app = angular.module('PCBookWebApp');
app.controller('StoreDeliveryController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "Purchase Controller";
        $scope.loading = true;

        $scope.isDateDisabled = false;
        $scope.isFactoryLocationDisabled = false;
        $scope.isProcessesListDisabled = false;

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
        //$scope.invoiceDatePickerIsOpen = false;
        //$scope.InvoiceDatePickerOpen = function () {
        //    this.invoiceDatePickerIsOpen = true;
        //};
        //***End DatePicker***
        $scope.changeSelectProductName = function (item) {
            $scope.challan.PurchasedProductDdl = item;
        };
        $scope.data = {
            cb1: true
        };











        $scope.challan = {
            //ProcesseLocationDdl: { ProcesseLocationId: null },
            //ProcessListDdl: { ProcessListId: null },
            PurchaseDate: new Date(),
            PChallanNo: ""
        };

        $scope.challan.items = [];
        $scope.addItem = function () {
            var productName = "";
            var productId = null;
            var dQuantity = 0;
            var processeLocationId = null;
            var processListId = null;

            if ($scope.challan.ProcesseLocationDdl) {
                processeLocationId = $scope.challan.ProcesseLocationDdl;
            } else {
                alert('Please Select Factory Location');
                angular.element('#ProcesseLocationDdl').focus();
                return false;
            }
            if ($scope.challan.ProcessListDdl) {
                processListId = $scope.challan.ProcessListDdl;
            } else {
                alert('Please Select Process');
                angular.element('#ProcessListDdl').focus();
                return false;
            }
            if ($scope.challan.PurchaseDate) {
                var fd1 = $scope.challan.PurchaseDate;
            } else {
                alert('Please input date');
                angular.element('#PurchaseDate').focus();
                return false;
            }


            if ($scope.challan.PurchasedProductDdl) {
                productName = $scope.challan.PurchasedProductDdl.PurchasedProductName;
                productId = $scope.challan.PurchasedProductDdl.PurchasedProductId;
            } else {
                alert('Please input Product Name');
                angular.element('#PurchasedProductDdl').focus();
                return false;
            }
            if ($scope.challan.DeliveryQuantity) {
                dQuantity = $scope.challan.DeliveryQuantity;

            } else {
                alert('Please input Quantity');
                angular.element('#DeliveryQuantity').focus();
                return false;
            }

            $scope.challan.items.push({
                PurchasedProductId: productId,
                PurchasedProductName: productName,
                DeliveryQuantity: dQuantity,
            });
            $scope.challan.PurchasedProductDdl = "";
            $scope.challan.DeliveryQuantity = 0;
            var addMoreProduct = confirm(productName + " Added to Cart. Are you sure you want to add more?");
            if (addMoreProduct) {
                angular.element('#PurchasedProductDdl').focus();
            } else {
                angular.element('#saveBtn').focus();
            }

        },

            $scope.SaveStoreDelivery = function (ev) {
                var challanNo = "";
                if ($scope.challan.PChallanNo) {
                    challanNo = $scope.challan.PChallanNo;
                } else {
                    alert('Please input chalan no');
                    angular.element('#PChallanNo').focus();
                    return false;
                }

                var fd = $filter('date')($scope.challan.PurchaseDate, "yyyy-MM-dd");
                //var challanItems = [];
                angular.forEach($scope.challan.items, function (item) {

                    var aStoreDeliveryObj = {
                        PurchaseDate: fd,
                        PChallanNo: $scope.challan.PChallanNo,
                        ProcesseLocationId: $scope.challan.ProcesseLocationDdl,
                        ProcessListId: $scope.challan.ProcessListDdl,
                        PurchasedProductId: item.PurchasedProductId,
                        PurchasedProductName: item.PurchasedProductName.trim(),
                        DeliveryQuantity: item.DeliveryQuantity,
                        ShowRoomId: 0,
                        SupplierId: null,
                        SE: 0,
                        Discount: 0
                    };

                    $http({
                        url: "/api/Purchases",
                        data: aStoreDeliveryObj,
                        method: "POST",
                        headers: authHeaders
                    }).success(function (data) {
                        var purchaseId = data.PurchaseId;
                        var aProcessReceiveObj = {
                            PurchaseId: purchaseId,
                            ProcessDate: fd,
                            ProcesseLocationId: $scope.challan.ProcesseLocationDdl,
                            ProcessListId: $scope.challan.ProcessListDdl,
                            LotNo: $scope.challan.PChallanNo,
                            PurchasedProductId: item.PurchasedProductId,
                            ReceiveQuantity: item.DeliveryQuantity,
                            DeliveryQuantity: 0,
                            SE: 0,
                            Rate: 0,
                            Amount: 0,
                            Discount: 0,
                            ShowRoomId: 0
                        };
                        //console.log(aProcessReceiveObj);
                        $http({
                            url: "/api/Processes/PostProcess",
                            data: aProcessReceiveObj,
                            method: "POST",
                            headers: authHeaders
                        }).success(function (data) {
                           
                            $scope.ShowDetailsStoreDelivery();
                            $scope.message = "Deliveryed to store successfully.";
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
                                $scope.validationErrors.push('Unable to add Process Receive.');
                            };
                            $scope.messageType = "danger";
                            $scope.serverMessage = false;
                            $timeout(function () { $scope.serverMessage = true; }, 5000);
                        });
                    }).error(function (error) {
                        $scope.validationErrors = [];
                        if (error.ModelState && angular.isObject(error.ModelState)) {
                            for (var key in error.ModelState) {
                                $scope.validationErrors.push(error.ModelState[key][0]);
                            }
                        } else {
                            $scope.validationErrors.push('Unable to add Store Delivery.');
                        };
                        $scope.messageType = "danger";
                        $scope.serverMessage = false;
                        $timeout(function () { $scope.serverMessage = true; }, 5000);
                    });


                })
                //$scope.challan = {
                //    PurchaseDate: new Date(),
                //    PChallanNo: "",
                //    ProcessListDdl: { ProcessListId: null, ProcessListName: null },
                //    ProcesseLocationDdl: { ProcesseLocationId: null, ProcesseLocationName: null }
                //};
                $scope.challan.items = [];
                angular.element('#ProcesseLocationDdl').focus();
            },
            $scope.removeItem = function (index) {
            
                $scope.challan.items.splice(index, 1);
            },
        $scope.totalQuantity = function () {
            var items = $scope.challan.items;
            var total = 0;
            angular.forEach(items, function (item) {
                total += parseFloat(item.DeliveryQuantity) ;
            })
            return total;
        }





        //**Get Related Tables Data for Dropdown ant autocomplete**
        $scope.GetRelatedTableList = function () {
            $http({
                url: "api/Purchases/GetPurchasesList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                //$scope.List = data.list;
                $scope.PurchasedProductList = data.purchasedProductList;
                $scope.SupplierList = data.supplierList;
                $scope.ProcesslocationList = data.processlocationList;
                $scope.ProcessList = data.processList;
            }).error(function (data) {
                $scope.message = "Store Delivery Related Data list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        };
        $scope.GetRelatedTableList();
        //**Show Store Delivery in Table**
        $scope.ShowDetailsStoreDelivery = function () {
            //alert('click');
            $http({
                traditional: true,
                url: '/api/StoreDelivery/GetDetailhData/',
                method: 'GET',
                headers: authHeaders
            }).success(function (data) {
                //console.log(data);
                $scope.ProcessDetailList = data;
            }).error(function (data) {
                $scope.message = "Data load atemp failed!";
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });
        };
        $scope.ShowDetailsStoreDelivery();

        //**Populate data for update **
        $scope.searchLotNo = '';
        $scope.FindLotNo = function (searchLotNo) {
            $http({
                traditional: true,
                url: '/api/StoreDelivery/GetSearchData/' + searchLotNo,
                method: 'GET',
                headers: authHeaders
            }).success(function (data) {
                $scope.editMode = true;
                //console.log(data);
                $scope.challan.ProcesseLocationDdl = data.list[0].ProcesseLocationId;
                $scope.challan.ProcessListDdl = data.list[0].ProcessListId;
                $scope.challan.PChallanNo = data.list[0].PChallanNo;
                $scope.challan.PurchaseDate = data.list[0].PurchaseDate;

                $scope.challan.items = data.list;
                //console.log(data.list);
            }).error(function (data) {
                $scope.message = "Store Delivery data saving attempt failed!";
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });

        };
        $scope.clear = function () {
            $scope.editMode = false;
            $scope.challan = {
                PurchaseDate: new Date(),
                PChallanNo: "",
                ProcessListDdl: { ProcessListId: null, ProcessListName:null},
                ProcesseLocationDdl: { ProcesseLocationId: null, ProcesseLocationName: null}
            };
            $scope.challan.items = [];
            angular.element('#ProcesseLocationDdl').focus();
        };
        //**Delete Store Delivery**
        $scope.DeleteItem = function (item, index) {

            var id = item.PurchaseId;
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {

                $http({
                    url: "/api/StoreDelivery/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {

                    $scope.challan.items.splice(index, 1);
                }).error(function (data) {
                    alert('error occord')
                    $scope.message = "Data could not be deleted!";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    $scope.ShowDetailsStoreDelivery();
                });
            };
        };

        $scope.saveNewItem = function () {

            //var date = $filter('date')($scope.purchase.PurchaseDate, "yyyy-MM-dd");
            //purchase.PurchaseDate = date;

            var fd = $filter('date')($scope.challan.PurchaseDate, "yyyy-MM-dd");
            var aStoreDeliveryObj = {
                PurchaseDate: fd,
                PChallanNo: $scope.challan.PChallanNo,
                ProcesseLocationId: $scope.challan.ProcesseLocationDdl,
                ProcessListId: $scope.challan.ProcessListDdl,
                PurchasedProductId: $scope.challan.PurchasedProductDdl.PurchasedProductId,
                PurchasedProductName: $scope.challan.PurchasedProductDdl.PurchasedProductName.trim(),
                DeliveryQuantity: $scope.challan.DeliveryQuantity,
                ShowRoomId: 0,
                SupplierId: null,
                SE: 0,
                Discount: 0

            };
            $http({
                url: "api/StoreDelivery/PostPurchase",
                method: "POST",
                data: aStoreDeliveryObj,
                headers: authHeaders
            }).success(function (data) {

                $scope.challan.items.push({
                    PurchaseId: data.PurchaseId,
                    PurchasedProductId: aStoreDeliveryObj.PurchasedProductId,
                    PurchasedProductName: aStoreDeliveryObj.PurchasedProductName,
                    DeliveryQuantity: aStoreDeliveryObj.DeliveryQuantity,
                });
                //console.log($scope.challan.items);
                $scope.ShowDetailsStoreDelivery();
                    $scope.ShowDetailsStoreDelivery();
                    $scope.message = "Deliveryed to store successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "Store Delivery Related Data list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
            console.log(aStoreDeliveryObj);
            $scope.challan.DeliveryQuantity = 0;
            $scope.challan.PurchasedProductDdl = '';
        };
        $scope.UpdateItem = function (item) {
            console.log(item)
        };
        $scope.DeleteByLotNo = function (item) {
            
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                var lotNo = item.PChallanNo;
                var processeLocationId = item.ProcesseLocationId;
            }
            $http({
                url: "/api/StoreDelivery/DeleteByLotNo/" + lotNo + "/" + processeLocationId,
                method: "DELETE",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Data deleted successfully.";
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.success("Data deleted successfully.", "Success");
                $scope.Cancel();
                $scope.GetSupplierList();
                }).error(function (data) {
                   // alert('error occord')
                    $scope.message = "Data could not be deleted!";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    //toastr.error("Data could not be deleted!", "Failed!");
                });
            //console.log(item);
            //$scope.ShowDetailsStoreDelivery();
        };
        $scope.GetNewLotNo = function () {
            GetLotNo()
        };
        //Create New Voucher No
        var currentTime = new Date();
        var strPiNo = currentTime.getFullYear() + "-";
        function GetLotNo() {
            var newStrId = 0;
            $http({
                url: "/api/StoreDelivery/GetMemoId",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                //alert(strPiNo + "00000" + data.MemoMasterId);
                $scope.challan.PChallanNo = strPiNo + "00000" + data.PChallanNo;

            }).error(function (data) {
                alert('Error in creating Lot No');

            });
        }
        $scope.editMode = false;
    }])
