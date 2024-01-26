// JavaScript Document
$(function() { //JS開頭

    var WINDOW = $(window).width(); //視窗寬度
    var WINDOWH = $(window).height(); //視窗高度

    //----------------gotop功能-----------------
    $("#gotop").click(function() {
        $("html,body").animate({ scrollTop: 0 }, 300);
        return false;
    })

    $("#mobile-gotop").click(function() {
        $("html,body").animate({ scrollTop: 0 }, 300);
        return false;
    })

    $(window).scroll(function() {
        if ($(this).scrollTop() > 100) { //若目前的位置距離網頁頂端>100px
            $("#gotop").fadeIn("fast");
        } else {
            $("#gotop").stop().fadeOut("fast");
        }
    });
    

}) //JS尾端




