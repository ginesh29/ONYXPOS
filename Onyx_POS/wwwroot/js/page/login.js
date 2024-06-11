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
    //if (frm.valid()) {
    loadingButton(btn);
    postAjax(`/account/login`, frm.serialize(), function (response) {
        if (response.success) {
            var returnUrl = $("#returnUrl").val();
            returnUrl = returnUrl ? returnUrl : "/";
            showSuccessToastr(response.message)
            setTimeout(function () {
                unloadingButton(btn);
                window.location.href = returnUrl;
            }, 1000)
        }
        else
            showErrorToastr(response.message);
        unloadingButton(btn);
    });
    //}
}
document.getElementById('Password').addEventListener('keypress', function (e) {
    if (e.keyCode === 13) {
        document.getElementById('btn-login').click();
    }
});

//$('.keyboard-input').keyboard({
//    layout: 'qwerty',
//});