document.addEventListener('DOMContentLoaded', function () {
    loadCurrentBalance();

    document.getElementById('withdraw-btn').addEventListener('click', function () {
        const amount = parseFloat(document.getElementById('withdraw-amount').value);
        if (isNaN(amount) || amount < 100000) {
            showToast("Notice", "Vui lòng nhập số tiền hợp lệ và tối thiểu 100.000 đ", "info");
            return;
        }

        // Kiểm tra số tiền không vượt quá số dư
        fetch(`/api/account/balance`)
            .then(response => response.json())
            .then(data => {
                if (amount > data.balance) {
                    showToast("Không thể thực hiện yêu cầu", "Số dư hiện tại không đủ với số tiền bạn cần rút", "error");
                } else {
                    // Hiển thị modal để nhập comment
                    $('#modelRequest').modal('show');
                    $('#modelRequest').data('withdraw-amount', amount); // Lưu số tiền vào data của modal
                }
            });
    });
});

$('#btn-request').click(function () {
    const amount = $('#modelRequest').data('withdraw-amount'); // Lấy số tiền từ data của modal
    const comment = $('.withdraw-comment-input').val().trim();

    if (comment.length === 0) {
        showToast("Error", "Vui lòng nhập nội dung đơn yêu cầu rút tiền", "error");
        return;
    }

    requestWithdrawal(amount, comment);
    $('#modelRequest').modal('hide');
});

function requestWithdrawal(amount, comment) {
    const requestData = {
        Amount: amount,
        Comments: comment
    };

    // API call to create withdrawal request
    fetch('/api/withdrawal-requests/create', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(requestData),
    })
        .then(response => response.json())
        .then(data => {
            showToast("Thành công!", "Đã yêu cầu rút tiền thành công", "success");
            loadCurrentBalance();
        })
        .catch(error => {
            showToast("Đã xảy ra lỗi!", "Lỗi khi thực hiện yêu cầu: " + error.responseText, "success");
            loadCurrentBalance();
        });
}

function loadCurrentBalance() {
    fetch(`/api/account/balance?userId=${window.userId}`)
        .then(response => response.json())
        .then(data => {
            document.getElementById('current-balance').innerText = `${data.balance} đ`;
        })
        .catch(error => console.error('Error:', error));
}

$('.modal').on('shown.bs.modal', function () {
    $('.modal-backdrop').hide(); // Ẩn modal backdrop khi một modal được hiển thị
});