var app = angular.module('PCBookWebApp');
app.controller('CheckBooksController', ['$scope', '$location', '$http', '$timeout', '$filter', function ($scope, $location, $http, $timeout, $filter) {
    $scope.Message = "Check Books Controller";

    var accesstoken = sessionStorage.getItem('accessToken');
    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }
    $scope.loading = true;

    $scope.addButton = false;
    $scope.updateButton = true;

    $scope.loginAlertMessage = true;
    $scope.successMessage = true;

    $scope.pageSize = 25;
    $scope.currentPage = 1;

    angular.element('#field1').focus();

    $http({
        method: 'GET',
        url: '/api/Check/GetUserBankAccounts',
        contentType: "application/json; charset=utf-8",
        headers: authHeaders,
        dataType: 'json'
    }).success(function (data) {
        $scope.userBankAccounts = data;
    });

    $scope.doSomething = function () {
        alert("Enter");
        return false;
    };
    $scope.sort = function (keyname) {
        $scope.sortKey = keyname;   //set the sortKey to the param passed
        $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    }
    $scope.cancel = function () {
        $scope.addButton = false;
        $scope.updateButton = true;
        $scope.CheckBook = {};
        $scope.submitted = false;
        $scope.checkBookForm.$setPristine();
        $scope.checkBookForm.$setUntouched();
        angular.element('#field1').focus();
    };


    // function to submit the form after all validation has occurred 
    $scope.submitCheckBookForm = function () {
        // Set the 'submitted' flag to true
        $scope.submitted = true;
        if ($scope.checkBookForm.$valid) {
            $scope.loading = false;
            
            var startNo = $scope.CheckBook.StartNo;
            var endNo = $scope.CheckBook.EndNo;
            var startSuffices = $scope.CheckBook.StartSuffices;


            $scope.CheckBook.BankAccountId = $scope.CheckBook.accountNo.BankAccountId;
            $scope.CheckBook.CheckBookNo = $scope.CheckBook.BookNo;


            $http({
                headers: authHeaders,
                method: 'POST',
                url: '/api/CheckBook/PostCheckBook',
                data: $scope.CheckBook
            }).success(function (data, status, headers, config) {
                var checkBookPageViewObj = {
                    BankAccountNumber: $scope.CheckBook.accountNo.BankAccountNumber,
                    CheckBookNo: $scope.CheckBook.BookNo,
                    CheckBookId: data.CheckBookId,
                    StartNo: data.StartNo,
                    EndNo: data.EndNo,
                    StartSuffices: startSuffices
                };

                $scope.checkBookList.push(checkBookPageViewObj);
                var checkBookId = data.CheckBookId;
                var checkBookPageObj = {
                    CheckBookId: checkBookId,
                    StartNo: startNo,
                    EndNo: endNo,
                    StartSuffices: startSuffices
                };
                $http({
                    headers: authHeaders,
                    method: 'POST',
                    url: '/api/CheckBookPage/PostCheckBookPage',
                    data: checkBookPageObj
                }).success(function (data, status, headers, config) {
                    $scope.Message = 'New CheckBook Saved Successfully.';
                    $scope.successMessage = false;
                    $timeout(function () { $scope.successMessage = true; }, 3000);

                    $scope.CheckBook = {};
                    $scope.submitted = false;
                    $scope.checkBookForm.$setPristine();
                    $scope.checkBookForm.$setUntouched();
                    $scope.loading = true;
                    angular.element('#field1').focus();
                }).error(function (error) {
                    $scope.Message = 'Unable to load CheckBook data step2: ' + error.message;
                });

            }).error(function (error) {
                $scope.Message = 'Unable to load CheckBook data step1: ' + error.message;
            });
        }
        else {
            alert("Please correct errors!");
            $scope.loading = true;
        }
    };


    $http({
        url: "/api/CheckBook/GetCheckBookList",
        method: "GET",
        headers: authHeaders
    }).success(function (data) {
        $scope.checkBookList = data;
    });

    $scope.changeSelectGroup = function (item) {
        $scope.group.groupListCmb = item;
    };

    //Delete CheckBook
    $scope.deleteCheckBook = function (CheckBook) {
        var Id = CheckBook.CheckBookId;

        var r = confirm("Are you sure You want to Delete Check Book?");
        if (r == true) {
            $scope.loading = false;
            $http({
                url: "/api/CheckBook/" + CheckBook.CheckBookId,
                method: "DELETE",
                headers: authHeaders
            }).success(function (data) {
                $.each($scope.checkBookList, function (i) {
                    if ($scope.checkBookList[i].CheckBookId === Id) {
                        $scope.checkBookList.splice(i, 1);
                        return false;
                    }
                });
                $scope.CheckBook = {};
                $scope.loading = false;
                $scope.Message = 'Check Book Deleted Successfull!';
                $scope.checkBookForm.$setPristine();
                $scope.checkBookForm.$setUntouched();
                $scope.loginAlertMessage = false;
                $timeout(function () { $scope.loginAlertMessage = true; }, 3000);
                $scope.loading = true;
                angular.element('#field1').focus();
            }).error(function (data) {
                $scope.error = "An Error has occured while Saving Customer! " + data;
                $scope.loading = true;
            });


        }        
    };

}])