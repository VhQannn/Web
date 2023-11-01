function loadComments(postId) {
    $.ajax({
        url: `/api/comments?postId=${postId}`,
        method: "GET",
        success: function (parentComments) {
            console.log(parentComments);
            let commentsHtml = "";

            parentComments.forEach(parentComment => {
                commentsHtml += `
                    <div class="comment">
                        <div class="comment-top">
                            <div class="comment-author">
                                <img src="https://cdn3.iconfinder.com/data/icons/login-5/512/LOGIN_6-512.png" alt="Avatar" class="avatar">
                                <span class="author-name">${parentComment.parentCommentUser === currentUsername ? 'Bạn' : parentComment.parentCommentUser}</span>
                            </div>
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
                    console.log(parentComment);
                                    commentsHtml += `
                        
                            <div class="comment-action">
                                    <button class="edit-price-btn" > Edit</button >
                                    <input type="text" value="${parentComment.price.toLocaleString('vi-VN')}đ" class="price-input price-vnd-format hidden" data-comment-id="${parentComment.parentCommentId}" />
                                    <button class="confirm-edit-btn hidden">Change</button>
                                    <button class="cancel-edit-btn hidden">Cancel</button>
                            </div>
                        `;
                }


                commentsHtml += `</div>`;
                


                if (parentComment.comments && parentComment.comments.length > 0) {
                    commentsHtml += `<div class="child-comments">`;
                    parentComment.comments.forEach(comment => {
                        commentsHtml += `
                            <div class="comment">
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

                if (parentComment.parentCommentUser === currentUsername || parentComment.parentCommentUser === postUserId) {

                    commentsHtml += `
                    <div class="reply-form">
                        <textarea placeholder="Phản hồi..."></textarea>
                        <button id="reply-btn-${parentComment.parentCommentId}" data-parent-id="${parentComment.parentCommentId}">Đăng</button>

                    </div>
                </div>`;

                }
            });

            $("#comments-section").html(commentsHtml);
        },
        error: function (error) {
            console.error("Error loading comments: ", error);
        }
    });

    $.ajax({
        url: `/api/comments/checkParentComment?postId=${postId}`,
        method: "GET",
        success: function (hasPosted) {
            if (hasPosted) {
                $('#commentText').hide();  // Ẩn textarea
                $('#postCommentBtn').hide();  // Ẩn nút Đăng
                $('#priceOffered').hide();
            } else {
                $('#commentText').show();  // Hiển thị textarea
                $('#postCommentBtn').show();  // Hiển thị nút Đăng
                $('#priceOffered').show();
            }
        },
        error: function (error) {
            console.error("Error checking parent comment: ", error);
        }
    });
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
let currentUserId = null;
let currentUsername = null;
if (postId) {
    $.ajax({
        url: '/api/account/current',
        method: 'GET',
        success: function (data) {
            console.log(data);
            currentUserId = data.Id;
            currentUsername = data.username;
            loadComments(postId);
        },
        error: function (error) {
            console.error("Error fetching current user: ", error);
        }
    });
}

$('#postCommentBtn').click(function () {
    let commentContent = $('#commentText').val();
    let currentPostId = getParameterByName('id');
    let priceOffer = $('#priceOffered').val();

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
            // Sau khi thêm comment thành công, thông báo cho các client khác
            connection.invoke("NotifyNewComment").catch(err => console.error(err.toString()));
        },
        error: function (error) {
            console.error("Error posting comment: ", error);
        }
    });
    $('#commentText').hide();  // Ẩn textarea
    $('#postCommentBtn').hide();  // Ẩn nút Đăng
    $('#priceOffered').hide();
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

    // Gọi API để thêm comment con
    $.ajax({
        url: `/api/comments/reply?parentCommentId=${parentCommentId}`,
        method: "POST",
        data: JSON.stringify(content),
        contentType: "application/json; charset=utf-8",
        success: function () {
            // Sau khi thêm comment con thành công, thông báo cho các client khác
            connection.invoke("NotifyNewChildComment").catch(err => console.error(err.toString()));
        },
        error: function (error) {
            console.error("Error posting reply: ", error);
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
    let newPrice = $(this).siblings('.price-input').val().replace(/[^0-9]/g, '');  // Xóa các ký tự không phải số

    let commentId = $(this).siblings('.price-input').data('comment-id');


    $.ajax({
        url: `/api/comments/updatePrice?commentId=${commentId}`,
        method: 'POST',
        data: JSON.stringify({ price: newPrice }),
        contentType: "application/json; charset=utf-8",
        success: function () {
            connection.invoke("NotifyNewComment").catch(err => console.error(err.toString()));
        },
        error: function (error) {
            console.error("Error updating price: ", error);
        }
    });
});


// Khi nhấp vào nút "Hủy bỏ"
$(document).on('click', '.cancel-edit-btn', function () {
    $(this).siblings('.price-display').show();
    $(this).siblings('.price-input, .confirm-edit-btn, .cancel-edit-btn').addClass('hidden');
    $(this).siblings('.edit-price-btn').show();
});

$(document).on('input', '.price-vnd-format', function () {
    // Xóa các ký tự không phải số và dấu phẩy
    let value = $(this).val().replace(/[^0-9,]/g, '');

    // Xóa các dấu phẩy
    value = value.replace(/,/g, '');

    // Định dạng lại giá trị theo định dạng tiền tệ VND
    let formattedValue = parseInt(value).toLocaleString('vi-VN');

    // Cập nhật giá trị cho ô input
    $(this).val(formattedValue + 'đ');
});

