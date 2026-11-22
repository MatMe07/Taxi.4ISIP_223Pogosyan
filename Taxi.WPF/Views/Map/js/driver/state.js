function cacheDriverElements() {
    driverElements.layer = document.getElementById('ui-layer');
    driverElements.overlay = document.querySelector('.overvlay-map');
    driverElements.balanceText = document.getElementById('balance-text');
    driverElements.ordersText = document.getElementById('orders-text');
    driverElements.statusText = document.getElementById('driver-status-text');
    driverElements.lineBtn = document.getElementById('line-btn');
    driverElements.lineBtnText = driverElements.lineBtn.querySelector('span');
    driverElements.lineHint = document.getElementById('line-hint');
    driverElements.acceptBtn = document.getElementById('accept-btn');
    driverElements.skipBtn = document.getElementById('skip-btn');
    driverElements.arriveBtn = document.getElementById('arrive-btn');
    driverElements.startBtn = document.getElementById('start-btn');
    driverElements.finishBtn = document.getElementById('finish-btn');
    driverElements.backBtn = document.getElementById('back-to-line-btn');
    driverElements.cancelBackBtn = document.getElementById('cancel-back-btn');
    driverElements.cancelBtn = document.getElementById('cancel-to-line-btn');
    driverElements.orderStatusText = document.getElementById('order-status-text');
    driverElements.completedFare = document.getElementById('completed-fare');
    
    driverElements.offerCard = document.getElementById('offerCard');
    driverElements.activeCard = document.getElementById('activeOrderCard');
}

function formatMoney(value) {
    return `${value.toFixed(2)} ₽`;
}

function formatOrders(count) {
    const mod10 = count % 10;
    const mod100 = count % 100;

    if (mod10 === 1 && mod100 !== 11) {
        return `${count} заказ`;
    }
    if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14)) {
        return `${count} заказа`;
    }

    return `${count} заказов`;
}

function updateDriverBalance() {
    driverElements.balanceText.textContent = formatMoney(driverBalance);
    driverElements.ordersText.textContent = formatOrders(driverOrdersCount);
}

function updateOrderStatusText(nextState) {
    if (nextState === DRIVER_STATE.TO_CLIENT) {
        driverElements.orderStatusText.textContent = 'В пути к клиенту';
    } else if (nextState === DRIVER_STATE.AT_CLIENT) {
        driverElements.orderStatusText.textContent = 'На месте';
    } else if (nextState === DRIVER_STATE.IN_TRIP) {
        driverElements.orderStatusText.textContent = 'В поездке';
    }
}

function setDriverState(nextState) {
    driverState = nextState;
    driverElements.layer.dataset.state = nextState;

    clearTimeout(offerTimer);
    offerTimer = null;

    cancelAllHolds();

    const isOffline = nextState === DRIVER_STATE.OFFLINE;
    const isWaiting = nextState === DRIVER_STATE.WAITING;

    driverElements.lineBtn.classList.toggle('offline', isOffline);
    // if (driverIsAllowed){
        
    // }
        driverElements.lineBtn.classList.toggle('online', isWaiting);

    driverElements.lineBtn.setAttribute('aria-pressed', String(isWaiting));
    driverElements.lineBtnText.textContent = isOffline ? 'На линию' : 'Завершить';

    driverElements.overlay.classList.toggle('offline', isOffline);
    // if (driverIsAllowed)
        driverElements.overlay.classList.toggle('online', !isOffline);
    console.info(isOffline);
    if (driverStatusOnline !== !isOffline){
        driverStatusOnline = !isOffline;
        postToHost({type:"driverToOnline", status:driverStatusOnline.toString()})
    }

    driverElements.statusText.textContent = isOffline
        ? 'Занят'
        : isWaiting
            ? 'На линии'
            : '';

    updateORderState(nextState);
    updateOrderStatusText(nextState);
    updateRouteForState(nextState);

    if (isWaiting) {
        // scheduleOffer();
    }
}

function initDriverState() {
    cacheDriverElements();
    updateDriverBalance();
    setDriverState(DRIVER_STATE.OFFLINE);

    // if (!driverIsAllowed) {
    //     driverElements.lineBtn.addEventListener('pointerdown', ()=>{
    //         console.info(driverElements.lineBtn.classList);
    //         driverElements.lineBtn.classList.remove('holding');
    //     });
    //     driverElements.lineBtn.classList.add('hidden');
    //     console.info("msdfjsldkfjsldfk");
    // }
    setupHold(driverElements.lineBtn, () => {
        if (driverState === DRIVER_STATE.OFFLINE) {
            setDriverState(DRIVER_STATE.WAITING);
        } else if (driverState === DRIVER_STATE.WAITING) {
            setDriverState(DRIVER_STATE.OFFLINE);
        }
    });


    window.addEventListener('pointerup', cancelAllHolds);
    window.addEventListener('blur', cancelAllHolds);
}
