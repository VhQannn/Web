const connection = new signalR.HubConnectionBuilder()
    .withUrl("/postHub")
    .build();

let currentPage = 1;
const pageSize = 2;



function createPostCard(post) {
    const dateOnly = post.dateSlot.split('T')[0];
    const displayedUsername = post.username || "Chưa có";
    //post.status
    return `<a href="./PostDetails?id=${post.postId}" class="card" style="width:40%;text-decoration:none;color:black">
    <div class="left">
        <img class="profile_img" src="https://t4.ftcdn.net/jpg/02/29/75/83/360_F_229758328_7x8jwCwjtBMmC6rgFzLFhZoEpLobB6L8.jpg" alt="">
    </div>
    <div class="right">
        <h2 class="post-title">${post.postTitle}</h2>
        <p class="post-time"><span class="badge bg-secondary">${dateOnly} at ${post.timeSlot}</span></p>
      
        <p class="post-category-title">Môn học: </p>

        <div class="post-category">
            <span class="post-category-item">${post.postCategoryName} </span>
        </div>
    </div>
</a>`
}

function createPostCard2(post) {
    const dateOnly = post.dateSlot.split('T')[0];
    const displayedUsername = post.username || "Chưa có";
    return `

        <a class="post-card" href="./PostDetails?id=${post.postId}">
            <div class="post-card-header">
                <h3 class="post-title">${post.postTitle}</h3>
                <div class="post-date">${dateOnly} at ${post.timeSlot}</div>
            </div>
            <div class="post-card-body">
                <p class="post-content">${post.postContent}</p>
                <div class="post-category">${post.postCategoryName}</div>
                <div class="post-status">${post.status}</div>
                <div class="post-username">Posted by: ${displayedUsername}</div>
            </div>
            <div class="post-card-footer">
           
            </div>
        </a>
    `;
}

function loadPosts(pageNumber) {
    $.ajax({
        url: `/api/posts?pageNumber=${pageNumber}&pageSize=${pageSize}`,
        method: "GET",
        success: function (response) {
            const { data, totalRecords, totalPages } = response;

            // Xóa nội dung hiện tại của card container
            $(".post-card-container").empty();

            // Duyệt qua mỗi bài viết và tạo card tương ứng
            data.forEach(post => {
                const postCard = createPostCard(post);
                $(".post-card-container").append(postCard);
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


function searchPosts() {
    var title = $("#search-key").val();

    $.ajax({
        url: `/api/posts/get-by-title?title=${title}`,
        method: "GET",
        success: function (response) {
            const { data } = response;
            $(".post-card-container").empty();

            // Duyệt qua mỗi bài viết và tạo card tương ứng
            data.forEach(post => {
                const postCard = createPostCard(post);
                $(".post-card-container").append(postCard);
            });

        },
        error: function (error) {
            console.error("Lỗi khi cập nhật bảng: ", error);
        }
    });
}