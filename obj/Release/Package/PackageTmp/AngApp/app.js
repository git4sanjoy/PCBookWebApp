﻿var app = angular.module('PCBookWebApp', ['ngRoute',
                                        'ngAria',
                                        'ngMessages',
                                        'ngAnimate',
                                        'ui.bootstrap',
                                        'ngResource',
                                        'angularMoment',
                                        'LocalStorageModule',
                                        'angularjs-dropdown-multiselect',
                                        'angular-loading-bar', 'xeditable', 'ngMaterial', 'lr.upload', 'angular.filter', 'chart.js'])

//global veriable for store service base path
app.filter('groupBy', function () {
    return _.memoize(function (items, field) {
        return _.groupBy(items, field);
    });
});
app.factory('Excel', function ($window) {
    var uri = 'data:application/vnd.ms-excel;base64,',
        template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>',
        base64 = function (s) { return $window.btoa(unescape(encodeURIComponent(s))); },
        format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) };
    return {
        tableToExcel: function (tableId, worksheetName) {
            var table = $(tableId),
                ctx = { worksheet: worksheetName, table: table.html() },
                href = uri + base64(format(template, ctx));
            return href;
        }
    };
});
app.filter('sumByKey', function () {
    return function (data, key) {
        if (typeof (data) === 'undefined' || typeof (key) === 'undefined') {
            return 0;
        }

        var sum = 0;
        for (var i = data.length - 1; i >= 0; i--) {
            sum += parseFloat(data[i][key]);
        }

        return sum;
    };
});

app.config(['$locationProvider', '$routeProvider', '$ariaProvider',

    function ($locationProvider, $routeProvider, $ariaProvider) {

        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        }).hashPrefix('!');

        $ariaProvider.config({
            tabindex: false,
            ariaHidden: false
        });

        $routeProvider
        .when('/', { 
            templateUrl: '/AngApp/Views/Home.html',
            controller: 'homeController'
        })
        .when('/Dashboard', {
            templateUrl: '/AngApp/BookModule/Views/Dashboard.html',
            controller: 'dashboardController'
        }) 
        .when('/Bookkeeping', {
            templateUrl: '/AngApp/BookModule/Views/Bookkeeping.html',
            controller: 'BookkeepingController'
        })

            .when('/Invoice', { // For Home Page
                templateUrl: '/AngApp/SalesModule/Views/Cart.html',
                controller: 'CartController'
            })
            .when('/MainCategory', {
                templateUrl: '/AngApp/SalesModule/Views/MainCategory.html',
                controller: 'MainCategoryController'
            })
            .when('/SubCategory', {
                templateUrl: '/AngApp/SalesModule/Views/SubCategory.html',
                controller: 'SubCategoryController'
            })
            .when('/Product', {
                templateUrl: '/AngApp/SalesModule/Views/Product.html',
                controller: 'ProductController'
            })
            .when('/District', {
                templateUrl: '/AngApp/SalesModule/Views/District.html',
                controller: 'DistrictController'
            })
            .when('/Upazila', {
                templateUrl: '/AngApp/SalesModule/Views/Upazila.html',
                controller: 'UpazilaController'
            })
            .when('/SalesMan', {
                templateUrl: '/AngApp/SalesModule/Views/SalesMan.html',
                controller: 'SalesManController'
            })
            .when('/Customers', {
                templateUrl: '/AngApp/SalesModule/Views/Customer.html',
                controller: 'CustomerController'
            })
            .when('/Payment', {
                templateUrl: '/AngApp/SalesModule/Views/Payment.html',
                controller: 'PaymentsController'
            })
            .when('/SalesSearch', {
                templateUrl: '/AngApp/SalesModule/Views/SalesSearch.html',
                controller: 'SalesSearchController'
            })
            .when('/SalesReports', {
                templateUrl: '/AngApp/SalesModule/Views/SalesReport.html',
                controller: 'SalesReportController'
            })
        .when('/Primaries', {
            templateUrl: '/AngApp/BookModule/Views/Primaries.html',
            controller: 'PrimariesController'
        }).
        when('/Search', {
            templateUrl: '/AngApp/BookModule/Views/Search.html',
            controller: 'SearchController'
        }).
        when('/Provision', {
                templateUrl: '/AngApp/BookModule/Views/Provision.html',
                controller: 'ProvisionController'
        })
        .when('/Groups', {
            templateUrl: '/AngApp/BookModule/Views/Groups.html',
            controller: 'GroupsController'
        })
        .when('/Ledgers', {
            templateUrl: '/AngApp/BookModule/Views/Ledgers.html',
            controller: 'LedgersController'
        })
        .when('/VoucherTypes', {
            templateUrl: '/AngApp/BookModule/Views/VoucherTypes.html',
            controller: 'VoucherTypesController'
        })
        .when('/Reports', {
            templateUrl: '/AngApp/BookModule/Views/Reports.html',
            controller: 'ReportsController'
        })
        .when('/ShowRoom', {
            templateUrl: '/AngApp/Views/ShowRoom.html',
            controller: 'ShowRoomController'
        })
        .when('/ShowRoomUser', {
            templateUrl: '/AngApp/Views/ShowRoomUser.html',
            controller: 'ShowRoomUserController'
        })
        .when('/TransctionTypes', {
            templateUrl: '/AngApp/BookModule/Views/TransctionTypes.html',
            controller: 'TransctionTypesController'
        })
        .when('/Banks', {
            templateUrl: '/AngApp/BankModule/Views/Banks.html',
            controller: 'BanksController'
        })
        .when('/BankAccounts', {
            templateUrl: '/AngApp/BankModule/Views/BankAccounts.html',
            controller: 'BankAccountsController'
        })
        .when('/CheckBooks', {
            templateUrl: '/AngApp/BankModule/Views/CheckBooks.html',
            controller: 'CheckBooksController'
        })
        .when('/Roles', {
            templateUrl: '/AngApp/Views/Roles.html',
            controller: 'RolesController'
        })
        .when('/Login', {
            templateUrl: '/AngApp/Views/Login.html',
            controller: 'loginController'
        })
        .when('/Register', {
            templateUrl: '/AngApp/Views/Register.html',
            controller: 'loginController'
        })
        .when('/Projects', {
            templateUrl: '/AngApp/Views/Projects.html',
            controller: 'ProjectsController'
        })
        .when('/Units', {
            templateUrl: '/AngApp/Views/Units.html',
            controller: 'UnitsController'
        })
        .when('/UnitUsers', {
            templateUrl: '/AngApp/Views/UnitUsers.html',
            controller: 'UnitUsersController'
        })
        .when('/Accounts', {
            templateUrl: '/AngApp/Views/Accounts.html',
            controller: 'AccountsController'
        })
        .when('/About', {
            templateUrl: '/AngApp/Views/About.html',
            controller: 'AboutController'
        })
        .when('/Contact', {
            templateUrl: '/AngApp/Views/Contact.html',
            controller: 'ContactController'
        })
        .when('/unauthorized', {
            templateUrl: '/AngApp/Views/Unauthorized.html',
            controller: 'UnauthorizedController'
        })
        .otherwise({   // This is when any route not matched => error
            templateUrl: '/AngApp/Views/Error.html',
            controller: 'errorController'
        })
        
    }]);


//http interceptor
app.config(['$httpProvider', function ($httpProvider) {
    var interceptor = function(userService, $q, $location)
    {
        return {
            request: function (config) {
                var currentUser = userService.GetCurrentUser();
                if (currentUser != null) {
                    config.headers['Authorization'] = 'Bearer ' + currentUser.access_token;
                }
                return config;
            },
            responseError : function(rejection)
            {
                if (rejection.status === 401) {
                    $location.path('/Login');
                    return $q.reject(rejection);
                }
                if (rejection.status === 403) {
                    $location.path('/unauthorized');
                    return $q.reject(rejection);
                }
                return $q.reject(rejection);
            }

        }
    }
    var params = ['userService', '$q', '$location'];
    interceptor.$inject = params;
    $httpProvider.interceptors.push(interceptor);
}])

//app.run(function (editableOptions) {
//    editableOptions.theme = 'bs3'; // bootstrap3 theme. Can be also 'bs2', 'default'
//})
//app.config(function ($mdDateLocaleProvider) {
//    $mdDateLocaleProvider.formatDate = function (date) {
//        return moment(date).format('DD-MM-YYYY');
//    };
//});
//app.config(function ($mdAriaProvider) {
//    // Globally disables all ARIA warnings.
//    $mdAriaProvider.disableWarnings();
//});
//app.config(function ($mdDateLocaleProvider) {
//    $mdDateLocaleProvider.formatDate = function (date) {
//        if (!date) { return ''; }
//        else {
//            return moment(date).format('DD-MM-YYYY');
//        }

//    };
//    $mdDateLocaleProvider.parseDate = function (dateString) {
//        var m = moment(dateString, 'DD-MM-YYYY', true);
//        return m.isValid() ? m.toDate() : new Date(NaN);
//    };
//});