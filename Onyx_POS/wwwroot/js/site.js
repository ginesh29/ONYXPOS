/*var isWeb = document.getElementById('AppType').value == "Web";*/
var dropdownItems = document.querySelectorAll(".language-dropdown-item");
dropdownItems.forEach(function (item) {
    item.addEventListener('click', function (e) {
        e.preventDefault();
        var lang = this.getAttribute("data-value");
        document.getElementById("culture").value = lang;
        document.getElementById("selectLanguage").submit();
    });
});

var liveClock = document.getElementById("live-clock");
if (liveClock)
    window.onload = displayClock();
function displayClock() {
    var date = new Date().toLocaleDateString();
    var time = new Date().toLocaleTimeString();
    liveClock.textContent = `${date} ${time}`;
    setTimeout(displayClock, 1000);
}
function loadingButton(btn) {
    btn.setAttribute("data-kt-indicator", "on");
    btn.disabled = true;
}
function unloadingButton(btn) {
    setTimeout(function () {
        btn.removeAttribute("data-kt-indicator", "on");
        btn.disabled = false;
    }, 1000);
}
function showToastr(msg, type) {
    type = type ? type : "success";
    var html = `<div id="toastr-container" class="toastr-top-right">
                    <div class="toastr toastr-${type}" aria-live="polite" style="">
                        <div class="toastr-message">${msg}</div>
                    </div>
                </div>`
    document.body.insertAdjacentHTML('afterbegin', html);
    setTimeout(function () {
        var toastrContainer = document.getElementById('toastr-container');
        toastrContainer.style.opacity = 0;
    }, 3000);
}
function getAjax(url, callback) {
    fetch(url)
        .then(response => {
            return response.json();
        })
        .then(data => {
            callback(data);
            return data;
        })
}
function loadAjax(url, callback) {
    fetch(url)
        .then(response => {
            return response.text();
        })
        .then(data => {
            callback(data);
            return data;
        })
}
function postAjax(url, frmData, callback) {
    fetch(url, {
        method: 'POST',
        body: frmData
    }).then(response => {
        return response.json();
    }).then(data => {
        callback(data);
        return data;
    });
}
function showModal(modalId) {
    var modal = new bootstrap.Modal(document.getElementById(modalId));
    modal.show();
}
function closeModal(modalId) {
    var myModalEl = document.getElementById(modalId);
    var modal = bootstrap.Modal.getInstance(myModalEl);
    modal.hide();
}
function playBeep() {
    const beep = document.getElementById('beep');
    beep.play();
}
function playErrorBeep() {
    const errorBeep = document.getElementById('error-beep');
    errorBeep.play();
}
function showConfirmation(title, text, callback) {
    Swal.fire({
        title: title,
        text: text,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#0A60E0",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes"
    }).then((result) => {
        if (result.isConfirmed)
            callback();
    });
}
function showErrorAlert(title, text) {
    Swal.fire({
        title: title,
        text: text,
        icon: "warning",
        confirmButtonColor: "#0A60E0",
        cancelButtonColor: "#d33",
        willOpen: function () {
            playErrorBeep();
        }
    });
}