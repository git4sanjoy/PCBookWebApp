var app = angular.module('PCBookWebApp');
app.controller('SearchController', ["$scope", "$http", "$filter", "$timeout", "$mdDialog", function ($scope, $http, $filter, $timeout, $mdDialog) {
    // $routeParams used for get query string value
    $scope.Message = "This is Search Controller";
    $scope.loading = true;
    $scope.class = "overlay";
    $scope.changeClass = function () {
        if ($scope.class === "overlay")
            $scope.class = "";
        else
            $scope.class = "overlay";
    };

    $scope.clientMessage = true;
    $scope.serverMessage = true;
    $scope.messageType = "";
    $scope.message = "";

    var accesstoken = sessionStorage.getItem('accessToken');
    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }
    $scope.pageSize = 25;
    $scope.currentPage = 1;

    $scope.checkList = [];

    // For 3 DatePicker
    $scope.FromDate = new Date();
    $scope.ToDate = new Date();
    // grab today and inject into field

    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.opened = true;
    };

    $scope.fromDatePickerIsOpen = false;
    $scope.toDatePickerIsOpen = false;


    $scope.FromDatePickerOpen = function () {
        this.fromDatePickerIsOpen = true;
    };
    $scope.ToDatePickerOpen = function () {
        this.toDatePickerIsOpen = true;
    };

    $scope.sort = function (keyname) {
        $scope.sortKey = keyname;   //set the sortKey to the param passed
        $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    };

    // End DatePicker
    $scope.vouchrTypeList = [];
    //$http({
    //    url: "/api/VoucherTypes/GetDropDownList",
    //    method: "GET",
    //    headers: authHeaders
    //}).success(function (data) {
    //    $scope.vouchrTypeList = data;
    //});

    $scope.loadVoucherTypeList = function () {
        // Use timeout to simulate a 650ms request.
        return $timeout(function () {

            $http({
                url: "/api/VoucherTypes/GetDropDownList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.vouchrTypeList = data;
            });

        }, 650);
    };

    $scope.searchVoucher = function (ev) {
        var vouchrTypeId = 0;
        var vouchrNo = "";

        if ($scope.vouchrType) {
            vouchrTypeId = $scope.vouchrType.VoucherTypeId;
        }
        else {
            $mdDialog.show(
                $mdDialog.alert()
                    .parent(angular.element(document.querySelector('#popupContainer')))
                    .clickOutsideToClose(true)
                    .title('Search Voucher')
                    .textContent('Select Voucher Type')
                    .ariaLabel('Alert Dialog Demo')
                    .ok('Ok!')
                    .targetEvent(ev)
            );
            //angular.element('#vouchrType').focus();
            element[1].focus(); 
            return false;
        }
        if ($scope.voucherNo){
            vouchrNo = $scope.voucherNo;
        } else {
            $mdDialog.show(
                $mdDialog.alert()
                    .parent(angular.element(document.querySelector('#popupContainer')))
                    .clickOutsideToClose(true)
                    .title('Search Voucher')
                    .textContent('Input Voucher No')
                    .ariaLabel('Alert Dialog Demo')
                    .ok('Ok!')
                    .targetEvent(ev)
            );
            //angular.element('#voucherNo').focus();
            element[2].focus(); 
            return false;
        }

        $http({
            url: "/api/Vouchers/GetVoucherById/" + vouchrTypeId + '/' + vouchrNo,
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            
            if (data == "No Record Found") {
                $mdDialog.show(
                    $mdDialog.alert()
                        .parent(angular.element(document.querySelector('#popupContainer')))
                        .clickOutsideToClose(true)
                        .title('Search Voucher')
                        .textContent('No Record Found')
                        .ariaLabel('Alert Dialog Demo')
                        .ok('Ok!')
                        .targetEvent(ev)
                );
                $scope.aVoucher = {};
                angular.element('#voucherNo').focus();
                $scope.voucherTable = true;
                $scope.bookDataTable = true;
                return false;
            } else {
                $scope.aVoucher = data;
                $scope.bookDataTable = true;
                $scope.voucherTable = false;
            }
            //console.log(data);
        });

    };
    $scope.totalDr = function () {
        var _totalDr = 0;
        angular.forEach($scope.aVoucher, function (item) {
            _totalDr += item.DrAmount;
        })
        return _totalDr
    }

    $scope.totalCr = function () {
        var _totalCr = 0;
        angular.forEach($scope.aVoucher, function (item) {
            _totalCr += item.CrAmount;
        })
        return _totalCr;
    }

    $scope.deleteVoucher = function (id, ev) {

        var confirm = $mdDialog.confirm()
            .title('Delete your voucher?')
            .textContent('Would you like to delete your voucher.')
            .ariaLabel('Lucky day')
            .targetEvent(ev)
            .ok('Yes!')
            .cancel('Cancel');

        $mdDialog.show(confirm).then(function () {
            //$scope.status = 'You decided to get delete.';
            $http({
                url: "/api/Vouchers/" + id,
                method: "DELETE",
                headers: authHeaders
            }).success(function (data) {
                $scope.aVoucher = {};
                $scope.message = "Successfully Voucher Deleted.";
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                $scope.aVoucher = {};
                $scope.voucherTable = true;
            }).error(function (data) {
                $scope.message = "An error has occured while deleting a voucher! " + data;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }, function () {
            //$scope.status = 'You decided to not delete.';
        });
        
    };

    $scope.searchClear = function () {
        $scope.voucherNo = '';
        $scope.vouchrType = {};
        $scope.voucherDate = '';
        $scope.aVoucher = '';
        $scope.bookData = '';
        $scope.voucherTable = true;
        $scope.bookDataTable = true;
    };
    $scope.clearSearchText = function () {
        $scope.searchText = '';
    };
    $scope.showBankPayment = function (aItem) {
        $scope.selectedBankLedger = '';
        $scope.showModal = true;
        //console.log(aItem);
        var acc = aItem.LedgerName;
        var iDate = aItem.VoucherDate;
        var amount = aItem.DrAmount;
        var voucherTypeName = aItem.VoucherTypeName;
        $scope.vType = voucherTypeName;
        if (aItem.VoucherTypeName === "Bank Receive") {
            $http({
                url: "/api/CheckReceives/" + aItem.VoucherDetailId,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.accNo = acc;
                $scope.issueDate = iDate;
                $scope.amountCr = amount;
                $scope.selectedBankLedger = data.checkReceive;
            }).error(function (data) {

            });
        } else if (aItem.VoucherTypeName === "Bank Payment"){
            $http({
                url: "/api/Check/" + aItem.VoucherDetailId,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.amountCr = 0;
                $scope.selectedBankLedger = data.check;
            }).error(function (data) {

            });
        }       
        //console.log($scope.selectedBankLedger);
    };
    ///Dailog Method
    $scope.status = '  ';
    $scope.customFullscreen = false;

    $scope.showAlert = function (ev) {
        // Appending dialog to document.body to cover sidenav in docs app
        // Modal dialogs should fully cover application
        // to prevent interaction outside of dialog
        $mdDialog.show(
            $mdDialog.alert()
                .parent(angular.element(document.querySelector('#popupContainer')))
                .clickOutsideToClose(true)
                .title('This is an alert title')
                .textContent('You can specify some description text in here.')
                .ariaLabel('Alert Dialog Demo')
                .ok('Got it!')
                .targetEvent(ev)
        );
    };


    $scope.showConfirm = function (ev) {
        // Appending dialog to document.body to cover sidenav in docs app
        var confirm = $mdDialog.confirm()
            .title('Would you like to delete your debt?')
            .textContent('All of the banks have agreed to forgive you your debts.')
            .ariaLabel('Lucky day')
            .targetEvent(ev)
            .ok('Please do it!')
            .cancel('Sounds like a scam');

        $mdDialog.show(confirm).then(function () {
            $scope.status = 'You decided to get rid of your debt.';
        }, function () {
            $scope.status = 'You decided to keep your debt.';
        });
    };

    $scope.letters = ['A', 'B', 'C'];
    $scope.$watch('letters', function (newValue, oldValue, scope) {
        //Do anything with $scope.letters
    }, true);

    //$scope.voucherDate = new Date();
    $scope.$watch('voucherDate', function (newVal, oldVal, ev) {
        //alert(newVal)
        if (angular.isUndefined(newVal)) {
            
        } else {           
            var fd = $filter('date')(newVal, "yyyy-MM-dd");
            //console.log(fd);
            $http({
                url: "/api/Vouchers/GetVoucherByDate/" + fd,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {

                if (data == "No Record Found") {
                    $mdDialog.show(
                        $mdDialog.alert()
                            .parent(angular.element(document.querySelector('#popupContainer')))
                            .clickOutsideToClose(true)
                            .title('Search Voucher')
                            .textContent('No Record Found')
                            .ariaLabel('Alert Dialog Demo')
                            .ok('Ok!')
                            .targetEvent(ev)
                    );
                    $scope.bookData = {};
                    angular.element('#voucherNo').focus();
                    $scope.bookDataTable = true;
                    $scope.voucherTable = true;
                    return false;
                } else {
                    $scope.bookData = data;
                    $scope.voucherTable = true;
                    $scope.bookDataTable = false;
                }
                //console.log(data);
            });
        }
        
    }, true);
    
    $scope.voucherTable = true;
    $scope.bookDataTable = true;
    $scope.loading = false;
}])