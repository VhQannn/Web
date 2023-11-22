let originalTitle = document.title;
let newMessageTitle = " đã gửi tin nhắn cho bạn!";
let isTabActive = true;

var currentConversationId = 0;
var currentConversation = null;
let tempImageFile = null;
let newMessagesCount = 0;
let typingTimeoutId = null;

var chat = document.getElementById('chat');
var chatBox = document.getElementById('chatBox');
var minimizeButton = document.getElementById('minimize-chat');
var maximizeButton = document.getElementById('maximize-chat');
var chatBubble = document.getElementById('chat-bubble');
var modal = document.getElementById("imageModal");
var span = document.getElementsByClassName("close")[0];
const inputField = document.getElementById('text-message');
const sendButton = document.getElementById('btn-send');
document.getElementById('image-input').addEventListener('change', handleFileSelect);

chatBox.style.display = 'none'
chatBubble.style.display = 'block';

var conversationsList = [];

document.addEventListener("visibilitychange", function () {
    if (document.visibilityState === 'visible') {
        isTabActive = true;
        if (chatBox.style.display === 'block') {
            openConversation(currentConversation);
        }
    } else {
        isTabActive = false;
    }
});

const connectionChat = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

initializeSignalRConnection = async () => {
    setupEventListeners();
    await startSignalRConnection();
    loadConversations();
}

async function startSignalRConnection() {
    try {
        await connectionChat.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.error('SignalR Connection failed: ', err);
        setTimeout(startSignalRConnection, 5000); // Thử kết nối lại sau 5 giây nếu không thành công
    }
}


connectionChat.onclose(async () => {
    await startSignalRConnection();
});




function setupEventListeners() {
    connectionChat.on("ReceiveMessage", function (message, conversationId) {
        console.log(isTabActive);
        if (currentConversationId === conversationId && chatBox.style.display === 'block') {
            if (isTabActive) {
                addMessageToUI(message);
                if (message.senderRole === 'Customer') {
                    markMessagesAsRead([message.messageId]);
                }
            } else {
                playNewMessageSound(message);
                loadConversations();
            }
        } else {
            playNewMessageSound(message);
            loadConversations();
        }
    });

    connectionChat.on("NewMessageNotification", function (message, conversationId) {
        if (currentConversationId !== conversationId) {
            playNewMessageSound(message);
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
    connectionChat.on("UserTyping", function () {
        showTypingIndicator();
    });

    connectionChat.on("UserCancelTyping", function () {
        hideTypingIndicator();
    });
}

function showTypingIndicator(conversationId) {
    let typingIndicator = document.querySelector('.typing-indicator');

    // Chỉ tạo mới nếu chưa có
    if (!typingIndicator) {
        typingIndicator = document.createElement("div");
        typingIndicator.className = "message stark typing-indicator";
        typingIndicator.innerHTML = `
            <div class="typing typing-1"></div>
            <div class="typing typing-2"></div>
            <div class="typing typing-3"></div>
        `;
        typingIndicator.dataset.conversationId = conversationId;

        // Thêm vào cuối danh sách tin nhắn
        const messagesContainer = document.getElementById("chat");
        messagesContainer.appendChild(typingIndicator);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    // Reset bộ đếm thời gian
    clearTimeout(typingTimeoutId);
    typingTimeoutId = setTimeout(() => {
        hideTypingIndicator();
    }, 5000);
}


function hideTypingIndicator() {
    // Tìm và xóa phần tử thông báo "đang nhập"
    const typingIndicator = document.querySelector('.typing-indicator');
    if (typingIndicator) {
        typingIndicator.remove();
    }

    // Hủy bộ đếm thời gian
    if (typingTimeoutId) {
        clearTimeout(typingTimeoutId);
    }
}


function playNewMessageSound(message) {
    var sound = document.getElementById("messageSound");
    sound.play();
    document.title = `${message.senderName}${newMessageTitle}`;
}




// Sự kiện thu nhỏ cửa sổ chat
minimizeButton.addEventListener('click', function () {
    chatBox.style.display = 'none'
    chatBubble.style.display = 'block';
    currentConversationId = 0;
});

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



async function openConversation(conversation) {
    if (document.title.includes(conversation.conversationName)) {
        document.title = originalTitle;
    }
    if (currentConversationId) {
        await connectionChat.invoke("LeaveGroup", `Conversation-${currentConversationId}`);
    }

    const messages = await getMessagesForConversation(conversation.conversationId);
    clearMessages();
    messages.forEach(addMessageToUI);

    const unreadMessageIds = messages
        .filter(msg => !msg.isRead && msg.senderRole === 'Customer')
        .map(msg => msg.messageId);

    if (unreadMessageIds.length > 0) {
        markMessagesAsRead(unreadMessageIds);
    }

    var conversationName = document.getElementById('name-conversation');
    conversationName.innerHTML = conversation.conversationName;
    currentConversationId = conversation.conversationId;
    currentConversation = conversation;
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
    hideTypingIndicator();
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

let typingTimer; // Timer để xác định khi người dùng ngưng nhập
const typingInterval = 3000; // Thời gian chờ (ví dụ: 3000ms = 3 giây)

inputField.addEventListener('input', function () {
    clearTimeout(typingTimer);
    if (inputField.value) {
        notifyTyping(currentConversationId);
        typingTimer = setTimeout(() => {
            notifyCancelTyping(currentConversationId);
        }, typingInterval);
    } else {
        // Người dùng đã xóa hết nội dung
        notifyCancelTyping(currentConversationId);
    }
});

// Hàm thông báo khi bắt đầu nhập
async function notifyTyping(conversationId) {
    await connectionChat.invoke("NotifyTyping", `Conversation-${conversationId}`);
}

// Hàm thông báo khi ngưng nhập
async function notifyCancelTyping(conversationId) {
    await connectionChat.invoke("NotifyCancelTyping", `Conversation-${conversationId}`);
}

sendButton.addEventListener('click', function () {
    clearTimeout(typingTimer);
    if (tempImageFile) {
        sendMessage(tempImageFile, 'Image', currentConversationId);
        tempImageFile = null;
        document.getElementById('image-attached-badge').style.display = 'none'; // Ẩn badge
    } else {
        sendMessage(inputField.value, 'Text', currentConversationId);
        inputField.value = '';
    }
});


function handleFileSelect(event) {
    const file = event.target.files[0];
    if (file && file.type.startsWith('image/')) {
        tempImageFile = file; // Lưu trữ file ảnh tạm thời
        document.getElementById('image-attached-badge').style.display = 'inline'; // Hiển thị badge
    }
}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

document.addEventListener("DOMContentLoaded", function () {
    initializeSignalRConnection();
    // Yêu cầu quyền thông báo
    if (Notification.permission !== "granted") {
        Notification.requestPermission().then(permission => {
            if (permission === "granted") {
                new Notification("Bạn sẽ nhận được thông báo từ chúng tôi!");
            }
        });
    }
});
