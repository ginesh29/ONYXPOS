const connection = new signalR.HubConnectionBuilder().withUrl("/connectionStatusHub").build();

connection.on("ReceiveConnectionStatus", function (connected) {
    const statusElement = document.getElementById("connection-status");
    statusElement.classList.remove("text-success");
    statusElement.classList.add("animation-blink");
    statusElement.classList.add("text-danger");
    statusElement.setAttribute("data-bs-original-title", "Pos Server Connection Lost");
    if (connected) {
        statusElement.classList.remove("animation-blink");
        statusElement.classList.remove("text-danger");
        statusElement.classList.add("text-success");
        statusElement.setAttribute("data-bs-original-title", "Pos Server Connected");
    }
});

connection.start();