async function initDriver() {
    initDriverState();
    initOrderOfferControls();
    initActiveOrderControls();

    await initMap();

    postToHost({ type: 'ready' });
}

initDriver();
