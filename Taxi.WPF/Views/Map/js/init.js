// import {YMapDefaultMarker} from '@yandex/ymaps3-default-ui-theme';
async function init() {
    await ymaps3.ready;

    outputFieldPayMethod = document.getElementById("methodPayment");
    initSuggest('search', 'suggest-list1', 'from');
    initSuggest('search2', 'suggest-list2', 'to');

    input_seach1 = document.getElementById("search");
    input_seach2 = document.getElementById("search2");
    placeholderSearch1 = document.getElementById('placeholderSearch1');
    placeholderSearch2 = document.getElementById('placeholderSearch2');
    setupSearchInput(
        input_seach1,
        placeholderSearch1,
        () => addressOtkuda,
        setOtkudaInput
    );

    setupSearchInput(
        input_seach2,
        placeholderSearch2,
        () => addressKuda
    );
    const { YMap, YMapDefaultSchemeLayer, YMapDefaultFeaturesLayer } = ymaps3;

    map = new YMap(
        document.getElementById('map'),

        {
            location: {
                center: [37.588144, 55.733842],
                zoom: 10
            }
        }
    );

    map.addChild(new YMapDefaultSchemeLayer());
    map.addChild(new YMapDefaultFeaturesLayer())

    inti_Traffics();
    
    getUserLocation();


    postToHost({ type: 'ready' });

    modelWindow = document.querySelector(".Modal");

    document.getElementsByClassName("btnPaymentMethod").onclick = btnModelOpen;
    document.getElementById("btn-confirm").onclick = btnClickConfrim;
    document.getElementsByClassName("modal-close").onclick = closeModel;

    init_RBPaymentMethod();
}

function geocodeAddress(address) {
    console.info('djfksjfld: ' + address);
    ymaps.geocode(address).then(function (res) {
        var firstGeoObject = res.geoObjects.get(0);
        if (firstGeoObject) {
            var coords = firstGeoObject.geometry.getCoordinates();
            alert('Координаты выбранного объекта: ' + coords);
        }
    });
}
