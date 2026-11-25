const CLIENT_STATE = Object.freeze({
    ORDER: 'order',
    WAITING: 'waiting',
    DRIVER_COMING: 'driver-coming',
    IN_TRIP: 'in-trip',
    ARRIVED: 'arrived',
    CANCELED: 'canceled'
});

const clientElements = {};

let clientState = CLIENT_STATE.ORDER;

var waiting_screen = null;
var order_screen = null;
let driver_coming_screen = null;
let in_trip_screen = null;
let arrived_screen = null;
let canceled_screen = null;

var suggestView = null;
var suggestView2 = null;

var selectTariff = tariffsList[0];

var modelWindow = null;

var points = { from: null, to: null };

var selectedPaymentMethod = -1;
// var PaymentMethodListElement = null; 
var PaymentMethodList = [];
var outputFieldPayMethod = null;
const paymentNames = {
    cash: 'Наличные',
    sbp: 'Онлайн (СБП)'
};

var addressKuda = null;
var addressOtkuda = null;
var addressOtkudaCoord = null

var input_seach1 = null;
var IsSearchInput1 = false;
var input_seach2 = null;

var suggestList1 = null;
var suggestList2 = null;

var placeholderSearch1 = null;
var placeholderSearch2 = null;

var CurrentOrder = null;
