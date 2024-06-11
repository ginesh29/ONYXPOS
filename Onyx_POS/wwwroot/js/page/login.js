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
function loginUser(btn) {
    debugger
    var frm = $("#login_frm");
    if (frm.valid()) {
        loadingButton(btn);
        postAjax(`/account/login`, frm.serialize(), function (response) {
            if (response.success) {
                var returnUrl = $("#returnUrl").val();
                returnUrl = returnUrl ? returnUrl : "/";
                showSuccessToastr(response.message)
                window.location.href = returnUrl;
                unloadingButton(btn);
            }
            else
                showErrorToastr(response.message);
            unloadingButton(btn);
        });
    }
}
//$('.keyboard-input').keyboard({
//    layout: 'qwerty',
//    stayOpen: true
//});