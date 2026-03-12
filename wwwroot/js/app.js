window.blazorHelpers = {
    scrollToElement: function (element) {
        if (element) {
            element.scrollIntoView({ behavior: "smooth", block: "start" });
        }
    }
};

window.modalHelpers = {
    show: function (modalId) {
        const el = document.getElementById(modalId);
        if (!el) return;

        const modal = bootstrap.Modal.getOrCreateInstance(el);
        modal.show();
    },
    hide: function (modalId) {
        const el = document.getElementById(modalId);
        if (!el) return;

        const modal = bootstrap.Modal.getOrCreateInstance(el);
        modal.hide();
    }
};