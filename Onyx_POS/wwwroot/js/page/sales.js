const container = document.getElementById('order-item-container');
const content = document.getElementById('order-item-content');
const scrollAmount = 100;
document.getElementById('btn-table-scroll-up').addEventListener('click', function () {
    container.scrollBy({
        top: -scrollAmount,
        left: 0,
        behavior: 'smooth'
    });
});
document.getElementById('btn-table-scroll-down').addEventListener('click', function () {
    container.scrollBy({
        top: scrollAmount,
        left: 0,
        behavior: 'smooth'
    });
});

const numpadInputs = document.querySelectorAll('.numpad-input');
const numButtons = document.querySelectorAll('.keypad-button');
let activeInput = document.getElementById("Barcode-General");

var inputElements = document.querySelectorAll('input[type="text"]');
inputElements.forEach(function (inputElement) {
    inputElement.addEventListener("focus", function (e) {
        activeInput = event.target;
    });
});
const handleClick = (event) => {
    if (activeInput) {
        if (event.currentTarget.dataset.key == "backspace") {
            activeInput.value = activeInput.value.slice(0, -1);
        }
        else if (event.currentTarget.dataset.key == "clear") {
            activeInput.value = null;
            document.getElementById('PriceCheckItems').innerHTML = "";
        }
        else {
            if (event.currentTarget.dataset.key != "enter")
                activeInput.value += event.currentTarget.textContent;
        }
    }
};
numButtons.forEach(button => {
    button.addEventListener('click', handleClick);
});
document.querySelectorAll('input, textarea').forEach(function (element) {
    element.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            var type = document.getElementById("PriceCheckModalType").value;
            if (type != "PriceCheck" && type != "Qty" && type != "Void")
                onEnter("General");
            else
                onEnter(type);
        }
    });
});
function showQtyModal(e, modalType) {
    document.getElementById("PriceCheckModalType").value = modalType;
    var modalTitle = modalType == "PriceCheck" ? "Item Price Check" : modalType == "Void" ? "Void Item" : modalType == "Refund" ? "Refund Item" : "Update Item Quantity";
    document.getElementById("PriceCheckModalLabel").textContent = modalTitle;
    document.getElementById("PriceCheckModalDialog").style.maxWidth = "1200px";
    document.getElementById("left-container").classList.add("col-md-6");
    document.getElementById("left-container").classList.remove("col-md-12");
    var authType = e.dataset.authType;
    if (authType) {
        var allowedAuth = false;
        checkAuth(authType, function (response) {
            allowedAuth = response.data.allowed;
            if (allowedAuth || authType == "allowed") {
                document.getElementById("Qty").value = 1;
                document.getElementById("Qty").closest('.form-group').parentElement.classList.add('d-none');
                if (modalType == "Qty") {
                    document.getElementById("Qty").closest('.form-group').parentElement.classList.remove('d-none');
                    document.getElementById("PriceCheckModalDialog").style.maxWidth = "600px";
                    document.getElementById("left-container").classList.remove("col-md-6");
                    document.getElementById("left-container").classList.add("col-md-12");
                }
                if (modalType == "Void" || modalType == "Refund") {
                    var table = document.getElementById("order-item-content");
                    var items = table.rows.length;
                    document.getElementById("PriceCheckModalDialog").style.maxWidth = "600px";
                    document.getElementById("left-container").classList.remove("col-md-6");
                    document.getElementById("left-container").classList.add("col-md-12");
                    if (items > 1 || modalType == "Void") {
                        var qtyVoidEnabled = document.getElementById("QtyVoidEnabled").value;
                        if (qtyVoidEnabled == 1)
                            document.getElementById("Qty").closest('.form-group').parentElement.classList.remove('d-none');
                    }
                    if (items > 1 || modalType == "Refund") {
                        document.getElementById("Qty").value = -1;
                        showModal("PriceCheckModal");
                    }
                    else
                        showErrorAlert("No Items to void");
                }
                if (modalType == "Qty")
                    showModal("PriceCheckModal");
            }
            else
                alert(allowedAuth);
        });
    }
    if (modalType == "PriceCheck")
        showModal("PriceCheckModal");
}
function priceCheck(numpadFor) {
    var code = document.getElementById(`Barcode-${numpadFor}`).value;
    var url = `/Sales/GetPriceCheckItems?code=${code}`;
    loadAjax(url, function (response) {
        document.getElementById('PriceCheckItems').innerHTML = response;
    });
}
document.getElementById('PriceCheckModal').addEventListener('shown.bs.modal', function () {
    activeInput = document.getElementById("Barcode-PriceCheck");
});
document.getElementById('PriceCheckModal').addEventListener('hidden.bs.modal', function () {
    document.getElementById(`Barcode-PriceCheck`).value = "";
    document.getElementById('PriceCheckItems').innerHTML = "";
    activeInput = document.getElementById("Barcode-General");
});
function addSaleItem(barcode) {
    var frmData = new FormData();
    var qty = document.getElementById("Qty").value;
    var modalType = document.getElementById("PriceCheckModalType").value;
    frmData.append("barcode", barcode);
    frmData.append("qty", qty);
    frmData.append("type", modalType);
    postAjax(`/Sales/AddItem`, frmData, function (response) {
        if (response.success) {
            playBeep();
            showSuccessToastr(response.message);
            loadOrderItems();
            if (modalType != "General")
                closeModal("PriceCheckModal");
        }
        else
            showErrorAlert("Please scan Again", response.message);
    })
}
function holdBill(e) {
    var authType = e.dataset.authType;
    var allowedAuth = false;
    checkAuth(authType, function (response) {
        allowedAuth = response.data.allowed;
        if (allowedAuth || authType == "allowed") {
            var table = document.getElementById("order-item-content");
            var items = table.rows.length;
            if (items > 1) {
                var frmData = new FormData();
                postAjax("/Sales/HoldBill", frmData, function (response) {
                    showSuccessToastr(response.message);
                    loadOrderItems();
                    incrementTransNo();
                });
            }
            else
                showErrorAlert("No Transactions available");
        }
        else
            alert(allowedAuth);
    })
}
function recallBill(e) {
    var authType = e.dataset.authType;
    var allowedAuth = false;
    checkAuth(authType, function (response) {
        allowedAuth = response.data.allowed;
        if (allowedAuth || authType == "allowed") {
        }
        else
            alert(allowedAuth);
    })
}
function cancelBill(e, transNo) {
    var authType = e.dataset.authType;
    var allowedAuth = false;
    checkAuth(authType, function (response) {
        allowedAuth = response.data.allowed;
        if (allowedAuth || authType == "allowed") {

            var table = document.getElementById("order-item-content");
            var items = table.rows.length;
            if (items > 1)
                showConfirmation("Are you sure?", "You want to cancel", function () {
                    var frmData = new FormData();
                    frmData.append("transNo", transNo);
                    postAjax("/Sales/CancelBill", frmData, function (response) {
                        showSuccessToastr(response.message);
                        loadOrderItems();
                        incrementTransNo();
                    });
                });
            else
                showErrorAlert("No Transactions available");
        }
        else
            alert(allowedAuth);
    })
}
function onEnter(numpadFor) {
    if (numpadFor == "General") {
        var barcode = document.getElementById("Barcode-General").value;
        addSaleItem(barcode);
    }
    else {
        var modalType = document.getElementById("PriceCheckModalType").value;
        if (modalType == "PriceCheck")
            priceCheck(numpadFor)
        else {
            var barcode = document.getElementById("Barcode-PriceCheck").value;
            addSaleItem(barcode);
        }
    }
    document.getElementById("Barcode-General").value = "";
}
function loadOrderItems() {
    loadAjax("/Sales/FetchSaleItems", function (response) {
        document.getElementById("order-item-container").innerHTML = response;
        setTimeout(function () {
            container.scrollTo({
                top: container.scrollHeight,
                left: 0,
                behavior: 'smooth'
            });
        }, 200)
    });
}
loadOrderItems();
function checkAuth(authType, callback) {
    getAjax(`/Sales/CheckAuth?key=${authType}`, function (response) {
        callback(response);
    });
}
function incrementTransNo() {
    var el = document.getElementById("bill-number");
    var transNo = el.textContent;
    el.textContent = Number(transNo) + 1;
}