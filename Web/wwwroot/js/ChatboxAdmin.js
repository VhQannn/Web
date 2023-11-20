
const connectionChat = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

async function initializeSignalRConnection() {
    try {
        await connectionChat.start();
        setupEventListeners();
        loadConversations();
    } catch (err) {
        console.error('Error during SignalR Connection: ', err);
    }
}

function setupEventListeners() {
    connectionChat.on("ReceiveMessage", function (message, conversationId) {
        if (currentConversationId === conversationId && chatBox.style.display === 'block') {
            addMessageToUI(message);
            if (message.senderRole === 'Customer') {
                markMessagesAsRead([message.messageId]);
            }
        } else {
            playNewMessageSound();
        }
    });

    connectionChat.on("NewMessageNotification", function (conversationId) {
        if (currentConversationId !== conversationId || chatBox.style.display === 'none') {
            playNewMessageSound();
            loadConversations();
        }

    });

    connectionChat.on("MessageRead", function (messageId) {
        const messageElement = document.querySelector(`div[data-message-id="${messageId}"]`);
        if (messageElement) {
            const readStatusElement = messageElement.querySelector('.read-status');
            if (readStatusElement) {
                readStatusElement.innerText = 'Đã xem';
            }
        }
    });
}

function playNewMessageSound() {
    var sound = document.getElementById("messageSound");
    sound.play();
}


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

    openLatestConversation();
});

var conversationsList = [];

async function loadConversations() {
    try {
        const response = await fetch('/api/chat/system-conversations');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        let conversations = await response.json();
        conversations = conversations.sort((a, b) => {
            if (b.unreadMessagesCount !== a.unreadMessagesCount) {
                return b.unreadMessagesCount - a.unreadMessagesCount; // Ưu tiên conversation có nhiều tin nhắn chưa đọc hơn
            }
            return new Date(b.UpdatedTime) - new Date(a.UpdatedTime); // Nếu số tin nhắn chưa đọc bằng nhau, sắp xếp theo thời gian cập nhật
        });
        displayConversations(conversations);
        conversationsList = conversations;
    } catch (error) {
        console.error('Fetch error:', error);
    }
}

function openLatestConversation() {
    if (conversationsList && conversationsList.length > 0) {
        const latestConversation = conversationsList.sort((a, b) => new Date(b.UpdatedTime) - new Date(a.UpdatedTime))[0];
        openConversation(latestConversation);
    }
}

function displayConversations(conversations) {
    const contactsContainer = document.getElementById('contacts');
    contactsContainer.innerHTML = ''; // Xóa danh sách cũ
    conversations.forEach(conversation => {
        const contactDiv = document.createElement('div');
        contactDiv.className = 'contact';
        contactDiv.innerHTML = `
            <div class="pic" style="background-image: url('https://cdn3.iconfinder.com/data/icons/login-5/512/LOGIN_6-512.png');"></div>
            <div class="name">${conversation.conversationName}</div>
            ${conversation.unreadMessagesCount !== 0 ? `<div class="badge">${conversation.unreadMessagesCount}</div>` : ''}
            <div class="message">${conversation.lastMessage || 'Không có tin nhắn mới'}</div>
        `;
        // Thêm sự kiện click để mở cuộc trò chuyện
        contactDiv.addEventListener('click', () => openConversation(conversation));
        contactsContainer.appendChild(contactDiv);
    });
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

function clearMessages() {
    const messagesContainer = document.getElementById("chat");
    messagesContainer.innerHTML = ''; // Xóa nội dung hiện tại của container
}

var currentConversationId = 0;

async function openConversation(conversation) {
    if (currentConversationId) {
        await connectionChat.invoke("LeaveGroup", `Conversation-${currentConversationId}`);
    }

    const messages = await getMessagesForConversation(conversation.conversationId);
    clearMessages();
    messages.forEach(addMessageToUI);

    // Lấy tất cả ID tin nhắn từ DOM và lọc ra những tin nhắn chưa đọc
    const unreadMessageIds = messages
        .filter(msg => !msg.isRead && msg.senderRole === 'Customer')
        .map(msg => msg.messageId);

    if (unreadMessageIds.length > 0) {
        // Chỉ gọi API nếu có tin nhắn chưa đọc
        markMessagesAsRead(unreadMessageIds);
    }

    var conversationName = document.getElementById('name-conversation');

    conversationName.innerHTML = conversation.conversationName;

    currentConversationId = conversation.conversationId;

    await connectionChat.invoke("JoinGroup", `Conversation-${conversation.conversationId}`);

}

function leaveConversation(conversationId) {
    connectionChat.invoke("LeaveGroup", `Conversation-${conversationId}`);
}


const inputField = document.querySelector('.input input');
const sendButton = document.querySelector('.input span');

inputField.addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && !e.repeat) {
        sendMessage(inputField.value, currentConversationId);
        inputField.value = '';
    }
});

sendButton.addEventListener('click', function () {
    sendMessage(inputField.value, currentConversationId);
    inputField.value = '';
});




function sendMessage(messageText, conversationId) {
    if (!messageText.trim()) {
        return; // Đừng gửi tin nhắn nếu không có nội dung
    }

    fetch('/api/chat/send-customer-message', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            ConversationId: conversationId,
            MessageText: messageText,
            MessageType: 'Text'
        })
    }).catch(error => console.error('Error:', error));
}


function addMessageToUI(message) {
    const messagesContainer = document.getElementById("chat");
    const messageDiv = document.createElement("div");

    // Set data-message-id cho messageDiv
    messageDiv.dataset.messageId = message.messageId;

    // Kiểm tra người gửi tin nhắn để quyết định class của thẻ div
    if (message.senderRole === 'Customer') {
        messageDiv.className = 'message stark';
    } else if (message.senderRole === 'Admin') {
        messageDiv.className = 'message parker';
    }

    // Nội dung tin nhắn
    const messageContent = document.createElement("div");
    messageContent.className = 'content';
    messageContent.innerText = message.messageText;
    messageDiv.appendChild(messageContent);

    // Container cho thời gian gửi và trạng thái đã xem
    const statusTimeContainer = document.createElement("div");
    statusTimeContainer.className = 'status-time-container'

    // Thời gian gửi tin nhắn
    const messageTime = document.createElement("div");
    messageTime.className = 'time';
    const sentTime = new Date(message.sentTime);
    messageTime.innerText = sentTime.toLocaleTimeString(); // Định dạng thời gian
    statusTimeContainer.appendChild(messageTime);

    // Trạng thái đã xem (nếu có)
    if (message.senderRole === 'Admin' && message.isRead) {
        const readStatus = document.createElement("div");
        readStatus.className = 'read-status';
        readStatus.innerText = 'Đã xem';
        statusTimeContainer.appendChild(readStatus);
    } else if (message.senderRole === 'Admin' && !message.isRead) {
        const readStatus = document.createElement("div");
        readStatus.className = 'read-status';
        readStatus.innerText = 'Đã gửi';
        statusTimeContainer.appendChild(readStatus);
    }

    messageDiv.appendChild(statusTimeContainer);
    messagesContainer.appendChild(messageDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
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
        loadConversations();
    } catch (error) {
        console.error('Error:', error);
    }
}


document.addEventListener("DOMContentLoaded", function () {
    initializeSignalRConnection();
});

