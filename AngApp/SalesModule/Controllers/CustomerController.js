var app = angular.module('PCBookWebApp');

app.controller('CustomerController', ['$scope', '$location', '$http', '$timeout', '$filter', 'upload', function ($scope, $location, $http, $timeout, $filter, upload) {
    //$scope.Message = "Customer Controller Angular JS";
    $scope.loading = true;

    $scope.clientMessage = true;
    $scope.serverMessage = true;
    $scope.messageType = "";
    $scope.message = "";

    $scope.pageSize = 200;
    $scope.currentPage = 1;

    $scope.addButton = false;
    $scope.updateButton = true;

    var accesstoken = sessionStorage.getItem('accessToken');
    var authHeaders = {};
    if (accesstoken) {
        authHeaders.Authorization = 'Bearer ' + accesstoken;
    }
    $scope.sort = function (keyname) {
        $scope.sortKey = keyname;   //set the sortKey to the param passed
        $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    }
    // Any function returning a promise object can be used to load values asynchronously
    $scope.getLocation = function (val) {
        return $http.get('http://maps.googleapis.com/maps/api/geocode/json', {
            params: {
                address: val,
                sensor: false
            }
        }).then(function (response) {
            return response.data.results.map(function (item) {
                return item.formatted_address;
            });
        });
    };

    $scope.users = [];
    $http({
        method: 'GET',
        url: '/api/Customer/GetCustomerList',
        contentType: "application/json; charset=utf-8",
        headers: authHeaders,
        dataType: 'json'
    }).success(function (data) {
        $scope.users = data;
    });
                               
    $scope.upazilaList = [];
    $http({
        method: 'GET',
        url: '/api/Upazila/GetDropDownList',
        contentType: "application/json; charset=utf-8",
        headers: authHeaders,
        dataType: 'json'
    }).success(function (data) {
        $scope.upazilaList = data;
    });

    $scope.salesManList = [];
    $http({
        method: 'GET',
        url: '/api/SalesMen/GetDropDownList',
        contentType: "application/json; charset=utf-8",
        headers: authHeaders,
        dataType: 'json'
    }).success(function (data) {
        $scope.salesManList = data;
    });

    $scope.submitCustomerForm = function () {
        // Set the 'submitted' flag to true
        $scope.submitted = true;
        if ($scope.customerForm.$valid) {
            var aInsertObj = {
                UpazilaId: $scope.customer.UpazilaId.UpazilaId,
                CustomerName: $scope.customer.CustomerName,
                CustomerNameBangla: $scope.customer.CustomerNameBangla,
                SalesManId: $scope.customer.SalesManId.SalesManId,
                Address: $scope.customer.Address,
                AddressBangla: $scope.customer.AddressBangla,
                Phone: $scope.customer.Phone,
                Email: $scope.customer.Email,
                ShowRoomId: 0,
                CreditLimit: $scope.customer.CreditLimit,
                ShopName: $scope.customer.ShopName
            };

            $http({
                url: "/api/Customer",
                data: aInsertObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                //Customer Image processing
                var file = $scope.myFile;
                var fileName = "";
                if (file) {
                    fileName = data.CustomerId + '.jpg';
                }
                if (fileName) {
                        upload({
                            url: 'Home/UploadCustomerImage',
                            method: 'POST',
                            data: {
                                aFile: $scope.myFile,
                                CustomerId: data.CustomerId
                            }
                        })
                        .then(
                        function (response) {
                            console.log(response.data);
                        },
                        function (response) {
                            console.error(response);
                        });
                        //Update Image Name
                        $http({
                            url: '/api/Customer/UpdateImageNameToCustomers/' + data.CustomerId ,
                            method: "PUT",
                            headers: authHeaders
                        }).success(function (data) {
                            $('.image-preview').attr("data-content", "").popover('hide');
                            $('.image-preview-filename').val("");
                            $('.image-preview-clear').hide();
                            $('.image-preview-input input:file').val("");
                            $(".image-preview-input-title").text("Browse");
                        }).error(function (error) {

                        });
                        //End Update Image Name
                }

                //View Object
                var aViewObj = {
                    id: data.CustomerId,
                    name: $scope.customer.CustomerName,
                    group: $scope.customer.UpazilaId.UpazilaId,
                    groupName: $scope.customer.UpazilaId.UpazilaName,
                    status: $scope.customer.SalesManId.SalesManId,
                    statusName: $scope.customer.SalesManId.SalesManName,
                    Address: $scope.customer.Address,
                    Phone: $scope.customer.Phone,
                    Email: $scope.customer.Email,
                    CustomerNameBangla: $scope.customer.CustomerNameBangla,
                    AddressBangla: $scope.customer.AddressBangla,
                    CreditLimit: $scope.customer.CreditLimit,
                    ShopName: $scope.customer.ShopName
                };
                $scope.users.push(aViewObj);
                $scope.myFile = "";
                $scope.customer = {};
                $scope.submitted = false;
                $scope.customerForm.$setPristine();
                $scope.customerForm.$setUntouched();
                $scope.loading = true;
                angular.element('#CustomerName').focus();
                $scope.message = "Successfully Customer Saved.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (error) {
                $scope.message = 'Unable to save Customer' + error.message;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }
        else {
            alert("Please  correct form errors!");
        }
    };
    //Select single customer data
    $scope.getSingle = function (customer) {
        $scope.loading = false;
        $scope.addButton = true;
        $scope.updateButton = false;

        $http({
            method: 'GET',
            url: '/api/Customer/GetSingleCustomer/' + customer.id,
            contentType: "application/json; charset=utf-8",
            headers: authHeaders,
            dataType: 'json'
        }).success(function (data) {
            
            var showCustomer = {
                CustomerId: data[0].id,
                CustomerName: data[0].name,
                Address: data[0].Address,
                SalesManId: {
                    SalesManId: data[0].status, SalesManName: data[0].statusName
                },
                UpazilaId: {
                    UpazilaId: data[0].group, UpazilaName: data[0].groupName
                },
                Phone: data[0].Phone,
                Email: data[0].Email,
                CustomerNameBangla : data[0].CustomerNameBangla,
                AddressBangla: data[0].AddressBangla,
                CreditLimit: data[0].CreditLimit,
                ShopName: data[0].ShopName,
            };
            $scope.customer = showCustomer;
            $scope.loading = true;
        });

    };

    // remove user
    $scope.removeUser = function (index, CustomerId) {

        deleteDepartment = confirm('Are you sure you want to delete the customer?');
        if (deleteDepartment) {
            //Your action will goes here
            $http.delete('/api/Customer/' + CustomerId)
                .success(function (data) {
                    $scope.users.splice(index, 1);
                })
                .error(function (data) {
                    $scope.error = "An error has occured while deleting! " + data;
                });
            //alert("Deleted successfully!!");
        }
    };

    $scope.cancel = function () {
        $scope.addButton = false;
        $scope.updateButton = true;
        $scope.customer = {};
        $scope.submitted = false;
        $scope.customerForm.$setPristine();
        $scope.customerForm.$setUntouched();
        angular.element('#CustomerName').focus();
    };
    //customer Update Operation
    $scope.update = function (customer) {
        $scope.loading = false;
        var Id = $scope.customer.CustomerId;
        var salesMan = $scope.customer.SalesManId.SalesManName;
        var upazila = $scope.customer.UpazilaId.UpazilaName;
        var file = $scope.myFile;
        var fileName = "";
        if (file) {
            fileName =  Id+'.jpg';
        }
        $scope.customer.SalesManId = $scope.customer.SalesManId.SalesManId;
        $scope.customer.UpazilaId = $scope.customer.UpazilaId.UpazilaId;
        $scope.customer.CustomerName = $scope.customer.CustomerName;
        $scope.customer.Address = $scope.customer.Address;
        $scope.customer.Phone = $scope.customer.Phone;
        $scope.customer.Email = $scope.customer.Email;
        $scope.customer.ShowRoomId = 0;
        $scope.customer.CustomerNameBangla = $scope.customer.CustomerNameBangla;
        $scope.customer.AddressBangla = $scope.customer.AddressBangla;
        $scope.customer.CreditLimit = $scope.customer.CreditLimit;
        $scope.customer.ShopName = $scope.customer.ShopName;
        $scope.customer.Image = fileName;
        //if (fileName === '' || fileName === null) {
        //    $scope.message = "Please Select Profile Image.";
        //    $scope.messageType = "warning";
        //    $scope.clientMessage = false;
        //    $timeout(function () { $scope.clientMessage = true; }, 5000);
        //    return false;
        //}


        $http({
            url: '/api/Customer/' + $scope.customer.CustomerId,
            data: $scope.customer,
            method: "PUT",
            headers: authHeaders
        }).success(function (data) {

            if (fileName) {
                upload({
                    url: 'Home/UploadCustomerImage',
                    method: 'POST',
                    data: {
                        aFile: $scope.myFile,
                        CustomerId:Id
                    }
                })
                    .then(
                    function (response) {
                        console.log(response.data);
                    },
                    function (response) {
                        console.error(response);
                    });
            }
            var aViewObj = {
                id: data.CustomerId,
                name: $scope.customer.CustomerName,
                group: data.group,
                groupName: upazila,
                status: data.status,
                statusName: salesMan,
                Address: $scope.customer.Address,
                Phone: $scope.customer.Phone,
                Email: $scope.customer.Email,
                CreditLimit: $scope.customer.CreditLimit,
                CustomerNameBangla: $scope.customer.CustomerNameBangla,
                AddressBangla: $scope.customer.AddressBangla,
                ShopName: $scope.customer.ShopName
            };

            $.each($scope.users, function (i) {
                if ($scope.users[i].id === Id) {
                    $scope.users[i] = aViewObj;
                    return false;
                }
            });
            $('.image-preview').attr("data-content", "").popover('hide');
            $('.image-preview-filename').val("");
            $('.image-preview-clear').hide();
            $('.image-preview-input input:file').val("");
            $(".image-preview-input-title").text("Browse");
            $scope.myFile = "";
            $scope.customer = {};
            $scope.Message = 'Customer Successfull Updated .';
            $scope.successMessage = false;
            $timeout(function () { $scope.successMessage = true; }, 3000);
            $scope.customerForm.$setPristine();
            $scope.customerForm.$setUntouched();
            $scope.addButton = false;
            $scope.updateButton = true;
            angular.element('#CustomerName').focus();
            $scope.loading = true;
            $scope.message = "Successfully Customer Updated.";
            $scope.messageType = "info";
            $scope.clientMessage = false;
            $timeout(function () { $scope.clientMessage = true; }, 5000);
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


    $scope.doUpload = function () {
        upload({
            url: 'Home/upload',
            method: 'POST',
            data: {
                aFile: $scope.myFile
            }
        }).then(
          function (response) {
              console.log(response.data);
          },
          function (response) {
              console.error(response);
          });
    };



}])




