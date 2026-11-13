async function setPoint(which, lat, lon, address) {
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
            input_seach1.value = address;

        placeholderSearch1.classList.add('active');


    } else {
        addressKuda = address;
        input_seach2.value = address;
    }
    await applyPoint(which, [lon, lat], address);
};

async function setAddressOtkuda(which, lat, lon, address) {
    await ymaps3.ready;
    addressOtkuda = address;
    addressOtkudaCoord = { lat: lat, lon: lon };
    addressUser = { lat: lat, lon: lon };

    input_seach1.value = address;
    placeholderSearch1.classList.add('active');


    await applyPoint(which, [lon, lat], address);
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
        order_screen.style.display = 'none';
        waiting_screen.style.display = 'block';
    }
}

onHostMessage('SetDefaultSet', payload => {
    console.info(payload);
    SetDefaultSet(payload.allPaymentsMethod, payload.selectPaymentMethod);
});

onHostMessage('setPoint', async payload => {
    await setPoint(payload.which, payload.lat, payload.lon, payload.address);
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
    await showRoute(payload.coordinates, payload.startPoint, payload.endPoint);
    SetAllTariff(payload.tariffs, payload.routeInfo);
});

onHostMessage('GoToWaitingDriver', payload => {
    console.info(payload);
    goToWaitingDriver(payload.isCreated, payload.orderId);
});
