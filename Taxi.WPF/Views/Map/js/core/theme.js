function changeTheme(themeUs) {
    theme = themeUs;
    root.setAttribute('data-theme', theme);

    if (map) {
        map.update({ theme: theme });
    }
    if (routeLine) {
        routeLine.update({
            style: {
                stroke: [{
                    color: rootStyles.getPropertyValue('--route-color').trim(),
                    width: 4,
                    opacity: 1.0
                }]
            }
        });
    }
}

onHostMessage('changeTheme', payload => changeTheme(payload.theme));
