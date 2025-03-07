mergeInto(LibraryManager.library, {
    OpenFileSelectionJS: function() {
        if (typeof window.OpenFileSelectionJS === "function") {
            window.OpenFileSelectionJS();
        } else {
            console.error("Функція OpenFileSelectionJS не знайдена.");
        }
    }
});