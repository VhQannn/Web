const connection = new signalR.HubConnectionBuilder()
    .withUrl("/postHub")
    .build();

connection.on("UpdatePosts", () => {
    // Gọi API để lấy danh sách bài viết mới
    $.ajax({
        url: "/api/posts",  // Đường dẫn tới API lấy danh sách bài viết
        method: "GET",
        success: function (data) {
            // Xóa nội dung hiện tại của bảng
            $("table tbody").empty();

            console.log(data);

            // Duyệt qua mỗi bài viết và thêm vào bảng
            data.forEach(post => {
                const row = `
                    <tr>
                        <td>${post.postTitle}</td>
                        <td>${post.postContent}</td>
                        <td>${post.postDate}</td>
                        <td>${post.timeSlot}</td>
                        <td>${post.status}</td>
                        <td>${post.postCategoryName}</td>
                        <td>${post.username}</td>
                        <td><a href="./Details?id=${post.postId}">View Details</a></td>
                    </tr>
                `;
                $("table tbody").append(row);
            });
        },
        error: function (error) {
            console.error("Lỗi khi cập nhật bảng: ", error);
        }
    });
});

connection.start().catch(err => console.error(err.toString()));
