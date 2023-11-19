
function showLoader() {
    document.querySelector(".spinner-overlay").style.display = "block";
}
function hideLoader() {
    document.querySelector(".spinner-overlay").style.display = "none";
}
hideLoader();


var chat = document.getElementById('chat');
var chatBox = document.getElementById('chatBox');
var minimizeButton = document.getElementById('minimize-chat');
var chatBubble = document.getElementById('chat-bubble');

// Sự kiện thu nhỏ cửa sổ chat
minimizeButton.addEventListener('click', function () {
    chatBox.style.display ='none'
    chatBubble.style.display = 'block';
});

// Sự kiện mở rộng cửa sổ chat từ bong bóng
chatBubble.addEventListener('click', function () {
    chatBox.style.display = 'block'; // hoặc 'block' tùy thuộc vào cách bạn đã định dạng nó
    chatBubble.style.display = 'none';
    chat.scrollTop = chat.scrollHeight - chat.clientHeight; // Cuộn đến tin nhắn cuối cùng
});
