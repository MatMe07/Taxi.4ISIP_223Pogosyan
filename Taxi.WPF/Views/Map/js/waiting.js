var totalSeconds = 0;
function updateClock(){
    totalSeconds++;

    // let hours = now.getHours();
    let minutes = Math.floor((totalSeconds%3600)/60);
    let seconds = Math.floor(totalSeconds%60);
    // console.info(seconds);
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;

    const timeString = `${minutes}:${seconds}`;
    document.getElementById('clockWaiting').textContent = timeString;
}

var zoomStart = 19.00;
var zoomEnd = 10.00;

function updateZoomMap() {
    if (zoomStart <= zoomEnd) {
        clearInterval(zoomInterval);
        return;
    }

    map.setLocation({
        center: [addressUser.lon, addressUser.lat],
        zoom: zoomStart,
        duration: 300 
    });

    zoomStart -= 0.01; 
}

function clearAnim(){
    clearInterval(zoomInterval);
    clearInterval(secondsInterval);
    totalSeconds = 0;
    zoomStart = 19.00;
    map.setLocation({
        bounds: lastBounds,
        duration: 600
    });


}
