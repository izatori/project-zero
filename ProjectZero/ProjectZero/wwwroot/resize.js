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
    const containerWidth = element.parentElement ? element.parentElement.clientWidth : element.clientWidth;
    const targetWidth = containerWidth * 0.8; // leave 10% margin on each side
    let size = max;
    element.style.fontSize = size + 'px';
    element.style.whiteSpace = 'nowrap';
    while (element.scrollWidth > targetWidth && size > min) {
        size -= 1;
        element.style.fontSize = size + 'px';
    }
};
