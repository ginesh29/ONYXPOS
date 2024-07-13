function showHidePasswordAsterisk(id, curEl) {
    var x = document.getElementById(id);
    var eyeIcon = curEl.parentNode.querySelector(".eye-icon");
    if (x.type === "password") {
        x.type = "text";
        eyeIcon.classList.remove("fa-eye");
        eyeIcon.classList.add("fa-eye-slash");
    } else {
        x.type = "password";
        eyeIcon.classList.remove("fa-eye-slash");
        eyeIcon.classList.add("fa-eye");
    }
}

document.getElementById('Password').addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        document.getElementById('btn-login').click();
    }
});

document.getElementById('btn-login').addEventListener('click', function (e) {
    var cur = this;
    e.preventDefault();
    var validForm = validateLoginForm();
    if (validForm.isValid) {
        loadingButton(cur);
        var form = document.getElementById("login_frm");
        var frmData = new FormData(form);
        postAjax(`/account/login`, frmData, function (response) {
            if (response.success) {
                var returnUrl = document.getElementById("returnUrl").value;
                returnUrl = returnUrl ? returnUrl : "/";
                setTimeout(function () {
                    showToastr(response.message, "success");
                    setTimeout(function () {
                        unloadingButton(this);
                        window.location.href = returnUrl;
                    }, 500)
                }, 500)
            }
            else
                showErrorAlert("Unauthorized User", response.message)
            unloadingButton(cur);
        });
    }
    else
        showErrorAlert(null, validForm.errorMessage);
});