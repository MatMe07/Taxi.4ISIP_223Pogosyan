


onHostMessage('SetAvailableOrders', payload => {
    // setAvailableOrders(payload.orders);
});

onHostMessage('SetActiveOrder', payload => {
    // setActiveOrder(payload.order);
});

onHostMessage('tryToOnline', payload =>{
    console.info(payload);
    driverIsAllowed = payload.isAllowed;
    messageIsAllowed = payload.message;
});
