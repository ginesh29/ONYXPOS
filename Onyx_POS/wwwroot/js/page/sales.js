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
    postAjax(`/Sales/AddItem?barcode=${barcode}`, null, function (response) {
        if (response.success) {
            playBeep();
            showToastr(response.message);
            loadOrderItem();
            scrollToLastItem();
            closeModal("PriceCheckModal");
        }
        else {
            Swal.fire({
                title: "Please scan Again",
                text: response.message,
                icon: "error",
                confirmButtonClass: 'btn btn-primary',
                willOpen: function () {
                    playErrorBeep();
                }
            });
        }
    })
}
function onEnter(numpadFor) {
    if (numpadFor == "General") {
        var barcode = document.getElementById("Barcode-General").value;
        addSaleItem(barcode);
    }
    else if (numpadFor == "PriceCheck")
        priceCheck(numpadFor)
    document.getElementById("Barcode-General").value = "";
}
function loadOrderItem() {
    loadAjax("/Sales/FetchSaleItems", function (response) {
        document.getElementById("order-item-container").innerHTML = response;
    });
}
loadOrderItem();
function scrollToLastItem() {
    setTimeout(function () {
        container.scrollTo({
            top: container.scrollHeight,
            left: 0,
            behavior: 'smooth'
        });
    }, 200)
}
scrollToLastItem();