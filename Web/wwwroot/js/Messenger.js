(function ($) {
    "use strict";
    $('.scrollable-chat-panel').perfectScrollbar();
    var position = $(".chat-search").last().position().top;
    $('.scrollable-chat-panel').scrollTop(position);
    $('.scrollable-chat-panel').perfectScrollbar('update');
    $('.pagination-scrool').perfectScrollbar();

    $('.chat-upload-trigger').on('click', function (e) {
        $(this).parent().find('.chat-upload').toggleClass("active");
    });
    $('.user-detail-trigger').on('click', function (e) {
        $(this).closest('.chat').find('.chat-user-detail').toggleClass("active");
    });
    $('.user-undetail-trigger').on('click', function (e) {
        $(this).closest('.chat').find('.chat-user-detail').toggleClass("active");
    });
})(jQuery);

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
        if (currentConversationId === conversationId) {
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
            
            loadConversations();
            playNewMessageSound();
        }

    });

    connectionChat.on("MessageRead", function (messageId) {
        const messageElement = document.querySelector(`div[data-message-id="${messageId}"]`);
        if (messageElement) {
            const readStatusElement = messageElement.querySelector('.time-row');
            if (readStatusElement) {
                const checkDiv = document.createElement("div");
                checkDiv.className = 'svg15 double-check'; // Thêm class cho icon trạng thái

                readStatusElement.appendChild(checkDiv);
            }
        }
    });
}

function playNewMessageSound() {
    var sound = document.getElementById("messageSound");
    sound.play();
}

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

function formatDate(dateString) {
    const date = new Date(dateString);
    const now = new Date();

    const yesterday = new Date(now);
    yesterday.setDate(yesterday.getDate() - 1);

    if (date.toDateString() === now.toDateString()) {
        // Cùng ngày: chỉ hiển thị thời gian
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    } else if (date.toDateString() === yesterday.toDateString()) {
        // Hôm qua
        return 'Yesterday';
    } else if (date.getFullYear() === now.getFullYear()) {
        // Cùng năm: hiển thị ngày và tháng
        return date.toLocaleDateString('en-GB', { day: 'numeric', month: 'short' });
    } else {
        // Khác năm: hiển thị ngày, tháng và năm
        return date.toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' });
    }
}


function displayConversations(conversations) {
    const contactsContainer = document.getElementById('contacts');
    contactsContainer.innerHTML = ''; // Xóa danh sách cũ
    conversations.forEach(conversation => {
        const contactDiv = document.createElement('div');

        const formattedDate = formatDate(conversation.updatedTime);

        contactDiv.className = 'chat-item d-flex pl-3 pr-0 pt-3 pb-3';
        contactDiv.innerHTML = `
										    <div class="w-100">
											    <div class="d-flex pl-0">
												    <img class="rounded-circle avatar-sm mr-3" src="https://user-images.githubusercontent.com/35243461/168796906-ab4fc0f3-551c-4036-b455-be2dfedb9680.svg">
												    <div>
													    <p class="margin-auto fw-400 text-dark-75 name">${conversation.conversationName}</p>
                                                        ${conversation.unreadMessagesCount !== 0 ? `<div class="badge"></div>` : ''}
													    <div class="d-flex flex-row mt-1">
														    <span class="message-shortcut margin-auto text-muted fs-13 ml-1 mr-4">${conversation.lastMessage || 'Không có tin nhắn mới'}</span>
													    </div>
												    </div>
											    </div>
										    </div>
                                            <div class="flex-shrink-0 margin-auto pl-2 pr-3">
                                                  <div class="d-flex flex-column">
                                                       <p class="text-muted text-right fs-13 mb-2">${formattedDate}</p>

                                                        ${conversation.unreadMessagesCount !== 0 ? `<span class="round badge badge-light-success margin-auto">${conversation.unreadMessagesCount}</span>` : ''}
                                                  </div>
                                            </div>`;
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

    var conversationTime = document.getElementById('time-conversation');

    conversationTime.innerHTML = `<i class="la la-clock mr-1"></i>${formatDate(conversation.updatedTime) }`;

    currentConversationId = conversation.conversationId;

    await connectionChat.invoke("JoinGroup", `Conversation-${conversation.conversationId}`);

}

function scrollConversation() {
    var chat = document.getElementById('chat-container');
    chat.scrollTop = chat.scrollHeight - chat.clientHeight;
}

function leaveConversation(conversationId) {
    connectionChat.invoke("LeaveGroup", `Conversation-${conversationId}`);
}


const inputField = document.getElementById('send-message');
const sendButton = document.getElementById('send-message-button');

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

    // Tạo thẻ div chính cho tin nhắn
    const messageDiv = document.createElement("div");
    messageDiv.dataset.messageId = message.messageId;
    
    if (message.senderRole === 'Customer') {
        messageDiv.className = 'left-chat-message fs-13 mb-2';

        // Tạo thẻ p cho nội dung tin nhắn
        const messageP = document.createElement("p");
        messageP.className = 'mb-0 mr-3 pr-4';
        messageP.innerText = message.messageText;
        messageDiv.appendChild(messageP);

        // Tạo container cho thông tin thời gian và nút mở rộng
        const messageOptionsDiv = document.createElement("div");
        messageOptionsDiv.className = 'message-options';

        // Tạo thẻ div cho thời gian tin nhắn
        const messageTimeDiv = document.createElement("div");
        messageTimeDiv.className = 'message-time';
        const sentTime = new Date(message.sentTime);
        messageTimeDiv.innerText = sentTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        messageOptionsDiv.appendChild(messageTimeDiv);


        messageDiv.appendChild(messageOptionsDiv);
    } else {
        // Đối với tin nhắn từ 'Admin'
        messageDiv.className = 'd-flex flex-row-reverse mb-2';

        const rightMessageDiv = document.createElement("div");
        rightMessageDiv.className = 'right-chat-message fs-13 mb-2';

        // Tạo thẻ div bọc ngoài cho nội dung tin nhắn
        const messageWrapperDiv = document.createElement("div");
        messageWrapperDiv.className = 'mb-0 mr-3 pr-4';

        // Tạo thẻ div cho nội dung tin nhắn
        const messageContentDiv = document.createElement("div");
        messageContentDiv.className = 'd-flex flex-row';

        const messageTextDiv = document.createElement("div");
        messageTextDiv.className = 'pr-2';
        messageTextDiv.innerText = message.messageText;


        const spaceDiv = document.createElement("div");
        spaceDiv.className = "pr-4";
        
        messageContentDiv.appendChild(messageTextDiv);
        messageContentDiv.appendChild(spaceDiv);

        messageWrapperDiv.appendChild(messageContentDiv);
        rightMessageDiv.appendChild(messageWrapperDiv);

        // Tạo container cho thông tin thời gian và nút mở rộng
        const messageOptionsDiv = document.createElement("div");
        messageOptionsDiv.className = 'message-options dark';

        // Tạo thẻ div cho thời gian tin nhắn
        const messageTimeDiv = document.createElement("div");
        messageTimeDiv.className = 'message-time';
        const timeRowDiv = document.createElement("div");
        timeRowDiv.className = 'd-flex flex-row time-row';

        const timeTextDiv = document.createElement("div");
        timeTextDiv.className = 'mr-2';
        const sentTime = new Date(message.sentTime);
        timeTextDiv.innerText = sentTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

        timeRowDiv.appendChild(timeTextDiv);
        

        if (message.isRead) {
            const checkDiv = document.createElement("div");
            checkDiv.className = 'svg15 double-check'; // Thêm class cho icon trạng thái

            timeRowDiv.appendChild(checkDiv);
           
        }

        
        messageTimeDiv.appendChild(timeRowDiv);
        messageOptionsDiv.appendChild(messageTimeDiv);

        rightMessageDiv.appendChild(messageOptionsDiv);
        messageDiv.appendChild(rightMessageDiv);
    }

    // Thêm tin nhắn vào container tin nhắn và cuộn đến cuối
    messagesContainer.appendChild(messageDiv);
    scrollConversation();
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



document.getElementById('search-conversation').addEventListener('input', async function (e) {
    var searchTerm = e.target.value.trim();

    // Gọi API tìm kiếm
    try {
        const response = await fetch(`/api/chat/search-conversations?searchTerm=${searchTerm}`);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const conversations = await response.json();
        displayConversations(conversations); // Cập nhật giao diện với kết quả tìm kiếm
    } catch (error) {
        console.error('Error:', error);
    }
});


