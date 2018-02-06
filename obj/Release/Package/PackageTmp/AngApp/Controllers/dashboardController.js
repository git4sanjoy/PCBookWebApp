var app = angular.module('PCBookWebApp');
app.controller('dashboardController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter, dataservice) {
        //$scope.message = "Dashboard Angular JS";
        $scope.loading = false;

        $scope.class = "overlay";
        $scope.changeClass = function () {
            if ($scope.class === "overlay")
                $scope.class = "";
            else
                $scope.class = "overlay";
        };

        $scope.userName = sessionStorage.getItem('userName');

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

            if ($scope.userRole == "Accounts Manager" || $scope.userRole == "Accounts") {

            }  else if ($scope.userRole == "Show Room Manager" || $scope.userRole == "Show Room Sales") {
                
            } else if ($scope.userRole == "Admin") {

            } else if ($scope.userRole == "GM") {

            }

        });
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
        //$scope.searchPayment.ToDate = new Date();
        // End DatePicker
        // *******************************************
        $http({
            url: "/api/Customer/GetCustomerBalance",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.customerClosingBalances = data;
            //console.log(data);
        });
        
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

        $scope.monthDdl = { id: month, name: monthsList[month_value] };
        $scope.monthDdlMonthlySales = { id: month, name: monthsList[month_value] };

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

        //// Date wise Sales in GoogleMap
        $http({
            url: "/api/MemoMasters/GetDateSalesInMap/" + $filter('date')(current_date, "yyyy-MM-dd") + "/" + 0,
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.dateSalesinMapList = data;
            //console.log(data);
        });
        $scope.dateSaleInGoogleMap = function () {
            var fd = $filter('date')($scope.saleDate, "yyyy-MM-dd");
            $http({
                url: "/api/MemoMasters/GetDateSalesInMap/"+fd+"/"+0,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.dateSalesinMapList = data;
                //console.log(data);
            });
        };
        

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
        $scope.salesManList = [];
        $http({
            method: 'GET',
            url: '/api/SalesMen/GetDropDownList',
            contentType: "application/json; charset=utf-8",
            headers: authHeaders,
            dataType: 'json'
        }).success(function (data) {
            $scope.salesManList = data;
        });
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


        $scope.printToCart = function (printSectionId) {
            var innerContents = document.getElementById(printSectionId).innerHTML;
            var popupWinindow = window.open('', '_blank', 'width=900,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWinindow.document.open();
            popupWinindow.document.write('<html><head><link rel="stylesheet" type="text/css" href="http://cotton.pakizagroup.com/Content/site.css" /></head><body onload="window.print()">' + innerContents + '</html>');
            popupWinindow.document.close();
        };




}])