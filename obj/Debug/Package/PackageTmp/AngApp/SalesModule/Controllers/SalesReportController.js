var app = angular.module('PCBookWebApp');
app.controller('SalesReportController', ['$scope', '$location', '$http', '$timeout', '$filter', "$mdDialog", "$mdToast",
    function ($scope, $location, $http, $timeout, $filter, $mdDialog, $mdToast) {
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
        $scope.reportOption = [];
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
        $scope.reportOption.fromDate = new Date();
        $scope.reportOption.toDate = new Date();
        //End DatePicker
        //*********************************************************
        //**************Report Option Print or Excel***************
        $scope.printTypeArray = [];
        $scope.printTypeArray.push({ name: "Print", Text: "Print", isDefault: true });
        $scope.printTypeArray.push({ name: "Excel", Text: "Excel", isDefault: false });
        $scope.printOptionRadios = $scope.printTypeArray[0];
        //**********************************************************


        $scope.data = {
            group2: 'DateBetweenCustomersBalance'
        };
        $scope.radioData = [
            { label: 'Date Between Customers Balance', value: 'DateBetweenCustomersBalance'},
            { label: 'Date Between Sales and Collections', value: 'DateBetweenSalesAndCollections' },
            { label: 'Date Between Customer Ledger', value: 'DateBetweenCustomerRegister' },
            { label: 'Date Between Memo Wise Sales', value: 'DateBetweenMemoWise' },
            { label: 'Date Between Product Wise Sales', value: 'DateBetweenProductSaleSummary' },
            { label: 'Date Between Customers Balance Summary', value: 'DateBetweenCustomersBalanceSummary' },
            { label: '7', value: '7' , isDisabled: true}
        ];

        $http({
            url: "/api/Customer/CustomerDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.customerList = data;
            //console.log(data);
        });
        $scope.changeSelectCustomerName = function (item) {
            $scope.reportOption.customerAutoComplite = item;
        };
        //$scope.searchTerm;
        //$scope.clearSearchTerm = function () {
        //    $scope.searchTerm = '';
        //};

        //angular.element('#searchTerm').on('keydown', function (ev) {
        //    ev.stopPropagation();
        //});





        // Show Report Button
        $scope.showReport = function () {
            var selectedReport = $scope.data.group2;

            if (selectedReport == "DateBetweenCustomersBalance") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.fromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.toDate, "yyyy-MM-dd");
                var ledgerIds = [];
                //if ($scope.supplierListMultiSelect.length > 0) {
                //    angular.forEach($scope.supplierListMultiSelect, function (item) {
                //        ledgerIds.push(item.id);
                //        console.log(item.id);
                //    })
                //}

                $http({
                    method: "GET",
                    url: "/SalesReport/ShowClosingBalanceRptInNewWin",
                    headers: authHeaders,
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
            } else if (selectedReport == "DateBetweenCustomersBalanceSummary") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.fromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.toDate, "yyyy-MM-dd");
                var ledgerIds = [];
                //if ($scope.supplierListMultiSelect.length > 0) {
                //    angular.forEach($scope.supplierListMultiSelect, function (item) {
                //        ledgerIds.push(item.id);
                //        console.log(item.id);
                //    })
                //}

                $http({
                    method: "GET",
                    url: "/SalesReport/ShowClosingBalanceSummaryRptInNewWin",
                    headers: authHeaders,
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
            }
            else if (selectedReport == "DateBetweenSalesAndCollections") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.fromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.toDate, "yyyy-MM-dd");
                var ledgerIds = [];
                //if ($scope.supplierListMultiSelect.length > 0) {
                //    angular.forEach($scope.supplierListMultiSelect, function (item) {
                //        ledgerIds.push(item.id);
                //        console.log(item.id);
                //    })
                //}

                $http({
                    method: "GET",
                    url: "/SalesReport/ShowSaleCollectionRptInNewWin",
                    headers: authHeaders,
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
            } else if (selectedReport == "DateBetweenCustomerRegister") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.fromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.toDate, "yyyy-MM-dd");
                var customerId = 0;
                if ($scope.reportOption.customerAutoComplite) {
                    customerId = $scope.reportOption.customerAutoComplite.CustomerId;
                } else {
                    alert('Select Customer Name');
                    return false;
                }

                $http({
                    method: "GET",
                    url: "/SalesReport/ShowCustomerLedgerRptInNewWin",
                    headers: authHeaders,
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        LedgerIds: customerId,
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
            } else if (selectedReport == "DateBetweenMemoWise") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.fromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.toDate, "yyyy-MM-dd");
                var ledgerIds = [];
                //if ($scope.supplierListMultiSelect.length > 0) {
                //    angular.forEach($scope.supplierListMultiSelect, function (item) {
                //        ledgerIds.push(item.id);
                //        console.log(item.id);
                //    })
                //}

                $http({
                    method: "GET",
                    url: "/SalesReport/ShowMemoWiseRptInNewWin",
                    headers: authHeaders,
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
            } else if (selectedReport == "DateBetweenProductSaleSummary") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.fromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.toDate, "yyyy-MM-dd");
                var ledgerIds = [];
                //if ($scope.supplierListMultiSelect.length > 0) {
                //    angular.forEach($scope.supplierListMultiSelect, function (item) {
                //        ledgerIds.push(item.id);
                //        console.log(item.id);
                //    })
                //}

                $http({
                    method: "GET",
                    url: "/SalesReport/ShowProductWiseRptInNewWin",
                    headers: authHeaders,
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
            }
            //End Report
        };




        $scope.printMemo = function (printSectionId) {
            var innerContents = document.getElementById(printSectionId).innerHTML;
            var popupWinindow = window.open('', '_blank', 'width=900,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWinindow.document.open();
            popupWinindow.document.write('<html><head><link rel="stylesheet" type="text/css" href="http://books.pakizagroup.com/Content/Site.css" /></head><body onload="window.print()">' + innerContents + '</html>');
            popupWinindow.document.close();
        };

        //Toster Settings
        var last = {
            bottom: true,
            top: false,
            left: false,
            right: true
        };

        $scope.toastPosition = angular.extend({}, last);

        $scope.getToastPosition = function () {
            sanitizePosition();

            return Object.keys($scope.toastPosition)
                .filter(function (pos) { return $scope.toastPosition[pos]; })
                .join(' ');
        };

        function sanitizePosition() {
            var current = $scope.toastPosition;

            if (current.bottom && last.top) current.top = false;
            if (current.top && last.bottom) current.bottom = false;
            if (current.right && last.left) current.left = false;
            if (current.left && last.right) current.right = false;

            last = angular.extend({}, current);
        }
        $scope.showSimpleToast = function () {
            var pinTo = $scope.getToastPosition();
            //$("#overlay").show();
            $mdToast.show(
                $mdToast.simple()
                    .textContent('Report Created!')
                    .position(pinTo)
                    .hideDelay(3000)
            );
        };
        $scope.showActionToast = function () {
            var pinTo = $scope.getToastPosition();
            var toast = $mdToast.simple()
                .textContent('Marked as read')
                .action('UNDO')
                .highlightAction(true)
                .highlightClass('md-accent')// Accent is used by default, this just demonstrates the usage.
                .position(pinTo);

            $mdToast.show(toast).then(function (response) {
                if (response === 'ok') {
                    alert('You clicked the \'UNDO\' action.');
                }
            });
        };
        //End toster
        //$scope.loading = false;
    }])