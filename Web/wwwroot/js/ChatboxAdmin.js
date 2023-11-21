﻿
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
let tempImageFile = null;
let newMessagesCount = 0;
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
            newMessagesCount++;
            updateNewMessageBadge(newMessagesCount);
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
var maximizeButton = document.getElementById('maximize-chat');
var chatBubble = document.getElementById('chat-bubble');

chatBox.style.display = 'none'
chatBubble.style.display = 'block';

// Sự kiện thu nhỏ cửa sổ chat
minimizeButton.addEventListener('click', function () {
    chatBox.style.display = 'none'
    chatBubble.style.display = 'block';
});// Sự kiện thu nhỏ cửa sổ chat
maximizeButton.addEventListener('click', function () {
    window.location.href = "/admin/messenger";
});

// Sự kiện mở rộng cửa sổ chat từ bong bóng
chatBubble.addEventListener('click', function () {
    chatBox.style.display = 'block'; // hoặc 'block' tùy thuộc vào cách bạn đã định dạng nó
    chatBubble.style.display = 'none';
    chat.scrollTop = chat.scrollHeight - chat.clientHeight; // Cuộn đến tin nhắn cuối cùng
    newMessagesCount = 0;
    updateNewMessageBadge(newMessagesCount);
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

        // Tính toán tổng số tin nhắn chưa đọc
        newMessagesCount = conversations.reduce((total, conversation) => total + conversation.unreadMessagesCount, 0);
        updateNewMessageBadge(newMessagesCount); // Cập nhật badge

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



function sendMessage(messageText, messageType, conversationId) {
    

    if (messageType === 'Text') {
        if (!messageText.trim()) {
            return; // Đừng gửi tin nhắn nếu không có nội dung
        }
        const messageData = {
            ConversationId: conversationId,
            MessageText: messageText,
            MessageType: messageType
        };

        fetch('/api/chat/send-customer-message', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(messageData)
        })
            .then()
            .catch(error => console.error('Error:', error));
        document.getElementById('image-attached-badge').style.display = 'none'; // Ẩn badge
    } else {
        if (messageText == null) {
            return; // Đừng gửi tin nhắn nếu không có nội dung
        }
        const formData = new FormData();
        formData.append('file', messageText); // messageContent ở đây là đối tượng File
        formData.append('ConversationId', conversationId);
        formData.append('MessageType', messageType);

        fetch('/api/chat/send-customer-message-file', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
            })
            .catch(error => console.error('Error:', error));
        document.getElementById('image-attached-badge').style.display = 'none'; // Ẩn badge
    }
}


function addMessageToUI(message) {
    const messagesContainer = document.getElementById("chat");
    if (message.messageType == 'Text') {
        const messageDiv = document.createElement("div");

        // Set data-message-id cho messageDiv
        messageDiv.dataset.messageId = message.messageId;


        // Kiểm tra người gửi tin nhắn để quyết định class của thẻ div
        if (message.senderRole === 'Admin') {
            messageDiv.className = 'message parker';
        } else if (message.senderRole === 'Customer') {
            messageDiv.className = 'message stark';
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
    } else if (message.messageType == 'Image') {
        const messageDiv = document.createElement("div");

        // Set data-message-id cho messageDiv
        messageDiv.dataset.messageId = message.messageId;


        // Kiểm tra người gửi tin nhắn để quyết định class của thẻ div
        if (message.senderRole === 'Admin') {
            messageDiv.className = 'message parker';
        } else if (message.senderRole === 'Customer') {
            messageDiv.className = 'message stark';
        }

        const img = document.createElement('img');
        img.src = message.messageText;
        img.alt = "Image message";
        img.onclick = function () {
            document.getElementById('modalImage').src = this.src;
            document.getElementById('imageModal').style.display = 'block';
        };
        messageDiv.appendChild(img);

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
    }

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

function updateNewMessageBadge(count) {
    const badge = document.getElementById('new-message-badge');
    if (count > 0) {
        badge.style.display = 'inline';
        badge.innerText = count;
    } else {
        badge.style.display = 'none';
    }
}


document.addEventListener("DOMContentLoaded", function () {
    initializeSignalRConnection();
});



const inputField = document.getElementById('text-message');
const sendButton = document.getElementById('btn-send');

inputField.addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && !e.repeat) {
        if (tempImageFile) {
            sendMessage(tempImageFile, 'Image', currentConversationId);
            tempImageFile = null;
            document.getElementById('image-attached-badge').style.display = 'none'; // Ẩn badge
        } else {
            sendMessage(inputField.value, 'Text', currentConversationId);
            inputField.value = '';
        }
    }
});

inputField.addEventListener('paste', function (e) {
    const items = (event.clipboardData || event.originalEvent.clipboardData).items;
    for (const item of items) {
        if (item.kind === 'file' && item.type.startsWith('image/')) {
            const file = item.getAsFile();
            tempImageFile = file; // Lưu trữ file ảnh tạm thời
            document.getElementById('image-attached-badge').style.display = 'inline'; // Hiển thị badge
        }
    }
});
sendButton.addEventListener('click', function () {
    if (tempImageFile) {
        sendMessage(tempImageFile, 'Image', currentConversationId);
        tempImageFile = null;
        document.getElementById('image-attached-badge').style.display = 'none'; // Ẩn badge
    } else {
        sendMessage(inputField.value, 'Text', currentConversationId);
        inputField.value = '';
    }
});



// JavaScript: Xử lý sự kiện chọn file và dán ảnh từ clipboard
document.getElementById('image-input').addEventListener('change', handleFileSelect);


function handleFileSelect(event) {
    const file = event.target.files[0];
    if (file && file.type.startsWith('image/')) {
        tempImageFile = file; // Lưu trữ file ảnh tạm thời
        document.getElementById('image-attached-badge').style.display = 'inline'; // Hiển thị badge
    }
}

// Get the modal
var modal = document.getElementById("imageModal");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}
