async function initMap() {
    await ymaps3.ready;

    rootStyles = window.getComputedStyle(document.documentElement);

    const { YMap, YMapDefaultSchemeLayer, YMapDefaultFeaturesLayer } = ymaps3;

    map = new YMap(
        document.getElementById('map'),

        {
            location: {
                center: [37.588144, 55.733842],
                zoom: 10
            },
            theme: theme

        }
    );
    map.addChild(new YMapDefaultSchemeLayer({
        theme: theme
    }));
    map.addChild(new YMapDefaultSchemeLayer());
    map.addChild(new YMapDefaultFeaturesLayer());
}
