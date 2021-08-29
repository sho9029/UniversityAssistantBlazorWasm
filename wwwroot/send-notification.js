async function sendNotification(title, options) {
    const registration = await navigator.serviceWorker.getRegistration();
    registration.showNotification(title, options);
}