function cacheClientElements() {
    clientElements.layer = document.getElementById('ui-layer');
    clientElements.orderScreen = document.getElementById('order-screen');
    clientElements.waitingScreen = document.getElementById('waiting-screen');
    clientElements.driverComingScreen = document.getElementById('driver-coming-screen');
    clientElements.inTripScreen = document.getElementById('in-trip-screen');
    clientElements.arrivedScreen = document.getElementById('arrived-screen');
    clientElements.canceledScreen = document.getElementById('canceled-screen');

    clientElements.searchInput = document.getElementById('search');
    clientElements.searchInput2 = document.getElementById('search2');
    clientElements.suggestList1 = document.getElementById('suggest-list1');
    clientElements.suggestList2 = document.getElementById('suggest-list2');
    clientElements.placeholderSearch1 = document.getElementById('placeholderSearch1');
    clientElements.placeholderSearch2 = document.getElementById('placeholderSearch2');

    clientElements.waitingSearchInput1 = document.getElementById('search1Waiting');
    clientElements.waitingSearchInput2 = document.getElementById('search2Waiting');
    clientElements.clockWaiting = document.getElementById('clockWaiting');
    clientElements.waitingPaymentMethod = document.querySelector('.waiting-paymentMethod');

    clientElements.btnPaymentMethod = document.querySelector('.btnPaymentMethod');
    clientElements.btnConfirm = document.getElementById('btn-confirm');
    clientElements.methodPayment = document.getElementById('methodPayment');
    clientElements.modal = document.querySelector('.Modal');
    clientElements.modalClose = document.querySelector('.modal-close');
    clientElements.paymentList = document.querySelector('.payment-list');
    clientElements.snackbar = document.getElementById('snackbar');
    clientElements.modalWait = document.querySelector('.ModalWait');
    clientElements.tariffWrapper = document.querySelector('.tariffWrapper');

    order_screen = clientElements.orderScreen;
    waiting_screen = clientElements.waitingScreen;
    driver_coming_screen = clientElements.driverComingScreen;
    in_trip_screen = clientElements.inTripScreen;
    arrived_screen = clientElements.arrivedScreen;
    canceled_screen = clientElements.canceledScreen;

    input_seach1 = clientElements.searchInput;
    input_seach2 = clientElements.searchInput2;
    suggestList1 = clientElements.suggestList1;
    suggestList2 = clientElements.suggestList2;
    placeholderSearch1 = clientElements.placeholderSearch1;
    placeholderSearch2 = clientElements.placeholderSearch2;
    modelWindow = clientElements.modal;
    outputFieldPayMethod = clientElements.methodPayment;
}

function setClientState(nextState) {

    clientState = nextState;
    clientElements.layer.dataset.state = nextState;
}

async function initClient() {
    cacheClientElements();
    setClientState(CLIENT_STATE.ORDER);

    await initMap();

    init_OrderScreen();
}

initClient();
