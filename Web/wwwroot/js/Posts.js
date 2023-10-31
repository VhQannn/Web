const connection = new signalR.HubConnectionBuilder()
    .withUrl("/postHub")
    .build();

let currentPage = 1;
const pageSize = 5;

function loadPosts(pageNumber) {
    $.ajax({
        url: `/api/posts?pageNumber=${pageNumber}&pageSize=${pageSize}`,
        method: "GET",
        success: function (response) {
            const { data, totalRecords, totalPages } = response;

            // Xóa nội dung hiện tại của bảng
            $("table tbody").empty();

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
        },
        error: function (error) {
            console.error("Lỗi khi cập nhật bảng: ", error);
        }
    });
}

connection.on("UpdatePosts", () => {
    loadPosts(currentPage);
});

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
