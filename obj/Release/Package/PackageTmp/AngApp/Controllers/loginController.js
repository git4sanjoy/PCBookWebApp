var app = angular.module('PCBookWebApp');
app.controller('loginController', ['$scope', '$location', '$http', '$timeout', '$filter', 'upload', 'loginService',
    function ($scope, $location, $http, $timeout, $filter, upload, loginservice) {
        
        var accesstoken = sessionStorage.getItem('accessToken');
        var authHeaders = {};
        if (accesstoken) {
            authHeaders.Authorization = 'Bearer ' + accesstoken;
        }
        //Scope Declaration
        $scope.clientMessage = true;
        $scope.serverMessage = true;
        $scope.messageType = "";
        $scope.message = "";

        $scope.userName = "";
        $scope.userRegistrationName = "";
        $scope.userRegistrationEmail = "";
        $scope.userRegistrationPassword = "";
        $scope.userRegistrationConfirmPassword = "";

        $scope.userLoginEmail = "";
        $scope.userLoginPassword = "";

        $scope.accessToken = "";
        $scope.refreshToken = "";
        //Ends Here
        $http({
            url: "/api/ShowRooms/GetShowRoomsDropdownList",
            method: "GET",
            headers: authHeaders
        }).success(function (data) {
            $scope.groups = data;
            //console.log(data);
        });
        //Functionn to register user
        $scope.registerUser = function () {

            $scope.message = "";
            var file = $scope.myFile;
            var fileName = "";
            if (file) {
                fileName = file.name;
            }
            
            if (fileName === '' || fileName===null) {
                $scope.message = "Please Select Profile Image.";
                $scope.messageType = "warning";
                $scope.clientMessage = false;
                $timeout(function () { $scope.clientMessage = true; }, 5000);
                return false;
            }

            //The User Registration Information
            var userRegistrationInfo = {
                FullName: $scope.userRegistrationName,
                UserImage: fileName,
                Email: $scope.userRegistrationEmail,
                Password: $scope.userRegistrationPassword,
                ConfirmPassword: $scope.userRegistrationConfirmPassword               
            };
            var promiseregister = loginservice.register(userRegistrationInfo);

            promiseregister.then(function (resp) {
                $scope.message = "User is Successfully";
                if (fileName) {
                    upload({
                        url: 'Home/upload',
                        method: 'POST',
                        data: {
                            aFile: $scope.myFile
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
                //// Add show Room User
                //console.log(resp);
                //var aShowRoomUserObj = {
                //    Id: resp.Id,
                //    ShowRoomId: $scope.userRegistrationUnitListDdl.id,
                //    UserName: $scope.userRegistrationName,
                //    Address: "None",
                //    Phone: "0000",
                //    Email: "aa@pakizagroup.com"
                //};
                //$http.post('/api/ShowRoomUsers', aShowRoomUserObj).success(function (data) {

                //}).error(function (error) {
                //    $scope.message = 'Unable to add Show Room User data: ' + error.message;
                //    console.log($scope.message);
                //});
                //// End Add Show Room User

                $scope.userRegistrationName = "";
                $scope.userRegistrationEmail = "";
                $scope.userRegistrationPassword = "";
                $scope.userRegistrationConfirmPassword = "";
                $scope.message = "Successfully Created.";
                $scope.messageType = "success";
                $scope.clientMessage = false;
                $scope.registerForm.$setPristine();
                $scope.registerForm.$setUntouched(); 
                $timeout(function () { $scope.clientMessage = true; }, 5000);
            }, function (err) {
                //$scope.message = "Error " + err.status;
                $scope.validationErrors = [];
                if (error.ModelState && angular.isObject(error.ModelState)) {
                    for (var key in error.ModelState) {
                        $scope.validationErrors.push(error.ModelState[key][0]);
                    }
                } else {
                    $scope.validationErrors.push('Unable to add Check User.');
                };
                $scope.messageType = "danger";
                $scope.serverMessage = false;
                $timeout(function () { $scope.serverMessage = true; }, 5000);
            });
        };


        $scope.redirect = function () {
            window.location.href = '/Dashboard';
        };

        //Function to Login. This will generate Token 
        $scope.submitLoginForm = function () {
            //This is the information to pass for token based authentication
            $scope.submitted = true;
            if ($scope.loginForm.$valid) {
                var userLogin = {
                    grant_type: 'password',
                    username: $scope.userLoginEmail,
                    password: $scope.userLoginPassword
                };

                var promiselogin = loginservice.login(userLogin);

                promiselogin.then(function (resp) {

                    $scope.userName = resp.data.userName;
                    //Store the token information in the SessionStorage
                    //So that it can be accessed for other views
                    sessionStorage.setItem('userName', resp.data.userName);
                    sessionStorage.setItem('accessToken', resp.data.access_token);
                    sessionStorage.setItem('refreshToken', resp.data.refresh_token);
                    window.location.href = '/Dashboard';
                }, function (error) {
                    $scope.validationErrors = [];
                    if (error.ModelState && angular.isObject(error.ModelState)) {
                        for (var key in error.ModelState) {
                            $scope.validationErrors.push(error.ModelState[key][0]);
                        }
                    } else {
                        $scope.validationErrors.push('Unable to add Check User.');
                    };
                    $scope.messageType = "danger";
                    $scope.serverMessage = false;
                    $timeout(function () { $scope.serverMessage = true; }, 5000);

                });
            }
            else {
                //alert("Please  correct form errors!");
            }

            

        };
        //separate method for parsing errors into a single flat array
        function parseErrors(response) {
            var errors = [];
            for (var key in response.ModelState) {
                for (var i = 0; i < response.ModelState[key].length; i++) {
                    errors.push(response.ModelState[key][i]);
                }
            }
            return errors;
        }

        // Social Media
        $scope.fbLogin = function () {

            FB.getLoginStatus(function (response) {
                if (response.status === 'connected') {                    
                    //console.log('Logged in.');
                    FB.api('/me', function (response) {
                        //console.log(response);
                        var accessToken = FB.getAuthResponse();
                        console.log(accessToken);
                        FB.api('/me/feed', 'post', { message: 'Test FB Javascript SDK Post' });
                    });
                }
                else {
                    FB.login();
                }
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
              }
            );

        };

    }])