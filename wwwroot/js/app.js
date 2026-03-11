window.blazorHelpers = {
    scrollToElement: function (element) {
        if (element) {
            element.scrollIntoView({ behavior: "smooth", block: "start" });
        }
    }
};