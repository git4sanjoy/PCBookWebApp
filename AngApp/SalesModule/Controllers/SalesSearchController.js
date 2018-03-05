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
            url: "/api/Customer/GetDropDownList",
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
        $scope.GetCustomerDetailById = function () {
            var customerId = 0;
            var customerName = "";
            $scope.memos = {};
            $scope.categoryWiseGroupingSaleList = {};
            if ($scope.memoMaster.customerAutoComplite) {
                customerId = $scope.memoMaster.customerAutoComplite.CustomerId;
                customerName = $scope.memoMaster.customerAutoComplite.CustomerName;
                $http({
                    url: "/api/Customer/GetSingleCustomer/" + customerId,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    if (data.length > 0) {
                        $scope.Address = data[0].Address;
                        $scope.DistrictName = data[0].DistrictName;
                        $scope.Image = data[0].Image;
                        $scope.BfAmount = data[0].BfAmount;
                        $scope.BFDate = data[0].BFDate;
                        $scope.CreditLimit = data[0].CreditLimit;
                        $scope.ActualCredit = data[0].ActualCredit;
                        $scope.TotalSale = data[0].TotalSale;
                        $scope.TotalCollection = data[0].TotalCollection;
                        $scope.TotalDiscount = data[0].TotalDiscount;

                    } else {
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
                    }

                }).error(function (error) {
                    $scope.message = 'Unable to get Customer info' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }
            else {
                //alert("Select Customer Name");
                $scope.memos = {};
                $scope.categoryWiseGroupingSaleList = {};
                $scope.Address = "";
            }
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

        $scope.deleteInvoice = function (memo) {

            deleteObj = confirm('Are you sure you want to delete the Memo?');
            if (deleteObj) {
                $http({
                    url: "/api/MemoMasters/" + memo.MemoMasterId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    //$.each($scope.checkBookList, function (i) {
                    //    if ($scope.checkBookList[i].CheckBookId === Id) {
                    //        $scope.checkBookList.splice(i, 1);
                    //        return false;
                    //    }
                    //});
                    //$scope.CheckBook = {};
                    $scope.memoMaster.memoNo = "";
                    $scope.memos = {};
                    alert("No Record Found");
                    angular.element('#memoNo').focus();
                    $scope.Message = 'Memo Deleted Successfull!';
                    $scope.searchForm.$setPristine();
                    $scope.searchForm.$setUntouched();
                    $scope.loginAlertMessage = false;
                    $timeout(function () { $scope.loginAlertMessage = true; }, 3000);
                    $scope.loading = true;
                    angular.element('#memoNo').focus();
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
                        // Get Collection Information
                        $http({
                            url: "/api/Payments/GetDateBetweenPaymentsSum/" + fd + '/' + td + '/' + customerId,
                            method: "GET",
                            headers: authHeaders
                        }).success(function (data) {
                            if (data.length > 0) {
                                $scope.collections = data;
                                // console.log(data);
                            }
                        });
                        //Get Memo DateBetween Category Wise Grouping List Data
                        $http({
                            url: "/api/MemoMasters/GetMemoByDateBetweenForCategoryWiseGrouping/" + fd + '/' + td + '/' + customerId,
                            method: "GET",
                            headers: authHeaders
                        }).success(function (data) {
                            if (data.length > 0) {
                                $scope.categoryWiseGroupingSaleList = data;
                                //console.log(data);
                            }
                        });
                        // Assign Memos Data
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

            }
        };
        $scope.total = function (memoItems) {
            var total = 0;
            angular.forEach(memoItems, function (item) {
                total += item.Quantity * (item.Rate - item.Discount);
            })
            return total;
        },
        $scope.totalSale = function (memos) {
            var total = 0;
            angular.forEach(memos, function (items) {
                //total += item.MemoDetailViews[i].Quantity * (item.MemoDetailViews[i].Rate - item.MemoDetailViews[i].Discount);
                angular.forEach(items.MemoDetailViews, function (item) {
                    total += item.Quantity * (item.Rate - item.Discount);
                })
            })
            return total;
        },
        $scope.grandTotal = function (mi, gat, fdis) {
            var _total = 0;
            angular.forEach(mi, function (i) {
                _total += i.Quantity * (i.Rate - i.Discount);
            })
            return (_total + gat - fdis);
        },
        $scope.dueTotal = function (mi, gat, fdis, paid_amount) {
            var _total = 0;
            angular.forEach(mi, function (i) {
                _total += i.Quantity * (i.Rate - i.Discount);
            })
            return (_total + gat - fdis - paid_amount);
        },


        $scope.totalCashSale = function (memos) {
            var _totalCashSale = 0;
            angular.forEach(memos, function (items) {
                if (items.SaleType === 'Cash Sale') {
                    angular.forEach(items.MemoDetailViews, function (item) {
                        _totalCashSale += item.Quantity * (item.Rate - item.Discount);
                    })
                }
            })
            return _totalCashSale;
        },
        $scope.totalCreditSale = function (memos) {
            var _totalCreditSale = 0;
            angular.forEach(memos, function (items) {
                if (items.SaleType !== 'Cash Sale') {
                    angular.forEach(items.MemoDetailViews, function (item) {
                        _totalCreditSale += item.Quantity * (item.Rate - item.Discount);
                    })
                }  
            })
            return _totalCreditSale;
        },



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
                groupWiseSum = groupWiseSum + (Number(group[i].Quantity) * (Number(group[i].Rate) - Number(group[i].Discount)));
            }
            return groupWiseSum;
        },
        $scope.GetGroupSumOfILineQu = function (group) {
            var groupWiseSum = 0;
            for (var i in group) {
                groupWiseSum = groupWiseSum + Number(group[i].Quantity);
            }
            return groupWiseSum;
        },

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