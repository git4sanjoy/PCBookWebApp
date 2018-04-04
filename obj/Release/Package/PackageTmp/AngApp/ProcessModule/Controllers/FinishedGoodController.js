
var app = angular.module('PCBookWebApp');
app.controller('FinishedGoodController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {
        $scope.message = "PC App. V-1.0.1";

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
        $scope.data = {
            cb1: true
        };

        //**Change Select Product Type**
        $scope.changeSelectProductType = function (item) {
            $scope.finishedGood.ProductTypeId= item;
        };


        //**Get All List**
        $scope.GetAllList = function () {
            $http({
                url: "/api/FinishedGoods/GetAllList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.List = data.list;
                $scope.ProductTypeList = data.productTypeList;
                
            }).error(function (data) {
                $scope.message = "Process list loading failed.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                //toastr.warning("Supplier list loading failed.", "Failed!");
            });
        }
        $scope.GetAllList();

        //**Save Finished Goods**
        $scope.Save = function (finishedGood) {

            console.log(finishedGood);      
            var finishedGoodObj = {
                FinishedGoodName: finishedGood.FinishedGoodName,
                DesignNo: finishedGood.DesignNo,
                ProductTypeId: finishedGood.ProductTypeId
                //ProductTypeId: finishedGood.ProductTypeId.ProductTypeId
            };
            $http({
                url: "/api/FinishedGoods",
                data: finishedGoodObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetAllList();
                $scope.message = "Data saved successfully.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                $scope.data = {
                    cb1: false
                };
            }).error(function (data) {
                $scope.message = "Data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });

        };

        //**Update Finished Goods**
        $scope.Update = function (finishedGood) {   
            //finishedGood.ProductTypeId = $scope.finishedGood.ProductTypeId.ProductTypeId;
            $http({
                url: '/api/FinishedGoods/' + finishedGood.FinishedGoodId,
                data: finishedGood,
                method: "PUT",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.GetAllList();
                $scope.message = "Updated successfully.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $scope.data = {
                    cb1: false
                };
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "Data could not be updated!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);                
            });

        };

        //**Delete Finished Goods**r
        $scope.Delete = function (item) {
            
            var msg = confirm("Do you want to delete this data?");
            if (msg == true) {
                $http({
                    url: "/api/FinishedGoods/" + item.FinishedGoodId,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.message = "Data deleted successfully.";
                    $scope.messageType = "danger";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);                    
                    $scope.Cancel();
                    $scope.GetAllList();
                    $scope.data = {
                        cb1: false
                    };
                }).error(function (data) {
                    alert('error occord')
                    $scope.message = "Data could not be deleted!";
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);                   
                });
            };

        };

        //**Edit Button**
        $scope.Edit = function (item) {
            $scope.finishedGood = angular.copy(item);
            //$scope.finishedGood.ProductTypeId = { ProductTypeId: item.ProductTypeId , ProductTypeName: item.ProductTypeName };
            $scope.editMode = true;
        };

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.finishedGood = '';           
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
            $scope.editMode = false;
            $scope.data = {
                cb1: false
            };
        };

       
    }])
