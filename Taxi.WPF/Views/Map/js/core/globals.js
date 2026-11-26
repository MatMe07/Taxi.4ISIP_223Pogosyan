var map = null;
var rootStyles = null;

var routeLine = null;
var lastBounds = null;

var addressUser = null;

var markers = {};

const MarkerType = Object.freeze({
    CLIENT_DEFAULT: 'clientDefault',
    DESTINATION: 'destination',
    CLIENT_WAITING: 'clientWaiting',
    DRIVER_TO_CLIENT: 'driverToClient',
    DRIVER: 'driver',
});

const clientMarkerHtml = `
    <div class="label-container">
        <svg class="user-icon" width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <circle cx="12" cy="8" r="4" fill="#ffffffff"/>
            <path d="M4 20C4 16.6863 6.68629 14 10 14H14C17.3137 14 20 16.6863 20 20V21H4V20Z" fill="#ffffffff"/>
        </svg>
    </div>
    <div class="placemark-tail"></div>
    
    `;

const destinationMarkerHtml = `
<div class="map-marker-station  pulse">
    <div class="marker-circle"></div>
</div>
    
    `;

const clientWaitingMarkerHtml = `
    <div class="label-container" style="transform: translate(-24px, -51px);">
<svg xmlns="http://w3.org" width="48" height="56" viewBox="0 0 48 56" fill="none">
  <defs>
    <filter id="pinShadow" x="0" y="0" width="48" height="48" filterUnits="userSpaceOnUse">
      <feDropShadow dx="0" dy="2" stdDeviation="2" flood-color="#FFFFFF" flood-opacity="0.15"/>
    </filter>
  </defs>
  <rect x="4" y="2" width="40" height="40" rx="13" fill="#FFFFFF" filter="url(#pinShadow)"/>
  <rect x="7" y="5" width="34" height="34" rx="10" fill="var(--button-action-color)"/>
  <circle cx="24.5" cy="16.5" r="2.6" fill="#FFFFFF"/>
  <path d="M14.5 21.5H25.2C26.7 21.5 27.6 22.3 28.4 23.7L30.8 27.8C31.2 28.5 30.9 29.3 30.2 29.7C29.5 30.1 28.7 29.8 28.3 29.1L26.8 26.5V30.5H20.8V24.3H14.5C13.7 24.3 13 23.7 13 22.9C13 22.1 13.7 21.5 14.5 21.5Z" fill="#FFFFFF"/>
  <circle cx="24" cy="51" r="3.5" fill="#FFFFFF" stroke="#1A1A1A" stroke-width="2"/>
</svg>

    </div>
    `;
const driverMarkerHtml = `
    <div style="transform: translate(-22px, -28px);">
    <svg xmlns="http://w3.org" width="48" height="48" viewBox="0 0 48 48" fill="none">
  <defs>
    <filter id="arrowShadow" x="0" y="0" width="48" height="48" filterUnits="userSpaceOnUse">
      <feDropShadow dx="1" dy="3" stdDeviation="2" flood-color="#000000" flood-opacity="0.25"/>
    </filter>
  </defs>

  <g filter="url(#arrowShadow)">
    <polygon points="15,6 22,28 11,42" fill="#7AB5F5"/>
    <polygon points="15,6 38,34 22,28" fill="#4A90E2"/>
    <polygon points="11,42 22,28 21,31" fill="#2A5C99"/>
    <polygon points="22,28 38,34 21,31" fill="#357ABD"/>
    <polygon points="15,6 38,34 21,31 11,42" stroke="#1E3A5F" stroke-width="2" stroke-linejoin="round" fill="none"/>
  </g>
</svg>

    </div>
    `;
const driverToClientMarkerHtml = `
    <div style="transform: translate(-22px, -28px);">

<svg xmlns="http://w3.org" width="48" height="48" viewBox="0 0 48 48" fill="none">
  <defs>
    <filter id="carShadow" x="6" y="2" width="36" height="46" filterUnits="userSpaceOnUse">
      <feDropShadow dx="0" dy="2" stdDeviation="2.5" flood-color="#000000" flood-opacity="0.3"/>
    </filter>
  </defs>

  <g filter="url(#carShadow)">
    <rect x="13" y="5" width="22" height="38" rx="6" fill="#4A90E2" stroke="#1E3A5F" stroke-width="1.8"/>
    <path d="M13 15L10 16.5V18.5L13 18" fill="#357ABD" stroke="#1E3A5F" stroke-width="1.2"/>
    <path d="M35 15L38 16.5V18.5L35 18" fill="#357ABD" stroke="#1E3A5F" stroke-width="1.2"/>
    <path d="M15.5 14C15.5 12.5 17 11.5 24 11.5C31 11.5 32.5 12.5 32.5 14L31 19H17L15.5 14Z" fill="#1E3A5F"/>
    <rect x="17" y="20" width="14" height="11" rx="2.5" fill="#7AB5F5"/>
    <path d="M17 32H31L32 36.5C32 37.5 30.5 38 24 38C17.5 38 16 37.5 16 36.5L17 32Z" fill="#1E3A5F"/>
    <rect x="15" y="5.5" width="4" height="2" rx="1" fill="#E6F2FF"/>
    <rect x="29" y="5.5" width="4" height="2" rx="1" fill="#E6F2FF"/>
  </g>
</svg>
</div>
`;
const MarkerTemplate = Object.freeze({
    [MarkerType.CLIENT_DEFAULT]: { className: 'placemark', html: clientMarkerHtml },
    [MarkerType.DESTINATION]: { className: '', html: destinationMarkerHtml },
    [MarkerType.CLIENT_WAITING]: { className: '', html: clientWaitingMarkerHtml },
    [MarkerType.DRIVER]: { className: '', html: driverMarkerHtml },
    [MarkerType.DRIVER_TO_CLIENT]: { className: '', html: driverToClientMarkerHtml },
});

class Address {
  constructor(latOrObj, lon, address) {
    if (typeof latOrObj === 'object' && latOrObj !== null) {
      this.lat = latOrObj.lat;
      this.lon = latOrObj.lon;
      this.address = latOrObj.address;
    } else {
      this.lat = latOrObj;
      this.lon = lon;
      this.address = address;
    }
    this.coords = [this.lon, this.lat];
  }

  static fromJSON(data) {
    console.info("Address");
    console.info(data);
    return data ? new Address(data) : null;
  }
}

class RouteInfo {
  constructor({ distanceMeters, durationSeconds, coordinates = [], addressOtkuda=null, addressKuda=null } = {}) {
    this.distanceMeters = distanceMeters;
    this.durationSeconds = durationSeconds;
    this.coordinates = coordinates;
    this.addressKuda = addressKuda;
    this.addressOtkuda = addressOtkuda;
  }

  get distanceKm() {
    return +(this.distanceMeters / 1000).toFixed(2);
  }

  get durationMinutes() {
    return Math.ceil(this.durationSeconds / 60);
  }
  static parse(json) {
    if (!json) return null;
    return typeof json === 'string' ? JSON.parse(json) : json;
  }

  static fromJSON(json) {
    const data = this.parse(json);
    console.info("RouteInfo");
    console.info(data);
    return data ? new RouteInfo(data) : null;
  }
  
}


class OrderRequest {
  constructor({ clientId, tariffId, addressOtkuda, addressKuda, paymentMethodId, paymentAmount, routeInfo} = {}) {
    this.clientId = clientId;
    this.tariffId = tariffId;
    this.addressOtkuda = Address.fromJSON(addressOtkuda);
    this.addressKuda = Address.fromJSON(addressKuda);
    this.paymentMethodId = paymentMethodId;
    this.paymentAmount = paymentAmount;
    this.routeInfo = RouteInfo.fromJSON(routeInfo);
  }

  static fromJSON(json) {
    const data = typeof json === 'string' ? JSON.parse(json) : json;
    console.info("OrderRequest");
    console.info(data);
    return data ? new OrderRequest(data) : null;
  }
}

class Order {
  constructor({ orderId, orderRequest, rating, paymentMethodName, tariffName, clientName} = {}) {
    this.id = orderId;
    this.request = OrderRequest.fromJSON(orderRequest);
    this.rating = rating;
    this.paymentMethodName = paymentMethodName;
    this.tariffName = tariffName;
    this.clientName = clientName;
  }

  static fromJSON(json) {
    const data = typeof json === 'string' ? JSON.parse(json) : json;
        console.info("Order");
    console.info(data);
    return new Order(data);
  }
}
