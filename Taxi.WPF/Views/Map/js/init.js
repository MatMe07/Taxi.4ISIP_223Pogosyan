// import {YMapDefaultMarker} from '@yandex/ymaps3-default-ui-theme';
async function init() {
    await ymaps3.ready;
    order_screen = document.getElementById('order-screen');
    waiting_screen = document.getElementById('waiting-screen');
    rootStyles = window.getComputedStyle(document.documentElement);
    
    const { YMap, YMapDefaultSchemeLayer, YMapDefaultFeaturesLayer } = ymaps3;

    map = new YMap(
        document.getElementById('map'),

        {
            location: {
                center: [37.588144, 55.733842],
                zoom: 10
            },
            theme: theme
            
        }
    );
    map.addChild(new YMapDefaultSchemeLayer({
        theme: theme
    }));
    map.addChild(new YMapDefaultSchemeLayer());
    map.addChild(new YMapDefaultFeaturesLayer())


    init_OrderScreen();
 
}

function init_OrderScreen(){

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

       inti_Traffics();
    
    getUserLocation();


    postToHost({ type: 'ready' });

    modelWindow = document.querySelector(".Modal");

    document.getElementsByClassName("btnPaymentMethod").onclick = btnModelOpen;
    document.getElementById("btn-confirm").onclick = btnClickConfrim;
    document.getElementsByClassName("modal-close").onclick = closeModel;

    init_RBPaymentMethod();
}

var zoomInterval;
var secondsInterval;

function init_WaitingScreen(){
    updateClock();
    secondsInterval = setInterval(updateClock, 1000);

    const payMethondBlock = document.querySelector('.waiting-paymentMethod');
    const classPayment = PaymentMethodList.find(p=>p.id==selectedPaymentMethod);
    payMethondBlock.innerHTML = `
                    <div class="payment-info">
                        <img src="${classPayment.pathIcon}" alt="${classPayment.name}" class="payment-icon">
                        <span class="payment-name">${classPayment.name}</span>
                    </div>
    `;


    document.getElementById('search1Waiting').value = addressOtkuda;
    document.getElementById('search2Waiting').value = addressKuda;
    console.info( [addressUser.lon, addressUser.lat]);
    map.setLocation({
        center: [addressUser.lon, addressUser.lat],
        zoom: 19.00,
        duration: 800
    })
    updateZoomMap();
    zoomInterval = setInterval(updateZoomMap, 300);
    

}




