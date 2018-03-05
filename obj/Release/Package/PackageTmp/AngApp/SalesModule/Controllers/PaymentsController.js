var app = angular.module('PCBookWebApp');

app.controller('PaymentsController', ['$scope', '$location', '$http', '$timeout', '$filter', 'upload', "$mdDialog", function ($scope, $location, $http, $timeout, $filter, upload, $mdDialog) {
    //$scope.Message = "customerAutoComplite Controller Angular JS";
    $scope.loading = true;

    $scope.clientMessage = true;
    $scope.serverMessage = true;
    $scope.messageType = "";
    $scope.message = "";

    $scope.pageSize = 20;
    $scope.currentPage = 1;

    $scope.addButton = false;
    $scope.updateButton = true;
    $scope.checkCollection = false;

    var accesstoken = sessionStorage.getItem('accessToken');
    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }
    $scope.searchPayment = {};
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
    $scope.searchPayment.FromDate = new Date();
    $scope.searchPayment.ToDate = new Date();
    //End DatePicker
    //*******************************************


    $scope.payment = {};
    $scope.searchPayment = {};
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
    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.opened = true;
    };

    $scope.invoiceDatePickerIsOpen = false;
    $scope.InvoiceDatePickerOpen = function () {
        this.invoiceDatePickerIsOpen = true;
    };
    $scope.honourDatePickerIsOpen = false;
    $scope.HonourDatePickerOpen = function () {
        this.honourDatePickerIsOpen = true;
    };
    // $scope.payment.invoiceDate = new Date();
    // End DatePicker
    $scope.collectionForm = true;
    $scope.data = {
        paymentType: 'Cash',
        formType:'Collections'
    };
    $scope.paymentTypeChange = function () {
        //alert($scope.data.paymentType);
        if ($scope.data.paymentType == 'Cash') {
            $scope.checkCollection = false;
        }
        else {
            $scope.checkCollection = true;
        }
    };
    $scope.formTypeChange = function () {
        if ($scope.data.formType == 'Collections') {
            $scope.collectionForm = true;
            $scope.collectionSearchForm = false;
        }
        else {
            $scope.collectionForm = false;
            $scope.collectionSearchForm = true;
        }
    };

    $scope.adjustPartyAccountChange = function () {
        //alert($scope.data.adjustPartyAccount);
        if ($scope.data.adjustPartyAccount==false) {
            $scope.discountTextBoxEditable = true;
        } else {
            $scope.discountTextBoxEditable = false;
        }
    };

    $scope.changeSelectCustomerName = function (item) {
        $scope.payment.customerAutoComplite = item;
    };
    $scope.changeSelectCustomerNameSearch = function (item) {
        $scope.searchPayment.customerAutoComplite = item;
    };
    $scope.savePaymentForm = function (ev) {

        $scope.submitted = true;
        if ($scope.paymentForm.$valid) {
            //$scope.loading = true;
            var fd = $filter('date')($scope.payment.invoiceDate, "yyyy-MM-dd");
            var hd = null;
            var customerId = 0;
            var customerName = "";
            var amount=0;
            var checkNo = null;
            var bankAccNo = null;

            // Form Validation But No Need
            if ($scope.payment.invoiceDate) {
                fd = $filter('date')($scope.payment.invoiceDate, "yyyy-MM-dd");
            } else {
                alert("Select Collection Date");
                angular.element('#invoiceDate').focus();
                return false;
            }
            if ($scope.payment.customerAutoComplite) {
                customerId = $scope.payment.customerAutoComplite.CustomerId;
                customerName = $scope.payment.customerAutoComplite.CustomerName;
                if (customerName == "Cash Party") {
                    //alert("You Select 'Cash Party' witch is not valid for credit collection.");
                    angular.element('#customerAutoComplite').focus();
                    $mdDialog.show(
                        $mdDialog.alert()
                            .parent(angular.element(document.querySelector('#popupContainer')))
                            .clickOutsideToClose(true)
                            .title('Add Payment')
                            .textContent("You Select 'Cash Party' witch is not valid for credit collection!!")
                            .ariaLabel('Alert Dialog Demo')
                            .ok('Ok!')
                            .targetEvent(ev)
                    );
                    $scope.loading = false;
                    return false;
                }
            } else {
                //alert("Select Customer Name");
                angular.element('#customerAutoComplite').focus();
                $mdDialog.show(
                    $mdDialog.alert()
                        .parent(angular.element(document.querySelector('#popupContainer')))
                        .clickOutsideToClose(true)
                        .title('Add Payment')
                        .textContent("You Select 'Cash Party' witch is not valid for credit collection!!")
                        .ariaLabel('Alert Dialog Demo')
                        .ok('Ok!')
                        .targetEvent(ev)
                );
                $scope.loading = false;
                return false;
            }
            if ($scope.payment.SCAmount) {
                //if ($scope.payment.SCAmount != 0) {
                //    alert("Amount Must be Positive");
                //    angular.element('#SCAmount').focus();
                //    return false;
                //}
                amount = $scope.payment.SCAmount;
            } 
            //else {
            //    alert("Input Amount");
            //    angular.element('#SCAmount').focus();
            //    return false;
            //}
            ////Bank payment Validation
            if ($scope.data.paymentType == 'Bank') {
                if ($scope.payment.honourDate) {
                    hd = $filter('date')($scope.payment.honourDate, "yyyy-MM-dd");
                } else {
                    alert("Select Honour date");
                    angular.element('#honourDate').focus();
                    return false;
                }
                if ($scope.payment.checkNo) {
                    checkNo = $scope.payment.checkNo;
                } else {
                    alert("Input Check No");
                    angular.element('#checkNo').focus();
                    return false;
                }
                if ($scope.payment.bankAccNo) {
                    bankAccNo = $scope.payment.bankAccNo;
                } else {
                    alert("Input Bank Account No");
                    angular.element('#bankAccNo').focus();
                    return false;
                }
            }


            var aPayment = {
                PaymentId: 0,
                MemoMasterId: 0,
                CustomerId: customerId,
                ShowRoomId: 0,
                PaymentDate: fd,
                SCAmount: amount,
                SDiscount: $scope.payment.SDiscount,
                PaymentType: $scope.data.paymentType,
                HonourDate: hd,
                BankAccountNo: $scope.payment.bankAccNo,
                CheckNo: $scope.payment.checkNo,
                Remarks: $scope.payment.remarks
            };
            //console.log(aPayment);

            $http({
                url: "/api/Payments",
                data: aPayment,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                var aViewObj = {
                    PaymentId: data.PaymentId,
                    PaymentDate: $scope.payment.invoiceDate,
                    CustomerName: $scope.payment.customerAutoComplite.CustomerName,
                    SCAmount: $scope.payment.SCAmount,
                    SDiscount: $scope.payment.SDiscount,
                    PaymentType: $scope.data.paymentType

                };
                $scope.users.push(aViewObj);
                $scope.submitted = false;
                $scope.CreditLimit = 0;
                $scope.TotalCredit = 0;
                $scope.Address = '';                
                $scope.payment = {
                    invoiceDate: fd
                };
                $scope.loading = false;
                $scope.paymentForm.$setPristine();
                $scope.paymentForm.$setUntouched();
                
                $mdDialog.show(
                    $mdDialog.alert()
                        .parent(angular.element(document.querySelector('#popupContainer')))
                        .clickOutsideToClose(true)
                        .title('Add Payment')
                        .textContent('Payment saved successfully.')
                        .ariaLabel('Alert Dialog Demo')
                        .ok('Ok!')
                        .targetEvent(ev)
                );
                angular.element(document.querySelector('#customerAutoComplite')).focus();
            }).error(function (error) {
                $scope.message = 'Unable to save Payment' + error.message;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }
        else {
            alert("Please  correct form errors!");
        }
    };

    $scope.GetCustomerDetailById = function () {
        var customerId =0;
        var customerName = "";
        if ($scope.payment.customerAutoComplite) {
            customerId = $scope.payment.customerAutoComplite.CustomerId;
            customerName = $scope.payment.customerAutoComplite.CustomerName;
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
                    $scope.CreditLimit =0;
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
            alert("Select Customer Name");
        }
    };

    $scope.GetCustomerDetailByIdSearch = function () {
        var customerId = 0;
        var customerName = "";
        if ($scope.searchPayment.customerAutoComplite) {
            customerId = $scope.searchPayment.customerAutoComplite.CustomerId;
            customerName = $scope.searchPayment.customerAutoComplite.CustomerName;
            $http({
                url: "/api/Customer/GetSingleCustomer/" + customerId,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.customerLedgers = [];
                if (data.length > 0) {
                    $scope.searchPayment.Address = data[0].Address;
                    $scope.searchPayment.DistrictName = data[0].DistrictName;
                    $scope.searchPayment.Image = data[0].Image;
                    $scope.searchPayment.BfAmount = data[0].BfAmount;
                    $scope.searchPayment.BFDate = data[0].BFDate;
                    $scope.searchPayment.CreditLimit = data[0].CreditLimit;
                    $scope.searchPayment.ActualCredit = data[0].ActualCredit;
                    $scope.searchPayment.TotalSale = data[0].TotalSale;
                    $scope.searchPayment.TotalCollection = data[0].TotalCollection;
                    $scope.searchPayment.TotalDiscount = data[0].TotalDiscount;
                } else {
                    $scope.searchPayment.CreditLimit = 0;
                    $scope.searchPayment.ActualCredit = 0;
                    $scope.searchPayment.TotalSale = 0;
                    $scope.searchPayment.TotalCollection = 0;
                    $scope.searchPayment.TotalDiscount = 0;
                    $scope.searchPayment.Address = "";
                    $scope.searchPayment.Image = "";
                }
            }).error(function (error) {
                $scope.message = 'Unable to get Customer info' + error.message;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }
        //else {
        //    alert("Select Customer Name");
        //}  
    };

    $scope.searchPayments = function () {
        var customerId = 0;
        var customerName = "";
        var fd = null;
        var td = null;
        $scope.customersLedger = [];
        $scope.submitted = true;
        if ($scope.searchPaymentForm.$valid) {
            
            fd = $filter('date')($scope.searchPayment.fromDate, "yyyy-MM-dd");
            td = $filter('date')($scope.searchPayment.toDate, "yyyy-MM-dd");
            if ($scope.searchPayment.customerAutoComplite) {
                customerId = $scope.searchPayment.customerAutoComplite.CustomerId;
                customerName = $scope.searchPayment.customerAutoComplite.CustomerName;
            }
            
            $http({
                url: '/api/Payments/GetPaymentList/' + fd + '/' + td + '/' + customerId,
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                if (data.length > 0) {
                    $scope.customerLedgers = data;
                }
            }).error(function (error) {
                $scope.message = 'Unable to get Payments info' + error.message;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });

            if ($scope.data.PartyAccount == true) {
                $http({
                    url: '/api/MemoMasters/GetMemoMastersSummary/' + fd + '/' + td + '/' + customerId,
                    method: "GET",
                    headers: authHeaders
                }).success(function (data) {
                    if (data.length > 0) {
                        $scope.customerSaleLedgers = data;
                    }
                }).error(function (error) {
                    $scope.message = 'Unable to get Payments info' + error.message;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            } else {
                $scope.customerSaleLedgers = {};
            }


            //$scope.searchPayment = {};
            $scope.submitted = false;
            //$scope.searchPayment.CreditLimit = 0;
            //$scope.searchPayment.TotalCredit = 0;
            //$scope.searchPayment.Address = "";
            $scope.searchPaymentForm.$setPristine();
            $scope.searchPaymentForm.$setUntouched();
            $scope.loading = false;
        } else {
            alert("Please  correct form errors!");
        }
    };

    $scope.deletePayment = function (aPayment) {
        deleteObj = confirm('Are you sure you want to delete the Payment Amount: ' + aPayment.SCAmount+'?');
        if (deleteObj) {
            var Id = aPayment.PaymentId;
            
            $http({
                url: "/api/Payments/" + Id,
                method: "DELETE",
                headers: authHeaders
            }).success(function (data) {
                $.each($scope.customerLedgers, function (i) {
                    if ($scope.customerLedgers[i].PaymentId === Id) {
                        $scope.customerLedgers.splice(i, 1);
                        return false;
                    }
                });

                $scope.message = 'Payment Deleted Successfull!';
                $scope.messageType = "danger";
                $scope.clientMessage = false;
                $timeout(function () { $scope.loginAlertMessage = true; }, 3000);
                $scope.loading = false;
                alert('Payment Deleted Successfull!');
            }).error(function (data) {
                $scope.error = "An Error has occured while Deleteding Payment! " + data;
                $scope.loading = true;
            });

        }
        
    };

    $scope.clear = function () {
        $scope.data.PartyAccount == false;
        $scope.customerLedgers = [];
        $scope.searchPayment = {};
        $scope.searchPayment.CreditLimit = 0;
        $scope.searchPayment.TotalCredit = 0;
        $scope.searchPayment.Address = "";
        $scope.submitted = false;
        $scope.searchPaymentForm.$setPristine();
        $scope.searchPaymentForm.$setUntouched();
        $scope.loading = false;
        //angular.element('#customerAutoCompliteSerch').focus();
    };
    //$scope.payment.SCAmount = 0;
    $scope.users = [];
    $scope.customerLedgers = [];
    $scope.searchPayment.CreditLimit = 0;
    $scope.searchPayment.TotalCredit = 0;
    $scope.searchPayment.Address = "";
    $scope.CreditLimit = 0;
    $scope.TotalCredit = 0;
    $scope.payment.SDiscount = 0;
    $scope.discountTextBoxEditable = true;
    $scope.data.adjustPartyAccount = false;
    $scope.loading = false;


    $scope.oneAtATime = true;
    $scope.status = {
        isFirstOpen: true,
        isFirstDisabled: false
    };
}])




