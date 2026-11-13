const hostHandlers = {};

function onHostMessage(type, handler) {
    hostHandlers[type] = handler;
}

if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', async event => {
        console.info('Message from C#');
        const command = event.data;
        const { type, payload } = command;

        const handler = hostHandlers[type];
        if (handler) {
            await handler(payload);
        } else {
            console.warn('Нет обработчика для сообщения:', type);
        }
    });
}
