window.chatHelpers = {
    scrollToBottomById: function (elementId) {
        const scrollNow = () => {
            const el = document.getElementById(elementId);
            if (!el) return;

            el.scrollTop = el.scrollHeight;
            el.scrollTop = 999999;
        };

        scrollNow();

        requestAnimationFrame(() => {
            scrollNow();

            setTimeout(() => {
                scrollNow();
            }, 50);
        });
    }
};