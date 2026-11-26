async function showRoute(coords) {
    await ymaps3.ready;
    console.info('Вызвана функция showRoute');
    // console.log(coords);



    var line = [];

    if (coords && coords.length) {
        for (var i = 0; i < coords.length; i++) {
            var p = coords[i];
            if (!p || p.length < 2 || !isFinite(p[0]) || !isFinite(p[1])) {
                continue;
            }
            line.push([p[1], p[0]]);
        }
    }

    if (line.length < 2) {
        console.warn('Геометрия не получена');
        return;
        // line = [from, to];
    }

    if (routeLine) {
        map.removeChild(routeLine);
    }

    routeLine = new ymaps3.YMapFeature({
        id: 'CustomLine',
        geometry: {
            type: 'LineString',
            coordinates: line
        },
        style: {
            stroke: [{
                color: rootStyles.getPropertyValue('--route-color').trim(),
                width: 4,
                opacity: 1.0
            }]
        }
    });

    const lons = line.map(p => p[0]);
    const lats = line.map(p => p[1]);

    const minLon = Math.min(...lons);
    const maxLon = Math.max(...lons);
    const minLat = Math.min(...lats);
    const maxLat = Math.max(...lats);
    const lonPad = Math.max((maxLon - minLon) * 0.15, 0.01);
    const latPad = Math.max((maxLat - minLat) * 0.15, 0.002);
    console.info(lonPad, latPad);
    const zoomProiz = 1.5;

    lastBounds = [
        [minLon - zoomProiz * lonPad, minLat - zoomProiz * latPad],
        [maxLon + lonPad, maxLat + latPad]
    ];
    /*
    [
    [
        37.314787,
        55.943246
    ],
    [
        37.318962,
        55.94495
    ]
]
    */
    console.info(lastBounds);
    map.setLocation({
        bounds: lastBounds,
        duration: 600
    });

    map.addChild(routeLine);
};
