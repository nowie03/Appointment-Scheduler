$(function () {
    var today = new Date().toISOString().split('T')[0];
    $('#app-date').attr('min', today);
});


document.addEventListener("DOMContentLoaded", function (event) {
    var scrollpos = localStorage.getItem('scrollpos');
    if (scrollpos) window.scrollTo(0, scrollpos);
});

window.onbeforeunload = function (e) {
    localStorage.setItem('scrollpos', window.scrollY);
};