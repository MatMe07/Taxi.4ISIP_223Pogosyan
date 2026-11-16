function cancelAllHolds() {
    holdCleanups.forEach(cleanup => cleanup());
}

function setupHold(button, onComplete, duration = 1500) {
    let timer = null;

    button.style.setProperty('--hold-duration', `${duration}ms`);

    const cleanup = () => {
        if (timer !== null) {
            clearTimeout(timer);
            timer = null;
        }
        button.classList.remove('holding');
    };

    const start = event => {
        if (event.type === 'pointerdown' && event.button !== 0) return;

        cancelAllHolds();

        button.classList.add('holding');
        timer = setTimeout(() => {
            timer = null;
            button.classList.remove('holding');
            onComplete();
        }, duration);
    };

    button.addEventListener('pointerdown', start);
    button.addEventListener('pointerup', cleanup);
    button.addEventListener('pointerleave', cleanup);
    button.addEventListener('pointercancel', cleanup);

    holdCleanups.push(cleanup);
}

function setupTimoutElement(duration = 5000){
    let timer = null;
    timer = setTimeout(() => {

            timer = null;
            // button.classList.remove('holding');
            // onComplete();
            console.info(driverState);
            if(driverState === DRIVER_STATE.OFFER){
                setDriverState(DRIVER_STATE.WAITING);

            }    
        }, duration);
}
