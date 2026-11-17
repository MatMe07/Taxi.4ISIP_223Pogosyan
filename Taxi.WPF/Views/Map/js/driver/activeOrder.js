let routeRenderToken = 0;

function removeDriverRoute() {
    if (!map) return;

    if (routeLine) {
        map.removeChild(routeLine);
        routeLine = null;
    }
    if (markerOtkuda) {
        map.removeChild(markerOtkuda);
        markerOtkuda = null;
    }
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
    const from = isTrip ? DEMO_ORDER.pickup : DEMO_ORDER.driver;
    const to = isTrip ? DEMO_ORDER.dropoff : DEMO_ORDER.pickup;
    const coordinates = isTrip ? DEMO_ORDER.tripRoute : DEMO_ORDER.toClientRoute;
    console.info(isTrip);

    removeDriverRoute();

    await applyPoint('from', [from.lon, from.lat], from.address);
    if (token !== routeRenderToken) {
        removeDriverRoute();
        return;
    }

    await applyPoint('to', [to.lon, to.lat], to.address);
    if (token !== routeRenderToken) {
        removeDriverRoute();
        return;
    }

    await showRoute(coordinates, [from.lon, from.lat], [to.lon, to.lat]);
    if (token !== routeRenderToken) {
        removeDriverRoute();
    }
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
