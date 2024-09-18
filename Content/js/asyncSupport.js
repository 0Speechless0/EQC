function resolveAfterMSeconds(msec) {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve();
        }, msec);
    });
}