﻿const connection = new signalR.HubConnectionBuilder()
    .withUrl("/postHub")
    .build();
connection.start().catch(err => console.error(err.toString()));

const connection2 = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection2.start().catch(err => console.error(err.toString()));

async function loadComments(postId) {
    // Security: Ensure postId is sanitized before use

    if (currentUserId === postUserId || currentRole === "Customer") {
        $('#comment-container').hide();
    }

    try {
        // Perform all AJAX calls in parallel
        const [parentCommentsResponse, hasPostedResponse, paymentExists] = await Promise.all([
            $.ajax({ url: `/api/comments?postId=${postId}`, method: "GET" }),
            $.ajax({ url: `/api/comments/checkParentComment?postId=${postId}`, method: "GET" }),
            $.ajax({ url: `/api/payment/check?postId=${postId}`, method: "GET" })
        ]);

        let commentsHtml = "";

        // Process parent comments
        parentCommentsResponse.forEach(parentComment => {
            let timeAgo = timeSince(parentComment.commentDate);
            commentsHtml += `
                    <div class="comment">
                        <div class="comment-top">
                            <div class="comment-author">
                                <img src="https://cdn3.iconfinder.com/data/icons/login-5/512/LOGIN_6-512.png" alt="Avatar" class="avatar">
                                <span class="author-name">${parentComment.parentCommentUser === currentUsername ? 'Bạn' : parentComment.parentCommentUser}</span>
                                
                            </div>
                            <div class="comment-time">${timeAgo}</div>
                             <div class="comment-price">
                                <span class="price-icon"></span>
                                Giá offer: <span class="price-display">${parentComment.price.toLocaleString('vi-VN')}đ</span></div></div>
                        <div class="comment-top">
                        `;

            commentsHtml += `
                            <div class="comment-text">
                                ${parentComment.content}
                            </div>
                        
                `;

            if (parentComment.parentCommentUser === currentUsername) {
                if (paymentExists != null) {
                    console.log(paymentExists);
                    if (paymentExists.status == "PENDING") {
                        commentsHtml += `
                        <div class="comment-action">
                            <span class="waiting-transaction">Nguời đăng đã chốt và đang chờ giao dịch</span>
                        </div>
                    `;
                    } else if (paymentExists.status == "COMPLETED") {
                        commentsHtml += `
                        <div class="comment-action">
                            <span class="waiting-transaction">Nguời đăng đã chốt và thanh toán </span>
                        </div>
                    `;
                    }

                } else {
                    commentsHtml += `
                        
                            <div class="comment-action">
                                    <button class="edit-price-btn" > Edit</button >
                                    <input type="text" value="${parentComment.price.toLocaleString('vi-VN')}đ" class="price-input price-vnd-format hidden" data-comment-id="${parentComment.parentCommentId}" />
                                    <button class="confirm-edit-btn hidden">Change</button>
                                    <button class="cancel-edit-btn hidden">Cancel</button>
                            </div>
                        `;
                }

            }

            if (currentUserId === postUserId) {
                if (paymentExists != null) {
                    console.log(paymentExists);
                    if (paymentExists.status == "PENDING") {
                        commentsHtml += `
                        <div class="comment-action">
                            <span class="waiting-transaction">Đã chốt và đang chờ giao dịch cho bài đăng này</span>
                        </div>
                    `;
                    } else if (paymentExists.status == "COMPLETED") {
                        commentsHtml += `
                        <div class="comment-action">
                            <span class="waiting-transaction">Đã chốt và thanh toán cho bài đăng này</span>
                        </div>
                    `;
                    }

                } else {
                    // If not, show the accept button
                    commentsHtml += `
                        <div class="comment-action">
                            <button class="accept-button" data-receiver-id="${parentComment.parentCommentUserId}">Chấp nhận giá này</button>
                        </div>
                    `;
                }
            }


            commentsHtml += `</div>`;



            if (parentComment.comments && parentComment.comments.length > 0) {
                commentsHtml += `<div class="child-comments">`;
                parentComment.comments.forEach(comment => {
                    console.log(comment);
                    let timeAgoCommentChild = timeSince(comment.commentDate);
                    commentsHtml += `
                            <div class="comment">
                                <div class="comment-time">${timeAgoCommentChild}</div>
                                <div class="comment-author">
                                    <img src="https://cdn3.iconfinder.com/data/icons/login-5/512/LOGIN_6-512.png" alt="Avatar" class="avatar">
                                    <span class="author-name">${comment.user === currentUsername ? 'Bạn' : comment.user}</span>
                                </div>
                                <div class="comment-text">
                                    ${comment.content}
                                </div>
                            </div>
                        `;
                });
                commentsHtml += `</div>`;
            }

            if (parentComment.parentCommentUser === currentUsername || parentComment.parentCommentUser === postUserId || currentUserId === postUserId) {

                commentsHtml += `
                    <div class="reply-form">
                        <textarea placeholder="Phản hồi..."></textarea>
                        <button id="reply-btn-${parentComment.parentCommentId}" data-parent-id="${parentComment.parentCommentId}">Đăng</button>

                    </div>
                </div>`;

            }


            commentsHtml += `</div>`;
        });

        // Update the DOM once
        $("#comments-section").html(commentsHtml);

        // Handle hasPostedResponse
        if (hasPostedResponse) {
            $('#comment-container').addClass('hidden');
        } else {
            $('#comment-container').removeClass('hidden');
        }

    } catch (error) {
        // Handle errors for any of the AJAX calls
        showToast("Error", error.responseText, "error");
    }
}

function timeSince(date) {
    var seconds = Math.floor((new Date() - new Date(date)) / 1000);

    var interval = seconds / 31536000;

    if (interval > 1) {
        return Math.floor(interval) + " năm trước";
    }
    interval = seconds / 2592000;
    if (interval > 1) {
        return Math.floor(interval) + " tháng trước";
    }
    interval = seconds / 86400;
    if (interval > 1) {
        return Math.floor(interval) + " ngày trước";
    }
    interval = seconds / 3600;
    if (interval > 1) {
        return Math.floor(interval) + " giờ trước";
    }
    interval = seconds / 60;
    if (interval > 1) {
        return Math.floor(interval) + " phút trước";
    }
    return Math.floor(seconds) + " giây trước";
}



function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

var postId = getParameterByName('id');
var paymentId = null;
let currentUserId = null;
let currentUsername = null;
let currentRole = null;
if (postId) {
    $.ajax({
        url: '/api/account/current',
        method: 'GET',
        success: function (data) {
            currentUserId = data.id;
            currentUsername = data.username;
            currentRole = data.role;
            loadComments(postId);
        },
        error: function (error) {
            showToast("Error", error.responseText, "error");
        }
    });
}

$('#postCommentBtn').click(function () {
    let commentContent = $('#commentText').val();
    let currentPostId = getParameterByName('id');
    let priceOffer = $('#priceOffered').val().replace(/[^0-9]/g, '');

    if (!commentContent) {
        showToast("Thông báo", "Vui lòng nhập nội dung bình luận", "info");
        return;
    }
    if (!priceOffer) {
        showToast("Thông báo", "Vui lòng nhập giá bạn offer cho bài viết này ", "info");
        return;
    }

    if (priceOffer <= 1000) {
        showToast("Error", "Giá phải lớn hơn 1000đ", "error");
        return;
    }

    // Gọi API để thêm comment mới
    $.ajax({
        url: `/api/comments?postId=${currentPostId}`,
        method: "POST",
        data: JSON.stringify({
            content: commentContent,
            price: priceOffer
        }
        ),
        contentType: "application/json; charset=utf-8",
        success: function () {
            showToast("Success", "Comment success", "success");
            connection.invoke("NotifyNewComment").catch(err => console.error(err.toString()));
        },
        error: function (error) {
            showToast("Error", error.responseText, "error");
        }
    });
});

// Cập nhật giao diện khi có comment mới
connection.on("NewComment", function () {
    loadComments(postId);
});


connection.on("NewComment", function (comment) {
    let newCommentHtml = `
        <div class="parent-comment">
            <p>${comment.User}: ${comment.Content}</p>
            <p>Date: ${comment.CommentDate}</p>
        </div>
    `;

    $(".comments-list").append(newCommentHtml);
});

connection.on("NewChildComment", function () {
    loadComments(postId);
});


$(document).on('click', '[id^="reply-btn-"]', function () {
    let parentCommentId = $(this).data('parent-id');
    let content = $(this).siblings('textarea').val();

    if (!content) {
        showToast("Thông báo", "Vui lòng nhập nội dung bình luận", "info");
        return;
    }

    // Gọi API để thêm comment con
    $.ajax({
        url: `/api/comments/reply?parentCommentId=${parentCommentId}`,
        method: "POST",
        data: JSON.stringify(content),
        contentType: "application/json; charset=utf-8",
        success: function () {
            showToast("Success", "Comment success", "success");
            connection.invoke("NotifyNewChildComment").catch(err => console.error(err.toString()));
        },
        error: function (error) {
            showToast("Error", error.responseText, "error");
        }
    });
});

$(document).on('click', '.edit-price-btn', function () {
    $(this).siblings('.price-display').hide();
    $(this).siblings('.price-input, .confirm-edit-btn, .cancel-edit-btn').removeClass('hidden');
    $(this).hide();
});


// Khi nhấp vào nút "Xác nhận"
$(document).on('click', '.confirm-edit-btn', function () {
    // Lấy giá trị mới từ ô input
    let newPrice = parseInt($(this).siblings('.price-input').val().replace(/[^0-9]/g, ''));  // Xóa các ký tự không phải số
    let originalPrice = $(this).siblings('.price-display').text().replace(/[^0-9]/g, '');

    let commentId = $(this).siblings('.price-input').data('comment-id');
    // So sánh giá trị mới và giá trị ban đầu
    if (newPrice === parseInt(originalPrice)) {
        showToast("Info", "Không có thay đổi về giá", "info");
        return;
    }

    $.ajax({
        url: `/api/comments/updatePrice?commentId=${commentId}`,
        method: 'POST',
        data: JSON.stringify({ price: newPrice }),
        contentType: "application/json; charset=utf-8",
        success: function () {
            showToast("Success", "Update price success", "success");
            connection.invoke("NotifyNewComment").catch(err => console.error(err.toString()));
        },
        error: function (error) {
            showToast("Error", error.responseText, "error");
        }
    });
});


// Khi nhấp vào nút "Hủy bỏ"
$(document).on('click', '.cancel-edit-btn', function () {
    showToast("Success", "Cancel edit price", "success");
    $(this).siblings('.price-display').show();
    $(this).siblings('.price-input, .confirm-edit-btn, .cancel-edit-btn').addClass('hidden');
    $(this).siblings('.edit-price-btn').show();
    $(this).hide();
});

$(document).on('input', '.price-vnd-format', function () {
    let rawValue = $(this).val();

    // Kiểm tra và xử lý nếu giá trị nhập vào quá lớn
    if (rawValue.length > 15) {
        showToast("Thông báo!", "Giá trị nhập vào quá lớn!", "error");
        $(this).val('');
        return;
    }

    // Xóa các ký tự không phải số và dấu phẩy
    let value = rawValue.replace(/[^0-9,]/g, '');

    // Xóa các dấu phẩy
    value = value.replace(/,/g, '');

    // Kiểm tra nếu giá trị rỗng hoặc không phải là số
    if (!value || isNaN(value)) {
        $(this).val('');
        return;
    }

    // Định dạng lại giá trị theo định dạng tiền tệ VND
    let formattedValue = parseInt(value).toLocaleString('vi-VN');

    // Cập nhật giá trị cho ô input
    $(this).val(formattedValue + 'đ');
});



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

$(document).on('click', '.accept-button', function () {
    $confirmBox.show();
    $overlay.show();
});




connection2.on("ProcessPayment", function () {
    showLoader();
    $("#vietqr-popup").hide();
    $('.popup-backdrop').removeClass('show');
    $overlay.hide();
    $confirmBox.hide();
    showToast("Thành công!", "Đã phát hiện giao dịch. Giao dịch cho bài đăng này đã hoàn tất. Xem lại trong lịch sử giao dịch", "success");
    hideLoader();
    loadComments(postId);
});


$yesButton.click(function () {
    var priceValue = $(".price-display").text().replace(/[^0-9]/g, '');
    var receiverId = $(".accept-button").data('receiver-id');
    $.ajax({
        type: "POST",
        url: "/api/account/create-payment",
        data: JSON.stringify({
            Amount: priceValue,
            ReceiverId: receiverId,
            RelatedId: postId,
            ServiceType: "Post",
            Status: "PENDING"
        }),

        contentType: "application/json",
        success: function (response) {
            console.log(response)
            paymentId = response.paymentId;
            $("#vietqr-popup").show();
            $('.popup-backdrop').addClass('show');
            var linkQRCode = "https://img.vietqr.io/image/970436-1014794186-lx65zFs.jpg?accountName=TRAN%20QUANG%20QUI&amount=";
            linkQRCode += priceValue;
            linkQRCode += "&addInfo=Payment%20OrderID";
            linkQRCode += response.paymentId + "%20";
            linkQRCode += currentUsername;
            $("#txtQRCode").attr('src', linkQRCode);
            showToast("Thành công!", "Giao dịch đã được tạo và chúng tôi đang chờ giao dịch từ bạn", "success");
            $('.accept-button').hide(); // Hide the accept button
            $('.accept-button').after('<div class="payment-status">Đã chốt và đang chờ giao dịch cho bài đăng này</div>'); // Add a new div with the message
        },
        error: function (error) {
            console.log(error);
            showToast("Error", "Error creating payment: " + error.message, "error");
        }
    });
});

$noButton.click(function () {
    $overlay.hide();
    $confirmBox.hide();
});

$closeBtn.click(function () {
    showToast("Thông báo!", "Đơn hàng của bạn đã được tạo và đang chờ giao dịch. Bạn có thể theo dõi trong phần lịch sử giao dịch của tôi", "info");
    $("#vietqr-popup").hide();
    $('.popup-backdrop').removeClass('show');
    $overlay.hide();
    $confirmBox.hide();
});

$backdrop.click(function () {
    showToast("Thông báo!", "Hủy thao tác xác nhận thanh toán", "info");
    $("#vietqr-popup").hide();
    $('.popup-backdrop').removeClass('show');
    $overlay.hide();
    $confirmBox.hide();
});
