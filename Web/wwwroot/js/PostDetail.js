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
                        <div class="comment-author">
                            <img src="profile-image.jpg" alt="Avatar" class="avatar">
                            <span class="author-name">${parentComment.parentCommentUser}</span>
                        </div>
                        <div class="comment-text">
                            ${parentComment.content}
                        </div>
                `;

                if (parentComment.comments && parentComment.comments.length > 0) {
                    commentsHtml += `<div class="child-comments">`;
                    parentComment.comments.forEach(comment => {
                        commentsHtml += `
                            <div class="comment">
                                <div class="comment-author">
                                    <img src="profile-image.jpg" alt="Avatar" class="avatar">
                                    <span class="author-name">${comment.user}</span>
                                </div>
                                <div class="comment-text">
                                    ${comment.content}
                                </div>
                            </div>
                        `;
                    });
                    commentsHtml += `</div>`;
                }

                commentsHtml += `
                    <div class="reply-form">
                        <textarea placeholder="Phản hồi..."></textarea>
                        <button id="reply-btn-${parentComment.parentCommentId}" data-parent-id="${parentComment.parentCommentId}">Đăng</button>

                    </div>
                </div>`;
            });

            $("#comments-section").html(commentsHtml);
        },
        error: function (error) {
            console.error("Error loading comments: ", error);
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
if (postId) {
    loadComments(postId);
}

$('#postCommentBtn').click(function () {
    let commentContent = $('#commentText').val();
    let currentPostId = getParameterByName('id');

    // Gọi API để thêm comment mới
    $.ajax({
        url: `/api/comments?postId=${currentPostId}`,
        method: "POST",
        data: JSON.stringify(commentContent),
        contentType: "application/json; charset=utf-8",
        success: function () {
            // Sau khi thêm comment thành công, thông báo cho các client khác
            connection.invoke("NotifyNewComment").catch(err => console.error(err.toString()));
        },
        error: function (error) {
            console.error("Error posting comment: ", error);
        }
    });

    $('#commentText').val('');  // Reset textarea
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

