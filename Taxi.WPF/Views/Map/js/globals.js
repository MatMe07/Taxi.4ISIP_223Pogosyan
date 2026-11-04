var map = null;
var suggestView = null;
var suggestView2 = null;
var routeLine = null;
var selectTariff = tariffsList[0] ;
var modelWindow = null;
var points = { from: null, to: null };
var selectedPaymentMethod = "card"; 
var PaymentMethodList = null; 
const outputFieldPayMethod = document.getElementById("methodPayment");
const paymentNames = {
    cash: 'Наличные',
    sbp: 'Онлайн (СБП)'
};

var addressKuda = null;
var addressOtkuda = null;
var addressOtkudaCoord = null

var input_seach1 = null;
var input_seach2 = null;

var addressUser = null;

const suggestList1 = document.getElementById('suggest-list1');
const suggestList2 = document.getElementById('suggest-list2');

var markerOtkuda = null;
var markerKuda = null;

var placeholderSearch1 = null;
var placeholderSearch2 = null;
