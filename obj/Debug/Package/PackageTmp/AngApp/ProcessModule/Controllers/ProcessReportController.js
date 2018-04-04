var app = angular.module('PCBookWebApp');
app.controller('ProcessReportController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {

        $scope.loading = true;
        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";


        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }
        $scope.reportOption = {
            FromDate: new Date(),
            ToDate: new Date()
        };
        // For 3 DatePicker
        $scope.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.opened = true;
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
        $scope.reportOption.FromDate = new Date();
        $scope.reportOption.ToDate = new Date();
        //End DatePicker
        //**********************************************************
        //**************Report Option*******************************
        $scope.printTypeArray = [];
        $scope.printTypeArray.push({ name: "Print", Text: "Print", isDefault: true });
        $scope.printTypeArray.push({ name: "Excel", Text: "Excel", isDefault: false });
        $scope.printOptionRadios = $scope.printTypeArray[0];
        $scope.printOption = [];
        //**********************************************************


        $scope.data = {
            group2: 'DateBetweenSupplierWisePurchase'
        };
        $scope.radioData = [
            { label: 'Supplier wise Date Between Purchase', value: 'DateBetweenSupplierWisePurchase' },
            { label: 'Product wise Date Between Purchase', value: 'DateBetweenProductWisePurchase' },
            { label: 'Factory wise Date Between Store Delivery ', value: 'DateBetweenStoreDeliveryFactoryWise' },
            { label: 'Product wise Store Balance', value: 'StoreBalanceProductWise' },
            { label: 'Factory wise Pending Process', value: 'PendingProcessBalanceFactoryWise' },
            { label: 'Factory wise Process History', value: 'FactoryWiseProcessHistory' },           
            { label: 'Finished Goods Stock', value: 'FinishedGoodsStock' },
            { label: 'Next', value: 'next', isDisabled: true }
        ];

        function conditionDivMakeFalse() {
            $scope.supplierDiv = false;
            $scope.productDiv = false;
            $scope.factoryDiv = false;
            //$scope.dateDiv = true;
        };
        conditionDivMakeFalse();
        //$scope.productDiv = true;
        $scope.supplierDiv = true;
        //$scope.factoryDiv = true;



        $scope.updateOptionGroupStatus = function () {
            var selectedReport = $scope.data.group2;
            conditionDivMakeFalse();
            $scope.dateDiv = true;
            if (selectedReport == "DateBetweenSupplierWisePurchase") {
                conditionDivMakeFalse();
                $scope.supplierDiv = true;
            } else if (selectedReport == "DateBetweenProductWisePurchase") {
                conditionDivMakeFalse();
                $scope.productDiv = true;
            } else if (selectedReport == "DateBetweenStoreDeliveryFactoryWise") {
                conditionDivMakeFalse();
                $scope.factoryDiv = true;
            } else if (selectedReport == "StoreBalanceProductWise") {
                conditionDivMakeFalse();
                $scope.productDiv = true;
            } else if (selectedReport == "PendingProcessBalanceFactoryWise") {
                conditionDivMakeFalse();
                $scope.factoryDiv = true;
            } else if (selectedReport == "FactoryWiseProcessHistory") {
                conditionDivMakeFalse();
                $scope.factoryDiv = true;
            } else if (selectedReport == "FinishedGoodsStock") {
                conditionDivMakeFalse();
            }
        }


        // Show Report Button
        $scope.showReport = function () {
            var selectedReport = $scope.data.group2;

            if (selectedReport == "DateBetweenSupplierWisePurchase") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                if ($scope.supplierListMultiSelect.length > 0) {
                    angular.forEach($scope.supplierListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                        console.log(item.id);
                    })
                }
                $http({
                    method: "GET",
                    url: "/ProcessReports/ShowPurchaseRptInNewWin",
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        LedgerIds: ledgerIds,
                        SelectedReportOption: selectedReport,
                        ShowType: reportShowType
                    }
                }).success(function (data) {
                    if (data == 'NoRecord') {
                        alert('No Record Found');
                    } else {
                        window.open("../GenericReportViewer/ShowGenericRpt", 'mywindow', 'fullscreen=yes, scrollbars=auto');
                    }
                }).error(function (error) {
                    alert(error);
                });
            } else if (selectedReport == "DateBetweenProductWisePurchase") {
              
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                //var ledgerIds = $scope.productListMultiSelect;
                if ($scope.productListMultiSelect.length > 0) {
                    angular.forEach($scope.productListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                        //ledgerIds = [ 1,2,3,4];
                    })
                    //ledgerIds = [1, 2, 3, 4];
                }
                //alert(ledgerIds);
                $http({
                    method: "GET",
                    url: "/ProcessReports/ShowProductRptInNewWin",
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        LedgerIds: ledgerIds,
                        SelectedReportOption: selectedReport,
                        ShowType: reportShowType
                    }
                }).success(function (data) {
                    if (data == 'NoRecord') {
                        alert('No Record Found');
                    } else {
                        window.open("../GenericReportViewer/ShowGenericRpt", 'mywindow', 'fullscreen=yes, scrollbars=auto');
                    }
                })
                    .error(function (error) {
                        alert(error);
                    });
            }
            else if (selectedReport == "DateBetweenStoreDeliveryFactoryWise") {
                // alert('sss');
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                if ($scope.factoryListMultiSelect.length > 0) {
                    angular.forEach($scope.factoryListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                    })
                }
                $http({
                    method: "GET",
                    url: "/ProcessReports/ShowFactoryRptInNewWin",
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        LedgerIds: ledgerIds,
                        SelectedReportOption: selectedReport,
                        ShowType: reportShowType
                    }
                }).success(function (data) {
                    if (data == 'NoRecord') {
                        alert('No Record Found');
                    } else {
                        window.open("../GenericReportViewer/ShowGenericRpt", 'mywindow', 'fullscreen=yes, scrollbars=auto');
                    }
                })
                    .error(function (error) {
                        alert(error);
                    });
            }
            else if (selectedReport == "PendingProcessBalanceFactoryWise") {
                // alert('sss');
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                if ($scope.factoryListMultiSelect.length > 0) {
                    angular.forEach($scope.factoryListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                    })
                }
                $http({
                    method: "GET",
                    url: "/ProcessReports/FactoryWisePendingProcess",
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        LedgerIds: ledgerIds,
                        SelectedReportOption: selectedReport,
                        ShowType: reportShowType
                    }
                }).success(function (data) {
                    if (data == 'NoRecord') {
                        alert('No Record Found');
                    } else {
                        window.open("../GenericReportViewer/ShowGenericRpt", 'mywindow', 'fullscreen=yes, scrollbars=auto');
                    }
                })
                    .error(function (error) {
                        alert(error);
                    });
            }
            else if (selectedReport == "FactoryWiseProcessHistory") {
                // alert('sss');
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                if ($scope.factoryListMultiSelect.length > 0) {
                    angular.forEach($scope.factoryListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                    })
                }
                $http({
                    method: "GET",
                    url: "/ProcessReports/FactoryWiseProcessHistory",
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        LedgerIds: ledgerIds,
                        SelectedReportOption: selectedReport,
                        ShowType: reportShowType
                    }
                }).success(function (data) {
                    if (data == 'NoRecord') {
                        alert('No Record Found');
                    } else {
                        window.open("../GenericReportViewer/ShowGenericRpt", 'mywindow', 'fullscreen=yes, scrollbars=auto');
                    }
                })
                    .error(function (error) {
                        alert(error);
                    });
            }
            else if (selectedReport == "StoreBalanceProductWise") {
                // alert('sss');
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                if ($scope.productListMultiSelect.length > 0) {
                    angular.forEach($scope.productListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                    })
                }
                $http({
                    method: "GET",
                    url: "/ProcessReports/ProductWiseStoreBalance",
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        LedgerIds: ledgerIds,
                        SelectedReportOption: selectedReport,
                        ShowType: reportShowType
                    }
                }).success(function (data) {
                    if (data == 'NoRecord') {
                        alert('No Record Found');
                    } else {
                        window.open("../GenericReportViewer/ShowGenericRpt", 'mywindow', 'fullscreen=yes, scrollbars=auto');
                    }
                })
                    .error(function (error) {
                        alert(error);
                    });
            }
            else if (selectedReport == "FinishedGoodsStock") {
                // alert('sss');
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                if ($scope.supplierListMultiSelect.length > 0) {
                    angular.forEach($scope.supplierListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                    })
                }
                $http({
                    method: "GET",
                    url: "/ProcessReports/FinishedGoodsStock",
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        LedgerIds: ledgerIds,
                        SelectedReportOption: selectedReport,
                        ShowType: reportShowType
                    }
                }).success(function (data) {
                    if (data == 'NoRecord') {
                        alert('No Record Found');
                    } else {
                        window.open("../GenericReportViewer/ShowGenericRpt", 'mywindow', 'fullscreen=yes, scrollbars=auto');
                    }
                })
                    .error(function (error) {
                        alert(error);
                    });
            }
        };
        //End Report Showing Methods*********** 
        //**************************************
      
        //*******************************************
        // Supplier List Multi select
        $scope.supplierListMultiSelect = [];
        $scope.supplierListMultiSelectSettings = {
            scrollableHeight: '300px',
            scrollable: true,
            enableSearch: true,
        };
        $scope.supplierListMultiSelectCustomTexts = { buttonDefaultText: '--- Select Supplier Name --' };
        $scope.supplierList = [];
        $http({           
            url: "/api/Suppliers/SuppliersMultiSelectList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {                     
            $scope.supplierList = data;           
        }).error(function (error) {
            console.log(error);
        });

        // Product List Multi select
        $scope.productListMultiSelect = [];
        $scope.productListMultiSelectSettings = {
            //externalIdProp: '',
            scrollableHeight: '300px',
            scrollable: true,
            enableSearch: true,
        };
        $scope.productListMultiSelectCustomText = { buttonDefaultText: '--- Select Product Name ---' };
        $scope.productList = [];
        $http({
            url: "/api/PurchasedProducts/PurchasedProductsMultiSelectList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.productList = data;
        });
        // Factory List Multi select
        $scope.factoryListMultiSelect = [];
        $scope.factoryListMultiSelectSettings = {
            //externalIdProp: '',
            scrollableHeight: '300px',
            scrollable: true,
            enableSearch: true,
        };
        $scope.factoryListMultiSelectCustomText = { buttonDefaultText: '--- Select Factory Name ---' };
        $scope.factoryList = [];
        $http({
            url: "/api/ProcesseLocations/ProcesseLocationsMultiSelectList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.factoryList = data;
        });
        // End Multi select
        //*****************************************************
    }])