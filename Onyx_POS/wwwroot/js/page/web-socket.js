var host = window.location.host;
var socketProtocol = window.location.protocol.includes("https") ? "wss" : "ws";
var socketUrl = `${socketProtocol}://${host}/ws`;
const socket = new WebSocket(socketUrl);
socket.onmessage = function (event) {
    var connected = event.data == "True";
    const statusElement = document.getElementById("connection-status");
    statusElement.classList.remove("text-warning");
    statusElement.classList.remove("text-success");
    statusElement.classList.add("fa-beat-fade");
    statusElement.classList.add("text-danger");
    statusElement.setAttribute("data-bs-original-title", "Pos Server Connection Lost");
    if (connected) {
        statusElement.classList.remove("fa-beat-fade");
        statusElement.classList.remove("text-danger");
        statusElement.classList.add("text-success");
        statusElement.setAttribute("data-bs-original-title", "Pos Server Connected");
    }
};