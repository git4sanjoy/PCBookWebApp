var app = angular.module('PCBookWebApp');
app.controller('ReportsController', ['$scope', '$http', '$filter', '$timeout',
    function ($scope, $http, $filter, $timeout) {

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";
        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }

        $scope.loading = true;

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 20;
        $scope.currentPage = 1;

        var inputYears = [];
        var startYear = 2016;
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

        $scope.yearList = inputYears;
        $scope.yearListSelectedData = endYear;


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

        $scope.reportArray = [];
        $scope.reportArray.push({ name: "DateBetweenBook", Text: "Date Between Book", isDefault: true });
        $scope.reportArray.push({ name: "DateBetweenLedger", Text: "Date Between Ledger", isDefault: false });
        $scope.reportArray.push({ name: "DateBetweenBookGroupWise", Text: "Date Between Book Group Wise", isDefault: false });
        

        $scope.reportOptionRadios = $scope.reportArray[0];
        $scope.reportOption = [];

        //**************Report Option*****************************
        $scope.printTypeArray = [];
        $scope.printTypeArray.push({ name: "Print", Text: "Print", isDefault: true });
        $scope.printTypeArray.push({ name: "Excel", Text: "Excel", isDefault: false });
        $scope.printOptionRadios = $scope.printTypeArray[0];
        $scope.printOption = [];
        //**********************************************************

        function conditionDivMakeFalse() {
            $scope.ledgerDiv = false;
            $scope.groupDiv = false;
            $scope.consumptionDiv = false
            $scope.dateDiv = true;
        };
        conditionDivMakeFalse();
        $scope.ledgerDiv = true;
        //
        $scope.updateControlStatus = function () {
            var selectedReport = $scope.reportOptionRadios.name;
            conditionDivMakeFalse();
            $scope.dateDiv = true;
            if (selectedReport == "DateBetweenBook") {
                $scope.ledgerDiv = true;
            } else if (selectedReport == "DateBetweenBookGroupWise") {
                $scope.groupDiv = true;
            } else if (selectedReport == "DateBetweenLedger") {
                $scope.ledgerDiv = true;
            }
        }
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
        //*******************************************
        // Ledger List Multi select
        $scope.ledgeListMultiSelect = [];
        $scope.ledgeListMultiSelectSettings = {
            scrollableHeight: '300px',
            scrollable: true,
            enableSearch: true,
        };
        $scope.ledgeListMultiSelectCustomTexts = { buttonDefaultText: '--Select Ledger Name--' };
        $scope.ledgerList = [];
        $http({
            url: "/api/Ledgers/LedgersMultiSelectList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.ledgerList = data;
            //console.log(data);
        });
        // Group List Multi select
        $scope.groupListMultiSelect = [];
        $scope.groupListMultiSelectSettings = {
            //externalIdProp: '',
            scrollableHeight: '300px',
            scrollable: true,
            enableSearch: true,
        };
        $scope.groupListMultiSelectCustomText = { buttonDefaultText: '---Select Group Name---' };
        $scope.groupList = [];
        $http({
            url: "/api/Groups/GroupListMultiSelect",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.groupList = data;
        });
        // End Multi select
        //*****************************************************



        // Show Report Button
        $scope.showReport = function () {
            var selectedReport = $scope.reportOptionRadios.name;

            if (selectedReport == "DateBetweenBook") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                if ($scope.ledgeListMultiSelect.length > 0) {
                    angular.forEach($scope.ledgeListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                    })
                }
                $http({
                    method: "GET",
                    url: "/Reports/ShowBookRptInNewWin",
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
            else if (selectedReport == "DateBetweenLedger") {
                var reportShowType = $scope.printOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var ledgerIds = [];
                if ($scope.ledgeListMultiSelect.length > 0) {
                    angular.forEach($scope.ledgeListMultiSelect, function (item) {
                        ledgerIds.push(item.id);
                    })
                }
                $http({
                    method: "GET",
                    url: "/Reports/ShowLedgerRptInNewWin",
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

            if (selectedReport == "DateBetweenMaterialBookSummary") {
                var selectedMaterialType = $scope.materialOptionRadios.name;
                var fd = $filter('date')($scope.reportOption.FromDate, "yyyy-MM-dd");
                var td = $filter('date')($scope.reportOption.ToDate, "yyyy-MM-dd");
                var productIds = [];

                if ($scope.mainMaterialListMultiSelect.length > 0 && selectedMaterialType == 'MainMaterial') {
                    angular.forEach($scope.mainMaterialListMultiSelect, function (item) {
                        productIds.push(item.id);
                    })
                } else if ($scope.subMaterialListMultiSelect.length > 0 && selectedMaterialType == 'SubMaterial') {
                    angular.forEach($scope.subMaterialListMultiSelect, function (item) {
                        productIds.push(item.id);
                    })
                } else if ($scope.productListMultiSelect.length > 0 && selectedMaterialType == 'Material') {
                    angular.forEach($scope.productListMultiSelect, function (item) {
                        productIds.push(item.id);
                    })
                }

                $http({
                    method: "GET",
                    url: "/Reports/ShowMaterialLedgerSummaryGenericRptInNewWin",
                    params: {
                        FromDate: fd,
                        ToDate: td,
                        ProductIds: productIds,
                        SelectedReportOption: selectedReport,
                        MaterialType: selectedMaterialType
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
    }]);