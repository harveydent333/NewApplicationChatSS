"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceiveCommand", function (message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = "console: " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var userEmail = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
   // let userLogin = document.querySelector('#user-login').value;

    if (message.match(/^\/\//) != null) {
        if (message.match(/^\/\/user/)) {
            connection.invoke("ReceivingUserInteractionCommand", userEmail, message).catch(function (err) { // ТУТ userEmail менять
                return console.error(err.toString());
            });
        }
        else if (message.match(/^\/\/room/)) {
            connection.invoke("ReceivingRoomInteractionCommand", userEmail, roomId, message).catch(function (err) { // ТУТ userEmail менять
                return console.error(err.toString());
            });
        }
        else {
            sendCommandHelp(userLogin, roomId, message);
        }
    }
    else {
        //connection.invoke("ReceivingInteractionMessage", userLogin, roomId, message).catch(function (err) {
        //    return console.error(err.toString());
        //});
        connection.invoke("SendMessage", userEmail, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
});
