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
        input_seach1.value = address;

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


function SetAllTariff(tariffs) {
    console.info(tariffs.data)
}

window.chrome.webview.addEventListener('message', async event => {
    const command = event.data; 
    const { type, payload } = command;
    switch (type) {
        case 'setPoint':
            await setPoint(payload.which, payload.lat, payload.lon, payload.address);
            break;
        case 'setAddressOtkuda':
            await setAddressOtkuda(payload.which, payload.lat, payload.lon, payload.address);
            break;
        case 'SetAllTariff':
            SetAllTariff(payload.tariffs);
            break;
        case 'showNotice':
            console.info(payload);
            showNoticeFunc(payload.text);
            break;
    }

});
