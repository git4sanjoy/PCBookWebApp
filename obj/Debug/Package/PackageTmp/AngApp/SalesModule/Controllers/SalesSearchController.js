var app = angular.module('PCBookWebApp');
app.controller('SalesSearchController', ['$scope', '$location', '$http', '$timeout', '$filter', "$mdDialog", "$mdToast",
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
        $scope.changeSelectCustomerName = function (item) {
            $scope.memoMaster.customerAutoComplite = item;
        };
        $http({
            url: "/api/Customer/CustomerDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.customerList = data;
            //console.log(data);
        });
        $scope.updatePartyTypeaheadList = function (searchTerm) {

            $http({
                url: "/api/Customer/GetCustomerTypeAheadList/" + searchTerm,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.customerList = data;
                //console.log(data);
            });
        };

        $scope.collections = [];
        $scope.memoMaster = [];
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
        //$scope.memoMaster.FromDate = new Date();
        //$scope.memoMaster.ToDate = new Date();
        ////End DatePicker

        $scope.deleteInvoice = function (memo, index) {

            deleteObj = confirm('Are you sure you want to delete the Memo?');
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

        $scope.clearForm = function () {
            $scope.searchForm.$setPristine();
            $scope.searchForm.$setUntouched();
            $scope.Address = "";
            $scope.memoMaster = {};
            $scope.memos = {};
            angular.element('#memoNo').focus();
        };

        $scope.submitSearchForm = function () {
            $scope.submitted = true;
            $scope.memos = {};
            $scope.categoryWiseGroupingSaleList = {};
            if ($scope.searchForm.$valid) {

                var memoNo = null
                if ($scope.memoMaster.memoNo) {
                    memoNo = $scope.memoMaster.memoNo;
                }
                if (memoNo !=null) {
                    
                    $http({
                        url: "/api/MemoMasters/GetMemo/" + memoNo,
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                        if (data.length > 0) {
                            $scope.memos = data;
                            //console.log(data);
                        } else {
                            $scope.memoMaster.memoNo = "";
                            $scope.memos = {};
                            alert("No Record Found");
                            angular.element('#memoNo').focus();
                        }
                        
                    });
                }
                if ($scope.memoMaster.FromDate && $scope.memoMaster.ToDate && memoNo==null) {
                    
                    var fd = $filter('date')($scope.memoMaster.FromDate, "yyyy-MM-dd");
                    var td = $filter('date')($scope.memoMaster.ToDate, "yyyy-MM-dd");
                    var customerId = 0;
                    if ($scope.memoMaster.customerAutoComplite) {
                        customerId = $scope.memoMaster.customerAutoComplite.CustomerId;
                    }

                    $http({
                        url: "/api/MemoMasters/GetMemoByDateBetween/" + fd + '/' + td + '/' + customerId,
                        method: "GET",
                        headers: authHeaders
                    }).success(function (data) {
                         // Assign Memos Data
                        if (data.listOfMemo.length > 0) {
                            $scope.memos = data.listOfMemo;
                            $scope.categoryWiseGroupingSaleList = data.categoryGroupingList;
                            $scope.SCAmount = data.cashPaymentList.SCAmount;
                            $scope.SDiscount = data.cashPaymentList.SDiscount;
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
        $scope.total = function (memoItems) {
            var total = 0;
            angular.forEach(memoItems, function (item) {
                total += parseFloat(item.Quantity) * (parseFloat(item.Rate) - parseFloat(item.Discount));
            })
            return total;
        };

        $scope.grandTotal = function (mi, gat, fdis) {
            var _total = 0;
            angular.forEach(mi, function (i) {
                _total += parseFloat(i.Quantity) * (parseFloat(i.Rate) - parseFloat(i.Discount));
            })
            return (_total + gat - fdis);
        };
        $scope.itemTotal = function (mi) {
            var _total = 0;
            angular.forEach(mi, function (i) {
                _total += parseFloat(i.Quantity) * (parseFloat(i.Rate) - parseFloat(i.Discount));
            })
            return (_total);
        };
        $scope.dueTotal = function (mi, gat, fdis, paid_amount) {
            var _total = 0;
            angular.forEach(mi, function (i) {
                _total += parseFloat(i.Quantity) * (parseFloat(i.Rate) - parseFloat(i.Discount));
            })
            return (_total + gat - fdis - paid_amount);
        };

        $scope.totalSale = function (memos) {
            var total = 0;
            angular.forEach(memos, function (item) {
                total += item.MemoCost + parseFloat(item.GatOther) - parseFloat(item.MemoDiscount);
            })

            return total;
        };
        $scope.totalCashSale = function (memos) {
            var _totalCashSale = 0;
            angular.forEach(memos, function (item) {
                if (item.SaleType == 'Cash Sale') {
                    _totalCashSale += parseFloat(item.MemoCost) + (parseFloat(item.GatOther) - parseFloat(item.MemoDiscount));
                }
            })
            return _totalCashSale;
        };
        $scope.totalCreditSale = function (memos) {
            var _totalCreditSale = 0;
            angular.forEach(memos, function (item) {
                if (item.SaleType != 'Cash Sale') {
                    _totalCreditSale += parseFloat(item.MemoCost) + (parseFloat(item.GatOther) - parseFloat(item.MemoDiscount));
                }
            })
            return _totalCreditSale;
        };



        $scope.printMemo = function (printSectionId) {
            var innerContents = document.getElementById(printSectionId).innerHTML;
            var popupWinindow = window.open('', '_blank', 'width=900,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWinindow.document.open();
            popupWinindow.document.write('<html><head><link rel="stylesheet" type="text/css" href="http://books.pakizagroup.com/Content/Site.css" /></head><body onload="window.print()">' + innerContents + '</html>');
            popupWinindow.document.close();
        },
        $scope.GetGroupSumOfILineTotal = function (group) {
            var groupWiseSum = 0;
            for (var i in group) {
                groupWiseSum = groupWiseSum + (parseFloat(group[i].Quantity) * (parseFloat(group[i].Rate) - parseFloat(group[i].Discount)));
            }
            return groupWiseSum;
        },
        $scope.GetGroupSumOfILineQu = function (group) {
            var groupWiseSum = 0;
            for (var i in group) {
                groupWiseSum = groupWiseSum + parseFloat(group[i].Quantity);
            }
            return groupWiseSum;
        },


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









        $scope.$watch('memoMaster.FromDate', function (newVal, oldVal, ev) {
            //alert(newVal)
            //if (angular.isUndefined(newVal)) {

            //} else {
            //    var fd = $filter('date')(newVal, "yyyy-MM-dd");
            //    //console.log(fd);
            //    $http({
            //        url: "/api/MemoMasters/GetMemoByDate/" + fd,
            //        method: "GET",
            //        headers: authHeaders
            //    }).success(function (data) {

            //        if (data == "No Record Found") {
            //            $mdDialog.show(
            //                $mdDialog.alert()
            //                    .parent(angular.element(document.querySelector('#popupContainer')))
            //                    .clickOutsideToClose(true)
            //                    .title('Search Voucher')
            //                    .textContent('No Record Found')
            //                    .ariaLabel('Alert Dialog Demo')
            //                    .ok('Ok!')
            //                    .targetEvent(ev)
            //            );
            //            $scope.memos = {};
            //            //angular.element('#voucherNo').focus();
            //            //return false;
            //        } else {     
            //            $scope.memos = data;
            //        }
            //        //console.log(data);
            //    });
            //}

            }, true);


        //Toster
        var last = {
            bottom: false,
            top: true,
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
                    .textContent('Simple Toast!')
                    .position(pinTo)
                    .hideDelay(90000)
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
                if (response == 'ok') {
                    alert('You clicked the \'UNDO\' action.');
                }
            });
        };
        //End toster

        //According
        $scope.oneAtATime = true;

        $scope.status = {
            isFirstOpen: true,
            isFirstDisabled: false
        };


        $scope.loading = false;
    }])