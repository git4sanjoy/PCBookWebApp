
var app = angular.module('PCBookWebApp');
app.controller('ConversionController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.ConversionObj = [];

        $scope.message = "Conversion Controller";

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
            console.log(authHeaders);
        }

        ////**Get Purchase Product List**
        //$scope.GetPurchaseProductList = function () {
        //    $http({
        //        url: "/api/Conversions/GetPurchaseProductList",
        //        method: "GET",
        //        headers: authHeaders
        //    }).success(function (data) {
        //        $scope.PurchaseProductList = data;
        //    }).error(function (data) {
        //        $scope.message = "Purchase Product list loading failed.";
        //        $scope.messageType = "warning";
        //        $scope.clientMessage = false;
        //        $timeout(function () { $scope.clientMessage = true; }, 5000);
        //        //toastr.warning("Supplier list loading failed.", "Failed!");
        //    });
        //}
        //$scope.GetPurchaseProductList();




        displayData();
        loadPproductOption();
        function loadPproductOption() {
            $.ajax
                ({
                    url: '/api/PurchasedProducts',
                    type: 'GET',
                    datatype: 'application/json',
                    contentType: 'application/json',

                    success: function (data) {
                        var options = $("#pProductId");
                        $.each(data, function (index, item) {
                            options.append($("<option />").val(item.PurchasedProductId).text(item.PurchasedProductName));
                        });
                    },
                    error: function () {
                        alert("Whooaaa! Something went wrong..")
                    },
                });
        }

        $(document).on("click", "#tableData a", function (e) {

            $("#hdId").val(this.id)
            if ($(this).text() == 'Edit') {
                //
                displayToEdit();
            }
            else if ($(this).text() == 'Delete') {
                alert('Do you want ot delete this item?');
                deleteData();
            }
        });
        
        $scope.submintForm = function () {
            
            if ($('#saveConversion').text().trim() == 'Save Conversion') {
                saveData();
            }
            else {
                updateData();
            }
        };

        $('#clear').click(function () {
            makeEmpty();
        });

        
        function displayToEdit() {

            $.ajax({
                url: '/api/Conversions/' + $("#hdId").val(),
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    console.log(data.Active);
                    $('#conversionName').val(data.ConversionName);
                    $('#pProductId').val(data.PurchaseProductId);
                    //$('#IsActive').prop('checked', data.Active);
                    $('#saveConversion').text('Update Conversion');
                    
                },
                error: function () {
                    alert('error');
                }
            });
        }

        function saveData() {

            //var isValid = CheckValidation();
            //if (isValid) {
                var Conversion = {
                    ConversionName: $('#conversionName').val(),
                    PurchaseProductId: $('#pProductId').val()
                };
                $.ajax({
                    url: '/api/Conversions/',
                    type: 'POST',
                    data: Conversion,
                    dataType: 'json',
                    success: function (data) {
                        displayData();
                        makeEmpty();

                        $scope.submitted = false;
                        $scope.ProcessListForm.$setPristine();
                        $scope.ProcessListForm.$setUntouched();
                        $scope.loading = true;
                        $scope.message = "Successfully Conversion Created.";

                        $scope.messageType = "success";
                        $route.reload();
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                    },
                    error: function () {
                        alert('error');
                    }
                });
            //}
        }

        function displayData() {
            $http({
                url: "/api/Conversions/",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.ConversionObj = data;
                console.log(data);
            }).error(function (error) {
                alert('er');
            });
        }

        
        $scope.updateData = function(data, id) {

           
           /// alert(id2);
            //alert(PurchasedProductId);
            var obj = {
                ConversionId: id,
                ConversionName: data.ConversionName
            };

            //alert(obj.PurchasedProductId);
            angular.extend(data, { ConversionId: id });           
           

            return $http({
                url: '/api/Conversions/' + id,
                data: obj,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.message = "Successfully Updated.";
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
                    $scope.validationErrors.push('Unable to Update Sub Material.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        }

        
        $scope.remove = function (index, id) {
           // alert(id)
            userPrompt = confirm('Are you sure you want to delete the Process List?');
            if (userPrompt) {
                $http({
                    url: "/api/Conversions/" + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    displayData();
                    $scope.ProcessLists.splice(index, 1);
                    $scope.message = "Successfully Deleted.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }).error(function (data) {
                    $scope.message = "An error has occured while deleting Process List! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                });
            }

        };

        function makeEmpty() {
            $('#conversionName').val("");
            $('#saveConversion').text("Save Conversion");

            //$("#IsActive").attr('checked', false);
            $('#hdId').val('hdId777');
        }

        function CheckValidation() {
            var isValid = true;
            if ($('#conversionName').val().trim() == '') {
                isValid = false;
                $('#conversionName').parent().prev().find('span').css('visibility', 'visible');
            }
            else {
                $('#conversionName').parent().prev().find('span').css('visibility', 'hidden');
            }

            if ($('#pProductId').val().trim() == '') {
                isValid = false;
                $('#pProductId').parent().prev().find('span').css('visibility', 'visible');
            }
            else {
                $('#pProductId').parent().prev().find('span').css('visibility', 'hidden');
            }
            return isValid;
        }


    }])