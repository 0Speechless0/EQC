!function t(i,s,l){function c(o,e){if(!s[o]){if(!i[o]){var r="function"==typeof require&&require;if(!e&&r)return r(o,!0);if(u)return u(o,!0);var a=new Error("Cannot find module '"+o+"'");throw a.code="MODULE_NOT_FOUND",a}var n=s[o]={exports:{}};i[o][0].call(n.exports,function(e){return c(i[o][1][e]||e)},n,n.exports,t,i,s,l)}return s[o].exports}for(var u="function"==typeof require&&require,e=0;e<l.length;e++)c(l[e]);return c}({1:[function(e,o,r){Dashboard=Object.assign({},window.Dashboard||{},{Layout:{init:function(o,r,e){function a(){return u.hasClass("sidebar--collapsed")}function n(){return!!r.matchMedia("(max-width: 991px)").matches}function t(e){var o=void 0===e?!a():!e;return n()?(u.toggleClass("sidebar--collapsed",o),u.removeClass("sidebar--slim")):(u.addClass("sidebar--slim"),u.toggleClass("sidebar--collapsed",o),c.toggleClass("layout__sidebar--slim",o),d.toggleClass("sidebar-menu--slim",o)),o}var i,s,l=o(".action--sidebar-trigger"),c=o(".layout__sidebar"),u=c.find(".sidebar"),d=o(".sidebar-menu"),f=o(".navbar"),g=function(e){void 0!==r.localStorage&&localStorage.setItem("isSlim",e?"on":"off")},b=function(){return void 0!==r.localStorage&&"on"===localStorage.getItem("isSlim")};l.on("click",function(e){e.stopImmediatePropagation();var o=t();n()||g(o)}),o(r).on("resize",function(){var e,o=n();o!==i&&(o?t(!1):(e=b(),t(!e)),i=o)}),o(e).on("click",function(e){n()&&0<c.length&&e.target!==c[0]&&!c[0].contains(e.target)&&!a()&&t(!1)}),f.on("show.bs.dropdown",function(e){0===o(e.target).closest(".navbar-collapse").length&&f.find(".navbar-collapse").collapse("hide")}),n()?t(!1):(s=b(),t(!s))}}})},{}]},{},[1]);
//# sourceMappingURL=layout.js.map