async function setPoint(which, lat, lon, address) {
    await ymaps3.ready;

    hideNotice('search');
    hideNotice("search2");
    
    if (which !== 'from' && which !== 'to') {
        console.warn('Неизвестная точка:', which);
        return;
    }

    console.info("which = "+which);
    console.info("lat = "+lat);
    console.info("lon = "+ lon);
    console.info("address = "+ address);
    if (which == 'from') {
        addressOtkuda = address;
        addressOtkudaCoord = { lat: lat, lon: lon };
        addressUser = { lat: addressOtkudaCoord.lat, lon: addressOtkudaCoord.lon };

        console.info("Новый адрес: " + JSON.stringify(addressOtkudaCoord));

        if (!IsSearchInput1)
            input_seach1.value = address;

        placeholderSearch1.classList.add('active');


    } else {
        addressKuda = address;
        console.info(addressKuda);
        input_seach2.value = address;
    }
    await applyPoint(which, [lon, lat], address);
};
async function setAddressOtkuda(which, lat, lon, address) {
    await ymaps3.ready;
    hideNotice('search');
    addressOtkuda = address;
    addressOtkudaCoord = { lat: lat, lon: lon };
    addressUser = { lat: lat, lon: lon };

    // addressUser = {lat:lat,lon}
    console.info("Новый адрес: " + JSON.stringify(addressOtkudaCoord));
    input_seach1.value = address;
    placeholderSearch1.classList.add('active');


    await applyPoint(which, [lon, lat], address);


};

function showNoticeFunc(text) {
    showNotice(text);
};


function SetAllTariff(tariffs, routeInfo) {
    tariffsList= [];
    tariffs.forEach(tar => {
        tariffsList.push(new Tariff(tar.tariffId, tar.className, tar.totalPrice, tar.displayTotalTime, tar.classPathVehicle, routeInfo?.distanceMeters !== -1))
    });
    UpdateTariff();
}


function SetDefaultSet(paymentMethodsLst, selectPaymentMethod){
    console.info(paymentMethodsLst);
    console.info(selectPaymentMethod);
    selectedPaymentMethod = selectPaymentMethod;
    PaymentMethodList = []
    paymentMethodsLst.forEach(element => {
        PaymentMethodList.push(new PaymentMethod(element.id, element.name, element.paymentIconPath))
    });
    
    UpdateRBPaymentMethod();

}

window.chrome.webview.addEventListener('message', async event => {
    const command = event.data; 
    const { type, payload } = command;
    switch (type) {
        case 'SetDefaultSet':
            console.info(payload);
            console.info("--------------------------");
            SetDefaultSet(payload.allPaymentsMethod, payload.selectPaymentMethod);
            break;
        case 'setPoint':
            console.info('payload_setPoint= ');
            console.info(payload);
            await setPoint(payload.which, payload.lat, payload.lon, payload.address);
            SetAllTariff(payload.tariffs, payload.routeInfo);
            break;
        case 'setAddressOtkuda':
            console.info('payload_setPoint= '+ payload);
            await setAddressOtkuda(payload.which, payload.lat, payload.lon, payload.address);
            SetAllTariff(payload.tariffs, payload.routeInfo);
            break;
        case 'showNotice':
            console.info(payload);
            showNoticeFunc(payload.text);
            break;
        case 'showRoute':
            console.info("-------------------------------------------------dsfsdfsdf----sdf");
            console.info(payload);
            
            await showRoute(payload.coordinates, payload.startPoint, payload.endPoint);
            SetAllTariff(payload.tariffs, payload.routeInfo);

            break;
    }

});
