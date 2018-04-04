var app = angular.module('PCBookWebApp');
app.controller('InvoiceController', ['$scope', '$location', '$http', '$timeout', '$filter', function ($scope, $location, $http, $timeout, $filter) {
    $scope.Message = "Invoice Angular JS";

    $scope.invoice = {
        showRoom: { ShowRoomId:1,ShowRoomName: 'PFC-SR-01'},
        invoiceDate: new Date(),
		number:0,
		tax: 0,
        payment:0,
		customer_info: {
		    name: "",
		    Address: 'None',
            Phone: 'None',
            Email: ''
		    //Image: '1209'
        },
	    items:[{
		    description:"ABC Sharee",
		    quentity: 10,
            discount:5,
		    cost:300
		
	    },
	    {
	        description: "XYZ Sharee",
	        quentity: 15,
	        discount: 10,
	        cost: 300

	    }]
    };


    $scope.addItem = function () {
        //alert($scope.productName);
	    $scope.invoice.items.push([{
	        description: $scope.productName,
	        quentity: $scope.itemQuentity,
	        discount: $scope.itemDiscount,
	        cost: $scope.itemCost
	    }]);
    }

    $scope.removeItem = function (m) {
        //$scope.invoice.items.splice($scope.invoice.items.indexOf(m), 1);

        var index = -1;		
        var comArr = eval($scope.invoice.items);
        for( var i = 0; i < comArr.length; i++ ) {
            if (comArr[i].description === m) {
                index = i;
                break;
            }
        }
        if( index === -1 ) {
            alert( "Something gone wrong" );
        }
        $scope.invoice.items.splice(index, 1);
    }
	$scope.subTotal=function(){
		var total=0.0;
		angular.forEach($scope.invoice.items, function(item,key){
		    total += item.quentity * (item.cost - item.discount);
		});
		return total;
	};

	$scope.calcuteTax=function(){
		return (($scope.subTotal()-$scope.invoice.tax));
	};

	$scope.grandTotal= function(){
	    return ($scope.subTotal() - $scope.invoice.tax);
	};


    $scope.dueTotal= function(){
	    return ($scope.subTotal() - $scope.invoice.tax -$scope.invoice.payment);
	};


    // For 3 DatePicker
    //$scope.invoice.invoiceDate = new Date();
    $scope.ToDate = new Date();
    // grab today and inject into field

    $scope.open = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.opened = true;
    };

    $scope.invoiceDatePickerIsOpen = false;
    $scope.toDatePickerIsOpen = false;


    $scope.InvoiceDatePickerOpen = function () {
        this.invoiceDatePickerIsOpen = true;
    };
    $scope.ToDatePickerOpen = function () {
        this.toDatePickerIsOpen = true;
    };
    // End DatePicker

    $scope.showRoomList = [];
    $http.get('/api/ShowRoom/GetDropDownList').success(function (data) {
        $scope.showRoomList = data;
        //console.log(data);
    });

    $scope.customerList = [];
    $http.get('/api/Customer/GetDropDownList').success(function (data) {
        $scope.customerList = data;
        //console.log(data);
    });

    $scope.changeSelectCustomerName = function (item) {
        $scope.invoice.customer_info.name = item;
    };

    $scope.GetCustomerDetailById = function () {        
        var customerId = $scope.invoice.customer_info.name.CustomerId;
        $http.get('/api/Customer/' + customerId).success(function (data) {
            $scope.invoice.customer_info.Address = data.Address;
            $scope.invoice.customer_info.Phone = data.Phone;
            $scope.invoice.customer_info.Email = data.Email;
            //console.log(data);
            //alert(data.Address);
        });
    };

}])
