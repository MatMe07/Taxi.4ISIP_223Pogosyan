async function initClient() {
    order_screen = document.getElementById('order-screen');
    waiting_screen = document.getElementById('waiting-screen');

    await initMap();

    init_OrderScreen();
}

initClient();
