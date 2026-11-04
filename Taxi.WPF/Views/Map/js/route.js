window.showRoute = async function (coords, from, to) {
    await ymaps3.ready;
    console.info('Вызвана функция showRoute');
    console.log(coords);



    hideNotice("search");
    hideNotice("search2");

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
        console.warn('Геометрия не получена, рисуем прямую между точками');
        line = [from, to];
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
                color: '#0000FF55',
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

    map.setLocation({
        bounds: [
            [minLon, minLat],
            [maxLon, maxLat]
        ],
        duration: 600
    });

    map.addChild(routeLine);
};
