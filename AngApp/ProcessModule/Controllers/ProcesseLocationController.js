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
            console.log(authHeaders);
        }

        loadData();
        
        $scope.ProcesseLicationObj = [];

        $scope.submintForm = function () {
           
            var isValid = CheckValidation();
            if (isValid) {
                if ($('#saveProcesssLocation').text().trim() == 'Save Location') {
                    saveData();
                }
            }
        };
        
        $scope.updateProcessLocation = function (data, id) {
            var aProcesseObj = {
                ProcesseLocationId: id,
                ProcesseLocationName: data.ProcesseLocationName,
                Active: $('#IsActive').prop('checked')
            };
            $.ajax({
                url: '/api/ProcesseLocations/' + id,
                type: 'PUT',
                data: aProcesseObj,
                dataType: 'json',
                success: function (data) {
                    loadData();
                    makeEmpty();

                    $scope.message = "Successfully Process Location Updated.";
                    $scope.messageType = "info";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);

                },
                error: function () {
                    $scope.message = "An error has occured while updateing Process location! " + data;
                    $scope.messageType = "warning";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                }
            });
        }
        $('#clear').click(function () {
            makeEmpty();
        });

        function loadData() {
            $http({
                url: "/api/ProcesseLocations",
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
        
        function saveData() {
            
            var aProcesseObj = {
                ProcesseLocationName: $scope.process.ProcesseLocationName,
                Active: $scope.process.IsActive
            };
            //console.log(aProcesseObj);
            $http({
                url: "/api/ProcesseLocations",
                data: aProcesseObj,
                method: "POST",
                headers: authHeaders
            }).success(function (data) {
                loadData();
                makeEmpty();

                $scope.message = "Successfully Process Location Updated.";
                $scope.messageType = "info";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);

            }).error(function (error) {
                $scope.message = "An error has occured while saving Process location! " + data;
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            });

        }
        
        $scope.remove = function (index, id) {
            userPrompt = confirm('Are you sure you want to delete the Process List?');
            if (userPrompt) {
                $.ajax({
                    url: '/api/ProcesseLocations/' + id,
                    type: 'DELETE',
                    dataType: 'json',
                    success: function (data) {
                        loadData();

                        $scope.ProcesseLicationObj.splice(index, 1);
                        $scope.message = "Successfully Deleted.";
                        $scope.messageType = "danger";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 50000);
                    },
                    error: function () {
                        $scope.message = "An error has occured while deleting Process List! " + data;
                        $scope.messageType = "warning";
                        $scope.clientMessage = false;
                        $timeout(function () { $scope.clientMessage = true; }, 5000);
                    }
                });
            }
        }

        function makeEmpty() {
            $('#locationName').val("");
            $('#saveProcesssLocation').text("Save Location");
           
            $("#IsActive").attr('checked', false);
            $('#hdId').val('hdId777');
        }

        function CheckValidation() {
            var isValid = true;
            if ($('#locationName').val().trim() =='') {
                isValid = false;
                $('#locationName').parent().prev().find('span').css('visibility', 'visible');
            }
            else {
                $('#locationName').parent().prev().find('span').css('visibility', 'hidden');
            }
            return isValid;
        }

    }])