$(window).on("scroll load resize", function () {
    checkScroll();
});


/**
* Listen to scroll to change header opacity class
*/
function checkScroll() {
    var scroll = $(window).scrollTop();
    var alpha = scroll / 100;

    // Transparência da barra de menu
    if (alpha < .8) {
        $('.menubar').css({"background-color": "rgba(1,60,153," + alpha + ")" });
        $('.menubar').css({ "box-shadow": " 0px 2px 5px 0px rgba(0,0,0," + alpha/2 + ")" });
    }
}