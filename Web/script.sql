CREATE TABLE [Post_Category] (
    [post_category_id] int NOT NULL IDENTITY,
    [post_category_name] nvarchar(255) NOT NULL,
    CONSTRAINT [PK__Post_Cat__B2316F122BCEB804] PRIMARY KEY ([post_category_id])
);
GO

CREATE TABLE [question_templates] (
    [question_template_id] int NOT NULL IDENTITY,
    [question_template_code] varchar(255) NOT NULL,
    [created_date] datetime NOT NULL DEFAULT ((getdate())),
    [created_by] varchar(255) NULL,
    CONSTRAINT [PK__question__7010F8899E0D0916] PRIMARY KEY ([question_template_id])
);
GO

CREATE TABLE [Tool_Category] (
    [tool_category_id] int NOT NULL IDENTITY,
    [tool_category_name] nvarchar(255) NOT NULL,
    CONSTRAINT [PK__Tool_Cat__77A0093DA0850FDD] PRIMARY KEY ([tool_category_id])
);
GO

CREATE TABLE [Users] (
    [user_id] int NOT NULL IDENTITY,
    [username] nvarchar(255) NOT NULL,
    [password] nvarchar(255) NOT NULL,
    [email] nvarchar(255) NOT NULL,
    [user_type] nvarchar(50) NULL,
    [facebook] nvarchar(255) NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    [updated_at] datetime NULL DEFAULT ((getdate())),
    [isVerify] bit NULL DEFAULT (((0))),
    [OTPCode] varchar(6) NULL DEFAULT (((0))),
    [OTPCreateTime] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Users__B9BE370F4036364C] PRIMARY KEY ([user_id])
);
GO

CREATE TABLE [question_templates_detail] (
    [question_templates_detail_id] int NOT NULL IDENTITY,
    [question_template_id] int NOT NULL,
    [q_id] int NOT NULL,
    [q_text] varchar(max) NOT NULL,
    CONSTRAINT [PK__question__50851D49666E7B47] PRIMARY KEY ([question_templates_detail_id]),
    CONSTRAINT [FK__question___quest__31B762FC] FOREIGN KEY ([question_template_id]) REFERENCES [question_templates] ([question_template_id])
);
GO

CREATE TABLE [Assignments] (
    [assignment_id] int NOT NULL IDENTITY,
    [user_id] int NULL,
    [title] nvarchar(255) NOT NULL,
    [description] nvarchar(max) NULL,
    [deadline] date NULL,
    [status] nvarchar(50) NULL,
    CONSTRAINT [PK__Assignme__DA891814BDA59FE2] PRIMARY KEY ([assignment_id]),
    CONSTRAINT [FK__Assignmen__user___10566F31] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Conversations] (
    [conversation_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [created_time] datetime NULL,
    [updated_time] datetime NULL,
    [isActive] bit NULL DEFAULT (((1))),
    [isArchived] bit NULL DEFAULT (((0))),
    [isDeleted] bit NULL DEFAULT (((0))),
    CONSTRAINT [PK__Conversa__311E7E9ABB4D7030] PRIMARY KEY ([conversation_id]),
    CONSTRAINT [FK__Conversat__user___5BAD9CC8] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Courses] (
    [course_id] int NOT NULL IDENTITY,
    [user_id] int NULL,
    [course_title] nvarchar(255) NOT NULL,
    [course_description] nvarchar(max) NULL,
    [coursera_email] nvarchar(255) NOT NULL,
    [coursera_password] nvarchar(255) NOT NULL,
    [start_date] date NULL,
    [end_date] date NULL,
    CONSTRAINT [PK__Courses__8F1EF7AED9538176] PRIMARY KEY ([course_id]),
    CONSTRAINT [FK__Courses__user_id__1332DBDC] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [mark_report] (
    [mark_report_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [mark_score] float NULL,
    [created_date] datetime NOT NULL DEFAULT ((getdate())),
    [updated_date] datetime NOT NULL DEFAULT ((getdate())),
    [created_by] varchar(255) NULL,
    [updated_by] varchar(255) NULL,
    CONSTRAINT [PK__mark_rep__AF0F18C1553D607B] PRIMARY KEY ([mark_report_id]),
    CONSTRAINT [FK__mark_repo__user___395884C4] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Notifications] (
    [notification_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [message] nvarchar(255) NOT NULL,
    [is_read] bit NOT NULL,
    [created_at] datetime NOT NULL DEFAULT ((getdate())),
    [redirect_url] nvarchar(255) NULL,
    CONSTRAINT [PK__Notifica__E059842FBC4CC11B] PRIMARY KEY ([notification_id]),
    CONSTRAINT [FK__Notificat__user___236943A5] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Payments] (
    [payment_id] int NOT NULL IDENTITY,
    [user_id] int NULL,
    [amount] decimal(10,2) NOT NULL,
    [payment_date] datetime NULL DEFAULT ((getdate())),
    [related_id] int NULL,
    [service_type] nvarchar(50) NULL,
    [status] nvarchar(50) NULL,
    [receiver_id] int NULL,
    CONSTRAINT [PK__Payments__ED1FC9EABF099B34] PRIMARY KEY ([payment_id]),
    CONSTRAINT [FK__Payments__user_i__160F4887] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Posts] (
    [post_id] int NOT NULL IDENTITY,
    [user_id] int NULL,
    [receiver_id] int NULL,
    [post_category_id] int NULL,
    [post_title] nvarchar(255) NOT NULL,
    [post_content] nvarchar(max) NULL,
    [post_date] date NOT NULL,
    [date_slot] date NOT NULL,
    [time_slot] nvarchar(255) NULL,
    [status] nvarchar(50) NULL,
    CONSTRAINT [PK__Posts__3ED78766E8CE2A40] PRIMARY KEY ([post_id]),
    CONSTRAINT [FK__Posts__post_cate__17036CC0] FOREIGN KEY ([post_category_id]) REFERENCES [Post_Category] ([post_category_id]),
    CONSTRAINT [FK__Posts__receiver___17F790F9] FOREIGN KEY ([receiver_id]) REFERENCES [Users] ([user_id]),
    CONSTRAINT [FK__Posts__user_id__18EBB532] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Ratings] (
    [rating_id] int NOT NULL IDENTITY,
    [rater_id] int NULL,
    [supporter_id] int NULL,
    [rating_value] int NULL,
    [comments] nvarchar(max) NULL,
    [rating_date] datetime NULL DEFAULT ((getdate())),
    [related_id] int NULL,
    [service_type] nvarchar(50) NULL,
    CONSTRAINT [PK__Ratings__D35B278B540964C3] PRIMARY KEY ([rating_id]),
    CONSTRAINT [FK__Ratings__rater_i__1BC821DD] FOREIGN KEY ([rater_id]) REFERENCES [Users] ([user_id]),
    CONSTRAINT [FK__Ratings__support__1CBC4616] FOREIGN KEY ([supporter_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Tools] (
    [tool_id] int NOT NULL IDENTITY,
    [tool_category_id] int NULL,
    [seller_id] int NULL,
    [tool_name] nvarchar(255) NOT NULL,
    [tool_description] nvarchar(max) NULL,
    [tool_price] decimal(10,2) NOT NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    [updated_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Tools__28DE264FB28D074B] PRIMARY KEY ([tool_id]),
    CONSTRAINT [FK__Tools__seller_id__1DB06A4F] FOREIGN KEY ([seller_id]) REFERENCES [Users] ([user_id]),
    CONSTRAINT [FK__Tools__tool_cate__1EA48E88] FOREIGN KEY ([tool_category_id]) REFERENCES [Tool_Category] ([tool_category_id])
);
GO

CREATE TABLE [Question_template_detail_qaids] (
    [question_templates_detail_qaids_id] int NOT NULL IDENTITY,
    [question_templates_detail_id] int NOT NULL,
    [q_aid] int NOT NULL,
    CONSTRAINT [PK__Question__3214EC27A4E22912] PRIMARY KEY ([question_templates_detail_qaids_id]),
    CONSTRAINT [FK__QuestionT__Quest__51300E55] FOREIGN KEY ([question_templates_detail_id]) REFERENCES [question_templates_detail] ([question_templates_detail_id])
);
GO

CREATE TABLE [Conversation_Members] (
    [conversation_member_id] int NOT NULL IDENTITY,
    [conversation_id] int NOT NULL,
    [user_id] int NOT NULL,
    [join_time] datetime NULL,
    CONSTRAINT [PK__Conversa__A390F78D2CE8C4FD] PRIMARY KEY ([conversation_member_id]),
    CONSTRAINT [FK__Conversat__conve__6AEFE058] FOREIGN KEY ([conversation_id]) REFERENCES [Conversations] ([conversation_id]),
    CONSTRAINT [FK__Conversat__user___6BE40491] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Messages] (
    [message_id] int NOT NULL IDENTITY,
    [conversation_id] int NOT NULL,
    [sender_id] int NOT NULL,
    [receiver_id] int NULL,
    [message_text] nvarchar(max) NULL,
    [sent_time] datetime NULL,
    [message_type] nvarchar(50) NULL,
    CONSTRAINT [PK__Messages__0BBF6EE6844778CC] PRIMARY KEY ([message_id]),
    CONSTRAINT [FK__Messages__messag__5E8A0973] FOREIGN KEY ([conversation_id]) REFERENCES [Conversations] ([conversation_id]),
    CONSTRAINT [FK__Messages__sender__5F7E2DAC] FOREIGN KEY ([sender_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [WithdrawalRequests] (
    [withdrawal_request_id] int NOT NULL IDENTITY,
    [payment_id] int NULL,
    [supporter_id] int NULL,
    [request_date] datetime NULL DEFAULT ((getdate())),
    [status] nvarchar(50) NULL,
    [comments] nvarchar(max) NULL,
    CONSTRAINT [PK__Withdraw__199972BE7AB5E2AC] PRIMARY KEY ([withdrawal_request_id]),
    CONSTRAINT [FK__Withdrawa__payme__2180FB33] FOREIGN KEY ([payment_id]) REFERENCES [Payments] ([payment_id]),
    CONSTRAINT [FK__Withdrawa__suppo__22751F6C] FOREIGN KEY ([supporter_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Parent_Comment] (
    [parent_comment_id] int NOT NULL IDENTITY,
    [post_id] int NULL,
    [user_id] int NULL,
    [price] money NOT NULL,
    [content] nvarchar(max) NOT NULL,
    [comment_date] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Parent_C__D23CEB6D95D7F8B2] PRIMARY KEY ([parent_comment_id]),
    CONSTRAINT [FK__Parent_Co__post___14270015] FOREIGN KEY ([post_id]) REFERENCES [Posts] ([post_id]),
    CONSTRAINT [FK__Parent_Co__user___151B244E] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Purchases] (
    [purchase_id] int NOT NULL IDENTITY,
    [buyer_id] int NULL,
    [tool_id] int NULL,
    [purchase_date] datetime NULL DEFAULT ((getdate())),
    [amount] int NOT NULL,
    CONSTRAINT [PK__Purchase__87071CB92C08F35E] PRIMARY KEY ([purchase_id]),
    CONSTRAINT [FK__Purchases__buyer__19DFD96B] FOREIGN KEY ([buyer_id]) REFERENCES [Users] ([user_id]),
    CONSTRAINT [FK__Purchases__tool___1AD3FDA4] FOREIGN KEY ([tool_id]) REFERENCES [Tools] ([tool_id])
);
GO

CREATE TABLE [User_Tools] (
    [user_tool_id] int NOT NULL IDENTITY,
    [tool_id] int NULL,
    [user_id] int NULL,
    [key_code] nvarchar(255) NOT NULL,
    CONSTRAINT [PK__User_Too__2ED437F3017703F1] PRIMARY KEY ([user_tool_id]),
    CONSTRAINT [FK__User_Tool__tool___1F98B2C1] FOREIGN KEY ([tool_id]) REFERENCES [Tools] ([tool_id]),
    CONSTRAINT [FK__User_Tool__user___208CD6FA] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Message_Read_Status] (
    [status_id] int NOT NULL IDENTITY,
    [message_id] int NULL,
    [user_id] int NULL,
    [read_time] datetime NULL,
    CONSTRAINT [PK__Message___3683B531800A81B0] PRIMARY KEY ([status_id]),
    CONSTRAINT [FK__Message_R__messa__6442E2C9] FOREIGN KEY ([message_id]) REFERENCES [Messages] ([message_id]),
    CONSTRAINT [FK__Message_R__user___65370702] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [multimedia] (
    [multimedia_id] int NOT NULL IDENTITY,
    [user_id] int NULL,
    [question_templates_detail_id] int NULL,
    [mark_report_id] int NULL,
    [post_category_id] int NULL,
    [post_id] int NULL,
    [assignment_id] int NULL,
    [course_id] int NULL,
    [multimedia_url] varchar(255) NOT NULL,
    [multimedia_type] varchar(50) NOT NULL,
    [created_date] datetime NOT NULL DEFAULT ((getdate())),
    [updated_date] datetime NOT NULL DEFAULT ((getdate())),
    [created_by] varchar(255) NULL,
    [updated_by] varchar(255) NULL,
    [message_id] int NULL,
    CONSTRAINT [PK__multimed__C5029F69D77E13EB] PRIMARY KEY ([multimedia_id]),
    CONSTRAINT [FK__multimedi__assig__4C6B5938] FOREIGN KEY ([assignment_id]) REFERENCES [Assignments] ([assignment_id]),
    CONSTRAINT [FK__multimedi__cours__4D5F7D71] FOREIGN KEY ([course_id]) REFERENCES [Courses] ([course_id]),
    CONSTRAINT [FK__multimedi__mark___498EEC8D] FOREIGN KEY ([mark_report_id]) REFERENCES [mark_report] ([mark_report_id]),
    CONSTRAINT [FK__multimedi__messa__6166761E] FOREIGN KEY ([message_id]) REFERENCES [Messages] ([message_id]),
    CONSTRAINT [FK__multimedi__post___4A8310C6] FOREIGN KEY ([post_category_id]) REFERENCES [Post_Category] ([post_category_id]),
    CONSTRAINT [FK__multimedi__post___4B7734FF] FOREIGN KEY ([post_id]) REFERENCES [Posts] ([post_id]),
    CONSTRAINT [FK__multimedi__quest__489AC854] FOREIGN KEY ([question_templates_detail_id]) REFERENCES [question_templates_detail] ([question_templates_detail_id]),
    CONSTRAINT [FK__multimedi__user___47A6A41B] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE TABLE [Comments] (
    [comment_id] int NOT NULL IDENTITY,
    [parent_comment_id] int NULL,
    [user_id] int NULL,
    [content] nvarchar(max) NOT NULL,
    [comment_date] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Comments__E79576872B1CF8B0] PRIMARY KEY ([comment_id]),
    CONSTRAINT [FK__Comments__parent__114A936A] FOREIGN KEY ([parent_comment_id]) REFERENCES [Parent_Comment] ([parent_comment_id]),
    CONSTRAINT [FK__Comments__user_i__123EB7A3] FOREIGN KEY ([user_id]) REFERENCES [Users] ([user_id])
);
GO

CREATE INDEX [IX_Assignments_user_id] ON [Assignments] ([user_id]);
GO

CREATE INDEX [IX_Comments_parent_comment_id] ON [Comments] ([parent_comment_id]);
GO

CREATE INDEX [IX_Comments_user_id] ON [Comments] ([user_id]);
GO

CREATE INDEX [IX_Conversation_Members_conversation_id] ON [Conversation_Members] ([conversation_id]);
GO

CREATE INDEX [IX_Conversation_Members_user_id] ON [Conversation_Members] ([user_id]);
GO

CREATE INDEX [IX_Conversations_user_id] ON [Conversations] ([user_id]);
GO

CREATE INDEX [IX_Courses_user_id] ON [Courses] ([user_id]);
GO

CREATE INDEX [IX_mark_report_user_id] ON [mark_report] ([user_id]);
GO

CREATE INDEX [IX_Message_Read_Status_message_id] ON [Message_Read_Status] ([message_id]);
GO

CREATE INDEX [IX_Message_Read_Status_user_id] ON [Message_Read_Status] ([user_id]);
GO

CREATE INDEX [IX_Messages_conversation_id] ON [Messages] ([conversation_id]);
GO

CREATE INDEX [IX_Messages_sender_id] ON [Messages] ([sender_id]);
GO

CREATE INDEX [IX_multimedia_assignment_id] ON [multimedia] ([assignment_id]);
GO

CREATE INDEX [IX_multimedia_course_id] ON [multimedia] ([course_id]);
GO

CREATE INDEX [IX_multimedia_mark_report_id] ON [multimedia] ([mark_report_id]);
GO

CREATE INDEX [IX_multimedia_message_id] ON [multimedia] ([message_id]);
GO

CREATE INDEX [IX_multimedia_post_category_id] ON [multimedia] ([post_category_id]);
GO

CREATE INDEX [IX_multimedia_post_id] ON [multimedia] ([post_id]);
GO

CREATE INDEX [IX_multimedia_question_templates_detail_id] ON [multimedia] ([question_templates_detail_id]);
GO

CREATE INDEX [IX_multimedia_user_id] ON [multimedia] ([user_id]);
GO

CREATE INDEX [IX_Notifications_user_id] ON [Notifications] ([user_id]);
GO

CREATE INDEX [IX_Parent_Comment_post_id] ON [Parent_Comment] ([post_id]);
GO

CREATE INDEX [IX_Parent_Comment_user_id] ON [Parent_Comment] ([user_id]);
GO

CREATE INDEX [IX_Payments_user_id] ON [Payments] ([user_id]);
GO

CREATE UNIQUE INDEX [UQ__Post_Cat__8CA54ABEE9E15AE4] ON [Post_Category] ([post_category_name]);
GO

CREATE INDEX [IX_Posts_post_category_id] ON [Posts] ([post_category_id]);
GO

CREATE INDEX [IX_Posts_receiver_id] ON [Posts] ([receiver_id]);
GO

CREATE INDEX [IX_Posts_user_id] ON [Posts] ([user_id]);
GO

CREATE INDEX [IX_Purchases_buyer_id] ON [Purchases] ([buyer_id]);
GO

CREATE INDEX [IX_Purchases_tool_id] ON [Purchases] ([tool_id]);
GO

CREATE INDEX [IX_Question_template_detail_qaids_question_templates_detail_id] ON [Question_template_detail_qaids] ([question_templates_detail_id]);
GO

CREATE INDEX [IX_question_templates_detail_question_template_id] ON [question_templates_detail] ([question_template_id]);
GO

CREATE INDEX [IX_Ratings_rater_id] ON [Ratings] ([rater_id]);
GO

CREATE INDEX [IX_Ratings_supporter_id] ON [Ratings] ([supporter_id]);
GO

CREATE UNIQUE INDEX [UQ__Tool_Cat__E74C77BE8B4FAA40] ON [Tool_Category] ([tool_category_name]);
GO

CREATE INDEX [IX_Tools_seller_id] ON [Tools] ([seller_id]);
GO

CREATE INDEX [IX_Tools_tool_category_id] ON [Tools] ([tool_category_id]);
GO

CREATE INDEX [IX_User_Tools_tool_id] ON [User_Tools] ([tool_id]);
GO

CREATE INDEX [IX_User_Tools_user_id] ON [User_Tools] ([user_id]);
GO

CREATE UNIQUE INDEX [UQ__Users__F3DBC5726D062E2B] ON [Users] ([username]);
GO

CREATE INDEX [IX_WithdrawalRequests_payment_id] ON [WithdrawalRequests] ([payment_id]);
GO

CREATE INDEX [IX_WithdrawalRequests_supporter_id] ON [WithdrawalRequests] ([supporter_id]);
GO

