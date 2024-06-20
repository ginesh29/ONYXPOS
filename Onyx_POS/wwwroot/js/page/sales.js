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