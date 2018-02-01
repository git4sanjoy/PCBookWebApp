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

            if ($scope.userRole == "Accounts Manager") {

            } else if ($scope.userRole == "Accounts") {

            } else if ($scope.userRole == "Show Room Manager") {

            } else if ($scope.userRole == "Show Room Sales") {

            } else if ($scope.userRole == "Admin") {

            } else if ($scope.userRole == "GM") {

            }

        });

        var inputYears = [];
        var startYear = 2008;
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





        $scope.printToCart = function (printSectionId) {
            var innerContents = document.getElementById(printSectionId).innerHTML;
            var popupWinindow = window.open('', '_blank', 'width=900,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWinindow.document.open();
            popupWinindow.document.write('<html><head><link rel="stylesheet" type="text/css" href="http://cotton.pakizagroup.com/Content/site.css" /></head><body onload="window.print()">' + innerContents + '</html>');
            popupWinindow.document.close();
        };




}])