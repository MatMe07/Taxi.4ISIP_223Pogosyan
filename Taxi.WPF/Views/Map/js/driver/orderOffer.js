function scheduleOffer() {
    clearTimeout(offerTimer);
    offerTimer = setTimeout(() => {
        if (driverState === DRIVER_STATE.WAITING) {
            setDriverState(DRIVER_STATE.OFFER);
            setupTimoutElement(5000);
        }
    }, OFFER_DELAY);
}

function initOrderOfferControls() {
    // setupHold(driverElements.acceptBtn, () => {
    //     if (driverState === DRIVER_STATE.OFFER) {
    //         setDriverState(DRIVER_STATE.TO_CLIENT);
    //     }
    // });

    driverElements.acceptBtn.addEventListener('click', () => {
        if (driverState === DRIVER_STATE.OFFER) {
            setDriverState(DRIVER_STATE.TO_CLIENT);
        }
    });
    driverElements.skipBtn.addEventListener('click', () => {
        if (driverState === DRIVER_STATE.OFFER) {
            setDriverState(DRIVER_STATE.WAITING);
        }
    });
}
