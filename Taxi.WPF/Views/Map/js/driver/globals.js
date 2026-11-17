const DRIVER_STATE = Object.freeze({
    OFFLINE: 'offline',
    WAITING: 'waiting',
    OFFER: 'offer',
    TO_CLIENT: 'to-client',
    AT_CLIENT: 'at-client',
    IN_TRIP: 'in-trip',
    COMPLETED: 'completed',
    CANCELED: 'canceled'
});

const OFFER_DELAY = 5000;

const DEMO_ORDER = Object.freeze({
    fare: 245,
    pickup: {
      lat: 55.943215,
      lon: 37.318936,
      address: "Овражная улица, 24к8"
    },
    dropoff: {
      lat: 55.941712,
      lon: 37.319547,
      address: "Овражная улица, 24к11"
    }, 
    driver: {
        lat: 55.944949,
        lon: 37.314786,
        address: "Вы здесь"
    },
    toClientRoute:  [
        [ 55.94495, 37.314787 ],
        [ 55.944872, 37.315037 ],
        [ 55.944483, 37.316286 ],
        [ 55.94442, 37.316492 ],
        [ 55.944196, 37.317227 ],
        [ 55.944117, 37.317487 ],
        [ 55.943932, 37.3181 ],
        [ 55.943685, 37.318962 ],
        [ 55.943637, 37.318938 ],
        [ 55.943246, 37.31874 ]
      ],
    tripRoute: [
        [ 55.943246, 37.31874 ],
        [ 55.942998, 37.318614 ],
        [ 55.942975, 37.318602 ],
        [ 55.942879, 37.318562 ],
        [ 55.942773, 37.319431 ],
        [ 55.942664, 37.320189 ],
        [ 55.942465, 37.320114 ],
        [ 55.942059, 37.319975 ],
        [ 55.942019, 37.319925 ],
        [ 55.941896, 37.319857 ],
        [ 55.941714, 37.31986 ]
      ]
});

let driverState = DRIVER_STATE.OFFLINE;
let driverBalance = 154.75;
let driverOrdersCount = 4;
let offerTimer = null;

const holdCleanups = [];
const driverElements = {};
