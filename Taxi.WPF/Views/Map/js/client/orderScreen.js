function init_OrderScreen() {

    outputFieldPayMethod = clientElements.methodPayment;
    initSuggest('search', 'suggest-list1', 'from');
    initSuggest('search2', 'suggest-list2', 'to');

    input_seach1 = clientElements.searchInput;
    input_seach2 = clientElements.searchInput2;
    placeholderSearch1 = clientElements.placeholderSearch1;
    placeholderSearch2 = clientElements.placeholderSearch2;
    setupSearchInput(
        input_seach1,
        placeholderSearch1,
        () => addressOtkuda,
        setOtkudaInput
    );

    setupSearchInput(
        input_seach2,
        placeholderSearch2,
        () => addressKuda
    );

    inti_Traffics();

    getUserLocation();


    postToHost({ type: 'ready' });

    modelWindow = clientElements.modal;

    clientElements.btnPaymentMethod.onclick = btnModelOpen;
    clientElements.btnConfirm.onclick = btnClickConfrim;
    clientElements.modalClose.onclick = closeModel;

    init_RBPaymentMethod();
}

var zoomInterval;
var secondsInterval;

function init_WaitingScreen() {
    updateClock();
    secondsInterval = setInterval(updateClock, 1000);

    const payMethondBlock = clientElements.waitingPaymentMethod;
    const classPayment = PaymentMethodList.find(p => p.id == selectedPaymentMethod);
    payMethondBlock.innerHTML = `
                    <div class="payment-info">
                        <img src="${classPayment.pathIcon}" alt="${classPayment.name}" class="payment-icon">
                        <span class="payment-name">${classPayment.name}</span>
                    </div>
    `;


    clientElements.waitingSearchInput1.value = addressOtkuda;
    clientElements.waitingSearchInput2.value = addressKuda;
    console.info([addressUser.lon, addressUser.lat]);
    map.setLocation({
        center: [addressUser.lon, addressUser.lat],
        zoom: 19.00,
        duration: 800
    })
    updateZoomMap();
    zoomInterval = setInterval(updateZoomMap, 300);


}

function getUserLocation() {
    // postToHost({type:"getUserLocation"});
}

function setOtkudaInput() {
    if (!clientElements.searchInput.value.trim()) {
        clientElements.searchInput.value = addressOtkuda;
    }
    else {
        if (clientElements.searchInput.value !== addressOtkuda) {
            clientElements.searchInput.value = addressOtkuda;
        }
    }
}

function setupSearchInput(input, placeholder, getAddress, onBlurExtra) {
    input.addEventListener('focus', () => {
        if (input.value.trim() !== '') {
            placeholder.classList.add('hidden');
        }
    });

    input.addEventListener('input', () => {
        if (input.value.trim() !== '') {
            placeholder.classList.add('hidden');
            if (input.id === 'search' && input.value.trim().length > 0) {
                // console.info('Уже пишется');
                IsSearchInput1 = true;
            }
        } else {
            placeholder.classList.remove('hidden');
        }
    });

    input.addEventListener('blur', () => {
        const value = input.value.trim();
        const address = getAddress();

        if (value && input.value !== address) {
            input.value = address;
        }

        if (value !== '') {
            placeholder.classList.remove('hidden');
            placeholder.classList.add('active');
        } else {
            placeholder.classList.remove('active', 'hidden');
            if (input.id === 'search') {
                // console.info('Вышел из фокуса');

                IsSearchInput1 = false;
            }
        }

        if (typeof onBlurExtra === 'function') {
            onBlurExtra();
        }
    });
}
