var app = angular.module('PCBookWebApp');
app.controller('BookkeepingController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {

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

        $scope.showModal = false;
        $scope.toggleModal = function () {
            //if ($scope.paymentOrReceiveModel===true){
            //    $scope.showModal = true;
            //    angular.element('#checkMoneyReceiptNo').focus();
            //} else {
            //    $scope.showModal = false;
            //}
        }

        $scope.voucher = {
            //vouchrType: { VoucherTypeId: 1, VoucherTypeName: 'Payment' },
            vouchrDate: new Date(),
            voucherNumber: "",
            narration: "",
            //items: [{
            //    drOrCr: "Dr",
            //    description: 'item',
            //    drAmount: 9.95,
            //    crAmount: 0,
            //    paymentOrReceive: true,
            //}, {
            //    drOrCr: "Cr",
            //    description: 'item',
            //    drAmount: 0,
            //    crAmount: 9.95,
            //    paymentOrReceive: false,
            //}]
        };
        

        var currentTime = new Date()
        // returns the month (from 0 to 11)
        var month = currentTime.getMonth() + 1
        var day = currentTime.getDate()
        var year = currentTime.getFullYear()
        var cYear = year;


        // For 3 DatePicker
        $scope.voucher.vouchrDate = new Date();
        $scope.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.opened = true;
        };
        $scope.voucherDatePickerIsOpen = false;
        $scope.VoucherDatePickerOpen = function () {
            this.voucherDatePickerIsOpen = true;
        };

        $scope.honourDate = new Date();
        $scope.honourDatePickerIsOpen = false;
        $scope.HonourDatePickerOpen = function () {
            this.honourDatePickerIsOpen = true;
        };
        // End DatePicker

        $scope.drAmountTrueFalse = false;
        $scope.crAmountTrueFalse = false;

        $scope.drOrCrChange = function () {
            if ($scope.transctionType) {
                var drOrCr = $scope.transctionType.TransctionTypeName;
                //alert(drOrCr);
                if (drOrCr == "Dr") {
                    $scope.drAmountTrueFalse = true;
                    $scope.crAmountTrueFalse = false;
                } else if (drOrCr == "Cr") {
                    $scope.crAmountTrueFalse = true;
                    $scope.drAmountTrueFalse = false;
                }
            } else {
                $scope.drAmountTrueFalse = false;
                $scope.crAmountTrueFalse = false;
            }

        };

        $scope.vouchrTypeList = [];
        $http({
            url: "/api/VoucherTypes/GetDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.vouchrTypeList = data;
        });

        $scope.transctionTypesList = [];
        $http({
            url: "/api/TransctionTypes/GetTransctionTypeDdlList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.transctionTypesList = data;
            //console.log(data);
        });

        $scope.ledgersList = [];
        $http({
            url: "/api/Ledgers/GetLedgerDropDownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.ledgersList = data;
        });
        $scope.changeSelectLedgerName = function (item) {
            $scope.ledgerNameTypeahead = item;
        };

        var strPiNo = cYear + "/" + month + "/";
        //Create New Id
        $scope.GetVoucherNo = function () {
            CreateVoutureTypeWiseNewId()
        };

        $scope.voucher.items = [];
        $scope.addItem = function () {
            var voucherTypeName = "";
            var voucherTypeId = 0;
            var drOrCr = 0;
            var drOrCrId = 0;
            var ledgerName = "";
            var ledgerId = 0;
            var drAmount = 0;
            var crAmount = 0;
            var voucherNumber = "";

            if ($scope.voucher.voucherNumber) {
                voucherNumber = $scope.voucher.voucherNumber;
            } else {
                alert('Vouchr id not created!! Select Voucher Type First.');
                angular.element('#vouchrType').focus();
                return false;
            }
            if ($scope.voucher.vouchrType) {
                voucherTypeName = $scope.voucher.vouchrType.VoucherTypeName;
                voucherTypeId = $scope.voucher.vouchrType.VoucherTypeId;
            } else {
                alert('Please Select Vouchr Type');
                angular.element('#vouchrType').focus();
                return false;
            }
            if ($scope.transctionType) {
                drOrCr = $scope.transctionType.TransctionTypeName;
                drOrCrId = $scope.transctionType.TransctionTypeId;
            } else {
                alert('Please Select Dr/Cr');
                angular.element('#transctionType').focus();
                return false;
            }

            if ($scope.ledgerNameTypeahead) {
                ledgerName = $scope.ledgerNameTypeahead.LedgerName;
                ledgerId = $scope.ledgerNameTypeahead.LedgerId;
            } else {
                alert('Please input Ledger Name');
                angular.element('#ledgerNameTypeahead').focus();
                return false;
            }

            if (drOrCr == "Dr") {
                if ($scope.drAmount) {
                    drAmount = $scope.drAmount;
                } else {
                    alert('Please input Dr. Amount');
                    angular.element('#drAmount').focus();
                    return false;
                }               
            } else if (drOrCr == "Cr") {
                if ($scope.crAmount) {
                    crAmount = $scope.crAmount;
                } else {
                    alert('Please input Cr. Amount');
                    angular.element('#crAmount').focus();
                    return false;
                }                
            }

            var duelMark = false;
            $.each($scope.voucher.items, function (ii) {
                if ($scope.voucher.items[ii].paymentOrReceive === true) {
                    duelMark = true;
                    return false;
                }
            });

            var alreadyAdded = false;
            $.each($scope.voucher.items, function (i) {
                if ($scope.voucher.items[i].description === ledgerName) {
                    console.log($scope.voucher.items[i].description);
                    alreadyAdded = true;
                    return false;
                }
            });

            if (duelMark == true && $scope.paymentOrReceiveModel==true) {
                alert("Already one ledger marked.");
                $scope.paymentOrReceiveModel = false;
                angular.element('#paymentOrReceiveModel').focus();
            } else if (alreadyAdded) {
                alert("Ledger name already included");
                angular.element('#ledgerNameTypeahead').focus();
            } else {
                $scope.voucher.items.push({
                    description: ledgerName,
                    ledgerId: ledgerId,
                    drOrCr: drOrCr,
                    drAmount: drAmount,
                    crAmount: crAmount,
                    drOrCrId: drOrCrId,
                    paymentOrReceive: $scope.paymentOrReceiveModel
                });
                $scope.ledgerNameTypeahead = "";
                $scope.drAmount = "";
                $scope.crAmount = "";

                var addMore = confirm(ledgerName + " Added to voucher. Are you sure you want to add more?");
                if (addMore) {
                    $scope.paymentOrReceiveModel = false;
                    angular.element('#transctionType').focus();
                } else {
                    $scope.transctionType = {};
                    $scope.drAmountTrueFalse = false;
                    $scope.crAmountTrueFalse = false;
                    $scope.paymentOrReceiveModel = false;
                    angular.element('#narration').focus();
                }
            }
        },

        $scope.removeItem = function (index) {
            $scope.voucher.items.splice(index, 1);
        },

        $scope.totalDr = function () {
            var _totalDr = 0;
            angular.forEach($scope.voucher.items, function (item) {
                _totalDr += item.drAmount;
            })
            return _totalDr
            }

        $scope.totalCr = function () {
            var _totalCr = 0;
            angular.forEach($scope.voucher.items, function (item) {
                _totalCr += item.crAmount;
            })
            return _totalCr;
        }



        $scope.saveInvoice = function () {

            if ($scope.voucher.items.length == 0) {
                alert('No Entry Found!! Please Add Voucher Item.');
                angular.element('#vouchrType').focus();
                return false;
            }
            var markCount = 0;
            $.each($scope.voucher.items, function (i) {
                if ($scope.voucher.items[i].paymentOrReceive === true) {
                    markCount++;
                    //return false;
                }
            });
            if (markCount !== 1 && $scope.PaymentOrReceive == true) {
                alert("No marked ledger found.");
                $scope.paymentOrReceiveModel = false;
                angular.element('#paymentOrReceiveModel').focus();
                return false;
            }
            var fd = $filter('date')($scope.voucher.vouchrDate, "yyyy-MM-dd");
            var voucherObj = {
                VoucherTypeId: $scope.voucher.vouchrType.VoucherTypeId,
                VoucherDate: fd,
                VoucherNo: $scope.voucher.voucherNumber,
                Naration: $scope.voucher.narration,
                ShowRoomId: 0
            };
            var voucherDetail = $scope.voucher.items;
            var paymentAndReciveVoucherDetail = $scope.paymentOrReceiveDetail;
            if ($scope.PaymentOrReceive) {
                if (paymentAndReciveVoucherDetail.length == 0) {
                    alert("Bank or Party information required!!");
                    angular.element('#checkMoneyReceiptNo').focus();
                    return false;
                }
            }

            $http({
                url: "/api/Vouchers",
                data: voucherObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                var voucherId = data.VoucherId;
                var voucherItems = [];
                angular.forEach(voucherDetail, function (item) {
                    voucherItems.push({
                        VoucherId: voucherId,
                        LedgerId: item.ledgerId,
                        TransctionTypeId: item.drOrCrId,
                        DrAmount: item.drAmount,
                        CrAmount: item.crAmount,
                        ReceiveOrPayment: item.paymentOrReceive,
                        ShowRoomId: 0
                    });
                })
                
                $http({
                    url: "/api/VoucherDetails/PostVoucherDetail",
                    data: voucherItems,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    if ($scope.PaymentOrReceive) {
                        var pAndRVoucherItems = [];
                        angular.forEach(paymentAndReciveVoucherDetail, function (item) {
                            pAndRVoucherItems.push({
                                VoucherId: voucherId,
                                BankOrPartyName: item.bankAccountNo,
                                CheckOrMoneyReceiptNo: item.checkOrMReceiptNo,
                                HonourDate: item.honourDate,
                                Amount: item.amount,
                                ShowRoomId: 0
                            });
                        })
                        $http({
                            url: "/api/PAndRVouchers",
                            data: pAndRVoucherItems,
                            method: "POST",
                            headers: authHeaders
                        }).success(function (data) {
                            $scope.paymentOrReceiveDetail = [];
                            $scope.PaymentOrReceive = false;
                        }).error(function (error) {

                        });
                    }
                    $scope.transctionType = {};
                    $scope.drAmountTrueFalse = false;
                    $scope.crAmountTrueFalse = false;

                    $scope.message = "Voucher Successfully Saved.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    angular.element('#vouchrType').focus();

                }).error(function (error) {
                    $scope.validationErrors = [];
                    if (error.ModelState && angular.isObject(error.ModelState)) {
                        for (var key in error.ModelState) {
                            $scope.validationErrors.push(error.ModelState[key][0]);
                        }
                    } else {
                        $scope.validationErrors.push('Unable to add Voucher.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
                

            }).error(function (error) {
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to add Voucher.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });

            //alert('PI Saved');
            $scope.voucher = {
                //vouchrType: { VoucherTypeId: $scope.voucher.vouchrType.VoucherTypeId, VoucherTypeName: $scope.voucher.vouchrType.VoucherTypeName },
                vouchrType: {},
                vouchrDate: $scope.voucher.vouchrDate,
                voucherNumber: "",
                narration: ""
            };            
            $scope.voucher.items = [];

            // Create ner Id Again
            angular.element('#vouchrType').focus();

        };

        $scope.paymentOrReceiveDetail = [];
        $scope.addAdditionalItem = function () {
            var aAddtionalInfo = {
                checkOrMReceiptNo: $scope.checkMoneyReceiptNo,
                honourDate: $scope.honourDate,
                amount: $scope.amount,
                bankAccountNo: $scope.accountNo
            };
            $scope.paymentOrReceiveDetail.push(aAddtionalInfo);
            $scope.checkMoneyReceiptNo = "";
            $scope.amount = "";
            $scope.accountNo = "";
            $scope.honourDate = new Date();
            angular.element('#checkMoneyReceiptNo').focus();
        }

        function CreateVoutureTypeWiseNewId() {
            //$scope.voucher.voucherNumber = 0;
            var voucherTypeId = 0;
            var voucherTypeName = "";
            var fd = null;

            if ($scope.voucher.vouchrDate) {
                fd = $filter('date')($scope.voucher.vouchrDate, "yyyy-MM-dd");
            } else {
                //alert("Select Date");
                angular.element('#vouchrDate').focus();
                return false;
            }
            if ($scope.voucher.vouchrType) {
                voucherTypeId = $scope.voucher.vouchrType.VoucherTypeId;
                voucherTypeName = $scope.voucher.vouchrType.VoucherTypeName;
            }
            if (voucherTypeName === "Payment" || voucherTypeName === "Receive") {
                $scope.PaymentOrReceive = true;
            } else {
                $scope.PaymentOrReceive = false;
            }

            if (voucherTypeId == 0) {
                //alert('Please Select Vouchr Type');
                angular.element('#vouchrType').focus();
                return false;
            } else {

                $http({
                    url: "/api/Vouchers/GetNewVoucherId/" + fd + "/" + voucherTypeId,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    if (data.VoucherId == 0) {
                        $scope.voucher.voucherNumber = strPiNo + "0001";
                    } else {
                        var len = 4;
                        //console.log(data);
                        if (data.VoucherId.length >= len)
                            newStrId = data.VoucherId;
                        else
                            newStrId = ("000" + data.VoucherId).slice(-len);
                        $scope.voucher.voucherNumber = strPiNo + newStrId;
                    }
                });
                angular.element('#vouchrType').focus();
            }
        };
        $scope.moveToSaveButton = function () {
            angular.element('#saveButton').focus();
        };

    }])




//var app = angular.module('PGAppCotton');

//app.controller('ProformaInvoicesController', ['$scope', '$location', '$http', '$timeout', '$filter',
//    function ($scope, $location, $http, $timeout, $filter) {

//        $scope.clientMessage = true;
//        $scope.serverMessage = true;
//        $scope.messageType = "";
//        $scope.message = "";

//        var accesstoken = sessionStorage.getItem('accessToken');

//        var authHeaders = {};
//        if (accesstoken) {
//            authHeaders.Authorization = 'Bearer ' + accesstoken;
//        }
//        var currentTime = new Date()
//        // returns the month (from 0 to 11)
//        var month = currentTime.getMonth() + 1
//        var day = currentTime.getDate()
//        var year = currentTime.getFullYear()
//        var cYear = year;


//        $scope.deliveryOptionList = [];
//        $http({
//            url: "/api/DeliveryOptions/GetDeliveryOptionsListDdl",
//            method: "GET",
//            headers: authHeaders
//        }).success(function (data) {
//            $scope.deliveryOptionList = data;
//        });

//        $scope.bankList = [];
//        $http({
//            url: "/api/Banks/GetBanksListDdl",
//            method: "GET",
//            headers: authHeaders
//        }).success(function (data) {
//            $scope.bankList = data;
//            //console.log(data);
//        });

//        $scope.invoice = {
//            deliveryOption: { DeliveryOptionId: 1, DeliveryOptionName: 'At Site' },
//            //bankDdl: { BankId: 1, BankName: 'At Site' },
//            invoiceDate: new Date(),
//            validity: 10,
//            number: strPiNo,
//            ratePersent: 17,
//            paymentDays: 90,
//            customer_info: {
//                name: "",
//                Address1: 'None',
//                Address2: 'None',
//                Address3: 'None'
//            }
//            //,
//            //items: [{
//            //    qty: 10,
//            //    description: 'item',
//            //    cost: 9.95,
//            //    discount:1
//            //}]
//        };
//        var strPiNo = "PCSML/EXP/" + cYear + "/";
//        //Create New Id
//        $scope.invoice.number = 0;
//        $http({
//            url: "/api/ProformaInvoices/GetNewProformaInvoiceId",
//            method: "GET",
//            headers: authHeaders
//        }).success(function (data) {
//            var len = 4;
//            if (data.ProformaInvoiceId.length >= len)
//                newStrId = data.ProformaInvoiceId;
//            else
//                newStrId = ("000" + data.ProformaInvoiceId).slice(-len);
//            //$scope.invoice.number = strPiNo + data.ProformaInvoiceId;
//            $scope.invoice.number = strPiNo + newStrId;
//        });

//        $scope.invoice.items = [];
//        $scope.invoice.deliveryOption = [];
//        $scope.invoice.deliveryOption = { DeliveryOptionId: 1, DeliveryOptionName: 'At Site' };

//        $scope.addItem = function () {
//            if ($scope.productName) {
//                var productName = $scope.productName.ProductName;

//            } else {
//                alert('Please input Product Name');
//                angular.element('#productName').focus();
//                return false;
//            }
//            if ($scope.itemQuentity) {
//                var quentity = $scope.itemQuentity;

//            } else {
//                alert('Please input Quantity');
//                angular.element('#itemQuentity').focus();
//                return false;
//            }

//            $scope.invoice.items.push({
//                description: $scope.productName,
//                qty: $scope.itemQuentity,
//                cost: $scope.itemCost
//            });
//            $scope.productName = "";
//            $scope.itemQuentity = "";
//            $scope.itemCost = "";


//            var addMoreProduct = confirm(productName + " Added to Cart. Are you sure you want to add more?");
//            if (addMoreProduct) {
//                angular.element('#productName').focus();
//            } else {
//                angular.element('#finalDiscount').focus();
//            }

//        },

//            $scope.removeItem = function (index) {
//                $scope.invoice.items.splice(index, 1);
//            },

//            $scope.total = function () {
//                var total = 0;
//                angular.forEach($scope.invoice.items, function (item) {
//                    total += item.qty * (item.cost);
//                })
//                return total;
//            }
//        $scope.totalQuantity = function () {
//            var totalQu = 0;
//            angular.forEach($scope.invoice.items, function (item) {
//                totalQu += item.qty;
//            })
//            return totalQu;
//        }


//        // For 3 DatePicker
//        //$scope.invoice.invoiceDate = new Date();

//        $scope.open = function ($event) {
//            $event.preventDefault();
//            $event.stopPropagation();
//            $scope.opened = true;
//        };

//        $scope.invoiceDatePickerIsOpen = false;
//        $scope.InvoiceDatePickerOpen = function () {
//            this.invoiceDatePickerIsOpen = true;
//        };

//        // End DatePicker


//        $scope.customerList = [];
//        $http({
//            url: "/api/Buyers/GetMatricsListDdl",
//            method: "GET",
//            headers: authHeaders
//        }).success(function (data) {
//            $scope.customerList = data;
//            //console.log(data);
//        });

//        $scope.productList = [];
//        $http({
//            url: "/api/Products/GetProductsDropDownList",
//            method: "GET",
//            headers: authHeaders
//        }).success(function (data) {
//            $scope.productList = data;
//        });

//        $scope.changeSelectCustomerName = function (item) {
//            $scope.invoice.customer_info.name = item;
//        };
//        $scope.changeSelectProductName = function (item) {
//            $scope.productName = item;
//        };
//        $scope.GetCustomerDetailById = function () {
//            var buyerId = $scope.invoice.customer_info.name.BuyerId;
//            $http({
//                url: "/api/Buyers/" + buyerId,
//                method: "GET",
//                headers: authHeaders
//            }).success(function (data) {
//                $scope.invoice.customer_info.Address1 = data.Address1;
//                $scope.invoice.customer_info.Address2 = data.Address2;
//                $scope.invoice.customer_info.Address3 = data.Address3;
//            });
//        };
//        $scope.GetBankDetailById = function () {
//            var bankId = $scope.invoice.bankDdl.BankId;
//            $http({
//                url: "/api/Banks/" + bankId,
//                method: "GET",
//                headers: authHeaders
//            }).success(function (data) {
//                $scope.bankDetail = data;
//            });
//        };
//        $scope.GetProductDetailById = function () {
//            if ($scope.productName) {
//                var productName = $scope.productName.ProductName;
//                var productId = $scope.productName.ProductId;
//                $http({
//                    url: "/api/Products/GetProductById/" + productId,
//                    method: "GET",
//                    headers: authHeaders
//                }).success(function (data) {
//                    $scope.itemCost = data[0].Rate;
//                    //console.log(data[0].Rate);
//                });
//            } else {
//                angular.element('#productName').focus();
//            }
//        };

//        $scope.saveInvoice = function () {
//            var deliveryOptionId = "";
//            var deliveryOptionName = "";
//            var customerName = "";

//            if ($scope.invoice.customer_info.name) {
//                customerName = $scope.invoice.customer_info.name;
//            } else {
//                alert('Please input customer name');
//                angular.element('#CustomerName').focus();
//                return false;
//            }

//            if ($scope.invoice.deliveryOption.DeliveryOptionId) {
//                deliveryOptionId = $scope.invoice.deliveryOption.DeliveryOptionId;
//                deliveryOptionName = $scope.invoice.deliveryOption.DeliveryOptionName;
//            } else {
//                alert('Select Delivery Option');
//                angular.element('#deliveryOption').focus();
//                return false;
//            }
//            if ($scope.invoice.items.length == 0) {
//                alert('No Entry Found!! Please Add Product.');
//                angular.element('#productName').focus();
//                return false;
//            }




//            var proformaInvoices = {
//                DeliveryOptionId: $scope.invoice.deliveryOption.DeliveryOptionId,
//                ProformaInvoiceDate: $scope.invoice.invoiceDate,
//                ProformaInvoiceNo: $scope.invoice.number,
//                RatePersent: $scope.invoice.ratePersent,
//                Validity: $scope.invoice.validity,
//                PaymentDays: $scope.invoice.paymentDays,
//                BankId: $scope.invoice.bankDdl.BankId,
//                BuyerId: $scope.invoice.customer_info.name.BuyerId
//            };
//            var piDetail = $scope.invoice.items;


//            $http({
//                url: "/api/ProformaInvoices/PostProformaInvoice",
//                data: proformaInvoices,
//                method: "POST",
//                headers: authHeaders
//            }).success(function (data) {
//                var proformaInvoiceId = data.ProformaInvoiceId;
//                var proformaInvoiceItems = [];
//                angular.forEach(piDetail, function (item) {
//                    proformaInvoiceItems.push({
//                        ProformaInvoiceId: proformaInvoiceId,
//                        ProductId: item.description.ProductId,
//                        Quantity: item.qty,
//                        Rate: item.cost
//                    });
//                })

//                $http({
//                    url: "/api/ProformaInvoiceDetails/PostProformaInvoiceDetail",
//                    data: proformaInvoiceItems,
//                    method: "POST",
//                    headers: authHeaders
//                }).success(function (data) {
//                    $scope.message = "Proforma Invoice Successfully Created.";
//                    $scope.messageType = "success";
//                    $scope.clientMessage = false;
//                    $timeout(function () { $scope.clientMessage = true; }, 5000);
//                    //alert('Requisition Saved');

//                    angular.element('#CustomerName').focus();

//                }).error(function (error) {
//                    $scope.validationErrors = [];
//                    if (error.ModelState && angular.isObject(error.ModelState)) {
//                        for (var key in error.ModelState) {
//                            $scope.validationErrors.push(error.ModelState[key][0]);
//                        }
//                    } else {
//                        $scope.validationErrors.push('Unable to add Unit.');
//                    };
//                    $scope.messageType = "danger";
//                    $scope.serverMessage = false;
//                    $timeout(function () { $scope.serverMessage = true; }, 5000);
//                });
//            }).error(function (error) {
//                $scope.validationErrors = [];
//                if (error.ModelState && angular.isObject(error.ModelState)) {
//                    for (var key in error.ModelState) {
//                        $scope.validationErrors.push(error.ModelState[key][0]);
//                    }
//                } else {
//                    $scope.validationErrors.push('Unable to add Unit.');
//                };
//                $scope.messageType = "danger";
//                $scope.serverMessage = false;
//                $timeout(function () { $scope.serverMessage = true; }, 5000);
//            });



//            //console.log($scope.invoice);
//            //alert('PI Saved');
//            $scope.invoice = {
//                deliveryOption: { DeliveryOptionId: deliveryOptionId, DeliveryOptionName: deliveryOptionName },
//                invoiceDate: new Date($scope.invoice.invoiceDate),
//                validity: 10,
//                number: strPiNo,
//                ratePersent: 17,
//                paymentDays: 90,
//                customer_info: {
//                    name: "",
//                    Address1: 'None',
//                    Address2: 'None',
//                    Address3: 'None'
//                }
//            };
//            $scope.invoice.items = [];
//            angular.element('#CustomerName').focus();

//        };

//        $scope.printToCart = function (printSectionId) {
//            var innerContents = document.getElementById(printSectionId).innerHTML;
//            var popupWinindow = window.open('', '_blank', 'width=900,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
//            popupWinindow.document.open();
//            popupWinindow.document.write('<html><head><link rel="stylesheet" type="text/css" href="http://cotton.pakizagroup.com/Content/site.css" /></head><body onload="window.print()">' + innerContents + '</html>');
//            popupWinindow.document.close();
//        };

//        $scope.numberInWords = function (num) {
//            return toWords(num);
//        };

//        var th = ['', 'thousand', 'million', 'billion', 'trillion'];
//        var dg = ['zero', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine'];
//        var tn = ['ten', 'eleven', 'twelve', 'thirteen', 'fourteen', 'fifteen', 'sixteen', 'seventeen', 'eighteen', 'nineteen'];
//        var tw = ['twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

//        function toWords(s) {
//            s = s.toString();
//            s = s.replace(/[\, ]/g, '');
//            if (s != parseFloat(s)) return 'not a number';
//            var x = s.indexOf('.');
//            if (x == -1) x = s.length;
//            if (x > 15) return 'too big';
//            var n = s.split('');
//            var str = '';
//            var sk = 0;
//            for (var i = 0; i < x; i++) {
//                if ((x - i) % 3 == 2) {
//                    if (n[i] == '1') {
//                        str += tn[Number(n[i + 1])] + ' ';
//                        i++;
//                        sk = 1;
//                    } else if (n[i] != 0) {
//                        str += tw[n[i] - 2] + ' ';
//                        sk = 1;
//                    }
//                } else if (n[i] != 0) {
//                    str += dg[n[i]] + ' ';
//                    if ((x - i) % 3 == 0) str += 'hundred ';
//                    sk = 1;
//                }


//                if ((x - i) % 3 == 1) {
//                    if (sk) str += th[(x - i - 1) / 3] + ' ';
//                    sk = 0;
//                }
//            }
//            if (x != s.length) {
//                var y = s.length;
//                str += 'point ';
//                for (var i = x + 1; i < y; i++) str += dg[n[i]] + ' ';
//            }
//            return str.replace(/\s+/g, ' ');
//        };



//    }]);
