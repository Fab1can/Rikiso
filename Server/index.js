const PORT = 18000;
const WebSocket = require("ws");

const {onReceive, onConnect, onDisconnect} = require("./commands.js")


const wss = new WebSocket.Server({ port: PORT });

wss.on("connection", function connection(ws) {
    ws.on("message", function incoming(message) {
        console.log("received: %s", message.toString());
        onReceive(message, ws);
    });
    ws.on("close", function () {
        onDisconnect(ws);
    });

    onConnect(ws);
});

console.log("Server started on " + PORT);
