$(document).ready(function () {
    // Get the current URL path
    var currentPath = window.location.pathname;

    // Iterate over each navigation link
    $('nav a').each(function () {
        var linkPath = $(this).attr('href');
        // If the link path matches the current path, add the active class
        if (linkPath === currentPath) {
            $(this).addClass('active-tab');
        }
    });
});
