let routeRenderToken = 0;

function removeDriverRoute() {
    if (!map) return;

    if (routeLine) {
        map.removeChild(routeLine);
        routeLine = null;
    }
    // if (markerOtkuda) {
    //     map.removeChild(markerOtkuda);
    //     markerOtkuda = null;
    // }
    if (markerKuda) {
        map.removeChild(markerKuda);
        markerKuda = null;
    }
}

function clearDriverRoute() {
    routeRenderToken++;
    removeDriverRoute();
}

async function drawDriverRoute(stage) {
    if (!map) return;

    const token = ++routeRenderToken;
    const isTrip = stage === 'in-trip';
    let from = null
    let to = null
    let coordinates = null
    if (isTrip){
        from = order.request.addressOtkuda;
        to = order.request.addressKuda;
        coordinates = order.request.routeInfo.coordinates;
    }
    else{
        from = driverAddress;
        to = toClientRoute.addressKuda;
        coordinates = toClientRoute.coordinates;
    }
    // from = isTrip ? DEMO_ORDER.pickup : DEMO_ORDER.driver;
    // to = isTrip ? DEMO_ORDER.dropoff : DEMO_ORDER.pickup;
    // coordinates = isTrip ? DEMO_ORDER.tripRoute : DEMO_ORDER.toClientRoute;
    console.info("trip = " + isTrip);
    console.info("from = ");
    console.info(from);
    console.info("to = ");
    console.info(to);

    removeDriverRoute();

    // await applyPoint('from', [from.lon, from.lat], from.address);
    // if (token !== routeRenderToken) {
    //     removeDriverRoute();
    //     return;
    // }

    await applyPoint('to', [to.lon, to.lat], to.address);
    // if (token !== routeRenderToken) {
    //     removeDriverRoute();
    //     return;
    // }

    await showRoute(coordinates, [from.lon, from.lat], [to.lon, to.lat]);
    // if (token !== routeRenderToken) {
    //     removeDriverRoute();
    // }
}

function updateRouteForState(nextState) {
    if (nextState === DRIVER_STATE.OFFER ||
        nextState === DRIVER_STATE.TO_CLIENT ) {
        drawDriverRoute('to-client');
    } else if (nextState === DRIVER_STATE.IN_TRIP||
        nextState === DRIVER_STATE.AT_CLIENT) {
        drawDriverRoute('in-trip');
    } else if (nextState === DRIVER_STATE.WAITING ||
               nextState === DRIVER_STATE.OFFLINE) {
        clearDriverRoute();
    }
}


function updateORderState(nextState){
    if (nextState === DRIVER_STATE.OFFER){
        renderOfferCard(order);
    }
    else if(nextState === DRIVER_STATE.TO_CLIENT){
        renderActiveCard(order);
    }
}

function renderOfferCard(order) {
    console.info("renderOffer");
    console.info(order);
    driverElements.offerCard.querySelector('.offer-subtitle').textContent = `${toClientRoute.distanceMeters} · ${toClientRoute.durationMinutes}`;
    driverElements.offerCard.querySelector('.offer-price').textContent = `${order.request.paymentAmount} ₽`;
    driverElements.offerCard.querySelector('.user-rating').textContent = order.rating;
    driverElements.offerCard.querySelector('#from-address').textContent = order.request.addressOtkuda.address;
    driverElements.offerCard.querySelector('#to-address').textContent = order.request.addressKuda.address;
    driverElements.offerCard.querySelector('.driver-class').textContent = order.tariffName;
}

function renderActiveCard(order) {
    driverElements.activeCard.querySelector('#order-status-text').textContent = 'В пути к клиенту';
    driverElements.activeCard.querySelector('.offer-price').textContent = `${order.request.paymentAmount} ₽`;
    driverElements.activeCard.querySelector('.client-avatar').textContent = order.clientName[0].toUpperCase();
    driverElements.activeCard.querySelector('.client-name').textContent = order.clientName;
    driverElements.activeCard.querySelector('.user-rating').textContent = order.rating;
    driverElements.activeCard.querySelector('#active-from-address').textContent = order.request.addressOtkuda.address;
    driverElements.activeCard.querySelector('#active-to-address').textContent = order.request.addressKuda.address;
    driverElements.activeCard.querySelector('.order-payment-value').textContent = order.paymentMethodName;
}

function completeTrip() {
    driverBalance += DEMO_ORDER.fare;
    driverOrdersCount += 1;

    updateDriverBalance();
    driverElements.completedFare.textContent = formatMoney(DEMO_ORDER.fare);

    setDriverState(DRIVER_STATE.COMPLETED);
}

function cancelTrip(){
    clearDriverRoute();
    btnModalCancelOpen();
}


function initActiveOrderControls() {
    // setupHold(driverElements.arriveBtn, () => {
    // });
    
    driverElements.arriveBtn.addEventListener('click', () => {
        if (driverState === DRIVER_STATE.TO_CLIENT) {
            setDriverState(DRIVER_STATE.AT_CLIENT);
        }
    });


    driverElements.startBtn.addEventListener('click', () => {
        if (driverState === DRIVER_STATE.AT_CLIENT) {
            setDriverState(DRIVER_STATE.IN_TRIP);
        }
    });

    driverElements.finishBtn.addEventListener('click', () => {
        if (driverState === DRIVER_STATE.IN_TRIP) {
            completeTrip();
        }
    });

    driverElements.backBtn.addEventListener('click', () => {
        if (driverState === DRIVER_STATE.COMPLETED) {
            setDriverState(DRIVER_STATE.WAITING);
        }
    });

    driverElements.cancelBackBtn.addEventListener('click', () => {
        if (driverState === DRIVER_STATE.CANCELED) {
            setDriverState(DRIVER_STATE.WAITING);
        }
    });

    driverElements.cancelBtn.addEventListener('click', () => {
        if (driverState === DRIVER_STATE.TO_CLIENT ||
            driverState === DRIVER_STATE.AT_CLIENT) {
            cancelTrip();
        }
    });
}
