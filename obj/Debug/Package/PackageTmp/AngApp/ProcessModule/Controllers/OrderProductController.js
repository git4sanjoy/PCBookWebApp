
var app = angular.module('PCBookWebApp');
app.controller('OrderProductController', ['$scope', '$location', '$http', '$timeout', '$filter',
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
            cb1: true
        };

        //***For Order DatePicker***        
        $scope.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.opened = true;
        };
        $scope.OrderDatePickerIsOpen = false;
        $scope.OrderDatePickerOpen = function () {
            this.OrderDatePickerIsOpen = true;
        };

        //**Get All List**
        $scope.GetAllList = function () {
            $http({
                url: "/api/FinishedGoodStocks/GetAllList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data.list;
                $scope.FinishedGoodList = data.finishedGoodList;

            }).error(function (data) {
                $scope.message = "Process list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetAllList();



        $scope.orderedProductItems = [];
        $scope.addItem = function () {
            var OrderNumber = "";
            var FinishedGoodName = "";
            var FinishedGoodId = null;
            var OrderQuantity = 0;

            if ($scope.orderedProduct.OrderNumber) {
                OrderNumber = $scope.orderedProduct.OrderNumber;
            } else {
                alert('Please Filup Order Number');
                angular.element('#OrderNumber').focus();
                return false;
            }
            if ($scope.orderedProduct.ReceiveDate) {
                var fd1 = $scope.orderedProduct.ReceiveDate;
            } else {
                alert('Please input date');
                angular.element('#ReceiveDate').focus();
                return false;
            }


            if ($scope.orderedProduct.FinishedGood) {
                FinishedGoodName = $scope.orderedProduct.FinishedGood.FinishedGoodName;
                FinishedGoodId = $scope.orderedProduct.FinishedGood.FinishedGoodId;
            } else {
                alert('Please Select Goods Name');
                angular.element('#FinishedGoodId').focus();
                return false;
            }
            if ($scope.orderedProduct.OrderQuantity) {
                OrderQuantity = $scope.orderedProduct.OrderQuantity;
            } else {
                alert('Please input Quantity');
                angular.element('#OrderQuantity').focus();
                return false;
            }

            for (var i = 0; i < $scope.orderedProductItems.length; i++) {
                if ($scope.orderedProductItems[i].FinishedGoodName == FinishedGoodName) {
                    alert('This Fished Goods already esists in the current lot')
                    return false;
                }
            }

            $scope.orderedProductItems.push({
                FinishedGoodName: FinishedGoodName,
                FinishedGoodId: FinishedGoodId,
                OrderQuantity: OrderQuantity
            });
            $scope.orderedProduct.FinishedGood = "";
            $scope.orderedProduct.OrderQuantity = 0;

            //OOOKKK


            //var addMoreProduct = confirm(productName + " Added to Cart. Are you sure you want to add more?");

            //if (addMoreProduct) {
            //    $scope.AvailableData = '';
            //    $scope.AvgRate = '';
            //    angular.element('#PurchasedProductDdl').focus();
            //} else {
            //    $scope.AvailableData = '';
            //    angular.element('#saveBtn').focus();
            //}

        },

            $scope.removeItem = function (index) {

                $scope.orderedProductItems.splice(index, 1);
            },

            $scope.totalQuantity = function () {
            var items = $scope.orderedProductItems;
                var total = 0;
                angular.forEach(items, function (item) {
                    total += parseFloat(item.OrderQuantity);
                })
                return total;
            }


        //$scope.saveNewItem = function () {

        //        //var date = $filter('date')($scope.purchase.PurchaseDate, "yyyy-MM-dd");
        //        //purchase.PurchaseDate = date;

        //        var fd = $filter('date')($scope.challan.PurchaseDate, "yyyy-MM-dd");
        //        var aStoreDeliveryObj = {
        //            PurchaseDate: fd,
        //            PChallanNo: $scope.challan.PChallanNo,
        //            ProcesseLocationId: $scope.challan.ProcesseLocationDdl,
        //            ProcessListId: $scope.challan.ProcessListDdl,
        //            PurchasedProductId: $scope.challan.PurchasedProductDdl.PurchasedProductId,
        //            PurchasedProductName: $scope.challan.PurchasedProductDdl.PurchasedProductName.trim(),
        //            DeliveryQuantity: $scope.challan.DeliveryQuantity,
        //            ShowRoomId: 0,
        //            SupplierId: null,
        //            SE: 0,
        //            Discount: 0

        //        };
        //        $http({
        //            url: "api/StoreDelivery/PostPurchase",
        //            method: "POST",
        //            data: aStoreDeliveryObj,
        //            headers: authHeaders
        //        }).success(function (data) {

        //            $scope.challan.items.push({
        //                PurchaseId: data.PurchaseId,
        //                PurchasedProductId: aStoreDeliveryObj.PurchasedProductId,
        //                PurchasedProductName: aStoreDeliveryObj.PurchasedProductName,
        //                DeliveryQuantity: aStoreDeliveryObj.DeliveryQuantity,
        //            });
        //            //console.log($scope.challan.items);                
        //            $scope.ShowDetailsStoreDelivery();
        //            $scope.message = "Deliveryed to store successfully.";
        //            $scope.messageType = "success";
        //            $scope.clientMessage = false;
        //            $timeout(function () { $scope.clientMessage = true; }, 5000);
        //        }).error(function (data) {
        //            $scope.message = "Store Delivery Related Data list loading failed.";
        //            $scope.messageType = "warning";
        //            $scope.clientMessage = false;
        //            $timeout(function () { $scope.clientMessage = true; }, 5000);
        //            //toastr.warning("Supplier list loading failed.", "Failed!");
        //        });
        //        console.log(aStoreDeliveryObj);
        //        $scope.challan.DeliveryQuantity = 0;
        //        $scope.challan.PurchasedProductDdl = '';
        //    };
        //$scope.UpdateItem = function (item) {
        //    console.log(item);

        //    return $http({
        //        url: '/api/StoreDelivery/' + item.PurchaseId,
        //        data: item,
        //        method: "PUT",
        //        headers: authHeaders
        //    }).success(function (data) {
        //        $scope.message = "Successfully Updated.";
        //        $scope.messageType = "info";
        //        $scope.clientMessage = false;
        //        $scope.ShowDetailsStoreDelivery();
        //        $timeout(function () { $scope.clientMessage = true; }, 5000);
        //    }).error(function (error) {
        //        $scope.validationErrors = [];
        //        if (error.ModelState && angular.isObject(error.ModelState)) {
        //            for (var key in error.ModelState) {
        //                $scope.validationErrors.push(error.ModelState[key][0]);
        //            }
        //        } else {
        //            $scope.validationErrors.push('Unable to Update.');
        //        };
        //        $scope.messageType = "danger";
        //        $scope.serverMessage = false;
        //        $timeout(function () { $scope.serverMessage = true; }, 5000);
        //    });

        //};
        //$scope.DeleteByLotNo = function (item) {

        //    var msg = confirm("Do you want to delete this data?");
        //    if (msg == true) {
        //        var lotNo = item.PChallanNo;
        //        var processeLocationId = item.ProcesseLocationId;
        //    }
        //    $http({
        //        url: "/api/StoreDelivery/DeleteByLotNo/" + lotNo + "/" + processeLocationId,
        //        method: "DELETE",
        //        headers: authHeaders
        //    }).success(function (data) {
        //        $scope.message = "Data deleted successfully.";
        //        $scope.messageType = "danger";
        //        $scope.clientMessage = false;
        //        $timeout(function () { $scope.clientMessage = true; }, 5000);
        //        //toastr.success("Data deleted successfully.", "Success");


        //        $scope.ShowDetailsStoreDelivery();
        //        $scope.Cancel();
        //    }).error(function (data) {
        //        // alert('error occord')
        //        $scope.message = "Data could not be deleted!";
        //        $scope.messageType = "warning";
        //        $scope.clientMessage = false;
        //        $timeout(function () { $scope.clientMessage = true; }, 5000);
        //        //toastr.error("Data could not be deleted!", "Failed!");
        //    });
        //    //console.log(item);
        //    //$scope.ShowDetailsStoreDelivery();
        //};







        //**Save Ordered Product**
        $scope.Save = function (orderedProduct) {
            var date = $filter('date')($scope.orderedProduct.ReceiveDate, "yyyy-MM-dd");
            var Obj = {
                ReceiveDate: date,
                OrderQuantity: orderedProduct.OrderQuantity,
                ReceiveQuantity: 0,
                DeliveryQuantity: 0,
                OrderNumber: orderedProduct.OrderNumber,
                BuyerName: orderedProduct.BuyerName
            }

            //console.log(Obj);

            $http({
                url: "/api/FinishedGoodStocks",
                data: Obj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {               

                //console.log(data.FinishedGoodStockId);
                //console.log($scope.orderedProductItems);                

                $http({
                    url: '/api/FinishedGoodStockDetails/PostFinishedGoodStockDetails/' + data.FinishedGoodStockId,
                    data: JSON.stringify($scope.orderedProductItems),
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.orderedProduct.FinishedGoodId = {};
                    $scope.orderedProduct.OrderQuantity = '';
                    $scope.orderedProductItems = '';
                    $scope.message = "Data saved successfully.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    $scope.data = {
                        cb1: false
                    };
                }).error(function (data) {
                    });

                $scope.orderedProduct.FinishedGoodId = {};
                $scope.orderedProduct.OrderQuantity = '';
                $scope.GetAllList();
                $scope.message = "Data saved successfully.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                $scope.data = {
                    cb1: false
                };
            }).error(function (data) {
                $scope.message = "Data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });

        };

        //**Update rdered Product**
        $scope.Update = function (orderedProduct) {

            console.log(orderedProduct);

            $http({
                url: '/api/FinishedGoodStocks/' + orderedProduct.FinishedGoodStockId,
                data: orderedProduct,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetAllList();
                $scope.message = "Updated successfully.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $scope.data = {
                    cb1: false
                };
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });

        };

        //**Delete rdered Product**r
        $scope.Delete = function (item) {

            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/FinishedGoodStocks/" + item.FinishedGoodStockId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    $scope.Cancel();
                    $scope.GetAllList();
                    $scope.data = {
                        cb1: false
                    };
                }).error(function (data) {
                    alert('error occord')
                    $scope.message = "Data could not be deleted!";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            };

        };

        //**Edit Button**
        $scope.Edit = function (item) {
            $scope.orderedProduct = angular.copy(item);
            $scope.editMode = true;
        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.orderedProduct = '';
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
            $scope.editMode = false;
            $scope.data = {
                cb1: false
            };
        };


    }])
