async function setPoint(which, lat, lon, address) {
    await ymaps3.ready;
    
    if (which !== 'from' && which !== 'to') {
        console.warn('Неизвестная точка:', which);
        return;
    }

    // console.info("which = "+which);
    // console.info("lat = "+lat);
    // console.info("lon = "+ lon);
    // console.info("address = "+ address);
    if (which == 'from') {
        addressOtkuda = address;
        addressOtkudaCoord = { lat: lat, lon: lon };
        addressUser = { lat: addressOtkudaCoord.lat, lon: addressOtkudaCoord.lon };

        // console.info("Новый адрес: " + JSON.stringify(addressOtkudaCoord));

        if (!IsSearchInput1)
            input_seach1.value = address;

        placeholderSearch1.classList.add('active');


    } else {
        addressKuda = address;
        // console.info(addressKuda);
        input_seach2.value = address;
    }
    await applyPoint(which, [lon, lat], address);
};
async function setAddressOtkuda(which, lat, lon, address) {
    await ymaps3.ready;
    addressOtkuda = address;
    addressOtkudaCoord = { lat: lat, lon: lon };
    addressUser = { lat: lat, lon: lon };

    // addressUser = {lat:lat,lon}
    // console.info("Новый адрес: " + JSON.stringify(addressOtkudaCoord));
    input_seach1.value = address;
    placeholderSearch1.classList.add('active');


    await applyPoint(which, [lon, lat], address);


};



function SetAllTariff(tariffs, routeInfo) {
    tariffsList= [];
    tariffs.forEach(tar => {
        tariffsList.push(new Tariff(tar.tariffId, tar.className, tar.totalPrice, tar.displayTotalTime, tar.classPathVehicle, routeInfo?.distanceMeters !== -1))
    });
    UpdateTariff();
}


function SetDefaultSet(paymentMethodsLst, selectPaymentMethod){
    selectedPaymentMethod = selectPaymentMethod;
    PaymentMethodList = []
    paymentMethodsLst.forEach(element => {
        PaymentMethodList.push(new PaymentMethod(element.id, element.name, element.paymentIconPath))
    });
    
    UpdateRBPaymentMethod();

}

function changeTheme(themeUs){
    theme = themeUs;
    root.setAttribute('data-theme', theme);
    
    map.update({ theme: theme });
    if (routeLine){
        routeLine.update({
        style: {
            stroke: [{
                color: rootStyles.getPropertyValue('--route-color').trim(),
                width: 4,
                opacity: 1.0
            }]
        }
    });

    }
}

var CurrentOrder = null;
function goToWaitingDriver(isCreated, orderId){
 if (isCreated){
    CurrentOrder = orderId;
    init_WaitingScreen();
    order_screen.style.display = 'none';
    waiting_screen.style.display = 'block';
 }
}

window.chrome.webview.addEventListener('message', async event => {
    console.info("Message from C#");
    const command = event.data; 
    const { type, payload } = command;
    console.info("payload = ");
    // console.info(payload);
    switch (type) {
        case 'SetDefaultSet':
            console.info(payload);
            SetDefaultSet(payload.allPaymentsMethod, payload.selectPaymentMethod);
            break;
        case 'setPoint':
            await setPoint(payload.which, payload.lat, payload.lon, payload.address);
            SetAllTariff(payload.tariffs, payload.routeInfo);
            break;
        case 'setAddressOtkuda':
            await setAddressOtkuda(payload.which, payload.lat, payload.lon, payload.address);
            SetAllTariff(payload.tariffs, payload.routeInfo);
            break;
        case 'changeTheme':{
            changeTheme(payload.theme);
            break;
        }
        case 'updateTariff':
            SetAllTariff(payload.tariffs, payload.routeInfo);
            break;
        case 'showRoute':
            await showRoute(payload.coordinates, payload.startPoint, payload.endPoint);
            SetAllTariff(payload.tariffs, payload.routeInfo);
            break;

        case 'GoToWaitingDriver':
            console.info(payload);
            goToWaitingDriver(payload.isCreated, payload.orderId);
            break;
    }

});
