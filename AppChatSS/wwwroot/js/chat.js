var connection = new signalR.HubConnectionBuilder().withUrl("/Chat/chathub").build();

document.getElementsByClassName("msg_send_btn").disabled = true;

// Users: Принимает входящее сообщение
connection.on("ReceiveMessage", function (messageInfoJSON) {
    let messageInfo = JSON.parse(messageInfoJSON);

    if (document.querySelector('#current-room-id').value == messageInfo['roomId']) {
        if (document.querySelector('#user-login').value == messageInfo['userLogin']) {
            createOutgoinglemMessage(messageInfo);
        }
        else {
            createIncomigElemMessage(messageInfo);
        }
    }
    else {
        addInfoForLastMessage(messageInfo);
    }
});

// Caller:  Ответ от консоли 
connection.on("ReceiveCommand", function (command) {
    createIncomigElemMessage(JSON.parse(command), true);
});

// Users:    //room remove {roomName}  
connection.on("RemoveRoomUsers", function (command, roomId) {
    if (isCurrentRoom(roomId)) {
        redirectToMainRoom();
    }
    else {
        removeRoomElem(roomId);
        createIncomigElemMessage(JSON.parse(command), true);
    }
});

// User:    //room speak -l {userLogin}
connection.on("UnmutedUser", function (command, roomId) {
    if (isCurrentRoom(roomId)) {
        createIncomigElemMessage(JSON.parse(command), true);
    }
});

// User:    //room connect {roomName} -l {userLogin}
connection.on("ConnectRoom", function (roomInfoJSON) {
    createRoomElem(JSON.parse(roomInfoJSON));
});

// User: //room rename {roomName}
connection.on("RenameRoomUser", function (roomId, nameRoom) {
    if (isCurrentRoom(roomId)) {
        renameTitleRoom(nameRoom);
        renameElemRoom(roomId, nameRoom);
    }
    else {
        renameElemRoom(roomId, nameRoom);
    }
});

// User:    //room disconnect {roomName} -l {loginUser}
connection.on("DisconnectFromRoomUser", function (roomId) {
    if (isCurrentRoom(roomId)) {
        redirectToMainRoom();
    }
});

// User:    //room mute -l {loginUser}
connection.on("MuteUser", function (roomId) {
    if (isCurrentRoom(roomId)) {
        alert("Вы были лишены возможности отправлять сообщения в чат.");
    }
});

// Caller: //help
connection.on("PrintAllowedCommands", function (command) {
    command = JSON.parse(command);
    var listCommands = '';
    command['messageContent'].forEach(function (item, i, arr) {
        listCommands += item + '\n';
        countRows = i;
    });
    command['messageContent'] = listCommands;
    createIncomigElemMessage(command, true, true, countRows + 2);
});

// Caller: //find {nameChannel}||{nameVideo}
connection.on("PrintInfoAboutVideo", function (command) {
    command = JSON.parse(command);

    infoAboutVideo = command['title'] + '\n';

    if (command['view'] != '') {
        infoAboutVideo += 'Количество просмотров: ' + command['view'] + '\n';
    }

    if (command['likes'] != '') {
        infoAboutVideo += 'Количество лайков: ' + command['likes'] + '\n';
    }
    command['messageContent'] = infoAboutVideo;

    createIncomigElemMessage(command, true, true, countRows = 6);
});

// Caller: //info {nameChannel}
connection.on("PrintInfoAboutChannel", function (command) {
    command = JSON.parse(command);
    if (command['messageContent'] != null) {
        var referencesOnVideos = 'Канал: ';
        referencesOnVideos += command['title'] + '\n';
        command['messageContent'].forEach(function (item, i, arr) {
            referencesOnVideos += item + '\n';
            countRows = i;
        });
        command['messageContent'] = referencesOnVideos;
    } else {
        countRows = 1;
        command['messageContent'] = command['title'];
    }

    createIncomigElemMessage(command, true, true, countRows + 3);
});

// Caller: //videoCommentRandom {nameChannel}||{nameVideo}
connection.on("PrintCommentInfo", function (command) {
    command = JSON.parse(command);

    infoAboutComment = command['authorComment'] + '\n';
    infoAboutComment += command['commentContext'];

    command['messageContent'] = infoAboutComment;
    createIncomigElemMessage(command, true, true, countRows = 10);
});

// Caller: //room rename {roomName}
connection.on("RenameRoom", function (command, roomId, nameRoom) {
    renameTitleRoom(nameRoom);
    renameElemRoom(roomId, nameRoom);
    createIncomigElemMessage(JSON.parse(command), true);
});

// Caller:    //room create {roomName}
connection.on("CreateRoom", function (commandInfoJSON, roomInfoJSON) {
    createRoomElem(JSON.parse(roomInfoJSON));
    createIncomigElemMessage(JSON.parse(commandInfoJSON), true);
});

// Caller:    //room remove {roomName}  
connection.on("RemoveRoomCaller", function (command, roomId) {
    if (isCurrentRoom(roomId)) {
        redirectToMainRoom();
    } else {
        removeRoomElem(roomId);
        createIncomigElemMessage(JSON.parse(command), true);
    }
});

// Caller:    //room disconnect {roomName}
connection.on("DisconnectFromRoom", function (command, roomId) {
    if (isCurrentRoom(roomId)) {
        redirectToMainRoom();
    } else {
        removeRoomElem(roomId);
        createIncomigElemMessage(JSON.parse(command), true);
    }
});

// Caller:    //room disconnect
connection.on("DisconnectFromCurrentRoom", function () {
    redirectToMainRoom();
});

// Caller    //user rename {oldLogin}||{newLogin}
connection.on("UserRenameClient", function (newUserLogin, command) {
    document.getElementById("user-login").value = newUserLogin;
    createIncomigElemMessage(JSON.parse(command), true);
});

function redirectToMainRoom() {
    document.location.href = "/Chat/1";
}

function isCurrentRoom(roomId) {
    return document.querySelector('#current-room-id').value == roomId;
}

connection.start().then(function () {
    document.querySelector('.msg_send_btn').disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.querySelector('.msg_send_btn').addEventListener("click", function (event) {
    let userLogin = document.querySelector('#user-login').value;

    let message = document.querySelector('.write_msg').value;
    document.querySelector('.write_msg').value = "";

    let roomId = document.querySelector('#current-room-id').value;

    if (document.querySelector('#type-room').value == 'BotRoom') {

        if (message.match(/^\/\/help/)) {
            sendCommandHelp(userLogin, roomId, message);
        } else {
            connection.invoke("ReceivingBotInteractionCommand", message).catch(function (err) {
                return console.error(err.toString());
            });
        }
        return;
    }

    if (message == '') {
        return;
    }

    if (message.match(/^\/\//) != null) {
        if (message.match(/^\/\/user/)) {
            connection.invoke("ReceivingUserInteractionCommand", userLogin, message).catch(function (err) {
                return console.error(err.toString());
            });
        }
        else if (message.match(/^\/\/room/)) {
            connection.invoke("ReceivingRoomInteractionCommand", userLogin, roomId, message).catch(function (err) {
                return console.error(err.toString());
            });
        }
        else {
            sendCommandHelp(userLogin, roomId, message);
        }
    }
    else {
        connection.invoke("ReceivingInteractionMessage", userLogin, roomId, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});

function sendCommandHelp(userLogin, roomId, message) {
    connection.invoke("ReceivingHelpCommand", userLogin, roomId, message).catch(function (err) {
        return console.error(err.toString());
    });
}

// Добавление элемента входящего сообщения
function createIncomigElemMessage(messageInfo, isCommand = false, isInfo = false, countRows = 0) {
    if (isCommand || isInfo) {
        var loginMessageSender = 'Консоль';
        var newMessageId = '0';
    }
    else {
        addInfoForLastMessage(messageInfo);
        var loginMessageSender = messageInfo['userLogin'];
        var newMessageId = messageInfo['messageId'];
    }
    let messageContent = messageInfo['messageContent'];
    let today = new Date(messageInfo['datePublication']);
    let datePublication = today.getDate() + '.' + (today.getMonth() + 1) + '.' + today.getFullYear() + ' ' + today.getHours() + ':' + today.getMinutes() + ':' + today.getSeconds();

    let divIncomingMsg = document.createElement("div");
    divIncomingMsg.className = "incoming_msg";
    let divIncomingMsgImg = document.createElement("div");
    divIncomingMsgImg.className = "incoming_msg_img";
    let img = document.createElement("img");
    img.src = "https://ptetutorials.com/images/user-profile.png";
    let divReceivedMsg = document.createElement("div");
    divReceivedMsg.className = "received_msg";
    let divReceivedWithdMsg = document.createElement('div');
    divReceivedWithdMsg.className = 'received_withd_msg';
    divReceivedWithdMsg.id = newMessageId;
    let p1 = document.createElement("p");
    p1.textContent = loginMessageSender;
    let p2 = document.createElement("p");
    p2.style.cssText = "word-wrap: break-word";
    p2.textContent = messageContent;
    let textarea = document.createElement('textarea');
    textarea.style.cssText = "word - wrap: break-word; resize: none; width: 100%;";
    textarea.className = "h-100";
    textarea.textContent = messageContent;
    textarea.readOnly = true;
    textarea.rows = countRows;
    let span = document.createElement("span");
    span.className = "time_date";
    span.textContent = datePublication;
    let parent = document.querySelector('.msg_history');
    parent.append(divIncomingMsg);
    parent = document.getElementsByClassName('incoming_msg')[document.getElementsByClassName('incoming_msg').length - 1];
    parent.prepend(divIncomingMsgImg);
    parent.append(divReceivedMsg);
    parent = document.getElementsByClassName('received_msg')[document.getElementsByClassName('received_msg').length - 1];
    parent.prepend(divReceivedWithdMsg);
    parent = document.getElementsByClassName('incoming_msg_img')[document.getElementsByClassName('incoming_msg_img').length - 1];
    parent.prepend(img);
    parent = document.getElementsByClassName('received_withd_msg')[document.getElementsByClassName('received_withd_msg').length - 1];
    if (isInfo) {
        parent.prepend(textarea);
    } else {
        parent.prepend(p2);
    }
    parent.prepend(p1);
    parent.append(span);
    parent = document.querySelector('.msg_history').scrollTop = 9999;
}

// Добавление элемента исходящего сообщения
function createOutgoinglemMessage(messageInfo) {
    addInfoForLastMessage(messageInfo);
    let newMessageId = messageInfo['messageId'];
    let messageContent = messageInfo['messageContent'];
    let today = new Date(messageInfo['datePublication']);
    let datePublication = today.getDate() + '.' + (today.getMonth() + 1) + '.' + today.getFullYear() + ' ' + today.getHours() + ':' + today.getMinutes() + ':' + today.getSeconds();

    let divOutgoingMsg = document.createElement("div");
    divOutgoingMsg.className = "outgoing_msg";
    let divSentMsg = document.createElement("div");
    divSentMsg.className = "sent_msg";
    divSentMsg.id = newMessageId;
    divSentMsg.setAttribute('onclick', 'HighlightMessagesForDeletion(this)');
    let p = document.createElement("p");
    p.style.cssText = "word-wrap: break-word";
    p.textContent = messageContent;
    let span = document.createElement("span");
    span.className = "time_date";
    span.textContent = datePublication;
    let parent = document.querySelector('.msg_history');
    parent.append(divOutgoingMsg);
    parent = document.getElementsByClassName('outgoing_msg')[document.getElementsByClassName('outgoing_msg').length - 1];
    parent.prepend(divSentMsg);
    parent = document.getElementsByClassName('sent_msg')[document.getElementsByClassName('sent_msg').length - 1];
    parent.prepend(p);
    parent.append(span);
    parent = document.querySelector('.msg_history').scrollTop = 9999;
}

function removeRoomElem(roomId) {
    var delRoomId = roomId;
    if (document.querySelector('.chat-ref-' + delRoomId) == null) {
        document.querySelector('#room' + delRoomId).remove();
    }
    else
        document.querySelector('.chat-ref-' + delRoomId).remove();
}

function renameTitleRoom(nameRoom) {
    document.querySelector('.text-center').textContent = nameRoom;
}

function renameElemRoom(roomId, nameRoom) {
    document.querySelector('.h5-room-name-' + roomId).textContent = nameRoom;
}



// Добавление элемента комнаты в списке комнат
function createRoomElem(roomInfo) {
    let divChatList = document.createElement("div");
    divChatList.className = "chat_list";
    divChatList.id = "room" + roomInfo['roomId'];
    divChatList.onclick = function () {
        document.location.href = "/Chat/" + roomInfo['roomId'];
    };
    let divChatPeople = document.createElement("div");
    divChatPeople.className = "chat_people";
    let divChatImg = document.createElement("div");
    divChatImg.className = "chat_img";
    let img = document.createElement("img");
    img.src = "https://ptetutorials.com/images/user-profile.png";
    let divChatIb = document.createElement("div");
    divChatIb.className = "chat_ib";
    let h5 = document.createElement("h5");
    h5.textContent = roomInfo['roomName'];
    h5.className = "new_h5";
    let span = document.createElement("span");
    span.className = "chat_date last-chat-date-" + roomInfo['roomId'];
    let p = document.createElement("p");
    p.className = "last-message-room-" + roomInfo['roomId'];

    let parent = document.getElementsByClassName('inbox_chat')[document.getElementsByClassName('inbox_chat').length - 1];
    parent.append(divChatList);
    parent = document.getElementsByClassName('chat_list')[document.getElementsByClassName('chat_list').length - 1];
    parent.prepend(divChatPeople);
    parent = document.getElementsByClassName('chat_people')[document.getElementsByClassName('chat_people').length - 1];
    parent.prepend(divChatIb);
    parent.prepend(divChatImg);
    parent = document.getElementsByClassName('chat_img')[document.getElementsByClassName('chat_img').length - 1];
    parent.prepend(img);
    parent = document.getElementsByClassName('chat_ib')[document.getElementsByClassName('chat_ib').length - 1];
    parent.prepend(h5);
    parent.append(p);
    parent = document.getElementsByClassName('new_h5')[document.getElementsByClassName('new_h5').length - 1];
    parent.prepend(span);
}

// Обновление информации о последнем сообщении в списке комнат: содержимое и дата
function addInfoForLastMessage(messageInfo) {
    let today = new Date(messageInfo['datePublication']);
    let datePublication = today.getDate() + '.' + (today.getMonth() + 1) + '.' + today.getFullYear();
    document.querySelector('.last-message-room-' + messageInfo['roomId']).textContent = messageInfo['messageContent'];
    document.querySelector('.last-chat-date-' + messageInfo['roomId']).textContent = datePublication;
}
