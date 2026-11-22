async function initDriver() {
    initDriverState();
    initOrderOfferControls();
    initActiveOrderControls();

    await initMap();

    postToHost({ type: 'ready' });
    // const payload = '{ "distanceMeters": 255.312, "durationSeconds": 77.219, "coordinates": [ [ 55.941714, 37.31986 ] ] }';
    //     const js = RouteInfo.fromJSON(payload);
    //     js.addressKuda = new Address()
    //     console.info(js.addressKuda);
    const geolocation = navigator.geolocation;
    const watchID = geolocation.watchPosition(successHandler, errorHandler);


}

initDriver();


async function setPoint(address) {
    // if (driverBusy) return;
    await ymaps3.ready;
    await applyPoint("from", address.coords, address.address, IsDriverNotBusy);
}

function successHandler(position) {
    // console.log("Широта: ", position.coords.latitude);
    // console.log("Долгота: ", position.coords.longitude);
    // console.info(addressOtkudaCoord);
    driverAddress = new Address(position.coords.latitude, position.coords.longitude, "driver address");
    console.info(driverAddress);

    postToHost({ type: 'driverAddressCoord', address_coord: [driverAddress.lat, driverAddress.lon] });


};
function errorHandler(error) {
    // console.log(error.message);
    driverAddress = new Address(55.755864, 37.617698, "KIP FIN")
}

