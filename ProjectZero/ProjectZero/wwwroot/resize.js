window.registerResizeHandler = function (dotNetRef) {
    const handler = () => dotNetRef.invokeMethodAsync('OnWindowResized', window.innerWidth);
    window.addEventListener('resize', handler);
    handler();
    return handler;
};

window.unregisterResizeHandler = function (dotNetRef, handler) {
    window.removeEventListener('resize', handler);
};
