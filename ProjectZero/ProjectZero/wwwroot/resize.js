window.getWindowWidth = function () {
    return window.innerWidth;
};

window.registerResizeHandler = function (dotNetRef) {
    const handler = () => dotNetRef.invokeMethodAsync('OnWindowResized', window.innerWidth);
    window.addEventListener('resize', handler);
    handler();
    return handler;
};

window.unregisterResizeHandler = function (dotNetRef, handler) {
    window.removeEventListener('resize', handler);
};

window.fitTextToWidth = function (element, maxSize, minSize) {
    if (!element) return;
    const max = maxSize || 128;
    const min = minSize || 16;
    let size = max;
    element.style.fontSize = size + 'px';
    element.style.whiteSpace = 'nowrap';
    const containerWidth = element.parentElement ? element.parentElement.clientWidth : element.clientWidth;
    while (element.scrollWidth > containerWidth && size > min) {
        size -= 1;
        element.style.fontSize = size + 'px';
    }
};
