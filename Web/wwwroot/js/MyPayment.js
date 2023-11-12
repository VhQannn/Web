
let currentPage = 1;
const pageSize = 2;

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

                // Check if logged in user is a supporter
                if (item.user.role === "Supporter") {
                    $('#payment-table-container .table th:nth-child(2)').text('Người Thực Hiện Giao Dịch');
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
                    // For regular users
                    if (item.status === "PENDING") {
                        actionButton = `<button class="view-qr-button btn btn-danger btn-sm text-white" data-payment-id="${item.paymentId}" data-payment-amount="${item.amount}" data-payment-username="${item.user.username}">View Again QR Code</button>`;
                    } else if (item.status === "COMPLETED") {
                        actionButton = `<button class="view-profile-button btn btn-primary btn-sm text-white" data-receiver-id="${item.receiver.id}">Contact With Supporter</button>`;
                    }
                }

                if (item.serviceType == "Post") {
                    tableBody.append(`<tr>
                    <td>${paymentDate}</td>
                    <td>${item.receiver.username}</td>
                    <td>${amountFormatted}</td>
                    <td><a href="./PostDetails?id=${item.relatedId}" class="post-details-link">${item.serviceType}</a></td>
                    <td>${item.status}</td>
                    <td>${actionButton}</td>
                </tr>`);
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
        var linkQRCode = "https://img.vietqr.io/image/970436-1014794186-lx65zFs.jpg?accountName=TRAN%20QUANG%20QUI&amount=" + amount;
        linkQRCode += "&addInfo=Payment%20OrderID" + paymentId + "%20" + encodeURIComponent(username);

        $("#vietqr-popup").show();
        $('.popup-backdrop').addClass('show');
        $("#txtQRCode").attr('src', linkQRCode);
    });

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

    $(document).on('click', '.view-withdrawal-request', function () {
        var requestComment = $(this).data('request-comment');
        var requestStatus = $(this).data('request-status');

        // Cập nhật nội dung modal
        $('#viewWithdrawalRequestModal .modal-body').html(`<p>Bình luận: ${requestComment}</p><p>Trạng thái: ${requestStatus}</p>`);

        // Hiển thị modal
        $('#viewWithdrawalRequestModal').modal('show');
    });



    $('.view-profile-button').click(function () {
        // Logic for viewing profile
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