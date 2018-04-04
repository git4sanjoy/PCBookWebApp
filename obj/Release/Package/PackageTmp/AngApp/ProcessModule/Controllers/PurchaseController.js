var app = angular.module('PCBookWebApp');
app.controller('PurchaseController', ['$scope', '$location', '$http', '$timeout', '$filter','$q',
    function ($scope, $location, $http, $timeout, $filter, $q) {
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
        $scope.data = {
            cb1: true
        };

        $scope.simulateQuery = false;
        $scope.isDisabled = false;

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
        $scope.purchaseSearch = {};
        $scope.purchaseSearch.FromDate = new Date();
        $scope.purchaseSearch.ToDate = new Date();
        //***End DatePicker***

        // loadAll();
        $scope.ProductLists = [];
        $scope.querySearch = querySearch;
        $scope.selectedItemChange = selectedItemChange;
        $scope.searchTextChange = searchTextChange;

        $scope.newProductList = newProductList;

        function newProductList(productList) {
            alert("Sorry! You'll need to create a Constitution for " + productList + " first!");
        }

        function querySearch(query) {

            var results = query ? $scope.ProductLists.filter(createFilterFor(query)) : $scope.ProductLists,
                deferred;
            if ($scope.simulateQuery) {
                deferred = $q.defer();
                $timeout(function () { deferred.resolve(results); }, Math.random() * 1000, false);
                return deferred.promise;
            } else {
                return results;
            }
        }

        function searchTextChange(text) {
            //console.log('Text changed to ' + text);
        }

        function selectedItemChange(item) {
            //console.log('Item changed to ' + JSON.stringify(item));
        }

        /**
         * Create filter function for a query string
         */
        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);

            return function filterFn(productList) {
                return (productList.PurchasedProductName.indexOf(lowercaseQuery) === 0);
            };
        }

        $scope.changeSelectProductName = function (item) {
            $scope.purchase.PurchasedProductName = item;
        };

        //**Get All List**
        $scope.GetPurchasesList = function () {
            $http({
                url: "api/Purchases/GetPurchasesList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                //$scope.List = data.list;
                //$scope.ProsessList = data.prosessList;
                $scope.PurchasedProductList = data.purchasedProductList;
                $scope.SupplierList = data.supplierList;
                $scope.ProductLists = data.purchasedProductList;
                $scope.ShowRoomList = data.showRoomList;
                $scope.OrderNumbers = data.orderNumber;
            }).error(function (data) {
                $scope.message = "purchase  list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                
            });
        }
        $scope.GetPurchasesList();

        //**Get Purchases List**
        $scope.GetPurchasesListShow = function () {
            $http({
                url: "api/Purchases/GetPurchasesListShow",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data.list;               
            }).error(function (data) {
                $scope.message = "Purchase  list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);

            });
        }

        //**Save Purchased Product**
        $scope.Save = function (purchase) {
           
            var date = $filter('date')($scope.purchase.PurchaseDate, "yyyy-MM-dd");
            purchase.PurchaseDate = date;
            purchase.PurchasedProductId = $scope.selectedItem.PurchasedProductId;
            purchase.SupplierId = $scope.supplierSelectedItem.SupplierId;
            purchase.ShowRoomId = 0;
            purchase.SE = !purchase.SE ? 0 : purchase.SE;
            purchase.Discount = !purchase.Discount ? 0 : purchase.Discount;
            purchase.OrderNo = !$scope.orderNumberSelectedItem.OrderNumber ? 0 : $scope.orderNumberSelectedItem.OrderNumber;
            //console.log(purchase);          
            $http({
                traditional: true,
                url: '/api/Purchases/PostPurchase',
                method: 'POST',
                data: JSON.stringify(purchase),
                contentType: "application/json",
                dataType: "json",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetPurchasesListShow();
                $scope.GetPurchasesList();               
                $scope.message = "purchase data saved successfully.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $scope.purchase.PurchasedProductId = '';
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "purchase  data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);

            });

        };
        $scope.Update = function (purchase) {
            var date = $filter('date')($scope.purchase.PurchaseDate, "yyyy-MM-dd");
            purchase.PurchaseDate = date;
            purchase.ShowRoomId = 0;
            if ($scope.selectedItem != null) {
                purchase.PurchasedProductId = $scope.selectedItem.PurchasedProductId;
            }
            if ($scope.selectesupplierSelectedItemdItem != null) {
                purchase.SupplierId = $scope.supplierSelectedItem.SupplierId;
            }

            $http({
                url: '/api/Purchases/' + purchase.PurchaseId,
                data: purchase,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetPurchasesListShow();
                $scope.GetPurchasesList();
                $scope.message = "Purchase data updated successfully.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);              
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
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);

                    $scope.Cancel();
                    $scope.GetPurchasesListShow();
                    $scope.GetPurchasesList();
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
            $scope.purchase = angular.copy(item);
            $scope.searchText = item.PurchasedProductName;
            $scope.supplierSearchText = item.SupplierName;
            $scope.editMode = true;
        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.purchase = '';
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
            $scope.searchText = '';
            $scope.supplierSearchText = '';
            $scope.orderNumberSearchText = '';
            $scope.editMode = false;
        };
        //**Search Cancel Button**
        //$scope.Cancel = function () {
        //    $scope.purchaseSearch = '';
        //    $scope.searchForm.$setPristine();
        //    $scope.searchForm.$setUntouched();
        //    $scope.searchSupplierText = '';
        //    $scope.searchTSupplierext = '';
        //    $scope.searchProductText = false;
        //    $scope.purchase = '';
        //    $scope.purchase.PurchasedProductId = '';
        //    $scope.purchase.SupplierId = '';
           
        //};

        $scope.showReport = function () {
            var reportShowType = 'Print';
            var fd = new Date();
            var td = new Date();
            var ledgerIds = [];
            //if ($scope.ledgeListMultiSelect.length > 0) {
            //    angular.forEach($scope.ledgeListMultiSelect, function (item) {
            //        ledgerIds.push(item.id);
            //    })
            //}
            $http({
                method: "GET",
                //url: "/ProcessReports/ShowMatricsRptInNewWin",
                url: "/PurchaseReport/ShowMatricsRptInNewWin",
                params: {
                    FromDate: fd,
                    ToDate: td,

                    SelectedReportOption: '1',
                    ShowType: reportShowType
                }
            }).success(function (data) {
                if (data == 'NoRecord') {
                    alert('No Record Found');
                } else {
                    //window.open("Home", "_blank");  
                    window.open("../GenericReportViewer/ShowGenericRpt", 'mywindow', 'fullscreen=yes, scrollbars=auto');
                }
            })
                .error(function (error) {
                    alert(error);
                });
        };

        //$http({
        //    url: 'api/Purchases/GetPurchasesList',
        //    method: "GET",
        //    headers: authHeaders
        //}).success(function (data) {
        //    $scope.pies = data.purchasedProductList;
        //}).error(function (data) {
        //    $scope.message = "Product  list loading failed.";
        //    $scope.messageType = "warning";
        //    $scope.clientMessage = false;
        //    $timeout(function () { $scope.clientMessage = true; }, 5000);
        //});

        $scope.getProductsMatchesList = function (searchText) {
            var deferred = $q.defer();

            $timeout(function () {
                var states = $scope.pies.filter(function (state) {
                    return (state.PurchasedProductName.toUpperCase().indexOf(searchText.toUpperCase()) !== -1);
                });
                deferred.resolve(states);

            }, 500);

            return deferred.promise;
        };
        // Add  Form supplier Name
        $scope.getSuppliers = function (searchText) {
            var deferred = $q.defer();

            $timeout(function () {
                var states = $scope.SupplierList.filter(function (state) {
                    return (state.SupplierName.toUpperCase().indexOf(searchText.toUpperCase()) !== -1);
                });
                deferred.resolve(states);

            }, 500);

            return deferred.promise;
        };
        $scope.supplierTextChange = function (text) {
            //console.log('Text changed to ' + text);
        };
        $scope.supplierSelectedItemChange = function (item) {
            //console.log('Item changed to ' + JSON.stringify(item));
        };

        // Add  Form Order Number
        $scope.getOrderNumbers = function (searchText) {
            var deferred = $q.defer();

            $timeout(function () {
                var states = $scope.OrderNumbers.filter(function (state) {
                    return (state.OrderNumber.toUpperCase().indexOf(searchText.toUpperCase()) !== -1);
                });
                deferred.resolve(states);

            }, 500);

            return deferred.promise;
        };
        $scope.orderNumberTextChange = function (text) {
            //console.log('Text changed to ' + text);
        };
        $scope.orderNumberSelectedItemChange = function (item) {
            //console.log('Item changed to ' + JSON.stringify(item));
        };

        //Search Supplier Name
        $scope.getSearchSuppliers = function (searchText) {
            var deferred = $q.defer();

            $timeout(function () {
                var states = $scope.SupplierList.filter(function (state) {
                    return (state.SupplierName.toUpperCase().indexOf(searchText.toUpperCase()) !== -1);
                });
                deferred.resolve(states);

            }, 500);

            return deferred.promise;
        };
        $scope.supplierSearchTextChange = function (text) {
            //console.log('Text changed to ' + text);
            if (!text) {
                $scope.searchSupplierSelectedItem = '';
            }  
        };
        $scope.supplierSearchSelectedItemChange = function (item) {
            //console.log('Item changed to ' + JSON.stringify(item));
        };

        $scope.searchProductTextChange = function (text) {
            //console.log('Text changed to ' + text);
            if (!text) {
                $scope.searchProductSelectedItem = '';
            }
        };
        $scope.searchProductSelectedItemChange = function (item) {
            //console.log('Item changed to ' + JSON.stringify(item));
        };

        //Search Purchase
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
                var productId = null;
                var supplierId = null;               

                if ($scope.searchSupplierSelectedItem != null) {
                    supplierId = $scope.searchSupplierSelectedItem.SupplierId;
                }
                if ($scope.searchProductSelectedItem != null) {
                    productId = $scope.searchProductSelectedItem.PurchasedProductId;
                }

                //console.log(supplierId, productId);
                //return false;
                
                //var supplierId = $scope.searchSelectedItemSupplier.SupplierId;
                //var productId = $scope.searchSelectedItem.PurchasedProductId;

             
                $http({
                    url: 'api/Purchases/GetSearch/' + fromdate + '/' + todate + '/' + supplierId + '/' + productId,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.List = data.list;
                })
                .error(function (data) {
                    $scope.message = "purchase  list loading failed.";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);

                });

            }
        };
        $scope.searchFormClear = function () {
            $scope.purchaseSearch = '';
            $scope.searchProductSelectedItem = '';
            $scope.searchSupplierSelectedItem = '';
        };

    }])
