const connection = new signalR.HubConnectionBuilder()
    .withUrl("/postHub")
    .build();

let currentPage = 1;
const pageSize = 2;

$(document).on('click', 'a[data-target="#modelRating"]', function () {
    $('#modelRating').modal('show');
});


connection.on("UpdatePosts", () => {
    loadPosts(currentPage);
});

function loadPosts(pageNumber) {
    $.ajax({
        url: `/api/my-assignment?pageNumber=${pageNumber}&pageSize=${pageSize}`,
        method: "GET",
        success: function (response) {
            const { data, totalRecords, totalPages } = response;
            $("table tbody").empty();

            data.forEach(post => {
                const dateOnly = post.dateSlot.split('T')[0];
                let buttonHtml = '';

                if (post.poster.role === "Customer") {
                    // Hiển thị nút "Tôi đã làm xong" cho Supporter
                    $('#payment-table-container .table th:nth-child(1)').text('Người Cần Hỗ Trợ');
                    if (post.status === "APPROVED") {

                        buttonHtml = `<button class="view-profile-button btn btn-primary btn-sm text-white" data-service-id="${post.postId}">Tôi đã làm xong</button>`;
                    } else {
                        buttonHtml = `<span class="text-primary">Đã Hoàn Thành</span>`;
                    }
                } else if (post.status === "COMPLETED" && post.poster.role === "Supporter") {
                    // Hiển thị tùy chọn đánh giá cho Customer
                    $('#payment-table-container .table th:nth-child(1)').text('Người Hỗ Trợ Cho Bạn');
                    if (post.rating != null) {
                        buttonHtml = `<span class="text-primary">Đã Hoàn Thành và Đánh Giá</span>`;
                    } else {
                        buttonHtml = `<a class="btn btn-danger btn-sm text-white" id="btnCreateRating" title="Rating Request" data-toggle="modal" data-target="#modelRating" data-supporter-id="${post.poster.id}" data-service-id="${post.postId}">Đánh Giá Dịch Vụ</a>`;
                    }
                }


                const row = `
                    <tr>
                        <td>${post.poster.username}</td>
                        <td>${dateOnly}</td>
                        <td>${post.timeSlot}</td>
                        <td><a href="./PostDetails?id=${post.postId}">Post</a></td>
                        <td>${post.status}</td>
                        <td>${buttonHtml}</td>
                    </tr>
                `;
                $("table tbody").append(row);
            });
            
            updatePaginationButtons(currentPage, totalPages);
        },
        error: function (error) {
            console.error("Lỗi khi cập nhật bảng: ", error);
        }
    });
}



function attachButtonClickEvents() {

    $('#btnCreateRating').click(function () {
        $('#modelRating').modal('show');
    });

    $('#btn-request').click(function () {
        var supporterId = $('#btnCreateRating').data('supporter-id');
        var postId = $('#btnCreateRating').data('service-id');
        var ratingValue = $('input[name="rate"]:checked').val();
        var comment = $('.rating-comment-input').val().trim();

        if (!ratingValue) {
            showToast("Error", "Vui lòng chọn một đánh giá.", "error");
            return; // Dừng hàm nếu không có giá trị đánh giá được chọn
        }

        // Check if the comment is not empty
        if (comment.length === 0) {
            showToast("Error", "Please enter a comment.", "error");
            return; // Stop the function if no comment is entered
        }

        var ratingData = {
            RelatedId: postId,
            ServiceType: "Post",
            SupporterId: supporterId,
            RatingValue: ratingValue,
            Comments: comment
        };

        fetch('/api/my-assignment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(ratingData),
        })
            .then(response => response.json())
            .then(data => {
                showToast("Thành công!", "Đánh giá đã được tạo thành công", "success");
                $('#modelRating').modal('hide');
            })
            .catch(error => {
                showToast("Error", "Lỗi khi tạo đánh giá: " + error, "error");
            });
    });

    $('.view-profile-button').click(function () {
        var postId = $(this).data('service-id');
        
        var data = {
            RelatedId: postId,
            ServiceType: "Post"
        };
        fetch('/api/posts/update-status-for-supporter', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        })
            .then(response => response.json())
            .then(data => {
                showToast("Thành công!", "Đã cập nhật trạng thái bài đăng sang đã xong", "success");
            })
            .catch(error => {
                showToast("Error", "Lỗi khi cập nhật: " + error, "error");
            });
    });

    $('.close').click(function () {
        $('#modelRating').modal('hide');
        $('#viewRatingModal').modal('hide');
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
        loadPosts(currentPage);
    }
});

$("#nextPage").click(function () {
    currentPage++;
    loadPosts(currentPage);
});


connection.start().catch(err => console.error(err.toString()));

// Load initial data
loadPosts(currentPage);
attachButtonClickEvents();