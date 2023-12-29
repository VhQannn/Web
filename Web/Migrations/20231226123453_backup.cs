using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Migrations
{
    /// <inheritdoc />
    public partial class backup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Post_Category",
                columns: table => new
                {
                    post_category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_category_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post_Cat__B2316F122BCEB804", x => x.post_category_id);
                });

            migrationBuilder.CreateTable(
                name: "question_templates",
                columns: table => new
                {
                    question_template_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_template_code = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    created_by = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__question__7010F8899E0D0916", x => x.question_template_id);
                });

            migrationBuilder.CreateTable(
                name: "Tool_Category",
                columns: table => new
                {
                    tool_category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tool_category_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tool_Cat__77A0093DA0850FDD", x => x.tool_category_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    user_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    facebook = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    isVerify = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    OTPCode = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: true, defaultValueSql: "((0))"),
                    OTPCreateTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__B9BE370F4036364C", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "question_templates_detail",
                columns: table => new
                {
                    question_templates_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_template_id = table.Column<int>(type: "int", nullable: false),
                    q_id = table.Column<int>(type: "int", nullable: false),
                    q_text = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__question__50851D49666E7B47", x => x.question_templates_detail_id);
                    table.ForeignKey(
                        name: "FK__question___quest__31B762FC",
                        column: x => x.question_template_id,
                        principalTable: "question_templates",
                        principalColumn: "question_template_id");
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    assignment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deadline = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Assignme__DA891814BDA59FE2", x => x.assignment_id);
                    table.ForeignKey(
                        name: "FK__Assignmen__user___10566F31",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    conversation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    isArchived = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Conversa__311E7E9ABB4D7030", x => x.conversation_id);
                    table.ForeignKey(
                        name: "FK__Conversat__user___5BAD9CC8",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    course_title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    course_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    coursera_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    coursera_password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Courses__8F1EF7AED9538176", x => x.course_id);
                    table.ForeignKey(
                        name: "FK__Courses__user_id__1332DBDC",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "mark_report",
                columns: table => new
                {
                    mark_report_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    mark_score = table.Column<double>(type: "float", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updated_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    created_by = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__mark_rep__AF0F18C1553D607B", x => x.mark_report_id);
                    table.ForeignKey(
                        name: "FK__mark_repo__user___395884C4",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    redirect_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__E059842FBC4CC11B", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__Notificat__user___236943A5",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    related_id = table.Column<int>(type: "int", nullable: true),
                    service_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    receiver_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__ED1FC9EABF099B34", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK__Payments__user_i__160F4887",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    receiver_id = table.Column<int>(type: "int", nullable: true),
                    post_category_id = table.Column<int>(type: "int", nullable: true),
                    post_title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    post_content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    post_date = table.Column<DateTime>(type: "date", nullable: false),
                    date_slot = table.Column<DateTime>(type: "date", nullable: false),
                    time_slot = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Posts__3ED78766E8CE2A40", x => x.post_id);
                    table.ForeignKey(
                        name: "FK__Posts__post_cate__17036CC0",
                        column: x => x.post_category_id,
                        principalTable: "Post_Category",
                        principalColumn: "post_category_id");
                    table.ForeignKey(
                        name: "FK__Posts__receiver___17F790F9",
                        column: x => x.receiver_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Posts__user_id__18EBB532",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    rating_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rater_id = table.Column<int>(type: "int", nullable: true),
                    supporter_id = table.Column<int>(type: "int", nullable: true),
                    rating_value = table.Column<int>(type: "int", nullable: true),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rating_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    related_id = table.Column<int>(type: "int", nullable: true),
                    service_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ratings__D35B278B540964C3", x => x.rating_id);
                    table.ForeignKey(
                        name: "FK__Ratings__rater_i__1BC821DD",
                        column: x => x.rater_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Ratings__support__1CBC4616",
                        column: x => x.supporter_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Tools",
                columns: table => new
                {
                    tool_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tool_category_id = table.Column<int>(type: "int", nullable: true),
                    seller_id = table.Column<int>(type: "int", nullable: true),
                    tool_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    tool_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tool_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tools__28DE264FB28D074B", x => x.tool_id);
                    table.ForeignKey(
                        name: "FK__Tools__seller_id__1DB06A4F",
                        column: x => x.seller_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Tools__tool_cate__1EA48E88",
                        column: x => x.tool_category_id,
                        principalTable: "Tool_Category",
                        principalColumn: "tool_category_id");
                });

            migrationBuilder.CreateTable(
                name: "Question_template_detail_qaids",
                columns: table => new
                {
                    question_templates_detail_qaids_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_templates_detail_id = table.Column<int>(type: "int", nullable: false),
                    q_aid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Question__3214EC27A4E22912", x => x.question_templates_detail_qaids_id);
                    table.ForeignKey(
                        name: "FK__QuestionT__Quest__51300E55",
                        column: x => x.question_templates_detail_id,
                        principalTable: "question_templates_detail",
                        principalColumn: "question_templates_detail_id");
                });

            migrationBuilder.CreateTable(
                name: "Conversation_Members",
                columns: table => new
                {
                    conversation_member_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conversation_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    join_time = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Conversa__A390F78D2CE8C4FD", x => x.conversation_member_id);
                    table.ForeignKey(
                        name: "FK__Conversat__conve__6AEFE058",
                        column: x => x.conversation_id,
                        principalTable: "Conversations",
                        principalColumn: "conversation_id");
                    table.ForeignKey(
                        name: "FK__Conversat__user___6BE40491",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conversation_id = table.Column<int>(type: "int", nullable: false),
                    sender_id = table.Column<int>(type: "int", nullable: false),
                    receiver_id = table.Column<int>(type: "int", nullable: true),
                    message_text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sent_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    message_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Messages__0BBF6EE6844778CC", x => x.message_id);
                    table.ForeignKey(
                        name: "FK__Messages__messag__5E8A0973",
                        column: x => x.conversation_id,
                        principalTable: "Conversations",
                        principalColumn: "conversation_id");
                    table.ForeignKey(
                        name: "FK__Messages__sender__5F7E2DAC",
                        column: x => x.sender_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalRequests",
                columns: table => new
                {
                    withdrawal_request_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payment_id = table.Column<int>(type: "int", nullable: true),
                    supporter_id = table.Column<int>(type: "int", nullable: true),
                    request_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Withdraw__199972BE7AB5E2AC", x => x.withdrawal_request_id);
                    table.ForeignKey(
                        name: "FK__Withdrawa__payme__2180FB33",
                        column: x => x.payment_id,
                        principalTable: "Payments",
                        principalColumn: "payment_id");
                    table.ForeignKey(
                        name: "FK__Withdrawa__suppo__22751F6C",
                        column: x => x.supporter_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Parent_Comment",
                columns: table => new
                {
                    parent_comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "money", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    comment_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Parent_C__D23CEB6D95D7F8B2", x => x.parent_comment_id);
                    table.ForeignKey(
                        name: "FK__Parent_Co__post___14270015",
                        column: x => x.post_id,
                        principalTable: "Posts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "FK__Parent_Co__user___151B244E",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    purchase_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    buyer_id = table.Column<int>(type: "int", nullable: true),
                    tool_id = table.Column<int>(type: "int", nullable: true),
                    purchase_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__87071CB92C08F35E", x => x.purchase_id);
                    table.ForeignKey(
                        name: "FK__Purchases__buyer__19DFD96B",
                        column: x => x.buyer_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Purchases__tool___1AD3FDA4",
                        column: x => x.tool_id,
                        principalTable: "Tools",
                        principalColumn: "tool_id");
                });

            migrationBuilder.CreateTable(
                name: "User_Tools",
                columns: table => new
                {
                    user_tool_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tool_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    key_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Too__2ED437F3017703F1", x => x.user_tool_id);
                    table.ForeignKey(
                        name: "FK__User_Tool__tool___1F98B2C1",
                        column: x => x.tool_id,
                        principalTable: "Tools",
                        principalColumn: "tool_id");
                    table.ForeignKey(
                        name: "FK__User_Tool__user___208CD6FA",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Message_Read_Status",
                columns: table => new
                {
                    status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    read_time = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Message___3683B531800A81B0", x => x.status_id);
                    table.ForeignKey(
                        name: "FK__Message_R__messa__6442E2C9",
                        column: x => x.message_id,
                        principalTable: "Messages",
                        principalColumn: "message_id");
                    table.ForeignKey(
                        name: "FK__Message_R__user___65370702",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "multimedia",
                columns: table => new
                {
                    multimedia_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    question_templates_detail_id = table.Column<int>(type: "int", nullable: true),
                    mark_report_id = table.Column<int>(type: "int", nullable: true),
                    post_category_id = table.Column<int>(type: "int", nullable: true),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    assignment_id = table.Column<int>(type: "int", nullable: true),
                    course_id = table.Column<int>(type: "int", nullable: true),
                    multimedia_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    multimedia_type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updated_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    created_by = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    updated_by = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    message_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__multimed__C5029F69D77E13EB", x => x.multimedia_id);
                    table.ForeignKey(
                        name: "FK__multimedi__assig__4C6B5938",
                        column: x => x.assignment_id,
                        principalTable: "Assignments",
                        principalColumn: "assignment_id");
                    table.ForeignKey(
                        name: "FK__multimedi__cours__4D5F7D71",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "course_id");
                    table.ForeignKey(
                        name: "FK__multimedi__mark___498EEC8D",
                        column: x => x.mark_report_id,
                        principalTable: "mark_report",
                        principalColumn: "mark_report_id");
                    table.ForeignKey(
                        name: "FK__multimedi__messa__6166761E",
                        column: x => x.message_id,
                        principalTable: "Messages",
                        principalColumn: "message_id");
                    table.ForeignKey(
                        name: "FK__multimedi__post___4A8310C6",
                        column: x => x.post_category_id,
                        principalTable: "Post_Category",
                        principalColumn: "post_category_id");
                    table.ForeignKey(
                        name: "FK__multimedi__post___4B7734FF",
                        column: x => x.post_id,
                        principalTable: "Posts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "FK__multimedi__quest__489AC854",
                        column: x => x.question_templates_detail_id,
                        principalTable: "question_templates_detail",
                        principalColumn: "question_templates_detail_id");
                    table.ForeignKey(
                        name: "FK__multimedi__user___47A6A41B",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parent_comment_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    comment_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comments__E79576872B1CF8B0", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK__Comments__parent__114A936A",
                        column: x => x.parent_comment_id,
                        principalTable: "Parent_Comment",
                        principalColumn: "parent_comment_id");
                    table.ForeignKey(
                        name: "FK__Comments__user_i__123EB7A3",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_user_id",
                table: "Assignments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_parent_comment_id",
                table: "Comments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_user_id",
                table: "Comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_Members_conversation_id",
                table: "Conversation_Members",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_Members_user_id",
                table: "Conversation_Members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_user_id",
                table: "Conversations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_user_id",
                table: "Courses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mark_report_user_id",
                table: "mark_report",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Message_Read_Status_message_id",
                table: "Message_Read_Status",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_Message_Read_Status_user_id",
                table: "Message_Read_Status",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_conversation_id",
                table: "Messages",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_sender_id",
                table: "Messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_multimedia_assignment_id",
                table: "multimedia",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_multimedia_course_id",
                table: "multimedia",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_multimedia_mark_report_id",
                table: "multimedia",
                column: "mark_report_id");

            migrationBuilder.CreateIndex(
                name: "IX_multimedia_message_id",
                table: "multimedia",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_multimedia_post_category_id",
                table: "multimedia",
                column: "post_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_multimedia_post_id",
                table: "multimedia",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_multimedia_question_templates_detail_id",
                table: "multimedia",
                column: "question_templates_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_multimedia_user_id",
                table: "multimedia",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Parent_Comment_post_id",
                table: "Parent_Comment",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Parent_Comment_user_id",
                table: "Parent_Comment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_user_id",
                table: "Payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Post_Cat__8CA54ABEE9E15AE4",
                table: "Post_Category",
                column: "post_category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_post_category_id",
                table: "Posts",
                column: "post_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_receiver_id",
                table: "Posts",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_user_id",
                table: "Posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_buyer_id",
                table: "Purchases",
                column: "buyer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_tool_id",
                table: "Purchases",
                column: "tool_id");

            migrationBuilder.CreateIndex(
                name: "IX_Question_template_detail_qaids_question_templates_detail_id",
                table: "Question_template_detail_qaids",
                column: "question_templates_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_templates_detail_question_template_id",
                table: "question_templates_detail",
                column: "question_template_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_rater_id",
                table: "Ratings",
                column: "rater_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_supporter_id",
                table: "Ratings",
                column: "supporter_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Tool_Cat__E74C77BE8B4FAA40",
                table: "Tool_Category",
                column: "tool_category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tools_seller_id",
                table: "Tools",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "IX_Tools_tool_category_id",
                table: "Tools",
                column: "tool_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Tools_tool_id",
                table: "User_Tools",
                column: "tool_id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Tools_user_id",
                table: "User_Tools",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__F3DBC5726D062E2B",
                table: "Users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_payment_id",
                table: "WithdrawalRequests",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_supporter_id",
                table: "WithdrawalRequests",
                column: "supporter_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Conversation_Members");

            migrationBuilder.DropTable(
                name: "Message_Read_Status");

            migrationBuilder.DropTable(
                name: "multimedia");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Question_template_detail_qaids");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "User_Tools");

            migrationBuilder.DropTable(
                name: "WithdrawalRequests");

            migrationBuilder.DropTable(
                name: "Parent_Comment");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "mark_report");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "question_templates_detail");

            migrationBuilder.DropTable(
                name: "Tools");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "question_templates");

            migrationBuilder.DropTable(
                name: "Tool_Category");

            migrationBuilder.DropTable(
                name: "Post_Category");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
