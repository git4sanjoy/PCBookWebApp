var app = angular.module('PCBookWebApp');


app.directive('datepickerPopup', function () {
    return {
        restrict: 'EAC',
        require: 'ngModel',
        link: function (scope, element, attr, controller) {
            //remove the default formatter from the input directive to prevent conflict
            controller.$formatters.shift();
        }
    }
});

app.directive('myEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.myEnter);

                });

                event.preventDefault();
            }
        });
    };
});
app.directive("moveFocusByEnterKey", function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            var destinationTabIndex = null;
            var destinationListIndex = null;
            var targetElementsCondition = 'textarea:enabled:visible,' +
                'input:enabled:visible:not([readonly]),' +
                'select:enabled:visible,' +
                'button:enabled:visible';
            var keyCode = event.which || event.keyCode;

            if (keyCode === 13 && !event.shiftKey && !event.altKey) {
                var $list = $(targetElementsCondition);
                $list.each(function (index) {
                    if (this.tabIndex &&
                        this.tabIndex >= 0 &&
                        this.tabIndex > event.target.tabIndex &&
                        (!destinationTabIndex || destinationTabIndex > this.tabIndex)) {
                        destinationTabIndex = this.tabIndex;
                        destinationListIndex = index;
                    }
                });
                if (destinationListIndex) {
                    $list.eq(destinationListIndex).focus();
                    event.preventDefault();
                }
            }
        });
    };
});
app.directive('modal', function () {
    return {
        template: '<div class="modal fade">' +
            '<div class="modal-dialog  modal-lg">' +
              '<div class="modal-content">' +
                '<div class="modal-header">' +
                  '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                  '<h4 class="modal-title">{{ title }}</h4>' +
                '</div>' +
                '<div class="modal-body" ng-transclude></div>' +
              '</div>' +
            '</div>' +
          '</div>',
        restrict: 'E',
        transclude: true,
        replace: true,
        scope: true,
        link: function postLink(scope, element, attrs) {
            scope.title = attrs.title;

            scope.$watch(attrs.visible, function (value) {
                if (value == true)
                    $(element).modal('show');
                else
                    $(element).modal('hide');
            });

            $(element).on('shown.bs.modal', function () {
                scope.$apply(function () {
                    scope.$parent[attrs.visible] = true;
                });
            });

            $(element).on('hidden.bs.modal', function () {
                scope.$apply(function () {
                    scope.$parent[attrs.visible] = false;
                });
            });
        }
    };
});

app.directive('serverValidate', ['$http', function ($http) {
    return {
        require: 'ngModel',
        link: function (scope, ele, attrs, c) {
            console.log('wiring up ' + attrs.ngModel + ' to controller ' + c.$name);              
            scope.$watch('modelState', function() {
                if (scope.modelState == null) return;
                var modelStateKey = attrs.serverValidate || attrs.ngModel;
                modelStateKey = modelStateKey.replace('$index', scope.$index);
                console.log('validation for ' + modelStateKey);
                if (scope.modelState[modelStateKey]) {                            
                    c.$setValidity('server', false);
                    c.$error.server = scope.modelState[modelStateKey];
                } else {
                    c.$setValidity('server', true);                            
                }                        
            });                  
        }
    };
}]);

app.directive('uploadFile', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {

            var file_uploaded = $parse(attrs.uploadFile);

            element.bind('change', function () {
                scope.$apply(function () {
                    file_uploaded.assign(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

app.directive('bsActiveLink', ['$location', function ($location) {
return {
    restrict: 'A', //use as attribute 
    replace: false,
    link: function (scope, elem) {
        //after the route has changed
        scope.$on("$routeChangeSuccess", function () {
            var hrefs = ['/#' + $location.path(),
                         '#' + $location.path(), //html5: false
                         $location.path()]; //html5: true
            angular.forEach(elem.find('a'), function (a) {
                a = angular.element(a);
                if (-1 !== hrefs.indexOf(a.attr('href'))) {
                    a.parent().addClass('active');
                } else {
                    a.parent().removeClass('active');   
                };
            });     
        });
    }
}
}]);

app.directive('googleplace', [function () {
    return {
        require: 'ngModel',
        scope: {
            ngModel: '=',
            details: '=?'
        },
        link: function (scope, element, attrs, model) {
            var options = {
                types: ['(cities)'],
                componentRestrictions: {}
            };

            scope.gPlace = new google.maps.places.Autocomplete(element[0], options);

            google.maps.event.addListener(scope.gPlace, 'place_changed', function () {
                var geoComponents = scope.gPlace.getPlace();
                var latitude = geoComponents.geometry.location.lat();
                var longitude = geoComponents.geometry.location.lng();
                var addressComponents = geoComponents.address_components;

                addressComponents = addressComponents.filter(function (component) {
                    switch (component.types[0]) {
                        case "locality": // city
                            return true;
                        case "administrative_area_level_1": // state
                            return true;
                        case "country": // country
                            return true;
                        default:
                            return false;
                    }
                }).map(function (obj) {
                    return obj.long_name;
                });

                addressComponents.push(latitude, longitude);

                scope.$apply(function () {
                    scope.details = addressComponents; // array containing each location component
                    model.$setViewValue(element.val());
                });
            });
        }
    };
}]);
