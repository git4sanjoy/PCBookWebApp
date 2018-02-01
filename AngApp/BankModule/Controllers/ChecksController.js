var app = angular.module('PCBookWebApp');
app.controller('ChecksController', ["$scope", "$http", "$filter", "$timeout" , function ($scope, $http, $filter, $timeout) {
    // $routeParams used for get query string value
    $scope.Message = "This is ChecksController Page  ";
    $scope.loading = true;

    $scope.addButton = false;
    $scope.updateButton = true;

    $scope.loginAlertMessageCheck = true;
    $scope.successMessageCheck = true;

    $scope.pageSize = 25;
    $scope.currentPage = 1;

    $scope.checkList = [];
    $scope.CheckBookPages = [];

    $scope.personArray = [];
    $scope.personArray.push({ name: "HonourDate", Text: "Honour Date", isDefault: false });
    $scope.personArray.push({ name: "CheckDate", Text: "Check Date", isDefault: false });
    $scope.personArray.push({ name: "IssueDate", Text: "Issue Date", isDefault: false });
    $scope.SearchBy = $scope.personArray[0];
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
    // End DatePicker

    //angular.element('#accountNo').focus();

    //$scope.Check.partyName = "";
     
    // For 3 DatePicker
    //$scope.Check.IssueDate = new Date();
    // grab today and inject into field


    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.opened = true;
    };  

    $scope.issueDatePickerIsOpen = false;
    $scope.checkDatePickerIsOpen = false;
    $scope.honourDatePickerIsOpen = false;

    $scope.IssueDatePickerOpen = function () {
        this.issueDatePickerIsOpen = true;
    };
    $scope.CheckDatePickerOpen = function () {
        this.checkDatePickerIsOpen = true;
    };
    $scope.HonourDatePickerOpen = function () {
        this.honourDatePickerIsOpen = true;
    };
    // End DatePicker


    $scope.sort = function (keyname) {
        $scope.sortKey = keyname;   //set the sortKey to the param passed
        $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    }

    //Auto complete Party
    $http({
        method: 'GET',
        url: '/Checks/GetPartyList',
        contentType: "application/json; charset=utf-8",
        dataType: 'json'
    }).success(function (data) {
        $scope.partyList = data;      
        $scope.states = data;
    });


    //End Auto complete
    $scope.changeSelect = function (item) {
        $scope.Check.partyName = item;
    };
    $scope.changeSelectCheckNo = function (item) {
        $scope.Check.CheckNumber = item;
    };

    
    $scope.GetCheckPageNoListByBankAccountId = function () {
        // CheckBookPages List
        $http({
            method: 'GET',
            url: '/api/CheckBookPage/GetCheckBookPageList/' + $scope.accountNo.BankAccountId,
            contentType: "application/json; charset=utf-8",
            dataType: 'json'
        }).success(function (data) { 
            $scope.checkBookPages = data;
            //console.log($scope.CheckBookPages);
        });
    };

    //$http({
    //    method: 'GET',
    //    url: '/api/Check/GetCheckList',
    //    contentType: "application/json; charset=utf-8",
    //    dataType: 'json'
    //}).success(function (data) {
    //    $scope.checkList = data;
    //    //console.log(data);
    //});

    $scope.updatePartyTypeaheadList = function (searchTerm) {
        $http.get('/api/Party/GetPartyTypeAheadList/' + searchTerm).success(function (data) {
            if (data.length > 0) {
                $scope.states = data;
            } else {
                $http({
                    method: 'GET',
                    url: '/Checks/GetPartyList',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json'
                }).success(function (data) {
                    $scope.partyList = data;
                    $scope.states = data;
                });
            }
        });
    };

    $http({
        method: 'GET',
        url: '/Checks/GetUserBankAccounts/',
        contentType: "application/json; charset=utf-8",
        dataType: 'json'
    }).success(function (data) {
        $scope.userBankAccounts = data;
    });

    // function to submit the form after all validation has occurred 
    $scope.submitForm = function () {
        // Set the 'submitted' flag to true
        $scope.submitted = true;

        if ($scope.checkForm.$valid) {
            //alert("Form is valid!");
            $scope.loading = false;
            var bankAccId = $scope.accountNo.BankAccountId;
            var partyId = $scope.Check.partyName.PartyId;
            var accountNumber = $scope.accountNo.BankAccountNumber;
            var PartyName = $scope.Check.partyName.PartyName;
            var checkNo = $scope.Check.CheckNumber.CheckBookPageNo;
            var checkBookPageId = $scope.Check.CheckNumber.CheckBookPageId;

            //$http.get('/api/Party/GetPartyByName/' + PartyName).success(function (data) {
            //    $scope.Party = data;
            //    var partyId = data.PartyId;
            //    //console.log(data);                
            //})
            //.error(function () {
            //    $scope.error = "An Error has occured while loading party information!";
            //});
            //alert(partyId);

            $scope.Check.BankAccountId = bankAccId;
            $scope.Check.PartyId = partyId;
            $scope.Check.CheckBookPageId = checkBookPageId;

            var aCheck = {
                CheckBookPageId:checkBookPageId,
                BankAccountId: bankAccId,
                PartyId: partyId,
                CheckNumber: checkNo,
                Amount: $scope.Check.Amount,
                CheckDate: $scope.CheckDate,
                IssueDate: $scope.IssueDate,
                HonourDate: $scope.HonourDate,
                ApprovedBy: "",
                Remarks: $scope.Check.Remarks,
                Active: false
            };
            
            $http.post('/api/Check/PostCheck', aCheck).success(function (data) {
                var aCheckBookPageObj = {
                    CheckBookId: $scope.Check.CheckNumber.CheckBookId,
                    CheckBookPageId: $scope.Check.CheckNumber.CheckBookPageId,
                    CheckBookPageNo:$scope.Check.CheckNumber.CheckBookPageNo,
                    Active:true
                };
                $http.put('/api/CheckBookPage/' + $scope.Check.CheckNumber.CheckBookPageId, aCheckBookPageObj).success(function (data) {
                    //$scope.checkList.push(data);
                    //$scope.checkList.push($scope.Check);
                    var putCheck = {
                        CheckBookPageId: data.CheckBookPageId,
                        CheckId: data.CheckId,
                        PartyName: PartyName,
                        CheckNo: checkNo,
                        Amount: $scope.Check.Amount,
                        CheckDate: $scope.CheckDate,
                        IssueDate: $scope.IssueDate,
                        HonourDate: $scope.HonourDate,
                        BankAccountNumber: accountNumber,
                        Remarks: $scope.Check.Remarks
                    };
                    $scope.checkList.push(putCheck);

                    $scope.Message = 'New Check Saved Successfully.';
                    $scope.successMessageCheck = false;
                    $timeout(function () { $scope.successMessageCheck = true; }, 3000);

                    $scope.Check = {};
                    $scope.submitted = false;
                    $scope.checkForm.$setPristine();
                    $scope.checkForm.$setUntouched();
                    $scope.loading = true;
                    $scope.accountNo = { BankAccountId: bankAccId, BankAccountNumber: accountNumber };
                    // CheckBookPages List
                    $http({
                        method: 'GET',
                        url: '/api/CheckBookPage/GetCheckBookPageList/' + $scope.accountNo.BankAccountId,
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json'
                    }).success(function (data) {
                        $scope.checkBookPages = data;
                        //console.log($scope.CheckBookPages);
                    });
                    angular.element('#accountNo').focus();

                })
                .error(function () {
                    $scope.error = "An Error has occured while loading check book information!";
                });


            }).error(function (error) {
                $scope.message = 'Unable to add Check data: ' + error.message;
                console.log($scope.message);
                $scope.message = error;
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to add Check data.');
                };
                $scope.loading = true;
            });          
        }
        else {
            alert("Please correct errors!");
        }
    };
    //Select single Check data
    $scope.getSingleCheck = function (Check) {
        $scope.loading = false;
        $scope.addButton = true;
        $scope.updateButton = false;
        $http.get('/api/Check/' + Check.CheckId).success(function (data) {
            //console.log(data);
            $scope.CheckDate=data.CheckDate;
            $scope.IssueDate=data.IssueDate;
            $scope.HonourDate = data.HonourDate;
            $scope.accountNo = { BankAccountId: data.BankAccountId, BankAccountNumber: data.BankAccountNumber };
            var showCheck = {
                CheckBookPageId: data.CheckBookPageId,
                CheckId: data.CheckId,
                CheckNumber: {
                    CheckBookPageId: data.CheckId, CheckBookPageNo: data.CheckNo
                    },
                partyName:{
                        PartyId: data.PartyId, PartyName: data.PartyName
                    },
                Amount: data.Amount,
                //CheckDate: data.CheckDate,
                //IssueDate: data.IssueDate,
                //HonourDate: data.HonourDate,
                //accountNo:{
                //        BankAccountId: data.BankAccountId, BankAccountNumber: data.BankAccountNumber
                //    },
                Remarks: data.Remarks
            };
            $scope.Check = showCheck;
            $scope.loading = true;
        })
        .error(function () {
            $scope.error = "An Error has occured while loading party information!";
            $scope.loading = true;
        });
    };
    $scope.cancel = function () {
        $scope.addButton = false;
        $scope.updateButton = true;
        $scope.Check = {};
        $scope.submitted = false;
        $scope.checkForm.$setPristine();
        $scope.checkForm.$setUntouched();
        angular.element('#accountNo').focus();
    };
    //Check Update Operation
    $scope.updateCheck = function (Check) {
        $scope.loading = false;

        var bankAccId = $scope.accountNo.BankAccountId;
        var partyId = $scope.Check.partyName.PartyId;
        var accountNumber = $scope.accountNo.BankAccountNumber;
        var checkNo = $scope.Check.CheckNumber.CheckBookPageNo;
        var checkBookPageId = $scope.Check.CheckNumber.CheckBookPageId;
        
        var PartyName = $scope.Check.partyName.PartyName;
        var Id = $scope.Check.CheckId;

        $scope.Check.CheckBookPageId = checkBookPageId;
        $scope.Check.BankAccountId = bankAccId;
        $scope.Check.PartyId = partyId;
        $scope.Check.CheckDate = $scope.CheckDate;
        $scope.Check.IssueDate = $scope.IssueDate;
        $scope.Check.HonourDate = $scope.HonourDate

        $scope.Check.CheckNumber = checkNo;
        
        $http.put('/api/Check/' + $scope.Check.CheckId, $scope.Check).success(function (data) {

            var putCheck = {
                    CheckBookPageId: checkBookPageId,
                    CheckId: data.CheckId,
                    PartyName:data.PartyName,
                    CheckNo: data.CheckNo,
                    Amount: $scope.Check.Amount,
                    CheckDate: $scope.CheckDate,
                    IssueDate: $scope.IssueDate,
                    HonourDate: $scope.HonourDate,
                    BankAccountNumber: data.BankAccountNumber,
                    Remarks: $scope.Check.Remarks
                };
            //console.log(putCheck);

            $.each($scope.checkList, function (i) {
                if ($scope.checkList[i].CheckId === Id) {
                    $scope.checkList[i] = putCheck;
                    return false;
                }
            });

            $scope.Check = {};     
            $scope.Message = 'Check Update Successfull.';
            $scope.successMessageCheck = false;
            $timeout(function () { $scope.successMessageCheck = true; }, 3000);
            $scope.checkForm.$setPristine();
            $scope.checkForm.$setUntouched();
            $scope.addButton = false;
            $scope.updateButton = true;
            angular.element('#accountNo').focus();
            $scope.loading = true;

        }).error(function (error) {
            $scope.error = "An Error has occured while updating! " + error;
            //displayServerSideValidationErrors($scope, error); 
            console.log(error);
            $scope.loading = true;
        });
    };
    //Delete Check
    $scope.deleteCheck = function (Check) {
        var Id = Check.CheckId;

        var r = confirm("Are you sure You want to Delete?");
        if (r == true) {
            $scope.loading = false;
            $http.delete('/api/Check/' + Check.CheckId).success(function (data) {

                //alert("Deleted Successfully!!");
                $.each($scope.checkList, function (i) {
                    if ($scope.checkList[i].CheckId === Id) {
                        $scope.checkList.splice(i, 1);
                        return false;
                    }
                });
                $scope.Check = {};
                $scope.loading = false;
                $scope.Message = 'Check Deleted Successfull!';
                $scope.checkForm.$setPristine();
                $scope.checkForm.$setUntouched();
                $scope.loginAlertMessageCheck = false;
                $timeout(function () { $scope.loginAlertMessageCheck = true; }, 3000);
                $scope.loading = true;
                angular.element('#accountNo').focus();
            }).error(function (data) {
                $scope.error = "An Error has occured while Saving Customer! " + data;
                $scope.loading = true;
            });
        }
    };
    //// Server Side error Message Display Function
    //function displayServerSideValidationErrors($scope, error) {
    //    $scope.validationErrors = [];
    //    var errorObj = error.data;
    //    if (errorObj) {
    //        for (var key in errorObj) {
    //            var errorMessage = errorObj[key][0];
    //            $scope.validationErrors.push(errorMessage);
    //        }
    //        console.log($scope.validationErrors);
    //    }
    //}
    // Another Function
    $scope.submitSearchForm = function () {
        // Set the 'submitted' flag to true
        $scope.submitted = true;
        if ($scope.searchForm.$valid) {
            //$scope.loading = false;
            //alert($scope.SearchBy.name);
            //alert(partyId);

            $scope.submitted = false;
            var fd = $filter('date')($scope.FromDate, "yyyy-MM-dd");
            var td = $filter('date')($scope.ToDate, "yyyy-MM-dd");
            var checkNo = $scope.checkNo;

            if (checkNo) {
                $http.get('/api/check/CheckListByCheckNo/' + checkNo).success(function (data) {
                    //console.log(data);
                    $scope.checkList = data;

                    var total = 0;
                    for (var i = 0; i < $scope.checkList.length; i++) {
                        var check = $scope.checkList[i];
                        total += (check.Amount);
                    }
                    $scope.Total = total;
                })
                .error(function () {
                    $scope.error = "An Error has occured while loading party information!";
                });
            } else {
                $http.get('/api/check/CheckListByDate/' + fd + '/' + td + '/' + $scope.SearchBy.name).success(function (data) {
                    //console.log(data);
                    $scope.checkList = data;

                    var total = 0;
                    for (var i = 0; i < $scope.checkList.length; i++) {
                        var check = $scope.checkList[i];
                        total += (check.Amount);
                    }
                    $scope.Total = total;
                })
                .error(function () {
                    $scope.error = "An Error has occured while loading party information!";
                });
            }            
        }
        else {
            alert("Please correct errors!");
            $scope.loading = true;
        }
    };






}])