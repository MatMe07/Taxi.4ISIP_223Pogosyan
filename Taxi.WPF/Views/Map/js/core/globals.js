var map = null;
var rootStyles = null;

var routeLine = null;
var lastBounds = null;

var addressUser = null;

var markerOtkuda = null;
var markerKuda = null;

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
