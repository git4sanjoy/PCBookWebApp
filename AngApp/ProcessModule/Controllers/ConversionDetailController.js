
var app = angular.module('PCBookWebApp');
app.controller('ConversionDetailController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.ConversionDetailObj = [];
        var accesstoken = sessionStorage.getItem('accessToken');
        $scope.message = "Conversion Controller";

        $scope.loading = true;

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.pageSize = 20;
        $scope.currentPage = 1;

        var authHeaders = {};

        if (accesstoken) {

            authHeaders.Authorization = 'Bearer ' + accesstoken;
            console.log(authHeaders);
        }

        displayData();
        loadPproductOption();
        loadOption1();

        //**Get Purchase Product List**
        $scope.GetPurchaseProductList = function () {
            $http({
                url: "/api/ConversionDetails/GetPurchaseProductList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.PurchaseProductList = data.list;
                $scope.ConversionList = data.list2;
            }).error(function (data) {
                $scope.message = "Purchase Product list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetPurchaseProductList();

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

            if ($('#saveConversionDetail').text().trim() == 'Save') {
                saveData();
            }
            else {
                updateData();
            }
        };

        $('#clear').click(function () {
            makeEmpty();
        });

        function loadOption1() {
           
            $.ajax
                ({
                    url: 'api/PurchasedProducts/',
                    type: 'GET',
                    datatype: 'application/json',
                    contentType: 'application/json',
                    headers: authHeaders,
                    success: function (data) {
                        var options = $("#detailsType1");
                        $.each(data, function (index, item) {
                            options.append($("<option />").val(item.PurchasedProductId).text(item.PurchasedProductName));
                        });
                    },
                    error: function () {
                        alert("Whooaaa! Something went wrong..")
                    },
                });
        }

        function loadPproductOption() {
            $.ajax
                ({
                    url: '/api/Conversions',
                    type: 'GET',
                    datatype: 'application/json',
                    contentType: 'application/json',

                    success: function (data) {
                        var options = $("#conversion");
                        $.each(data, function (index, item) {
                            options.append($("<option />").val(item.ConversionId).text(item.ConversionName));
                        });
                    },
                    error: function () {
                        alert("Whooaaa! Something went wrong..")
                    },
                });
        }

        function displayToEdit() {

            $.ajax({
                url: '/api/ConversionDetails/' + $("#hdId").val(),
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    $('#conversion').val(data.ConversionId);
                    $('#detailsType1').val(data.PurchaseProductId);
                    $('#saveConversionDetail').text('Update');
                },
                error: function () {
                    alert('error');
                }
            });
        }

        function saveData() {
            //if ($('#conversion').val() == 'Select Name') {
                
            //}

            var ConversionDetails = {
                ConversionId: $('#conversion').val(),
                PurchaseProductId: $('#detailsType1').val()

            };
            $.ajax({
                url: '/api/ConversionDetails/',
                type: 'POST',
                data: ConversionDetails,
                dataType: 'json',
                success: function (data) {
                    if (data == 0) {
                        $scope.message = "This Product already Exist for this Convertion.";
                        $scope.messageType = "warning";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                    }
                    else {
                        $scope.message = "Data saved successfully.";
                        //$scope.message = "Product Type data saved successfully.";
                        $scope.messageType = "success";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                    }
                    displayData();
                    makeEmpty();
                    
                },
                error: function () {
                    alert('error');
                }
            });

        }

        function displayData() {
           
            $http({
                url: "/api/ConversionDetails/",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                console.log(data);
                $scope.ConversionDetailObj = data;
                //console.log(data);
            }).error(function (error) {
                alert('er');
            });
        }



        $scope.updateData = function (data, id, id1, id2) {

            //alert(id);
            //alert(id1);
            //alert(id2);
            
            var obj = {
                ConversionDetailsId: id,
                ConversionId: String(id1),
                PurchaseProductId: String(id2),
               
            };
            //alert(obj.PurchasedProductId);
            angular.extend(data, { ConversionDetailsId : id });

            return $http({                
                url: '/api/ConversionDetails/' + id,
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


        function deleteData() {

            $.ajax({
                url: '/api/ConversionDetails/' + $("#hdId").val(),
                type: 'DELETE',
                dataType: 'json',
                success: function (data) {
                    displayData();
                },
                error: function () {
                    alert('error');
                }
            });
        }

        function makeEmpty() {
            $('#detailsType1').val("Select Name");
            
            $('#saveConversionDetail').text("Save");
            $('#hdId').val('hdId777');
        }
    }])