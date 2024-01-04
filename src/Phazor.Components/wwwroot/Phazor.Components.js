function subscribeOnScrollFinished(elementId, handler) {
    const el = document.getElementById(elementId);

    el.addEventListener('scroll', async () => {
        if (el.scrollHeight - el.offsetHeight - el.scrollTop < 1) {
            await handler.invokeMethodAsync('OnScrollFinished');
        }
    });
}

function isContentFillingParent(elementId) {
    const el = document.getElementById(elementId);
    return el.offsetHeight < el.scrollHeight;
}