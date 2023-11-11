document.getElementById('notificationBell').addEventListener('click', function (event) {
    event.preventDefault();
    var dropdown = document.getElementById('notificationDropdown');
    dropdown.style.display = dropdown.style.display === 'none' ? 'block' : 'none';
});
