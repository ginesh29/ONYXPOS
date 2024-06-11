var isWeb = document.getElementById('AppType').value == "Web";
function loadingButton(btn) {
    var $this = $(btn);
    //loadingPage();
    $this.attr("data-kt-indicator", "on")
    $this.prop("disabled", true);
}
function unloadingButton(btn) {
    var $this = $(btn);
    setTimeout(function () {
        $this.prop("disabled", false);
        $this.removeAttr("data-kt-indicator")
        //unloadingPage();
    }, 1000);
}
function showSuccessToastr(msg) {
    if (isWeb) {
        toastr.clear()
        toastr.success(msg);
    }
}
function showWarningToastr(msg) {
    if (isWeb) {
        toastr.clear()
        toastr.warning(msg);
    }
}
function showErrorToastr(msg) {
    if (isWeb) {
        toastr.clear()
        toastr.error(msg);
    }
}
function getAjax(url, callback) {
    $.get(url, callback);
}
function postAjax(url, formdata, callback) {
    $.post(url, formdata, callback);
}
function filePostAjax(url, formData, callback) {
    $.ajax({
        url: url,
        type: 'POST',
        data: new FormData(formData),
        processData: false,
        contentType: false,
        success: function (response) {
            callback(response);
        },
    });
}
function deleteAjax(url, callback) {
    $.ajax({
        url: url,
        type: 'DELETE',
        success: callback,
    })
}