async function initDriver() {
    await initMap();

    postToHost({ type: 'ready' });
}

initDriver();
