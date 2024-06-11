function showHidePasswordAsterisk(id, curEl) {
    var x = document.getElementById(id);
    if (x.type === "password") {
        x.type = "text";
        $(curEl).parent().find(".eye-icon").removeClass("fa-eye").addClass("fa-eye-slash");
    }
    else {
        x.type = "password";
        $(curEl).parent().find(".eye-icon").removeClass("fa-eye-slash").addClass("fa-eye");
    }
}