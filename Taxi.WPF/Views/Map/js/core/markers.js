async function applyPoint(markerType, coords, isZoom = true) {
    await ymaps3.ready;

    const template = MarkerTemplate[markerType];
    if (!template) {
        console.warn('Неизвестный тип метки:', markerType);
        return;
    }

    removeMarker(markerType);

    const markerElement = document.createElement('div');
    if (template.className)
        markerElement.classList.add(template.className);
    markerElement.innerHTML = template.html;

    markers[markerType] = new ymaps3.YMapMarker(
        {
            coordinates: coords,
        },
        markerElement
    );

    map.addChild(markers[markerType]);

    if (markerType === MarkerType.CLIENT_DEFAULT && isZoom) {
        map.setLocation({
            center: coords,
            zoom: 16,
            duration: 800
        });
    }
}

function removeMarker(markerType) {
    const marker = markers[markerType];
    if (marker != null && map) {
        map.removeChild(marker);
    }
    markers[markerType] = null;
}
