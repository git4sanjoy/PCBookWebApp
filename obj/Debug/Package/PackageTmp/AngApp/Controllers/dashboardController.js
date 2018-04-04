var app = angular.module('PCBookWebApp');
app.controller('dashboardController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ( $scope, $location, $http, $timeout, $filter) {

        //$scope.message = "Dashboard Angular JS";
        $scope.loading = false;

        $scope.class = "overlay";
        $scope.changeClass = function () {
            if ($scope.class === "overlay")
                $scope.class = "";
            else
                $scope.class = "overlay";
        };
        $scope.pageSize = 200;
        $scope.currentPage = 1;
        $scope.searchCustomerLedger = {};
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
        $scope.saleDate = new Date();
        //$scope.searchCustomerLedger.ToDate = new Date();
        $scope.invoiceDatePickerIsOpen = false;
        $scope.InvoiceDatePickerOpen = function () {
            this.invoiceDatePickerIsOpen = true;
        };
        $scope.honourDatePickerIsOpen = false;
        $scope.HonourDatePickerOpen = function () {
            this.honourDatePickerIsOpen = true;
        };



        // End DatePicker
        $scope.userName = sessionStorage.getItem('userName');
        var inputYears = [];
        var startYear = 2005;
        var currentTime = new Date()
        // returns the month (from 0 to 11)
        month_value = currentTime.getMonth();
        var month = currentTime.getMonth() + 1
        var day = currentTime.getDate()
        var year = currentTime.getFullYear()
        var endYear = year;
        for (var i = startYear; i <= endYear; i++) {
            inputYears.push(i);
        }

        $scope.yearDdlMonthlySales = endYear;
        $scope.yearList = inputYears;
        $scope.yearListSelectedData = endYear;
        $scope.yearDdlMonthlyProductSales = endYear;
        $scope.dashbordYearListSelectedData = endYear;

        //Search List
        var monthsList = new Array(12);
        monthsList[0] = "January";
        monthsList[1] = "February";
        monthsList[2] = "March";
        monthsList[3] = "April";
        monthsList[4] = "May";
        monthsList[5] = "June";
        monthsList[6] = "July";
        monthsList[7] = "August";
        monthsList[8] = "September";
        monthsList[9] = "October";
        monthsList[10] = "November";
        monthsList[11] = "December";

        var current_date = new Date();
        month_value = current_date.getMonth();
        day_value = current_date.getDate();
        year_value = current_date.getFullYear();
        $scope.today = new Date();
        $scope.monthDdl = { id: month, name: monthsList[month_value] };
        $scope.monthDdlMonthlySales = { id: month, name: monthsList[month_value] };
        $scope.monthDdlMonthlyProductSales = { id: month, name: monthsList[month_value] };
        $scope.dashbordMonth = monthsList[month_value];
        //Display List
        $scope.months = [
            { id: 1, name: 'January' },
            { id: 2, name: 'February' },
            { id: 3, name: 'March' },
            { id: 4, name: 'April' },
            { id: 5, name: 'May' },
            { id: 6, name: 'June' },
            { id: 7, name: 'July' },
            { id: 8, name: 'August' },
            { id: 9, name: 'September' },
            { id: 10, name: 'October' },
            { id: 11, name: 'November' },
            { id: 12, name: 'December' }
        ];
        //loadDatas();

        //function loadDatas() {
        //    var promise = dataservice.get();
        //    promise.then(function (resp) {
        //        $scope.data = resp.data;
        //        //console.log(resp.data);
        //        //$scope.message = "Call Completed Successfully";
        //    }, function (err) {
        //        $scope.message = "Error!!! " + err.status
        //    });
        //};

        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }
        $scope.sort = function (keyname) {
            $scope.sortKey = keyname;   //set the sortKey to the param passed
            $scope.reverse = !$scope.reverse; //if true make it false and vice versa
        }

        $http({
            url: "/api/Projects/AppDetails",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.appDetails = data;
            //console.log(data);
            });

        //$scope.customerList = {};
        $http({
            url: "/api/Customer/CustomerDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.customerList = data;
            //console.log(data);
        });

        // Loged in user detail
        $scope.userRole = "";
        $scope.userDetails = [];
        $http({
            url: "/api/Projects/LogedInUserRole",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.userDetails = data;
            $scope.userRole = data.role[0];

            if ($scope.userRole == "Accounts Manager") {

            } else if ($scope.userRole == "Accounts" ) {
                loadAccOfficerDashbord()
            } else if ($scope.userRole == "Unit Manager") {
                loadUnitMagerDashbord()
            } else if ($scope.userRole == "Show Room Manager") {
                
            } else if ($scope.userRole == "Zone Manager") {
                loadZoneManagerDashbord()
            }  else if ($scope.userRole == "Show Room Sales") {
                loadShowRoomOfficerDashbord()
            } else if ($scope.userRole == "Process Manager") {

            } else if ($scope.userRole == "Process Officer") {

            } else if ($scope.userRole == "Design Manager") {
                loadDesignManagerDashbord()
            } else if ($scope.userRole == "Admin") {

            } else if ($scope.userRole == "GM") {

            } else if ($scope.userRole == "MD") {

            }

        });

        // *******************************************
        $scope.loadShowRoomManagerDashbord = loadShowRoomManagerDashbord;
        $scope.loadShowRoomOfficerDashbord = loadShowRoomOfficerDashbord;
        $scope.loadUnitMagerDashbord = loadUnitMagerDashbord;
        $scope.loadZoneManagerDashbord = loadZoneManagerDashbord;
        $scope.loadAccOfficerDashbord = loadAccOfficerDashbord;

        function loadShowRoomManagerDashbord() {

        };
        function loadZoneManagerDashbord() {
            $http({
                url: "/api/MemoMasters/GetDateSalesInMap/" + $filter('date')(current_date, "yyyy-MM-dd") + "/" + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.dateSalesinMapList = data;
                //console.log(data);
            });
            $scope.zoneWiseCustomerListForZoneManager = [];
            $http({
                method: 'Get',
                headers: authHeaders,
                url: '/api/Customer/ZoneWiseCustomerClosingBalanceList'
            }).success(function (data, status, headers, config) {
                $scope.zoneWiseCustomerListForZoneManager = data;
            }).error(function (data, status, headers, config) {
                $scope.message = 'Unexpected Error';
            });
        };
        function loadShowRoomOfficerDashbord() {
            
            //// Date wise Sales in GoogleMap
            $http({
                url: "/api/MemoMasters/GetDateSalesInMap/" + $filter('date')(current_date, "yyyy-MM-dd") + "/" + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.dateSalesinMapList = data;
                //console.log(data);
            });

            //// Sales Managers Chart View 
            $http({
                url: "/api/MemoMasters/GetSalesManWiseMonthlySale/" + endYear + '/' + month + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.salesManWiseMonthlySale = data;
                //console.log(data);
                var daysList = [];
                var expectedProductionList = [];
                var actualProductionList = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].SalesManName;
                    var targetProduction = data[i].TotalSaleTaka;
                    daysList.push(labelName);
                    expectedProductionList.push(targetProduction);
                    var actualProduction = data[i].TotalCollectionTaka;
                    actualProductionList.push(actualProduction);
                }

                $scope.labelsBar = daysList;
                $scope.seriesBar = ['SALE', 'COLLECTION'];
                $scope.dataBar = [
                    expectedProductionList,
                    actualProductionList
                ];
            });

            $http({
                url: "/api/MemoMasters/GetSalesManWiseYearlySale/" + endYear + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.salesManWiseYearlySale = data;
                //console.log(data);
                var daysListYear = [];
                var sales = [];
                var collections = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].SalesManName;
                    var sale = data[i].TotalSaleTaka;
                    daysListYear.push(labelName);
                    sales.push(sale);
                    var collection = data[i].TotalCollectionTaka;
                    collections.push(collection);
                }

                $scope.labelsBarYear = daysListYear;
                $scope.seriesBarYear = ['SALE', 'COLLECTION'];
                $scope.dataBarYear = [
                    sales,
                    collections
                ];
            });
            
            $http({
                method: 'GET',
                url: '/api/SalesMen/GetDropDownList',
                contentType: "application/json; charset=utf-8",
                headers: authHeaders,
                dataType: 'json'
            }).success(function (data) {
                $scope.salesManList = data;
            });
            $http({
                url: "/api/MemoMasters/GetProductWiseMonthlySale/" + endYear + '/' + month + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.productWiseMonthlySale = data;
                ////Collection Bar Chart

                var daysList = [];
                var expectedProductionList = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].ProductName;
                    var targetProduction = data[i].TotalQuantity;
                    daysList.push(labelName);
                    expectedProductionList.push(targetProduction);
                }

                $scope.labelsProductBar = daysList;
                $scope.seriesProductBar = ['QUANTITY'];
                $scope.dataProductBar = [
                    expectedProductionList
                ];
            });
            $http({
                url: "/api/MemoMasters/GetProductsWiseYearlySale/" + endYear + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.salesProductWiseYearly = data;
                ////console.log(data);
                var daysListYear = [];
                var sales = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].ProductName;
                    var sale = data[i].TotalQuantity;
                    daysListYear.push(labelName);
                    sales.push(sale);
                }

                $scope.labelsProductBarYear = daysListYear;
                $scope.seriesProductBarYear = ['QUANTITY'];
                $scope.dataProductBarYear = [
                    sales
                ];
            });
            $http({
                url: "/api/Customer/GetCustomerBalance",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.customerClosingBalances = data;
                //console.log(data);
            });
        };

        function loadGmDashbord() {

        };
        function loadMdDashbord() {

        };
        function loadProcessOfficerDashbord() {

        };
        function loadProcessManagerDashbord() {

        };

        function loadDesignManagerDashbord() {
            $scope.designList = [];
            $http({
                method: 'Get',
                url: '/api/Deals/DealsLists'
            }).success(function (data, status, headers, config) {
                $scope.designList = data;
            }).error(function (data, status, headers, config) {
                $scope.message = 'Unexpected Error';
            });
        };
        



        function loadUnitMagerDashbord() {
            $scope.zoneWiseCustomerList = [];
            $http({
                method: 'Get',
                headers: authHeaders,
                url: '/api/Customer/ZoneCustomerList'
            }).success(function (data, status, headers, config) {
                $scope.customerList = data.customerList;
                //$scope.linkQListDateBeteen = data.linkQListDateBeteen;
                //$scope.zoneWiseCustomerList = data.zoneWiseCustomerList;
                //$scope.divisionWiseCustomerList = data.divisionWiseCustomerList;
                //$scope.zoneManagerWiseCustomerList = data.zoneManagerWiseCustomerList;
            }).error(function (data, status, headers, config) {
                $scope.message = 'Unexpected Error';
            });
        };

        function loadAccOfficerDashbord() {
            $scope.pendingCheckList = [];
            $http({
                method: 'Get',
                headers: authHeaders,
                url: '/api/Check/PendingCheckList'
            }).success(function (data, status, headers, config) {
                $scope.pendingCheckList = data;
            }).error(function (data, status, headers, config) {
                $scope.message = 'Unexpected Error';
            });
        };

        $scope.checkHonour = function (index, aCheck) {
            //console.log(aCheck)
            confirmPrompt = confirm('Are you sure you want to Honour the check: ' + aCheck.CheckNumber +' ?');
            if (confirmPrompt) {
                $http({                    
                    url: '/api/Vouchers/RateMgrUpdateRate/' + aCheck.VoucherId,
                    method: "PUT",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.pendingCheckList.splice(index, 1);
                }).error(function (error) {

                });
            }
        };


        ///******************************************
        $scope.salesManList = [];
        $scope.changedYearDdlMonthlySales = function () {
            //console.log($scope.yearDdlMonthlySales);
            //console.log($scope.monthDdlMonthlySales.id);
            $http({
                url: "/api/MemoMasters/GetSalesManWiseMonthlySale/" + $scope.yearDdlMonthlySales + '/' + $scope.monthDdlMonthlySales.id + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.salesManWiseMonthlySale = data;
                //Collection Bar Chart

                var daysList = [];
                var expectedProductionList = [];
                var actualProductionList = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].SalesManName;
                    var targetProduction = data[i].TotalSaleTaka;
                    daysList.push(labelName);
                    expectedProductionList.push(targetProduction);
                    var actualProduction = data.ChartObj[i].TotalCollectionTaka;
                    actualProductionList.push(actualProduction);
                }

                $scope.labelsBar = daysList;
                $scope.seriesBar = ['USD', 'BDT'];
                $scope.dataBar = [
                    expectedProductionList,
                    actualProductionList
                ];
            });

        };
        $scope.changedMonthDdlMonthlySales = function () {
            $http({
                url: "/api/MemoMasters/GetSalesManWiseMonthlySale/" + $scope.yearDdlMonthlySales + '/' + $scope.monthDdlMonthlySales.id + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.salesManWiseMonthlySale = data;
                //Collection Bar Chart

                var daysList = [];
                var expectedProductionList = [];
                var actualProductionList = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].SalesManName;
                    var targetProduction = data[i].TotalSaleTaka;
                    daysList.push(labelName);
                    expectedProductionList.push(targetProduction);
                    var actualProduction = data[i].TotalCollectionTaka;
                    actualProductionList.push(actualProduction);
                }

                $scope.labelsBar = daysList;
                $scope.seriesBar = ['SALE', 'COLLECTION'];
                $scope.dataBar = [
                    expectedProductionList,
                    actualProductionList
                ];
            });
        };
        $scope.changedYearDdlYearlySales = function () {
            //console.log($scope.yearDdlYearlySales);
            $http({
                url: "/api/MemoMasters/GetSalesManWiseYearlySale/" + $scope.yearDdlYearlySales + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.salesManWiseYearlySale = data;
                //console.log(data);
                var daysListYear = [];
                var sales = [];
                var collections = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].SalesManName;
                    var sale = data[i].TotalSaleTaka;
                    daysListYear.push(labelName);
                    sales.push(sale);
                    var collection = data[i].TotalCollectionTaka;
                    collections.push(collection);
                }

                $scope.labelsBarYear = daysListYear;
                $scope.seriesBarYear = ['SALE', 'COLLECTION'];
                $scope.dataBarYear = [
                    sales,
                    collections
                ];
            });

        };
        ///**********************************************************************
        /// Product wise Yearly Sales summary
        $scope.changedYearDdlYearlyProductsSales = function () {
            //console.log($scope.yearDdlYearlyProductsSales);
            $http({
                url: "/api/MemoMasters/GetProductsWiseYearlySale/" + $scope.yearDdlYearlyProductsSales + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.salesProductWiseYearly = data;
                //Collection Bar Chart
                var daysListYear = [];
                var sales = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].ProductName;
                    var sale = data[i].TotalQuantity;
                    daysListYear.push(labelName);
                    sales.push(sale);
                }

                $scope.labelsProductBarYear = daysListYear;
                $scope.seriesProductBarYear = ['QUANTITY'];
                $scope.dataProductBarYear = [
                    sales
                ];
            });
        };
        $scope.changedMonthDdlMonthlyProductsSales = function () {
            //console.log($scope.yearDdlMonthlyProductSales);
            $http({
                url: "/api/MemoMasters/GetProductWiseMonthlySale/" + $scope.yearDdlMonthlyProductSales + '/' + $scope.monthDdlMonthlyProductSales.id + '/' + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.productWiseMonthlySale = data;
                //Collection Bar Chart

                var daysList = [];
                var expectedProductionList = [];

                for (var i = 0; i < data.length; i++) {
                    var labelName = data[i].ProductName;
                    var targetProduction = data[i].TotalQuantity;
                    daysList.push(labelName);
                    expectedProductionList.push(targetProduction);
                }

                $scope.labelsProductBar = daysList;
                $scope.seriesProductBar = ['QUANTITY'];
                $scope.dataProductBar = [
                    expectedProductionList
                ];
            });

        };
        $scope.dateSaleInGoogleMap = function () {
            var fd = $filter('date')($scope.saleDate, "yyyy-MM-dd");
            $http({
                url: "/api/MemoMasters/GetDateSalesInMap//" + fd + "/" + 0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.dateSalesinMapList = data;
                //console.log(data);
            });
        };
        ///***************************************

        $scope.printToCart = function (printSectionId) {
            var innerContents = document.getElementById(printSectionId).innerHTML;
            var popupWinindow = window.open('', '_blank', 'width=900,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWinindow.document.open();
            popupWinindow.document.write('<html><head><link rel="stylesheet" type="text/css" href="http://cotton.pakizagroup.com/Content/site.css" /></head><body onload="window.print()">' + innerContents + '</html>');
            popupWinindow.document.close();
        };

        $scope.exportToExcel = function (tableId) { // ex: '#my-table'
            var exportHref = Excel.tableToExcel(tableId, 'WireWorkbenchDataExport');
            $timeout(function () { location.href = exportHref; }, 100); // trigger download
        };
        $scope.colors = ['#45b7cd', '#ff6384', '#ff8e72'];
        $scope.datasetOverride = [
            {
                label: "Sales Bar",
                borderWidth: 1,
                type: 'bar'
            },
            {
                label: "Collection Line",
                borderWidth: 3,
                hoverBackgroundColor: "rgba(255,99,132,0.4)",
                hoverBorderColor: "rgba(255,99,132,1)",
                type: 'line'
            }
        ];
        $scope.changeSelectCustomerName = function (item) {
            $scope.searchCustomerLedger.customerAutoComplite = item;
        };
        $scope.GetCustomerDetailById = function (customer) {
            var customerId = 0;
            $scope.Address = "";
            if (customer) {
                $http({
                    url: "/api/Customer/GetSingleCustomer/" + customer.CustomerId,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.Address = data[0].Address;
                    $scope.DistrictName = data[0].DistrictName;
                    $scope.Image = data[0].Image;
                    $scope.BfAmount = data[0].TotalBf;
                    //$scope.BFDate = data[0].BFDate;
                    $scope.CreditLimit = data[0].CreditLimit;
                    $scope.ActualCredit = parseFloat(data[0].TotalBf) + (parseFloat(data[0].GrossSales) - parseFloat(data[0].MemoDiscount) + parseFloat(data[0].GatOther)) - parseFloat(data[0].TotalPayments) - parseFloat(data[0].TotalDiscounts);
                    $scope.TotalSale = data[0].GrossSales;
                    $scope.TotalCollection = data[0].TotalPayments;
                    $scope.TotalDiscount = data[0].TotalDiscounts;
                }).error(function (error) {
                    $scope.Address = '';
                    $scope.DistrictName = '';
                    $scope.Image = '';
                    $scope.BfAmount = 0;
                    $scope.BFDate = 0;
                    $scope.CreditLimit = 0;
                    $scope.ActualCredit = 0;
                    $scope.TotalSale = 0;
                    $scope.TotalCollection = 0;
                    $scope.TotalDiscount = 0;
                    $scope.message = 'Unable to get party info' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
        };

        $scope.submitSearchCustomerLedgerForm = function () {
            var customerId = 0;
            var customerName = "";
            var fd = null;
            var td = null;
            //$scope.customersLedger = [];
            $scope.submitted = true;
            if ($scope.searchCustomerLedgerForm.$valid) {

                fd = $filter('date')($scope.searchCustomerLedger.fromDate, "yyyy-MM-dd");
                td = $filter('date')($scope.searchCustomerLedger.toDate, "yyyy-MM-dd");
                if ($scope.searchCustomerLedger.customerAutoComplite) {
                    customerId = $scope.searchCustomerLedger.customerAutoComplite.CustomerId;
                    customerName = $scope.searchCustomerLedger.customerAutoComplite.CustomerName;
                }

                $http({
                    url: '/api/Customer/PartyLedger/' + fd + '/' + td + '/' + customerId,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.customerLedgers = data.paymentsList;
                    $scope.customerSaleLedgers = data.saleList;
                    $scope.categoryWiseGroupingSaleList = data.categoryGroupingList;
                    $scope.bfList = data.bfList;
                    $scope.opening = parseFloat(data.bfList[0].TotalBf) + (parseFloat(data.bfList[0].GrossSales) + parseFloat(data.bfList[0].GatOther) - parseFloat(data.bfList[0].MemoDiscount)) - parseFloat(data.bfList[0].TotalPayments) - parseFloat(data.bfList[0].TotalDiscounts);

                    

                }).error(function (error) {

                });

                ////if ($scope.data.PartyAccount == true) {
                //    $http({
                //        url: '/api/MemoMasters/GetMemoMastersSummary/' + fd + '/' + td + '/' + customerId,
                //        method: "GET",
                //        headers: authHeaders
                //    }).success(function (data) {
                //        if (data.length > 0) {
                //            $scope.customerSaleLedgers = data;
                //        }
                //    }).error(function (error) {
                //        $scope.message = 'Unable to get Payments info' + error.message;
                //        $scope.messageType = "warning";
                //        $scope.clientMessage = false;
                //        $timeout(function () { $scope.clientMessage = true; }, 5000);
                //    });
                ////} else {
                ////    $scope.customerSaleLedgers = {};
                ////}

                ////Get Memo DateBetween Category Wise Grouping List Data
                //$http({
                //    url: "/api/MemoMasters/GetMemoByDateBetweenForCategoryWiseGrouping/" + fd + '/' + td + '/' + customerId,
                //    method: "GET",
                //    headers: authHeaders
                //}).success(function (data) {
                //    if (data.length > 0) {
                //        $scope.categoryWiseGroupingSaleList = data;
                //        //console.log(data);
                //    }
                //});

                //$scope.searchPayment = {};
                $scope.submitted = false;
                //$scope.searchPayment.CreditLimit = 0;
                //$scope.searchPayment.TotalCredit = 0;
                //$scope.searchPayment.Address = "";
                $scope.searchCustomerLedgerForm.$setPristine();
                $scope.searchCustomerLedgerForm.$setUntouched();
                $scope.loading = false;
            } else {
                alert("Please  correct form errors!");
            }
        };
        $scope.totalSale = function (memos) {
            var total = 0;
            angular.forEach(memos, function (items) {
                //total += item.MemoDetailViews[i].Quantity * (item.MemoDetailViews[i].Rate - item.MemoDetailViews[i].Discount);
                angular.forEach(items.MemoDetailViews, function (item) {
                    total += parseFloat(item.Quantity) * (parseFloat(item.Rate) - parseFloat(item.Discount));
                })
            })
            return total;
        };
        $scope.GetGroupSumOfILineTotal = function (group) {
            var groupWiseSum = 0;
            for (var i in group) {
                groupWiseSum = groupWiseSum + (parseFloat(group[i].Quantity) * (parseFloat(group[i].Rate) - parseFloat(group[i].Discount)));
            }
            return groupWiseSum;
        };
        $scope.GetGroupSumOfILineQu = function (group) {
            var groupWiseSum = 0;
            for (var i in group) {
                groupWiseSum = groupWiseSum + parseFloat(group[i].Quantity);
            }
            return groupWiseSum;
        };
        $scope.showMemo = function (aMemo) {
            
            console.log(aMemo);
            $http({
                url: "/api/MemoMasters/" + aMemo.MemoMasterId ,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                if (data.length > 0) {
                    $scope.memos = data;
                    //console.log(data);
                    $scope.showModal = true;
                }
            });
            
        };
        $scope.clear = function () {
            $scope.searchCustomerLedger = '';
            $scope.Address = '';
            $scope.customerLedgers = '';
            $scope.categoryWiseGroupingSaleList = '';
            $scope.searchCustomerLedger = {};
            $scope.searchCustomerLedgerForm.$setPristine();
            $scope.searchCustomerLedgerForm.$setUntouched();
        };
        $scope.exportToExcel = function (tableId) { // ex: '#my-table'
            var exportHref = Excel.tableToExcel(tableId, 'WireWorkbenchDataExport');
            $timeout(function () { location.href = exportHref; }, 100); // trigger download
        };

        //Date between NetBalance
        $scope.DateBetweenNetBalance = function (recordsArray) {
            var total = 0;
            angular.forEach(recordsArray, function (item) {
                total += parseFloat(item.Opening) + parseFloat(item.TotalBf) + parseFloat(item.ActualSales) - parseFloat(item.TotalPayments) - parseFloat(item.TotalDiscounts);

            })
            return total;
        };
        $scope.customerNetBalance = function ( memos, collections) {
            var open = 0;
            if ($scope.opening) {
                open = $scope.opening;
            }
            var totalSale = 0;
            var totalSaleCol = 0;
            var totalDis = 0;
            angular.forEach(memos, function (item) {
                totalSale += parseFloat(item.NetMemoAmount);
            })
            angular.forEach(collections, function (i) {
                totalSaleCol += parseFloat(i.SCAmount);
                totalDis += parseFloat(i.SDiscount);
            })
            return parseFloat(open + totalSale - totalSaleCol - totalDis);
        };

}])