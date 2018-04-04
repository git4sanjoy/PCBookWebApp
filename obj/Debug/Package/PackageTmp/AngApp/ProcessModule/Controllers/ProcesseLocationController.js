//var app = angular.module('PCBookWebApp');
//app.controller('ProcesseLocationsController', ['$scope', '$location', '$http', '$timeout', '$filter',
//    function ($scope, $location, $http, $timeout, $filter) {
//        $scope.message = "Processe Location Controller";
//    }])

var app = angular.module('PCBookWebApp');
app.controller('ProcesseLocationsController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {

        $scope.message = "Processe Location Controller";

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

        $scope.ProcesseLicationObj = [];
        $scope.loadData = function () {
            $http({
                url: "/api/ProcesseLocations/GetProcesseLocationsList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.ProcesseLicationObj = data;
                //console.log(data);
            }).error(function (error) {
                $scope.message = "An error has occured while loading Process location! " + data;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });
        }
        $scope.loadData();


        $scope.Save = function (processeLocation) {
            console.log(processeLocation);
            $http({
                url: "/api/ProcesseLocations",
                data: processeLocation,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                $scope.Cancel();
                $scope.loadData();
                $scope.message = "Data saved successfully.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }).error(function (data) {
                $scope.message = "Data saving attempt failed!";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });

        };

        $scope.updateProcessLocation = function (data, id) {
            /// alert(id2);
            //alert(PurchasedProductId);
            var obj = {
                ProcesseLocationId: id,
                ProcesseLocationName: data.ProcesseLocationName,
                ShowRoomId: data.ShowRoomId
            };
            //alert(obj.PurchasedProductId);
            angular.extend(data, { ProcesseLocationId: id });
            return $http({
                url: '/api/ProcesseLocations/' + id,
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
            userPrompt = confirm('Are you sure you want to delete the Processe Locations?');
            if (userPrompt) {
                $http({
                    url: '/api/ProcesseLocations/' + id,
                    method: "DELETE",
                    headers: authHeaders
                }).success(function (data) {
                    $scope.loadData();
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

        //**Cancel Button**
        $scope.Cancel = function () {
            $scope.processeLocation = '';
            $('.saveButton').show();
            $scope.entryForm.$setPristine();
            $scope.entryForm.$setUntouched();
        };





        //function saveData() {

        //    var aProcesseObj = {
        //        ProcesseLocationName: $scope.process.ProcesseLocationName,
        //        Active: $scope.process.IsActive
        //    };
        //    //console.log(aProcesseObj);
        //    $http({
        //        url: "/api/ProcesseLocations",
        //        data: aProcesseObj,
        //        method: "POST",
        //        headers: authHeaders
        //    }).success(function (data) {
        //        loadData();
        //        makeEmpty();

        //        $scope.message = "Successfully Process Location Updated.";
        //        $scope.messageType = "info";
        //        $scope.clientMessage = false;
        //        $timeout(function () { $scope.clientMessage = true; }, 5000);

        //    }).error(function (error) {
        //        $scope.message = "An error has occured while saving Process location! " + data;
        //        $scope.messageType = "warning";
        //        $scope.clientMessage = false;
        //        $timeout(function () { $scope.clientMessage = true; }, 5000);
        //    });

        //}

        //$scope.remove = function (index, id) {
        //    userPrompt = confirm('Are you sure you want to delete the Process List?');
        //    if (userPrompt) {
        //        $.ajax({
        //            url: '/api/ProcesseLocations/' + id,
        //            type: 'DELETE',
        //            dataType: 'json',
        //            success: function (data) {
        //                loadData();

        //                $scope.ProcesseLicationObj.splice(index, 1);
        //                $scope.message = "Successfully Deleted.";
        //                $scope.messageType = "danger";
        //                $scope.clientMessage = false;
        //                $timeout(function () { $scope.clientMessage = true; }, 50000);
        //            },
        //            error: function () {
        //                $scope.message = "An error has occured while deleting Process List! " + data;
        //                $scope.messageType = "warning";
        //                $scope.clientMessage = false;
        //                $timeout(function () { $scope.clientMessage = true; }, 5000);
        //            }
        //        });
        //    }
        //}



    }])