
const connectionChat = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();


connectionChat.start().catch(err => console.error(err.toString()));

var chat = document.getElementById('chat');
var chatBox = document.getElementById('chatBox');
var minimizeButton = document.getElementById('minimize-chat');
var chatBubble = document.getElementById('chat-bubble');

chatBox.style.display = 'none'
chatBubble.style.display = 'block';

// Sự kiện thu nhỏ cửa sổ chat
minimizeButton.addEventListener('click', function () {
    chatBox.style.display = 'none'
    chatBubble.style.display = 'block';
});

// Sự kiện mở rộng cửa sổ chat từ bong bóng
chatBubble.addEventListener('click', function () {
    chatBox.style.display = 'block'; // hoặc 'block' tùy thuộc vào cách bạn đã định dạng nó
    chatBubble.style.display = 'none';
    chat.scrollTop = chat.scrollHeight - chat.clientHeight; // Cuộn đến tin nhắn cuối cùng
});


async function getOrCreateConversationForCustomer() {
    try {
        const response = await fetch('/api/chat/get-or-create-conversation');
        const data = await response.json();
        if (response.ok) {
            return data;
        } else {
            throw new Error(data.message || "Không thể lấy hoặc tạo cuộc trò chuyện");
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

async function getMessagesForConversation(conversationId) {
    try {
        const response = await fetch(`/api/chat/conversation-messages/${conversationId}`);
        const data = await response.json();
        if (response.ok) {
            return data;
        } else {
            throw new Error(data.message || "Không thể lấy tin nhắn");
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

var currentConversationId = 0;

function clearMessages() {
    const messagesContainer = document.getElementById("chat");
    messagesContainer.innerHTML = ''; // Xóa nội dung hiện tại của container
}

async function loadChat() {
    clearMessages();
    const conversation = await getOrCreateConversationForCustomer();
    if (conversation) {
        currentConversationId = conversation;
        await connectionChat.invoke("JoinGroup", `Conversation-${currentConversationId}`);
        
        const messages = await getMessagesForConversation(conversation);
        messages.forEach(addMessageToUI);
        // Lấy tất cả ID tin nhắn từ DOM và lọc ra những tin nhắn chưa đọc
        const unreadMessageIds = messages
            .filter(msg => !msg.isRead && msg.senderRole === 'Admin')
            .map(msg => msg.messageId);

        if (unreadMessageIds.length > 0) {
            // Chỉ gọi API nếu có tin nhắn chưa đọc
            markMessagesAsRead(unreadMessageIds);
        }
    }
}

document.addEventListener("DOMContentLoaded", loadChat());
function addMessageToUI(message) {
    const messagesContainer = document.getElementById("chat");
    const messageDiv = document.createElement("div");

     // Set data-message-id cho messageDiv
    messageDiv.dataset.messageId = message.messageId;


    // Kiểm tra người gửi tin nhắn để quyết định class của thẻ div
    if (message.senderRole === 'Admin') {
        messageDiv.className = 'message stark';
    } else if (message.senderRole === 'Customer') {
        messageDiv.className = 'message parker';
    }

    // Nội dung tin nhắn
    const messageContent = document.createElement("div");
    messageContent.className = 'content';
    messageContent.innerText = message.messageText;
    messageDiv.appendChild(messageContent);

    // Container cho thời gian gửi và trạng thái đã xem
    const statusTimeContainer = document.createElement("div");
    statusTimeContainer.className = 'status-time-container';

    // Thời gian gửi tin nhắn
    const messageTime = document.createElement("div");
    messageTime.className = 'time';
    const sentTime = new Date(message.sentTime);
    messageTime.innerText = sentTime.toLocaleTimeString(); // Định dạng thời gian
    statusTimeContainer.appendChild(messageTime);

    // Trạng thái đã xem (nếu có)
    if (message.senderRole === 'Customer' && message.isRead) {
        const readStatus = document.createElement("div");
        readStatus.className = 'read-status';
        readStatus.innerText = 'Đã xem';
        statusTimeContainer.appendChild(readStatus);
    } else if (message.senderRole === 'Customer' && !message.isRead) {
        const readStatus = document.createElement("div");
        readStatus.className = 'read-status';
        readStatus.innerText = 'Đã gửi';
        statusTimeContainer.appendChild(readStatus);
    }

    messageDiv.appendChild(statusTimeContainer);
    messagesContainer.appendChild(messageDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}




const inputField = document.querySelector('.input input');
const sendButton = document.querySelector('.input span');

inputField.addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        sendMessage(inputField.value);
        inputField.value = '';
    }
});

sendButton.addEventListener('click', function () {
    sendMessage(inputField.value);
    inputField.value = '';
});


// Xử lý tin nhắn nhận được
connectionChat.on("ReceiveMessage", function (message, _conversationId) {
    if (currentConversationId === _conversationId) {
        if (chatBox.style.display === 'none') {
            console.log("hello");
            playNewMessageSound();
        } else {
            addMessageToUI(message);
            if (message.senderRole === 'Admin') {
                markMessagesAsRead([message.messageId]);
            }
        }
        
    }
});


connectionChat.on("MessageRead", function (messageId) {
    const messageElement = document.querySelector(`div[data-message-id="${messageId}"]`);
    if (messageElement) {
        const readStatusElement = messageElement.querySelector('.read-status');
        if (readStatusElement) {
            // Cập nhật nội dung của phần tử này thành "Đã xem"
            readStatusElement.innerText = 'Đã xem';
        }
    }
});


function playNewMessageSound() {
    var sound = document.getElementById("messageSound");
    sound.play();
}



// Cập nhật hàm sendMessage để gửi tin nhắn qua SignalR
function sendMessage(messageText) {
    fetch('/api/chat/send-customer-message', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            ConversationId: currentConversationId,
            MessageText: messageText,
            MessageType: 'Text'
        })
    })
        .then()
        .catch(error => console.error('Error:', error));
}

async function markMessagesAsRead(messageIds) {
    if (messageIds.length === 0) {
        return; // Không có tin nhắn nào để đánh dấu
    }

    try {
        const response = await fetch('/api/chat/mark-messages-as-read', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ MessageIds: messageIds })
        });
        if (!response.ok) {
            throw new Error('Error marking messages as read');
        }
    } catch (error) {
        console.error('Error:', error);
    }
}


