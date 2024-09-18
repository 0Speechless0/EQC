var StyleChangeAction;

window.observer = new MutationObserver(function(mutations) {
    console.log('style changed!');
    StyleChangeAction(); 
});

window.setStyleObserver = (id, action) => {

    var target = document.getElementById(id);
    console.log("setStyleObserver", id);
    StyleChangeAction = action;
    window.observer.observe(target, { attributes : true, attributeFilter : ['style'] });
}
