self.addEventListener('push', event => {
    const payload = event.data.json();
    event.waitUntil(
        self.registration.showNotification('UniversityAssistant', {
            body: payload.message,
            vibrate: [100, 50, 100],
            data: { url: payload.url }
        })
    );
});