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

    //--------------------- menu 伸縮 ----------------------

    $(".menu-control").click(function() {
        if ($(window).width() > 576) {
            if ($(".main-left").hasClass('off')) {
                $(".main-left").animate({ left: 0 }, 200);
                $(".main-left").removeClass('off');
                $(this).find('i').removeClass('fa-chevron-right');
                $(this).find('i').addClass('fa-chevron-left');
                $(".main-right").animate({ "marginLeft": 230 }, 500);
            } else {
                $(".main-left").animate({ left: -204 }, 100);
                $(".main-left").addClass('off');
                $(this).find('i').removeClass('fa-chevron-left');
                $(this).find('i').addClass('fa-chevron-right');
                $(".main-right").animate({ "marginLeft": 30 }, 500);
            }
        } else {
            if ($(".main-left").hasClass('off')) {
                $(".main-left").animate({ left: 0 }, 200);
                $(".main-left").removeClass('off');
                $(this).find('i').removeClass('fa-chevron-right');
                $(this).find('i').addClass('fa-chevron-left');

            } else {
                $(".main-left").animate({ left: -204 }, 100);
                $(".main-left").addClass('off');
                $(this).find('i').removeClass('fa-chevron-left');
                $(this).find('i').addClass('fa-chevron-right');
                $(".main-right").animate({ "marginLeft": 30 }, 500);
            }
        }

    });

    //---------------------callapse展開----------------------

    $("div[id^=heading] > h4 > a").click(function() {
        if ($(this).hasClass('collapsed')) {
            $("div[id^=heading] > h4 > a").removeClass('chosen');
            $(this).addClass('chosen');
        } else {
            $(this).removeClass('chosen');
        }
    });


}) //JS尾端