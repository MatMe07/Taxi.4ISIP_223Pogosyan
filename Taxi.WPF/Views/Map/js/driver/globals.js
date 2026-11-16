const DRIVER_STATE = Object.freeze({
    OFFLINE: 'offline',
    WAITING: 'waiting',
    OFFER: 'offer',
    TO_CLIENT: 'to-client',
    AT_CLIENT: 'at-client',
    IN_TRIP: 'in-trip',
    COMPLETED: 'completed'
});

const OFFER_DELAY = 500;

const DEMO_ORDER = Object.freeze({
    fare: 245,
    pickup: {
        address: 'ул. Ленина, 15, подъезд 2',
        lon: 37.588144,
        lat: 55.733842
    },
    dropoff: {
        address: 'ТЦ «Галерея», Лиговский пр., 30',
        lon: 37.600500,
        lat: 55.740500
    }, 
    driver: {
        address: 'Вы здесь',
        lon: 37.581500,
        lat: 55.729500
    },
    toClientRoute: [    
        [55.729500, 37.581500],
        [55.730400, 37.583200],
        [55.731400, 37.585300],
        [55.732500, 37.586900],
        [55.733842, 37.588144]
    ],
    tripRoute: [
        [55.733842, 37.588144],
        [55.735100, 37.591200],
        [55.736600, 37.594000],
        [55.738300, 37.597200],
        [55.740500, 37.600500]
    ]
});

let driverState = DRIVER_STATE.OFFLINE;
let driverBalance = 154.75;
let driverOrdersCount = 4;
let offerTimer = null;

const holdCleanups = [];
const driverElements = {};
