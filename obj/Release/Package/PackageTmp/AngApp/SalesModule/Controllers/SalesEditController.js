var app = angular.module('PCBookWebApp');

app.controller('SalesEditController', ['$scope', '$location', '$http', '$timeout', '$filter',

    function ($scope, $location, $http, $timeout, $filter) {
        $scope.loading = true;

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 100;
        $scope.currentPage = 1;

        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        };
        $scope.invoice = [];
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
        $scope.invoiceDatePickerIsOpen = false;
        $scope.InvoiceDatePickerOpen = function () {
            this.invoiceDatePickerIsOpen = true;
        };
        $scope.honourDatePickerIsOpen = false;
        $scope.HonourDatePickerOpen = function () {
            this.honourDatePickerIsOpen = true;
        };
        $scope.memos = [];
        $scope.submitSearchForm = function () {
            $scope.submitted = true;
            $scope.memos = [];
            $scope.categoryWiseGroupingSaleList = {};
            if ($scope.searchForm.$valid) {

                if ($scope.memoMaster.FromDate && $scope.memoMaster.ToDate) {

                    var fd = $filter('date')($scope.memoMaster.FromDate, "yyyy-MM-dd");
                    var td = $filter('date')($scope.memoMaster.ToDate, "yyyy-MM-dd");
                    var customerId = 0;
                    $http({
                        url: "/api/MemoMasters/GetMemoByDate/" + fd + '/' + td,
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {

                        // Assign Memos Data
                        if (data.listOfMemo.length > 0) {
                            $scope.memos = data.listOfMemo;
                            //console.log(data);
                        } else {
                            $scope.memoMaster.memoNo = "";
                            $scope.memos = {};
                            alert("No Record Found");
                            angular.element('#memoNo').focus();
                        }

                    });
                }

            }
        };

        $scope.editInvoice = function (memo, index) {
            $scope.showModal = true;
            //console.log(memo);
            $scope.editObj = memo;
            $scope.editObjIndex = index;
            $scope.invoice.MemoMasterId = memo.MemoMasterId;
            $scope.invoice.invoiceDate = memo.MemoDate;
            $scope.invoice.MemoDiscount = memo.MemoDiscount;
            $scope.invoice.GatOther = memo.GatOther;
            $scope.invoice.MemoCost = memo.MemoCost;
        };
        $scope.updateMemo = function () {
            var fd = $filter('date')($scope.invoice.invoiceDate, "yyyy-MM-dd");
            var Id = $scope.invoice.MemoMasterId;
            var aMemoObj = {
                MemoMasterId: $scope.invoice.MemoMasterId,
                MemoDate: fd,
                MemoCost: $scope.invoice.MemoCost,
                GatOther: $scope.invoice.GatOther,
                MemoDiscount: $scope.invoice.MemoDiscount
            }
            var aMemoViewObj = {
                MemoMasterId: $scope.invoice.MemoMasterId,
                MemoDate: fd,
                MemoCost: $scope.invoice.MemoCost,
                GatOther: $scope.invoice.GatOther,
                MemoDiscount: $scope.invoice.MemoDiscount,
                CustomerName: $scope.editObj.CustomerName,
                DistrictName: $scope.editObj.DistrictName,
                MemoNo: $scope.editObj.MemoNo
            }
            $http({
                url: '/api/MemoMasters/UpdateMemoMaster/' + $scope.invoice.MemoMasterId,
                data: aMemoObj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                var editIndex = $scope.editObjIndex;
                //$.each($scope.memos, function (i) {
                //    if ($scope.memos[i].MemoMasterId === Id) {
                $scope.memos[editIndex] = aMemoViewObj;
                //        return false;
                //    }
                //});
                $scope.loading = true;
                $scope.message = "Successfully Memo Updated.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                $scope.editObjIndex = '';
                $scope.showModal = false;
            }).error(function (error) {
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to Update Transction Type.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        };

        $scope.netSale = function (memoItems) {
            var total = 0;
            angular.forEach(memoItems, function (item) {
                total += parseFloat(item.MemoCost) + parseFloat(item.GatOther) - parseFloat(item.MemoDiscount);
            })
            return total;
        };

        $scope.deleteInvoice = function (memo, index) {

            deleteObj = confirm('Are you sure you want to delete the Memo No: ' + memo.MemoNo +' ?');
            if (deleteObj) {
                $http({
                    url: "/api/MemoMasters/" + memo.MemoMasterId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.Message = 'Memo Deleted Successfull!';
                    $scope.loginAlertMessage = false;
                    $timeout(function () { $scope.loginAlertMessage = true; }, 3000);
                    $scope.memos.splice(index, 1);
                }).error(function (data) {
                    $scope.error = "An Error has occured while Saving Customer! " + data;
                    $scope.loading = true;
                });
            }
        };
    }])