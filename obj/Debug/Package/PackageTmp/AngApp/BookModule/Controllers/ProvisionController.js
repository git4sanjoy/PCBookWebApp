var app = angular.module('PCBookWebApp');

app.controller('ProvisionController', ['$scope', '$location', '$http', '$timeout', '$filter',
    function ($scope, $location, $http, $timeout, $filter) {

        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";
        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }


        var inputYears = [];
        var startYear = 2008;
        var currentTime = new Date()
        // returns the month (from 0 to 11)
        month_value = currentTime.getMonth();
        var month = currentTime.getMonth() + 1
        var day = currentTime.getDate()
        var year = currentTime.getFullYear()
        var endYear = year;
        for (var i = startYear; i <= endYear; i++) {
            inputYears.push(i);
        }

        $scope.yearList = inputYears;
        $scope.yearListSelectedData = endYear;
        $scope.dashbordYearListSelectedData = endYear;

        //Search List
        var monthsList = new Array(12);
        monthsList[0] = "January";
        monthsList[1] = "February";
        monthsList[2] = "March";
        monthsList[3] = "April";
        monthsList[4] = "May";
        monthsList[5] = "June";
        monthsList[6] = "July";
        monthsList[7] = "August";
        monthsList[8] = "September";
        monthsList[9] = "October";
        monthsList[10] = "November";
        monthsList[11] = "December";

        var current_date = new Date();
        month_value = current_date.getMonth();
        day_value = current_date.getDate();
        year_value = current_date.getFullYear();

        $scope.monthDdl = { id: month, name: monthsList[month_value] };
        $scope.dashbordMonth = monthsList[month_value];
        //Display List
        $scope.months = [
            { id: 1, name: 'January' },
            { id: 2, name: 'February' },
            { id: 3, name: 'March' },
            { id: 4, name: 'April' },
            { id: 5, name: 'May' },
            { id: 6, name: 'June' },
            { id: 7, name: 'July' },
            { id: 8, name: 'August' },
            { id: 9, name: 'September' },
            { id: 10, name: 'October' },
            { id: 11, name: 'November' },
            { id: 12, name: 'December' }
        ];
        $scope.users = [];
        $http({
                url: "/api/Provisions/GetLedgerXeditList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.ledgerList = data;
            });

        $scope.provision = {
            openingAmount: 0,
            provisionAmount: 0,
            actualAmount:0
        };
        $scope.addNewProvisionItem = function () {
            $scope.users.push({
                groupName: $scope.provision.ledgerListDdl.text,
                group: $scope.provision.ledgerListDdl.id,
                OpeningAmount:$scope.provision.openingAmount,
                ProvisionAmount: $scope.provision.provisionAmount,
                ActualAmount: $scope.provision.actualAmount
            });
            $scope.provision = {
                ledgerListDdl: {},
                openingAmount: 0,
                provisionAmount: 0,
                actualAmount: 0
            };
        };
        $scope.saveProvision = function () {
            var month = $scope.monthDdl.id;
            var year = $scope.dashbordYearListSelectedData;
            //alert(year+'-'+month+'-01');
            var provisionItems = [];
            angular.forEach($scope.users, function (item) {
                provisionItems.push({
                    ProvisionId: 0,
                    ProvisionDate: year + '-' + month + '-01',
                    LedgerId: item.group,
                    ProvisionAmount: item.ProvisionAmount,
                    ActualAmount: item.ActualAmount,
                    OpeningAmount: item.OpeningAmount + item.ProvisionAmount - item.ActualAmount,
                    ShowRoomId: 0
                });
            })
            //console.log(provisionItems);
            $http({
                    url: "/api/Provisions/PostProvision",
                    data: provisionItems,
                    method: "POST",
                    headers: authHeaders
                }).success(function (data) {
                    //$http({
                    //    url: "/api/Provisions/GetProvisionList",
                    //    method: "GET",
                    //    headers: authHeaders
                    //}).success(function (data) {
                    //    $scope.users = data;
                    //});
                    $scope.message = "Successfully Unit Created.";
                    $scope.messageType = "success";
                    $scope.clientMessage = false;
                    $timeout(function () { $scope.clientMessage = true; }, 5000);
                    $scope.users = {};
                }).error(function (error) {
                    $scope.validationErrors = [];
                    if (error.ModelState && angular.isObject(error.ModelState)) {
                        for (var key in error.ModelState) {
                            $scope.validationErrors.push(error.ModelState[key][0]);
                        }
                    } else {
                        $scope.validationErrors.push('Unable to add Unit.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);
                });
        };

        $scope.loadLastProvision = function () {
            $scope.users = [];
            $http({
                url: "/api/Provisions/GetProvisionListByLastDate",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.users = data;
            });
        };



        //$scope.users = [];
        //$http({
        //    url: "/api/Provisions/GetProvisionList",
        //    method: "GET",
        //    headers: authHeaders
        //}).success(function (data) {
        //    $scope.users = data;
        //});


        $scope.groups = [];
        $scope.loadGroups = function () {
            return $scope.groups.length ? null : $http({
                url: "/api/Provisions/GetLedgerXeditList",
                method: "GET",
                headers: authHeaders
            }).success(function (data) {
                $scope.groups = data;
            });
        };



        $scope.showGroup = function (user) {
            if (user.group && $scope.groups.length) {
                var selected = $filter('filter')($scope.groups, { id: user.group });
                return selected.length ? selected[0].text : 'Not set';
            } else {
                return user.groupName || 'Not set';
            }
        };



        $scope.saveUser = function (data, id) {
            //$scope.user not updated yet
            angular.extend(data, { id: id });
            var month = $scope.monthDdl.id;
            var year = $scope.dashbordYearListSelectedData;
            var productObj = {
                ProvisionId: id,
                ProvisionDate: year + '-' + month + '-01',
                LedgerId: data.group,
                OpeningAmount: data.OpeningAmount,
                ProvisionAmount: data.ProvisionAmount,
                ActualAmount: data.ActualAmount,
                ShowRoomId: 0
            };

            if (id == 0) {
                //$http({
                //    url: "/api/Provisions/PostProvision",
                //    data: productObj,
                //    method: "POST",
                //    headers: authHeaders
                //}).success(function (data) {
                //    //$http({
                //    //    url: "/api/Provisions/GetProvisionList",
                //    //    method: "GET",
                //    //    headers: authHeaders
                //    //}).success(function (data) {
                //    //    $scope.users = data;
                //    //});
                //    $scope.message = "Successfully Unit Created.";
                //    $scope.messageType = "success";
                //    $scope.clientMessage = false;
                //    $timeout(function () { $scope.clientMessage = true; }, 5000);
                //}).error(function (error) {
                //    $scope.validationErrors = [];
                //    if (error.ModelState && angular.isObject(error.ModelState)) {
                //        for (var key in error.ModelState) {
                //            $scope.validationErrors.push(error.ModelState[key][0]);
                //        }
                //    } else {
                //        $scope.validationErrors.push('Unable to add Unit.');
                //    };
                //    $scope.messageType = "danger";
                //    $scope.serverMessage = false;
                //    $timeout(function () { $scope.serverMessage = true; }, 5000);
                //});

            } else {
                //alert('Updated');
                //$http({
                //    url: '/api/Provisions/' + id,
                //    data: productObj,
                //    method: "PUT",
                //    headers: authHeaders
                //}).success(function (data) {
                //    //alert('Updated');
                //    $scope.message = "Successfully Unit Updated.";
                //    $scope.messageType = "info";
                //    $scope.clientMessage = false;
                //    $timeout(function () { $scope.clientMessage = true; }, 5000);
                //}).error(function (error) {
                //    $scope.validationErrors = [];
                //    if (error.ModelState && angular.isObject(error.ModelState)) {
                //        for (var key in error.ModelState) {
                //            $scope.validationErrors.push(error.ModelState[key][0]);
                //        }
                //    } else {
                //        $scope.validationErrors.push('Unable to Update project.');
                //    };
                //    $scope.messageType = "danger";
                //    $scope.serverMessage = false;
                //    $timeout(function () { $scope.serverMessage = true; }, 5000);
                //});
            }
        };


        $scope.remove = function (index) {
            $scope.users.splice(index, 1);
            //deleteProduct = confirm('Are you sure you want to delete the Unit?');
            //if (deleteProduct) {
            //    $http({
            //        url: "/api/Units/" + UnitId,
            //        method: "DELETE",
            //        headers: authHeaders
            //    }).success(function (data) {
            //        $scope.users.splice(index, 1);
            //        $scope.message = "Successfully Unit Deleted.";
            //        $scope.messageType = "danger";
            //        $scope.clientMessage = false;
            //        $timeout(function () { $scope.clientMessage = true; }, 5000);

            //    }).error(function (data) {
            //        $scope.message = "An error has occured while deleting! " + data;
            //        $scope.messageType = "warning";
            //        $scope.clientMessage = false;
            //        $timeout(function () { $scope.clientMessage = true; }, 5000);
            //    });
            //}
        };

        $scope.checkName = function (data, id) {
            //if (id === 2 && data !== 'awesome') {
            //    return "Username 2 should be `awesome`";
            //}
        };

        //// remove user
        //$scope.removeUser = function (index) {
        //    $scope.users.splice(index, 1);
        //};

        // add user
        $scope.addUser = function () {
            $scope.inserted = {
                //id: $scope.users.length + 1,
                id: 0,
                name: '',
                group: null
            };
            $scope.users.push($scope.inserted);
        };


    }]);
