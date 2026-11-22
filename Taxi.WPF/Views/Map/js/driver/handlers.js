


onHostMessage('setPoint', async payload => {
    // setAvailableOrders(payload.orders);
    driverAddress = Address.fromJSON(payload.point);
    console.info("setPoint");
    console.info(driverAddress);
    toClientRoute.addressOtkuda = driverAddress;
    await setPoint(driverAddress );

});

onHostMessage('newOrder', payload => {
    // setActiveOrder(payload.order);
    console.info(payload);
    order = Order.fromJSON(payload.request);
    // postToHost({type: ''})
    toClientRoute= RouteInfo.fromJSON(payload.routeDriverToClent);
    console.info("newOrder");
    console.info(toClientRoute);

    toClientRoute.addressKuda = order.request.addressOtkuda;

    setDriverState(DRIVER_STATE.OFFER);

});

onHostMessage('tryToOnline', payload => {
    console.info(payload);
    driverIsAllowed = payload.isAllowed;
    messageIsAllowed = payload.message;
});


// { "orderId": 2011, "orderRequest": { "clientId": 4, "tariffId": 1, "addressOtkuda": { "lat": 55.943215, "lon": 37.318936, "address": "Овражная улица, 24к8" }, "addressKuda": { "lat": 55.941712, "lon": 37.319547, "address": "Овражная улица, 24к11" }, "paymentMethodId": 1, "paymentAmount": 339, "routeInfo": { "distanceMeters": 255.312, "durationSeconds": 77.219, "coordinates": [[55.943246, 37.31874], [55.942998, 37.318614], [55.942975, 37.318602], [55.942879, 37.318562], [55.942773, 37.319431], [55.942664, 37.320189], [55.942465, 37.320114], [55.942059, 37.319975], [55.942019, 37.319925], [55.941896, 37.319857], [55.941714, 37.31986]] } }, "rating": 5.00, "paymentMethodName": "Наличные", "tariffName": "Эконом", "clientName": "client" }
