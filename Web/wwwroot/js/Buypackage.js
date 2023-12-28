const $confirmBox = $("#confirm-box");
const $overlay = $("#overlay");
const $yesButton = $("#yes-button");
const $noButton = $("#no-button");

const $closeBtn = $('#popup-close');
const $backdrop = $('#popup-backdrop');

$overlay.click(function () {
    $overlay.hide();
    $confirmBox.hide();
});

$noButton.click(function () {
    $overlay.hide();
    $confirmBox.hide();
});

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.action').forEach(function (button) {
        button.addEventListener('click', function () {
            var packageId = this.getAttribute('data-package-id');
            $yesButton.data('package-id', packageId);
            // Hiển thị confirm box
            $confirmBox.show();
            $overlay.show();
        });
    });
});

function showToast(title, message, type) {
    toast({
        title: title,
        message: message,
        type: type,
        duration: 3000
    });
}

function processPackageSelection(packageId) {
    fetch('/api/buy-package', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: 'packageId=' + packageId
    })
        .then(response => {
            if (response.status === 401) { // Unauthorized
                showToast("Chưa Đăng Nhập", "Vui lòng đăng nhập để thực hiện thao tác này.", "error");
                return;
            } else if (response.status === 404){
                showToast("Error", "Có vấn đề về thông tin gói bảo hiểm, thử lại sau...", "error");
                return;
            }
            if (!response.ok) {
                throw new Error('Lỗi server: ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            if (data && data.success) {
                showToast("Thành công", "Giao dịch đã được tạo", "success");
            }
        })
        .catch(error => {
            showToast("Lỗi", error.toString(), "error");
        });
}





$yesButton.click(function () {
    var packageId = $(this).data('package-id');
    processPackageSelection(packageId);
    $overlay.hide();
    $confirmBox.hide();
});