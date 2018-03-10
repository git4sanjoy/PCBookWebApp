var app = angular.module('PCBookWebApp');
app.controller('CartController', ['$scope', '$location', '$http', '$timeout', '$filter', "$mdDialog", "$mdToast",
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

    // For 3 DatePicker
    //$scope.invoice.invoiceDate = new Date();
    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.opened = true;
    };

    $scope.invoiceDatePickerIsOpen = false;
    $scope.InvoiceDatePickerOpen = function () {
        this.invoiceDatePickerIsOpen = true;
    };
    // End DatePicker

    $scope.changeSelectCustomerName = function (item) {
        $scope.invoice.customer_info.name = item;
    };
    $scope.changeSelectProductName = function (item) {
        $scope.productName = item;
    };

    $scope.showRoomList = [];
    $http({
        url: "/api/ShowRooms/GetShowRoomDropDownList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.showRoomList = data;
        //console.log($scope.showRoomList);
        if ($scope.showRoomList.length > 0 && $scope.showRoomList.length == 1) {
            angular.forEach($scope.showRoomList, function (item) {
                $scope.invoice.showRoom = { ShowRoomId: item.ShowRoomId, ShowRoomName: item.ShowRoomName, ShowRoomNameBangla: item.ShowRoomNameBangla };
            })
        }

    });
    $scope.creditParty = false;
    $scope.customerList = {};
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

    $scope.productList = {};
    $http({
        url: "/api/Product/GetDropDownList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.productList = data;
        //console.log(data);
    });

    $scope.invoice = {
        showRoom: { ShowRoomId: null, ShowRoomName: "" },
        invoiceDate: new Date(),
        number: 0,
        finalDiscount: 0,
        payment: 0,
        otherExpencess: 0,
        customer_info: {
            name: '',
            Address: '',
            Phone: '',
            Email: ''
        }
    };

    $scope.invoice.items = [];
    $scope.addItem = function () {
        if ($scope.productName) {
            var productName = $scope.productName.ProductName;

        } else {
            alert('Please input Product Name');
            angular.element('#productName').focus();
            return false;
        }
        if ($scope.itemQuentity) {
            var quentity = $scope.itemQuentity;

        } else {
            alert('Please input Quantity');
            angular.element('#itemQuentity').focus();
            return false;
        }
        //if ($scope.itemCost < $scope.itemMinCost) {
        //    alert('Rate Mast Be Grater Than Min Rate: ' + $scope.itemMinCost);
        //    angular.element('#itemCost').focus();
        //    return false;
        //}
        //if ($scope.itemDiscount > $scope.itemMinDiscount) {
        //    alert('Discount Mast Be Less Than Min Discount Rate: ' + $scope.itemMinDiscount);
        //    angular.element('#itemDiscount').focus();
        //    return false;
        //}
        $scope.invoice.items.push({
            description: $scope.productName,
	        qty: $scope.itemQuentity,
	        discount: $scope.itemDiscount,
            cost: $scope.itemCost,
            SubCategoryId: $scope.itemSubCategoryId,
            MainCategoryName: $scope.itemMainCategoryName
        });
        $scope.productName = "";
        $scope.itemQuentity = 0;
        $scope.itemCost = 0;
        $scope.itemDiscount = 0;

        var addMoreProduct = confirm(productName + " Added to Cart. Are you sure you want to add more?");
        if (addMoreProduct) {
            angular.element('#productName').focus();
        } else {
            angular.element('#finalDiscount').focus();
        }

    },

    $scope.removeItem = function (index) {
        $scope.invoice.items.splice(index, 1);
    },

    $scope.total = function () {
        var items = $scope.invoice.items;
        var total = 0;
        angular.forEach(items, function (item) {
            total += item.qty * (item.cost - item.discount);
        })
        return total;
    },
    $scope.grandTotal = function () {
        return ($scope.total() + $scope.invoice.otherExpencess - $scope.invoice.finalDiscount);
    },
    $scope.dueTotal = function () {
        $scope.memoDueAmount = ($scope.total() + $scope.invoice.otherExpencess - $scope.invoice.finalDiscount - $scope.invoice.payment);
        return ($scope.total() + $scope.invoice.otherExpencess - $scope.invoice.finalDiscount - $scope.invoice.payment);
    },

    $scope.saveInvoice = function (ev) {
        var showRoomId = "";
        var showRoomName = "";
        var customerName = "";
        if ($scope.invoice.showRoom.ShowRoomId) {
            // Check Show Room have value
            showRoomId = $scope.invoice.showRoom.ShowRoomId;
            showRoomName = $scope.invoice.showRoom.ShowRoomName;

        } else {
            alert('Select Show Room No');
            angular.element('#showRoom').focus();
            return false;
        }
        if ($scope.invoice.customer_info.name) {
            // Check Customer Name have value
            customerName = $scope.invoice.customer_info.name.CustomerName;

        } else {
            alert('Please input customer name');
            angular.element('#CustomerName').focus();
            return false;
        }

        var fd = $filter('date')($scope.invoice.invoiceDate, "yyyy-MM-dd");
        var memoDiscount = 0;
        var gatOther = 0;
        var paidAmount = 0;
        if ($scope.invoice.finalDiscount) {
            memoDiscount = $scope.invoice.finalDiscount
        }
        if ($scope.invoice.otherExpencess) {
            gatOther = $scope.invoice.otherExpencess
        }
        if ($scope.invoice.payment) {
            paidAmount = $scope.invoice.payment;
        }

        // Cash Party Payment Validation
        var due = 0;
        var takenItems = $scope.invoice.items;
        if (customerName === "Cash Party") {
            var _t = 0;
            angular.forEach(takenItems, function (aItem) {
                _t += aItem.qty * (aItem.cost - aItem.discount);
            })
            due = _t + gatOther - memoDiscount - paidAmount;
            if (due > 0){
                alert("Grand Total and Paid Amount should be Same for 'CASH PARTY'");
                angular.element('#payment').focus();
                return false;
            }
        }
        var aMemo = {
            MemoMasterId: 0,
            CustomerId: $scope.invoice.customer_info.name.CustomerId,
            MemoDate: fd,
            ShowRoomId: 0,
            MemoNo: $scope.invoice.number,
            MemoDiscount: memoDiscount,
            GatOther: gatOther,
            ExpencessRemarks: $scope.invoice.otherExpencessRemarks
        };       
        
        $http({
            url: "/api/MemoMasters",
            data: aMemo,
            method: "POST",
            headers: authHeaders
        }).success(function (data) {
            var memoMasterId = data.MemoMasterId;
            var memoItems = [];
            angular.forEach($scope.invoice.items, function (item) {
                memoItems.push({
                    MemoMasterId: memoMasterId,
                    ProductId: item.description.ProductId,
                    Quantity: item.qty,
                    Rate: item.cost,
                    Discount: item.discount
                });
            })
            $http({
                url: "/api/MemoDetails",
                data: memoItems,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                var aPayment = {};
                if (customerName == "Cash Party") {
                    aPayment = {
                        PaymentId: 0,
                        MemoMasterId: memoMasterId,
                        CustomerId: $scope.invoice.customer_info.name.CustomerId,
                        ShowRoomId: 0,
                        PaymentDate: fd,
                        SCAmount: $scope.invoice.payment,
                        PaymentType: 'Cash Party'
                    };
                } else {
                    aPayment = {
                        PaymentId: 0,
                        MemoMasterId: memoMasterId,
                        CustomerId: $scope.invoice.customer_info.name.CustomerId,
                        ShowRoomId: 0,
                        PaymentDate: fd,
                        SCAmount: $scope.invoice.payment,
                        PaymentType: 'Cash'
                    };
                }
                //console.log(aPayment);
                if (paidAmount) {
                    $http({
                        url: "/api/Payments",
                        data: aPayment,
                        method: "POST",
                        headers: authHeaders
                    }).success(function (data) {
            
                    }).error(function (error) {
                        $scope.message = 'Unable to save payment' + error.message;
                        $scope.messageType = "warning";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                    });
                } else {

                }
                //Success Alert
                //$mdDialog.show(
                //    $mdDialog.alert()
                //        .parent(angular.element(document.querySelector('#popupContainer')))
                //        .clickOutsideToClose(true)
                //        .title('Add Payment')
                //        .textContent('Payment saved successfully.')
                //        .ariaLabel('Alert Dialog Demo')
                //        .ok('Ok!')
                //        .targetEvent(ev)
                //);

                //Print the memo
                var innerContents = document.getElementById('printable-memo-bangla').innerHTML;
                var popupWinindow = window.open('', '_blank', 'width=900,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
                popupWinindow.document.open();
                popupWinindow.document.write('<html><head><link rel="stylesheet" type="text/css" href="http://books.pakizagroup.com/Content/Site.css" /></head><body onload="window.print()">' + innerContents + '</html>');
                popupWinindow.document.close();

                //Rset Form
                $scope.invoice = {
                    showRoom: { ShowRoomId: showRoomId, ShowRoomName: showRoomName },
                    invoiceDate: new Date($scope.invoice.invoiceDate),
                    number: 0,
                    finalDiscount: 0,
                    payment: 0,
                    otherExpencess: 0,
                    customer_info: {
                        name: "",
                        Address: 'None',
                        Phone: 'None',
                        Email: ''
                    }
                };
                $scope.invoice.items = [];
                angular.element('#CustomerName').focus();

            }).error(function (error) {

            });
        }).error(function (error) {

        });       

    }

    $scope.GetCustomerDetailById = function () {
        var customerId = $scope.invoice.customer_info.name.CustomerId;
        var customerName = $scope.invoice.customer_info.name.CustomerName;

        $scope.invoice.customer_info.Address = "";
        $scope.invoice.customer_info.Phone = "";
        $scope.invoice.customer_info.Email = "";
        $http({
            url: "/api/Customer/GetSingleCustomer/" + customerId,
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            if (data.length > 0) {
                //$scope.invoice.customer_info.Address = data[0].Address;
                //$scope.invoice.customer_info.Phone = data[0].Phone;
                //$scope.invoice.customer_info.Email = data[0].Email;
                //$scope.invoice.customer_info.CreditLimit = data[0].CreditLimit;
                //$scope.invoice.customer_info.TotalCredit = data[0].TotalCredit;
                //$scope.invoice.customer_info.AddressBangla = data[0].AddressBangla;
                //$scope.invoice.customer_info.Image = data[0].Image;

                $scope.invoice.customer_info.Address = data[0].Address;
                $scope.invoice.customer_info.DistrictName = data[0].DistrictName;
                $scope.invoice.customer_info.Image = data[0].Image;
                $scope.invoice.customer_info.BfAmount = data[0].BfAmount;
                $scope.invoice.customer_info.BFDate = data[0].BFDate;
                $scope.invoice.customer_info.CreditLimit = data[0].CreditLimit;
                $scope.invoice.customer_info.ActualCredit = data[0].ActualCredit;
                $scope.invoice.customer_info.TotalSale = data[0].TotalSale;
                $scope.invoice.customer_info.TotalCollection = data[0].TotalCollection;
                $scope.invoice.customer_info.TotalDiscount = data[0].TotalDiscount;

                //if (customerName == "Cash Party") {
                //    $scope.creditParty = false;
                //} else {
                //    $scope.creditParty = true;
                //}
            } else {
                $scope.invoice.customer_info.name = {};
                angular.element('#CustomerName').focus();
            }

        }).error(function (error) {
            $scope.message = 'Unable to get party info' + error.message;
            $scope.messageType = "warning";
            $scope.clientMessage = false;
            $timeout(function () { $scope.clientMessage = true; }, 5000);
        });
    }

    $scope.GetProductDetailById = function () {
        if ($scope.productName) {

            var productName = $scope.productName.ProductName;
            var productId = $scope.productName.ProductId;

            $http({
                url: "/api/Product/GetProductRateById/" + productName,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.itemCost = data.Rate;
                $scope.itemDiscount = data.Discount;
                $scope.itemMinDiscount = data.Discount;
                $scope.itemMinCost = data.Rate;
                $scope.itemProductNameBangla = data.ProductNameBangla;
                $scope.itemSubCategoryId = data.SubCategoryId;
                $scope.itemMainCategoryName = data.MainCategoryName;
            });

        } else {
            angular.element('#productName').focus();
        }

    }
    $scope.GetNewMemoId = function () {
        CreateNewMemoId()
    };

    var currentTime = new Date()
    // returns the month (from 0 to 11)
    var month = currentTime.getMonth() + 1
    var day = currentTime.getDate()
    var year = currentTime.getFullYear()
    var cYear = year;
    var strPiNo = cYear + "-";

    function CreateNewMemoId() {
        //$scope.voucher.voucherNumber = 0;

            $http({
                url: "/api/MemoMasters/GetMemoId" ,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {

                if (data.MemoMasterId == 0) {
                    $scope.invoice.number = strPiNo + "000001";
                } else {
                    var len = 6;
                    //console.log(data);
                    if (data.MemoMasterId.length >= len)
                        newStrId = data.MemoMasterId;
                    else
                        newStrId = ("00000" + data.MemoMasterId).slice(-len);
                        $scope.invoice.number = strPiNo + newStrId;
                }
            });

    };

    $scope.printMemo = function (printSectionId) {
        var innerContents = document.getElementById(printSectionId).innerHTML;
        var popupWinindow = window.open('', '_blank', 'width=900,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
        popupWinindow.document.open();
        popupWinindow.document.write('<html><head><link rel="stylesheet" type="text/css" href="http://localhost:12005/Content/Site.css" /></head><body onload="window.print()">' + innerContents + '</html>');
        popupWinindow.document.close();
    };
    $scope.printMemoWithTableBorder = function (printSectionId) {
        var divToPrint = document.getElementById(printSectionId);
        var htmlToPrint = '' +
            '<style type="text/css">' +
            '.print-hide-button {display: none;}'+
            'table {' +
            'width: 100%;max-width: 100%;border:1px solid #000;border-width:1px 0 0 1px !important;' +
            '}' +
            'table th, table td {' +
            'border:1px solid #000;border-width:0 1px 1px 0 !important;' +
            'padding;0.5em;' +
            '}' +
            '</style>';
        htmlToPrint += divToPrint.outerHTML;
        newWin = window.open("");
        newWin.document.write(htmlToPrint);
        newWin.print();
        newWin.close();
    };
    $scope.GetGroupSumOfILineTotal = function (group) {
        var groupWiseSum = 0;
        for (var i in group) {
            groupWiseSum = groupWiseSum + (parseFloat (group[i].qty) * (parseFloat (group[i].cost) - parseFloat (group[i].discount)));
        }
        return groupWiseSum;
    };
    $scope.GetGroupSumOfILineQu = function (group) {
        var groupWiseSum = 0;
        for (var i in group) {
            groupWiseSum = groupWiseSum + parseFloat (group[i].qty);
        }
        return groupWiseSum;
    };
    $scope.totalSale = function (memoItems) {
        //console.log(memos);
        var total = 0;
        angular.forEach(memoItems, function (item) {
            total += parseFloat(item.qty) * (parseFloat(item.cost) - parseFloat(item.discount));
        })
        return total;
    };
    //Toster
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
                .textContent('Invoice saved successfully.')
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
            if (response == 'ok') {
                alert('You clicked the \'UNDO\' action.');
            }
        });
    };
        //End toster

}])