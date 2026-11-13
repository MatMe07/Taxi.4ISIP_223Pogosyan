async function applyPoint(which, coords, address) {
    // console.info(map);
    // var pm = new YMapDefaultMarker(new YMaps.GeoPoint(coords));
    await ymaps3.ready;



    if (which === 'from') {
        // console.info(markerOtkuda);
        if (markerOtkuda != null)
            map.removeChild(markerOtkuda);

        const markerElement = document.createElement('div');
        markerElement.classList.add("placemark");
        markerElement.innerHTML = `
    <div class="label-container">
        <svg class="user-icon" width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <circle cx="12" cy="8" r="4" fill="#ffffffff"/>
            <path d="M4 20C4 16.6863 6.68629 14 10 14H14C17.3137 14 20 16.6863 20 20V21H4V20Z" fill="#ffffffff"/>
        </svg>
    </div>
    <div class="placemark-tail"></div>
    
    `;

        markerOtkuda = new ymaps3.YMapMarker(
            {
                coordinates: coords,
            },
            markerElement
        );

        map.addChild(markerOtkuda);
        map.setLocation({
            center: coords,
            zoom: 16,
            duration: 800
        });
    }
    else {
        // console.info(markerKuda);
        if (markerKuda != null)
            map.removeChild(markerKuda);
        const markerElement = document.createElement('div');
        markerElement.innerHTML = `
<div class="map-marker-station  pulse">
    <div class="marker-circle"></div>
</div>
    
    `;

        markerKuda = new ymaps3.YMapMarker(
            {
                coordinates: coords
            },
            markerElement
        );

        map.addChild(markerKuda);
    }

}
