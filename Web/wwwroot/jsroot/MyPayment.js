
let currentPage = 1;
const pageSize = 5;

$(document).on('click', 'a[data-target="#modelRequest"]', function () {
    var paymentId = $(this).data('payment-id');
    $('#modelRequest').data('payment-id', paymentId); // Store paymentId in modal's data
    $('#modelRequest').modal('show');
});

const errorBox = document.getElementById('regError');

function showToast(title, message, type) {
    toast({
        title: title,
        message: message,
        type: type,
        duration: 3000
    });
}
function loadListPayment(pageNumber) {
    fetch(`/api/account/my-payment?pageNumber=${pageNumber}&pageSize=${pageSize}`)
        .then(response => {
            if (!response.ok) {
                showToast("Error", "Network response was not ok", "error");
            }
            return response.json();
        })
        .then(response => {
            const { data, totalRecords, totalPages } = response; // Log to see what data is returned

            var tableBody = $('#payment-table-container .table tbody');
            tableBody.empty(); 
            data.forEach(item => {
                var paymentDate = new Date(item.paymentDate).toLocaleDateString('vi-VN');
                var amountFormatted = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(item.amount);
                var actionButton = '';
                let labelForRole;
                // Check if logged in user is a supporter
                if (item.user.role === "Supporter") {
                    $('#payment-table-container .table th:nth-child(2)').text('Người Thực Hiện Giao Dịch');
                    labelForRole = "Người Thực Hiện";
                    if (item.status === "COMPLETED") {
                        if (item.withdrawalRequest != null) {
                            actionButton = `<a class="btn btn-primary btn-sm text-white view-withdrawal-request" 
                        data-request-id="${item.withdrawalRequest.withdrawalRequestId}" 
                        data-request-comment="${item.withdrawalRequest.comments}" 
                        data-request-status="${item.withdrawalRequest.status}" 
                        title="View Withdrawal Request">
                        Xem Đơn Rút Tiền
                        </a>`;
                        } else {
                            actionButton = `<a class="btn btn-danger btn-sm text-white" id="btnCreateRequest" title="Withdrawal Request" data-toggle="modal" data-target="#modelRequest" data-payment-id="${item.paymentId}">
                                            Yêu Cầu Rút Tiền
                                            </a>`;
                        }

                    }
                } else {
                    $('#payment-table-container .table th:nth-child(2)').text('Người Nhận Bài');
                    labelForRole = "Người Nhận Bài";
                    console.log(item);
                    // For regular users
                    if (item.status === "PENDING") {
                        actionButton = `<button class="view-qr-button btn btn-danger btn-sm text-white" data-payment-id="${item.paymentId}" data-payment-amount="${item.amount}" data-payment-username="${item.user.username}">View QR Code</button>`;
                    } else if (item.status === "COMPLETED") {
                        if (item.serviceType == "Post") {
                            actionButton = `<button class="view-profile-button btn btn-primary btn-sm text-white" data-receiver-id="${item.receiver.id}">Contact With Supporter</button>`;
                        } else if (item.serviceType == "Check-Score") {
                            actionButton = `<span class="text-primary">Giao dịch hoàn tất, theo dõi kết quả ở trang kết quả</span>`;;
                        }
                        
                    }
                }

                if (item.serviceType == "Post") {
                    tableBody.append(`<tr>
                    <td data-label="Ngày Giao Dịch">${paymentDate}</td>
                    <td data-label="${labelForRole}">${item.receiver.username}</td>
                    <td data-label="Số Tiền">${amountFormatted}</td>
                    <td data-label="Loại Dịch Vụ"><a href="./PostDetails?id=${item.relatedId}" class="post-details-link">${item.serviceType}</a></td>
                    <td data-label="Trạng Thái Giao Dịch">${item.status}</td>
                    <td data-label="Hành Động">${actionButton}</td>
                </tr>`);
                } else if (item.serviceType == "Check-Score") {
                    tableBody.append(`<tr>
                    <td data-label="Ngày Giao Dịch">${paymentDate}</td>
                    <td data-label="Số Tiền">${amountFormatted}</td>
                    <td data-label="Loại Dịch Vụ"><a href="./CheckResult?id=${item.relatedId}" class="post-details-link">${item.serviceType}</a></td>
                    <td data-label="Trạng Thái Giao Dịch">${item.status}</td>
                    <td data-label="Hành Động">${actionButton}</td>
                </tr>`);
                    $('#payment-table-container .table th:nth-child(2)').hide();

                }


            });
            attachButtonClickEvents();
            updatePaginationButtons(currentPage, totalPages);
        })
        .catch(error => {
            showToast("Error", "Unable to load payments: " + error, "error");
        });
}

function attachButtonClickEvents() {
    // Đính kèm sự kiện click cho các nút vừa thêm vào

    $('.view-qr-button').click(function () {
        var paymentId = $(this).data('payment-id');
        var amount = $(this).data('payment-amount');
        var username = $(this).data('payment-username');
        
        var linkQRCode = "https://api.vietqr.io/image/970422-0911589806-BHJrmo8.jpg?accountName=MA%20VAN%20TUONG&amount=" + amount;
        linkQRCode += "&addInfo=Payment%20OrderID" + paymentId + "%20" + encodeURIComponent(username);

        $("#vietqr-popup").show();
        $('.popup-backdrop').addClass('show');
        $("#txtQRCode").attr('src', linkQRCode);
    });

    

    $(document).on('click', '.view-withdrawal-request', function () {
        var requestComment = $(this).data('request-comment');
        var requestStatus = $(this).data('request-status');

        // Cập nhật nội dung modal
        $('#viewWithdrawalRequestModal .modal-body').html(`<p>Bình luận: ${requestComment}</p><p>Trạng thái: ${requestStatus}</p>`);

        // Hiển thị modal
        $('#viewWithdrawalRequestModal').modal('show');
    });



    $('.view-profile-button').click(function () {
        var receiverId = $(this).data('receiver-id');
        console.log(receiverId);
        window.location.href = "/users/detail?uId=" + receiverId;
    });

    $('#popup-close').click(function () {
        $("#vietqr-popup").hide();
        $('.popup-backdrop').removeClass('show');
        $('#modelRequest').modal('hide');
    });

    $('.close').click(function () {
        $('#modelRequest').modal('hide');
        $('#viewWithdrawalRequestModal').modal('hide');
    });
}

$('#btn-request').click(function () {
    var paymentId = $('#btnCreateRequest').data('payment-id');
    var comment = $('.withdraw-comment-input').val().trim();

    // Check if the comment is not empty
    if (comment.length === 0) {
        showToast("Error", "Please enter a comment.", "error");
        return; // Stop the function if no comment is entered
    }

    var requestData = {
        PaymentId: paymentId,
        Comments: comment
    };
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
        })
        .catch(error => {
            showToast("Error", "Failed to submit withdrawal request: " + error, "error");
        });

    $('#modelRequest').modal('hide');
    loadListPayment(currentPage);
});

function updatePaginationButtons(currentPage, totalPages) {
    if (currentPage >= totalPages) {
        $("#nextPage").hide();
    } else {
        $("#nextPage").show();
    }

    if (currentPage <= 1) {
        $("#prevPage").hide();
    } else {
        $("#prevPage").show();
    }

    $("#currentPage").text(currentPage);
    $("#totalPages").text(totalPages);
}
$("#prevPage").click(function () {
    if (currentPage > 1) {
        currentPage--;
        loadListPayment(currentPage);
    }
});

$("#nextPage").click(function () {
    currentPage++;
    loadListPayment(currentPage);
});


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start().catch(err => console.error(err.toString()));

connection.on("ProcessPayment", function () {
    $("#vietqr-popup").hide();
    $('.popup-backdrop').removeClass('show');
    showToast("Thông báo!", "Đã phát hiện giao dịch mới", "info");
    loadListPayment(currentPage);
});

connection.on("NewWithdrawalRequest", function () {
    loadListPayment(currentPage);
});

loadListPayment(currentPage);