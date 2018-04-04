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
        $scope.AvailableData = 0;
        $scope.AvgRate = 0;

        $scope.changeSelectOrderNo = function (item) {
            $scope.challan.PChallanNo = item.OrderNumber;
        }
        $scope.changeSelectProductName = function (item) {
            $scope.challan.PurchasedProductDdl = item;
            var orderNo = $scope.challan.PChallanNo;
            var purchasedProductId = $scope.challan.PurchasedProductDdl.PurchasedProductId;


            $http({
                traditional: true,
                url: '/api/PurchasedProductRates/GetAvailableQuantity/' + purchasedProductId + '/' + orderNo,
                method: 'GET',
                headers: authHeaders
            }).success(function (data) {
                if (data.length > 0) {
                    $scope.AvailableData = data[0].Quantity;
                    $scope.AvgRate = data[0].AvgRate;
                } else {
                    $scope.AvailableData = 0;
                    $scope.AvgRate = 0;
                }
            }).error(function (data) {
                $scope.message = "Data load atemp failed!";
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.error("Supplier data saving attempt failed!", "Error");
            });
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
            var orderNo = "";
            var productName = "";
            var productId = null;
            var dQuantity = 0;
            var processeLocationId = null;
            var processListId = null;
            var avgRate = 0;

            if ($scope.challan.PurchaseDate) {
                var fd1 = $scope.challan.PurchaseDate;
            } else {
                alert('Please input date');
                angular.element('#PurchaseDate').focus();
                return false;
            }
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

            if ($scope.challan.PChallanNo) {
                orderNo = $scope.challan.PChallanNo;
            } else {
                alert('Please Select Order No');
                angular.element('#PChallanNo').focus();
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
                if (dQuantity > $scope.AvailableData) {
                    alert(dQuantity + ' Quantity in not available in sotck.');
                    return false;
                }


            } else {
                alert('Please input Quantity');
                angular.element('#DeliveryQuantity').focus();
                return false;
            }

            for (var i = 0; i < $scope.challan.items.length; i++) {
                if ($scope.challan.items[i].PurchasedProductId == productId) {
                    alert('Product : ' + productName + ' already esists in the current lot')
                    return false;
                }
            }
            if ($scope.AvgRate) {
                avgRate = $scope.AvgRate;
            }
            $scope.challan.items.push({
                PurchasedProductId: productId,
                PurchasedProductName: productName,
                DeliveryQuantity: dQuantity,
                AvgRate: avgRate
            });
            $scope.challan.PurchasedProductDdl = "";
            $scope.challan.DeliveryQuantity = 0;
            var addMoreProduct = confirm(productName + " Added to Cart. Are you sure you want to add more?");
            
            if (addMoreProduct) {
                $scope.AvailableData = '';
                $scope.AvgRate = '';
                angular.element('#PurchasedProductDdl').focus();
            } else {
                $scope.AvailableData = '';
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
                var processeLocationId = $scope.challan.ProcesseLocationDdl;
                var processListId = $scope.challan.ProcessListDdl;
                //console.log(processeLocationId);

                //return false;
                //var challanItems = [];
                angular.forEach($scope.challan.items, function (item) {

                    var aStoreDeliveryObj = {
                        PurchaseDate: fd,
                        PChallanNo: 'Delivery',
                        OrderNo: challanNo,
                        ProcesseLocationId: processeLocationId,
                        ProcessListId: processListId,
                        PurchasedProductId: item.PurchasedProductId,
                        PurchasedProductName: item.PurchasedProductName.trim(),
                        DeliveryQuantity: item.DeliveryQuantity,
                        Amount: parseFloat(item.DeliveryQuantity) * parseFloat(item.AvgRate),
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
                            ProcesseLocationId: processeLocationId,
                            ProcessListId: processListId,
                            LotNo: challanNo,
                            PurchasedProductId: item.PurchasedProductId,
                            ReceiveQuantity: item.DeliveryQuantity,
                            DeliveryQuantity: 0,
                            SE: 0,
                            Rate: item.AvgRate,
                            Amount: parseFloat(item.DeliveryQuantity) * parseFloat(item.AvgRate),
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
                $scope.challan.PChallanNo = '';
                $scope.challan.PurchaseDate = '';
                $scope.challan.ProcesseLocationDdl = '';
                $scope.challan.ProcessListDdl = '';
                angular.element('#PChallanNo').focus();
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
                $scope.orderNoList = data.orderNumber;
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
      //  $scope.ShowDetailsStoreDelivery();

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
                    $scope.ShowDetailsStoreDelivery();
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
            console.log(item);
            
            return $http({
                url: '/api/StoreDelivery/' + item.PurchaseId,
                data: item,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Updated.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $scope.ShowDetailsStoreDelivery();
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to Update.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
                });

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
               
                
                $scope.ShowDetailsStoreDelivery();
                $scope.Cancel();
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

        $scope.submitSearchForm = function () {
            $scope.submitted = true;
            //if ($scope.searchProductSelectedItem != null) {
            //    $scope.purchaseSearch.PurchasedProductId = $scope.searchProductSelectedItem.PurchasedProductId;
            //}
            //if ($scope.searchSupplierSelectedItem != null) {
            //    $scope.purchaseSearch.SupplierId = $scope.searchSupplierSelectedItem.SupplierId;
            //}           

            if ($scope.searchForm.$valid) {
                //console.log($scope.purchaseSearch);
                var fdate = $filter('date')($scope.purchaseSearch.FromDate, "yyyy-MM-dd");
                var tdate = $filter('date')($scope.purchaseSearch.ToDate, "yyyy-MM-dd");
                var fromdate = fdate;
                var todate = tdate;
                var lotNO = null;
               

                if ($scope.searchSupplierSelectedItem != null) {
                    supplierId = $scope.searchSupplierSelectedItem.SupplierId;
                }
                if ($scope.purchaseSearch.PChallanNo != null) {
                    lotNO = $scope.purchaseSearch.PChallanNo;
                }

                //console.log(supplierId, productId);
                //return false;

                //var supplierId = $scope.searchSelectedItemSupplier.SupplierId;
                //var productId = $scope.searchSelectedItem.PurchasedProductId;


                $http({
                    url: 'api/StoreDelivery/GetSearch/' + fromdate + '/' + todate + '/' + lotNO,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.ProcessDetailList = data.list;
                    //console.log(data.list);
                })
                    .error(function (data) {
                        $scope.message = "purchase  list loading failed.";
                        $scope.messageType = "warning";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);

                    });

            }
        };
    }])
