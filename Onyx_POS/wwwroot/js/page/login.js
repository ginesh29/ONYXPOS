function validateForm() {
    var userId = document.getElementById('UserId').value;
    var password = document.getElementById('Password').value;

    var userIdError = document.querySelector('[data-valmsg-for="UserId"]');
    var passwordError = document.querySelector('[data-valmsg-for="Password"]');
    userIdError.textContent = "";
    passwordError.textContent = "";
    var isValid = true;
    if (!userId) {
        userIdError.textContent = "Please enter User Id";
        isValid = false;
    }
    if (!password) {
        passwordError.textContent = "Please enter Password";
        isValid = false;
    }
    return isValid;
}

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
    if (validateForm()) {
        loadingButton(cur);
        var form = document.getElementById("login_frm");
        var frmData = new FormData(form);
        postAjax(`/account/login`, frmData, function (response) {
            if (response.success) {
                var returnUrl = document.getElementById("returnUrl").value;
                returnUrl = returnUrl ? returnUrl : "/";
                setTimeout(function () {
                    showSuccessToastr(response.message);
                    setTimeout(function () {
                        unloadingButton(this);
                        window.location.href = returnUrl;
                    }, 500)
                }, 500)
            }
            else
                showSuccessToastr(response.message, "error");
            unloadingButton(cur);
        });
    }
});