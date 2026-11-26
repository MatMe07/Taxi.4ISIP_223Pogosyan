async function setPoint(who, which, lat, lon, address) { 
    await ymaps3.ready;

    if (which !== 'from' && which !== 'to') {
        console.warn('Неизвестная точка:', which);
        return;
    }

    if (which == 'from') {
        addressOtkuda = address;
        addressOtkudaCoord = { lat: lat, lon: lon };
        addressUser = { lat: addressOtkudaCoord.lat, lon: addressOtkudaCoord.lon };

        if (!IsSearchInput1)
            clientElements.searchInput.value = address;

        clientElements.placeholderSearch1.classList.add('active');


    } else {
        addressKuda = address;
        clientElements.searchInput2.value = address;
    }
    let markerType;
    if (who === 'client'){
        markerType = which === 'from' ? MarkerType.CLIENT_DEFAULT : MarkerType.DESTINATION;
    }
    else{
        markerType = which === 'from' ? MarkerType.DRIVER_TO_CLIENT : MarkerType.CLIENT_WAITING;
    }
    await applyPoint(markerType, [lon, lat]);
};

async function setAddressOtkuda(which, lat, lon, address) {
    await ymaps3.ready;
    console.info("setAddressOtkuda");
    addressOtkuda = address;
    addressOtkudaCoord = { lat: lat, lon: lon };
    addressUser = { lat: lat, lon: lon };

    clientElements.searchInput.value = address;
    clientElements.placeholderSearch1.classList.add('active');


    const markerType = which === 'from' ? MarkerType.CLIENT_DEFAULT : MarkerType.DESTINATION;
    await applyPoint(markerType, [lon, lat]);
};

function SetAllTariff(tariffs, routeInfo) {
    tariffsList = [];
    tariffs.forEach(tar => {
        tariffsList.push(new Tariff(tar.tariffId, tar.className, tar.totalPrice, tar.displayTotalTime, tar.classPathVehicle, routeInfo?.distanceMeters !== -1))
    });
    UpdateTariff();
}

function SetDefaultSet(paymentMethodsLst, selectPaymentMethod) {
    selectedPaymentMethod = selectPaymentMethod;
    PaymentMethodList = []
    paymentMethodsLst.forEach(element => {
        PaymentMethodList.push(new PaymentMethod(element.id, element.name, element.paymentIconPath))
    });

    UpdateRBPaymentMethod();

}

function goToWaitingDriver(isCreated, orderId) {
    if (isCreated) {
        CurrentOrder = orderId;
        init_WaitingScreen();
        setClientState(CLIENT_STATE.WAITING);
    }
}


function acceptOrder(){

}

onHostMessage('SetDefaultSet', payload => {
    console.info(payload);
    SetDefaultSet(payload.allPaymentsMethod, payload.selectPaymentMethod);
});

onHostMessage('setPoint', async payload => {
    await setPoint(payload.who, payload.which, payload.lat, payload.lon, payload.address);
    SetAllTariff(payload.tariffs, payload.routeInfo);
});

onHostMessage('setAddressOtkuda', async payload => {
    await setAddressOtkuda(payload.which, payload.lat, payload.lon, payload.address);
    SetAllTariff(payload.tariffs, payload.routeInfo);
});

onHostMessage('updateTariff', payload => {
    SetAllTariff(payload.tariffs, payload.routeInfo);
});

onHostMessage('showRoute', async payload => {
    // await showRoute(payload.coordinates, payload.startPoint, payload.endPoint);
    await showRoute(payload.coordinates);
    SetAllTariff(payload.tariffs, payload.routeInfo);
     
});

onHostMessage('GoToWaitingDriver', payload => {
    console.info(payload);
    goToWaitingDriver(payload.isCreated, payload.orderId);
});

onHostMessage('setClientState', payload => {
    const nextState = typeof payload === 'string' ? payload : payload?.state;
    setClientState(nextState);
});

onHostMessage('acceptOrder', payload=>{
    
});
