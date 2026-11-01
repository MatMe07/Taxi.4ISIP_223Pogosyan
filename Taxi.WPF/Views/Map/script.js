
class Tariff{
    constructor(id,title, price, time,imgPath){
        this.id = id;
        this.title = title;
        this.price = price;
        this.time=time;
        this.imgPath=imgPath;
    }
}

const tariffsList = [
  new Tariff(1,'Эконом', 339, '12 мин', 'imgs/classTariff/ekonomClass.png'),
  new Tariff(2,'Комфорт', 450, '7 мин', 'imgs/classTariff/komfort.png'),
  new Tariff(3,'Бизнес', 790, '4 мин', 'imgs/classTariff/business.png'),
  new Tariff(4,'Минивэн', 340, '5 мин', 'imgs/classTariff/miniven.png')
];

var map = null;
var suggestView = null;
var suggestView2 = null;
var routeLine = null;
var selectTariff = tariffsList[0] ;
var modelWindow = null;
var points = { from: null, to: null };
var selectedPaymentMethod = "card"; 
var PaymentMethodList = null; 
const outputFieldPayMethod = document.getElementById("methodPayment");
const paymentNames = {
    cash: 'Наличные',
    sbp: 'Онлайн (СБП)'
};

var addressKuda = null;
var addressOtkuda = null;

var input_seach1 = null;
var input_seach2 = null;

ymaps.ready(init);

function postToHost(message) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage(JSON.stringify(message));
    } else {
        console.log('[-> C#]', JSON.stringify(message));
    }
}

function showNotice(text) {
    var notice = document.getElementById('notice');
    notice.textContent = text;
    notice.classList.add('visible');
    document.getElementById('search').classList.add('input_error');
}

function hideNotice(elementId) {
    var notice = document.getElementById('notice');
    notice.textContent = '';
    notice.classList.remove('visible');
    document.getElementById(elementId).classList.remove('input_error');
}

function getUserLocation(map) {
    ymaps.geolocation.get({
        provider: 'browser',
        autoReverseGeocode: false, 

        mapStateAutoApply: true
    }).then(function (result) {
        // map.geoObjects.add(result.geoObjects);
        var coord = result.geoObjects.get(0).geometry.getCoordinates();
        postToHost({ type: 'destinationOtkudaCoord', address_coord:  coord});

        console.log(result.geoObjects);
        console.log("Точные координаты (Браузер):", coord);
    }, function (err) {
        console.warn("Браузер заблокировал гео или выдал ошибку.", err);

    });
}

function init() {
    inti_Traffics();
input_seach1 = document.getElementById("search");
input_seach2 = document.getElementById("search2");

    map = new ymaps.Map('map', {
        center: [55.75, 37.65],
        zoom: 12,
        controlls: []
    });

    getUserLocation(map);

    //map.controls.add('zoomControl', {
    //    position: {
    //        right: 10,
    //        bottom: 10
    //    }
    //});

    suggestView = new ymaps.SuggestView('search');
    suggestView.events.add('select', function (e) {
        hideNotice("search");
        addressOtkuda = e.get('item').value;
        map.geoObjects.add(result.geoObjects);

        postToHost({ type: 'destinationSelectedOtkuda', address: e.get('item').value });
    });

    suggestView2 = new ymaps.SuggestView('search2');
    suggestView2.events.add('select', function (e) {
        hideNotice("search2");
        addressKuda = e.get('item').value;
        console.info(addressKuda)
        // geocodeAddress(addressKuda);
        postToHost({ type: 'destinationSelectedKuda', address: e.get('item').value });
    });
    postToHost({ type: 'ready' });
    map.controls.remove('geolocationControl');
    map.controls.remove('searchControl');
    map.controls.remove('trafficControl');
    // map.controls.remove('typeSelector');
    map.controls.remove('fullscreenControl');
    map.controls.remove('zoomControl');
    map.controls.remove('rulerControl');


    modelWindow = document.querySelector(".Modal");

    document.getElementsByClassName("btnPaymentMethod").onclick = btnModelOpen;
    document.getElementById("btn-confirm").onclick = btnClickConfrim;
    document.getElementsByClassName("modal-close").onclick = closeModel;

    // PaymentMethodList = document.querySelector(".payment-list");
    init_RBPaymentMethod();
}

    function geocodeAddress(address) {
        console.info('djfksjfld: '+address);
        ymaps.geocode(address).then(function (res) {
            var firstGeoObject = res.geoObjects.get(0);
            if (firstGeoObject) {
                var coords = firstGeoObject.geometry.getCoordinates();
                alert('Координаты выбранного объекта: ' + coords);
            }
        });
    }

function btnClickConfrim(){
    console.info(addressOtkuda);
    console.info(addressKuda);
    console.info(selectTariff)
    
}

function init_RBPaymentMethod(){
    PaymentMethodList = document.querySelector(".payment-list");
    // console.info(PaymentMethodList);    

    // PaymentMethodList.childNodes.forEach(method=>{
    //     // console.info(method);

        
    // });

    PaymentMethodList.addEventListener('click', (event) => {
        const clickedRadio = event.target.closest('.payment-row');
        // console.info(clickedRadio);
        
        PaymentMethodList.querySelectorAll('.payment-row').forEach(radioBtn=>{
            radioBtn.classList.remove("selected");

            
        });
        clickedRadio.classList.add("selected");
        selectedPaymentMethod = clickedRadio.dataset.method;
        outputFieldPayMethod.textContent = paymentNames[selectedPaymentMethod];
        closeModel();


    });
}


function btnModelOpen(){
    modelWindow.classList.add("open");
}

function closeModel(){
    modelWindow.classList.remove("open");
    console.info(selectedPaymentMethod);
}

function applyPoint(which, coords, address) {
    var pm = new ymaps.Placemark(coords, {
        hintContent: address,
        balloonContent: address
    }, {
        preset: which === 'from' ? 'islands#bluePersonIcon' : 'islands#redCircleDotIcon',
        zIndex: which === 'from' ? 500 : 600
    });

    pm.properties.set('hint', address);

    if (points[which]) {
        map.geoObjects.remove(points[which]);
    }
    points[which] = pm;
    map.geoObjects.add(pm);

    if (!routeLine) {
        map.setCenter(coords, 15, { duration: 300 });
    }
}


window.showRoute = function (coords, from, to) {
    if (!map) {
        console.warn('Карта ещё не инициализирована');
        return;
    }
    console.info('Вызвана функция showRoute');
    console.log(coords);



    hideNotice("search");
    hideNotice("search2");

    var line = [];

    if (coords && coords.length) {
        for (var i = 0; i < coords.length; i++) {
            var p = coords[i];
            if (!p || p.length < 2 || !isFinite(p[0]) || !isFinite(p[1])) {
                continue;
            }
            line.push(p);
        }
    }

    if (line.length < 2) {
        console.warn('Геометрия не получена, рисуем прямую между точками');
        line = [from, to];
    }

    if (routeLine) {
        map.geoObjects.remove(routeLine);
    }

    routeLine = new ymaps.Polyline(line, {
        strokeColor: '#0a7cff',
        strokeWidth: 20,
        // strokeOpacity: 0.85,
        // zIndex: 400,
        // balloonContentHeader: 'Маршрут',
        // balloonContentBody:
        //     '<b>Расстояние:</b> ' + formatDistance(distance) + '<br/>' +
        //     '<b>Время в пути:</b> ' + formatDuration(duration) + '<br/>' +
        //     '<span style="font-weight: bold; font-style: italic">Стоимость поездки: ' + price + ' р.</span>'
    });

    map.geoObjects.add(routeLine);
    map.setBounds(ymaps.geometry.bounds.fromCoordinates(line), {
        checkZoomRange: true,
        duration: 300
    });

    routeLine.balloon.open();
};

window.setPoint = function (which, lat, lon, address) {
    hideNotice('search');
    hideNotice("search2");
    if (!map) {
        console.warn('Карта ещё не инициализирована');
        return;
    }
    if (which !== 'from' && which !== 'to') {
        console.warn('Неизвестная точка:', which);
        return;
    }
    applyPoint(which, [lat, lon], address);
};
window.setAddressOtkuda = function (which, lat, lon, address) {
    hideNotice('search');
    var search = document.getElementById('search');
    console.info(search);

    if (!map) {
        console.warn('Карта ещё не инициализирована');
        return;
    }
    // if (which !== 'from' && which !== 'to') {
    //     console.warn('Неизвестная точка:', which);
    //     return;
    // }
    applyPoint(which, [lat, lon], address);
    addressOtkuda = address;
    search.value  = address;


};

window.showNotice = function (text) {
    showNotice(text);
};







function inti_Traffics(){
    const tariffContainer  = document.querySelector(".tariffWrapper");
    console.info(parent);
    tariffsList.forEach(tarif=>{
                const buttonHTML = `
        <button role="radio" aria-pressed="false" aria-checked="false" id="${tarif.id}" class="btnTariffs ${tarif.title == 'Эконом' ? 'active' : ''}">
            <span style="flex-shrink: 0;" class="box--img">
                <img src=${tarif.imgPath} alt="Желтая машина" width="56" height="20" class="car-img">
                <span class="etaTimes">${tarif.time}</span>
            </span>
            <span class="content">
                <span class="title">${tarif.title}</span>
                <span class="price--WnKzJ">
                    <span class="priceSurge--jcKcC"></span>
                    <span class="priceText--YA8Je">от ${tarif.price} ₽</span>
                </span>
            </span>
        </button>
        `;
    
        tariffContainer.insertAdjacentHTML('beforeend', buttonHTML);
    });
    // for (let index = 0; index < tariffsList.length; index++) {

        
    // }

    tariffContainer.addEventListener('click', (event) => {
        const clickedButton = event.target.closest('.btnTariffs');
        if (!clickedButton) return;
        
        selectTariff = tariffsList[ clickedButton.id-1];
        console.info(selectTariff);


        tariffContainer.querySelectorAll('.btnTariffs').forEach(btn =>{
            btn.setAttribute('aria-pressed', 'false');
            btn.setAttribute('aria-checked', 'false');
            btn.classList.remove('active'); 
        });
          clickedButton.setAttribute('aria-pressed', 'true');
        clickedButton.setAttribute('aria-checked', 'true');
        clickedButton.classList.add('active'); 
        
    });
}
