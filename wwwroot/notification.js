self.addEventListener('notificationclick', event => {
    event.notification.close();
    event.waitUntil(clients.matchAll({ type: 'window' }).then(clientArr => {
        const hadWindowToFocus = clientArr.some(windowClient => windowClient.url === event.notification.data.url ? (windowClient.focus(), true) : false);
        if (!hadWindowToFocus) {
            clients.openWindow(event.notification.data.url).then(windowClient => {
                windowClient ? windowClient.focus() : null;
            });
        }
    }));
});
