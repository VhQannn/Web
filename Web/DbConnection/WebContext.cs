using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Web.DbConnection;

public partial class WebContext : DbContext
{
    public WebContext()
    {
    }

    public WebContext(DbContextOptions<HotrohoctapContext> options)
        : base(options)
    {
    }

	public virtual DbSet<Assignment> Assignments { get; set; }

	public virtual DbSet<Comment> Comments { get; set; }

	public virtual DbSet<Conversation> Conversations { get; set; }

	public virtual DbSet<ConversationMember> ConversationMembers { get; set; }

	public virtual DbSet<Course> Courses { get; set; }

	public virtual DbSet<MarkReport> MarkReports { get; set; }

	public virtual DbSet<Message> Messages { get; set; }

	public virtual DbSet<MessageReadStatus> MessageReadStatuses { get; set; }

	public virtual DbSet<Multimedium> Multimedia { get; set; }

	public virtual DbSet<Notification> Notifications { get; set; }

	public virtual DbSet<ParentComment> ParentComments { get; set; }

	public virtual DbSet<Payment> Payments { get; set; }

	public virtual DbSet<Post> Posts { get; set; }

	public virtual DbSet<PostCategory> PostCategories { get; set; }

	public virtual DbSet<Purchase> Purchases { get; set; }

	public virtual DbSet<QuestionTemplate> QuestionTemplates { get; set; }

	public virtual DbSet<QuestionTemplateDetailQaid> QuestionTemplateDetailQaids { get; set; }

	public virtual DbSet<QuestionTemplatesDetail> QuestionTemplatesDetails { get; set; }

	public virtual DbSet<Rating> Ratings { get; set; }

	public virtual DbSet<Tool> Tools { get; set; }

	public virtual DbSet<ToolCategory> ToolCategories { get; set; }

	public virtual DbSet<User> Users { get; set; }

	public virtual DbSet<UserTool> UserTools { get; set; }

	public virtual DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
		=> optionsBuilder.UseSqlServer("Server=hotrohoctap.database.windows.net;Database=Hotrohoctap;uid=dbroot;pwd=bEFbd7sXUf;TrustServerCertificate=true");

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Assignment>(entity =>
		{
			entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__DA891814BDA59FE2");

			entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
			entity.Property(e => e.Deadline)
				.HasColumnType("date")
				.HasColumnName("deadline");
			entity.Property(e => e.Description).HasColumnName("description");
			entity.Property(e => e.Status)
				.HasMaxLength(50)
				.HasColumnName("status");
			entity.Property(e => e.Title)
				.HasMaxLength(255)
				.HasColumnName("title");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.Assignments)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Assignmen__user___10566F31");
		});

		modelBuilder.Entity<Comment>(entity =>
		{
			entity.HasKey(e => e.CommentId).HasName("PK__Comments__E79576872B1CF8B0");

			entity.Property(e => e.CommentId).HasColumnName("comment_id");
			entity.Property(e => e.CommentDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("comment_date");
			entity.Property(e => e.Content).HasColumnName("content");
			entity.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.ParentComment).WithMany(p => p.Comments)
				.HasForeignKey(d => d.ParentCommentId)
				.HasConstraintName("FK__Comments__parent__114A936A");

			entity.HasOne(d => d.User).WithMany(p => p.Comments)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Comments__user_i__123EB7A3");
		});

		modelBuilder.Entity<Conversation>(entity =>
		{
			entity.HasKey(e => e.ConversationId).HasName("PK__Conversa__311E7E9ABB4D7030");

			entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
			entity.Property(e => e.CreatedTime)
				.HasColumnType("datetime")
				.HasColumnName("created_time");
			entity.Property(e => e.IsActive)
				.HasDefaultValueSql("((1))")
				.HasColumnName("isActive");
			entity.Property(e => e.IsArchived)
				.HasDefaultValueSql("((0))")
				.HasColumnName("isArchived");
			entity.Property(e => e.IsDeleted)
				.HasDefaultValueSql("((0))")
				.HasColumnName("isDeleted");
			entity.Property(e => e.UpdatedTime)
				.HasColumnType("datetime")
				.HasColumnName("updated_time");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.Conversations)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Conversat__user___5BAD9CC8");
		});

		modelBuilder.Entity<ConversationMember>(entity =>
		{
			entity.HasKey(e => e.ConversationMemberId).HasName("PK__Conversa__A390F78D2CE8C4FD");

			entity.ToTable("Conversation_Members");

			entity.Property(e => e.ConversationMemberId).HasColumnName("conversation_member_id");
			entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
			entity.Property(e => e.JoinTime)
				.HasColumnType("datetime")
				.HasColumnName("join_time");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.Conversation).WithMany(p => p.ConversationMembers)
				.HasForeignKey(d => d.ConversationId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Conversat__conve__6AEFE058");

			entity.HasOne(d => d.User).WithMany(p => p.ConversationMembers)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Conversat__user___6BE40491");
		});

		modelBuilder.Entity<Course>(entity =>
		{
			entity.HasKey(e => e.CourseId).HasName("PK__Courses__8F1EF7AED9538176");

			entity.Property(e => e.CourseId).HasColumnName("course_id");
			entity.Property(e => e.CourseDescription).HasColumnName("course_description");
			entity.Property(e => e.CourseTitle)
				.HasMaxLength(255)
				.HasColumnName("course_title");
			entity.Property(e => e.CourseraEmail)
				.HasMaxLength(255)
				.HasColumnName("coursera_email");
			entity.Property(e => e.CourseraPassword)
				.HasMaxLength(255)
				.HasColumnName("coursera_password");
			entity.Property(e => e.EndDate)
				.HasColumnType("date")
				.HasColumnName("end_date");
			entity.Property(e => e.StartDate)
				.HasColumnType("date")
				.HasColumnName("start_date");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.Courses)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Courses__user_id__1332DBDC");
		});

		modelBuilder.Entity<MarkReport>(entity =>
		{
			entity.HasKey(e => e.MarkReportId).HasName("PK__mark_rep__AF0F18C1553D607B");

			entity.ToTable("mark_report");

			entity.Property(e => e.MarkReportId).HasColumnName("mark_report_id");
			entity.Property(e => e.CreatedBy)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("created_by");
			entity.Property(e => e.CreatedDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("created_date");
			entity.Property(e => e.MarkScore).HasColumnName("mark_score");
			entity.Property(e => e.UpdatedBy)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("updated_by");
			entity.Property(e => e.UpdatedDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("updated_date");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.MarkReports)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__mark_repo__user___395884C4");
		});

		modelBuilder.Entity<Message>(entity =>
		{
			entity.HasKey(e => e.MessageId).HasName("PK__Messages__0BBF6EE6844778CC");

			entity.Property(e => e.MessageId).HasColumnName("message_id");
			entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
			entity.Property(e => e.MessageText).HasColumnName("message_text");
			entity.Property(e => e.MessageType)
				.HasMaxLength(50)
				.HasColumnName("message_type");
			entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
			entity.Property(e => e.SenderId).HasColumnName("sender_id");
			entity.Property(e => e.SentTime)
				.HasColumnType("datetime")
				.HasColumnName("sent_time");

			entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
				.HasForeignKey(d => d.ConversationId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Messages__messag__5E8A0973");

			entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
				.HasForeignKey(d => d.SenderId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Messages__sender__5F7E2DAC");
		});

		modelBuilder.Entity<MessageReadStatus>(entity =>
		{
			entity.HasKey(e => e.StatusId).HasName("PK__Message___3683B531800A81B0");

			entity.ToTable("Message_Read_Status");

			entity.Property(e => e.StatusId).HasColumnName("status_id");
			entity.Property(e => e.MessageId).HasColumnName("message_id");
			entity.Property(e => e.ReadTime)
				.HasColumnType("datetime")
				.HasColumnName("read_time");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.Message).WithMany(p => p.MessageReadStatuses)
				.HasForeignKey(d => d.MessageId)
				.HasConstraintName("FK__Message_R__messa__6442E2C9");

			entity.HasOne(d => d.User).WithMany(p => p.MessageReadStatuses)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Message_R__user___65370702");
		});

		modelBuilder.Entity<Multimedium>(entity =>
		{
			entity.HasKey(e => e.MultimediaId).HasName("PK__multimed__C5029F69D77E13EB");

			entity.ToTable("multimedia");

			entity.Property(e => e.MultimediaId).HasColumnName("multimedia_id");
			entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
			entity.Property(e => e.CourseId).HasColumnName("course_id");
			entity.Property(e => e.CreatedBy)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("created_by");
			entity.Property(e => e.CreatedDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("created_date");
			entity.Property(e => e.MarkReportId).HasColumnName("mark_report_id");
			entity.Property(e => e.MessageId).HasColumnName("message_id");
			entity.Property(e => e.MultimediaType)
				.HasMaxLength(50)
				.IsUnicode(false)
				.HasColumnName("multimedia_type");
			entity.Property(e => e.MultimediaUrl)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("multimedia_url");
			entity.Property(e => e.PostCategoryId).HasColumnName("post_category_id");
			entity.Property(e => e.PostId).HasColumnName("post_id");
			entity.Property(e => e.QuestionTemplatesDetailId).HasColumnName("question_templates_detail_id");
			entity.Property(e => e.UpdatedBy)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("updated_by");
			entity.Property(e => e.UpdatedDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("updated_date");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.Assignment).WithMany(p => p.Multimedia)
				.HasForeignKey(d => d.AssignmentId)
				.HasConstraintName("FK__multimedi__assig__4C6B5938");

			entity.HasOne(d => d.Course).WithMany(p => p.Multimedia)
				.HasForeignKey(d => d.CourseId)
				.HasConstraintName("FK__multimedi__cours__4D5F7D71");

			entity.HasOne(d => d.MarkReport).WithMany(p => p.Multimedia)
				.HasForeignKey(d => d.MarkReportId)
				.HasConstraintName("FK__multimedi__mark___498EEC8D");

			entity.HasOne(d => d.Message).WithMany(p => p.Multimedia)
				.HasForeignKey(d => d.MessageId)
				.HasConstraintName("FK__multimedi__messa__6166761E");

			entity.HasOne(d => d.PostCategory).WithMany(p => p.Multimedia)
				.HasForeignKey(d => d.PostCategoryId)
				.HasConstraintName("FK__multimedi__post___4A8310C6");

			entity.HasOne(d => d.Post).WithMany(p => p.Multimedia)
				.HasForeignKey(d => d.PostId)
				.HasConstraintName("FK__multimedi__post___4B7734FF");

			entity.HasOne(d => d.QuestionTemplatesDetail).WithMany(p => p.Multimedia)
				.HasForeignKey(d => d.QuestionTemplatesDetailId)
				.HasConstraintName("FK__multimedi__quest__489AC854");

			entity.HasOne(d => d.User).WithMany(p => p.Multimedia)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__multimedi__user___47A6A41B");
		});

		modelBuilder.Entity<Notification>(entity =>
		{
			entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842FBC4CC11B");

			entity.Property(e => e.NotificationId).HasColumnName("notification_id");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("created_at");
			entity.Property(e => e.IsRead).HasColumnName("is_read");
			entity.Property(e => e.Message)
				.HasMaxLength(255)
				.HasColumnName("message");
			entity.Property(e => e.RedirectUrl)
				.HasMaxLength(255)
				.HasColumnName("redirect_url");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.Notifications)
				.HasForeignKey(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__Notificat__user___236943A5");
		});

		modelBuilder.Entity<ParentComment>(entity =>
		{
			entity.HasKey(e => e.ParentCommentId).HasName("PK__Parent_C__D23CEB6D95D7F8B2");

			entity.ToTable("Parent_Comment");

			entity.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
			entity.Property(e => e.CommentDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("comment_date");
			entity.Property(e => e.Content).HasColumnName("content");
			entity.Property(e => e.PostId).HasColumnName("post_id");
			entity.Property(e => e.Price)
				.HasColumnType("money")
				.HasColumnName("price");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.Post).WithMany(p => p.ParentComments)
				.HasForeignKey(d => d.PostId)
				.HasConstraintName("FK__Parent_Co__post___14270015");

			entity.HasOne(d => d.User).WithMany(p => p.ParentComments)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Parent_Co__user___151B244E");
		});

		modelBuilder.Entity<Payment>(entity =>
		{
			entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EABF099B34");

			entity.Property(e => e.PaymentId).HasColumnName("payment_id");
			entity.Property(e => e.Amount)
				.HasColumnType("decimal(10, 2)")
				.HasColumnName("amount");
			entity.Property(e => e.PaymentDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("payment_date");
			entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
			entity.Property(e => e.RelatedId).HasColumnName("related_id");
			entity.Property(e => e.ServiceType)
				.HasMaxLength(50)
				.HasColumnName("service_type");
			entity.Property(e => e.Status)
				.HasMaxLength(50)
				.HasColumnName("status");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.User).WithMany(p => p.Payments)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Payments__user_i__160F4887");
		});

		modelBuilder.Entity<Post>(entity =>
		{
			entity.HasKey(e => e.PostId).HasName("PK__Posts__3ED78766E8CE2A40");

			entity.Property(e => e.PostId).HasColumnName("post_id");
			entity.Property(e => e.DateSlot)
				.HasColumnType("date")
				.HasColumnName("date_slot");
			entity.Property(e => e.PostCategoryId).HasColumnName("post_category_id");
			entity.Property(e => e.PostContent).HasColumnName("post_content");
			entity.Property(e => e.PostDate)
				.HasColumnType("date")
				.HasColumnName("post_date");
			entity.Property(e => e.PostTitle)
				.HasMaxLength(255)
				.HasColumnName("post_title");
			entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
			entity.Property(e => e.Status)
				.HasMaxLength(50)
				.HasColumnName("status");
			entity.Property(e => e.TimeSlot)
				.HasMaxLength(255)
				.HasColumnName("time_slot");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.PostCategory).WithMany(p => p.Posts)
				.HasForeignKey(d => d.PostCategoryId)
				.HasConstraintName("FK__Posts__post_cate__17036CC0");

			entity.HasOne(d => d.Receiver).WithMany(p => p.PostReceivers)
				.HasForeignKey(d => d.ReceiverId)
				.HasConstraintName("FK__Posts__receiver___17F790F9");

			entity.HasOne(d => d.User).WithMany(p => p.PostUsers)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Posts__user_id__18EBB532");
		});

		modelBuilder.Entity<PostCategory>(entity =>
		{
			entity.HasKey(e => e.PostCategoryId).HasName("PK__Post_Cat__B2316F122BCEB804");

			entity.ToTable("Post_Category");

			entity.HasIndex(e => e.PostCategoryName, "UQ__Post_Cat__8CA54ABEE9E15AE4").IsUnique();

			entity.Property(e => e.PostCategoryId).HasColumnName("post_category_id");
			entity.Property(e => e.PostCategoryName)
				.HasMaxLength(255)
				.HasColumnName("post_category_name");
		});

		modelBuilder.Entity<Purchase>(entity =>
		{
			entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__87071CB92C08F35E");

			entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");
			entity.Property(e => e.Amount).HasColumnName("amount");
			entity.Property(e => e.BuyerId).HasColumnName("buyer_id");
			entity.Property(e => e.PurchaseDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("purchase_date");
			entity.Property(e => e.ToolId).HasColumnName("tool_id");

			entity.HasOne(d => d.Buyer).WithMany(p => p.Purchases)
				.HasForeignKey(d => d.BuyerId)
				.HasConstraintName("FK__Purchases__buyer__19DFD96B");

			entity.HasOne(d => d.Tool).WithMany(p => p.Purchases)
				.HasForeignKey(d => d.ToolId)
				.HasConstraintName("FK__Purchases__tool___1AD3FDA4");
		});

		modelBuilder.Entity<QuestionTemplate>(entity =>
		{
			entity.HasKey(e => e.QuestionTemplateId).HasName("PK__question__7010F8899E0D0916");

			entity.ToTable("question_templates");

			entity.Property(e => e.QuestionTemplateId).HasColumnName("question_template_id");
			entity.Property(e => e.CreatedBy)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("created_by");
			entity.Property(e => e.CreatedDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("created_date");
			entity.Property(e => e.QuestionTemplateCode)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("question_template_code");
		});

		modelBuilder.Entity<QuestionTemplateDetailQaid>(entity =>
		{
			entity.HasKey(e => e.QuestionTemplatesDetailQaidsId).HasName("PK__Question__3214EC27A4E22912");

			entity.ToTable("Question_template_detail_qaids");

			entity.Property(e => e.QuestionTemplatesDetailQaidsId).HasColumnName("question_templates_detail_qaids_id");
			entity.Property(e => e.QAid).HasColumnName("q_aid");
			entity.Property(e => e.QuestionTemplatesDetailId).HasColumnName("question_templates_detail_id");

			entity.HasOne(d => d.QuestionTemplatesDetail).WithMany(p => p.QuestionTemplateDetailQaids)
				.HasForeignKey(d => d.QuestionTemplatesDetailId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__QuestionT__Quest__51300E55");
		});

		modelBuilder.Entity<QuestionTemplatesDetail>(entity =>
		{
			entity.HasKey(e => e.QuestionTemplatesDetailId).HasName("PK__question__50851D49666E7B47");

			entity.ToTable("question_templates_detail");

			entity.Property(e => e.QuestionTemplatesDetailId).HasColumnName("question_templates_detail_id");
			entity.Property(e => e.QId).HasColumnName("q_id");
			entity.Property(e => e.QText)
				.IsUnicode(false)
				.HasColumnName("q_text");
			entity.Property(e => e.QuestionTemplateId).HasColumnName("question_template_id");

			entity.HasOne(d => d.QuestionTemplate).WithMany(p => p.QuestionTemplatesDetails)
				.HasForeignKey(d => d.QuestionTemplateId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK__question___quest__31B762FC");
		});

		modelBuilder.Entity<Rating>(entity =>
		{
			entity.HasKey(e => e.RatingId).HasName("PK__Ratings__D35B278B540964C3");

			entity.Property(e => e.RatingId).HasColumnName("rating_id");
			entity.Property(e => e.Comments).HasColumnName("comments");
			entity.Property(e => e.RaterId).HasColumnName("rater_id");
			entity.Property(e => e.RatingDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("rating_date");
			entity.Property(e => e.RatingValue).HasColumnName("rating_value");
			entity.Property(e => e.RelatedId).HasColumnName("related_id");
			entity.Property(e => e.ServiceType)
				.HasMaxLength(50)
				.HasColumnName("service_type");
			entity.Property(e => e.SupporterId).HasColumnName("supporter_id");

			entity.HasOne(d => d.Rater).WithMany(p => p.RatingRaters)
				.HasForeignKey(d => d.RaterId)
				.HasConstraintName("FK__Ratings__rater_i__1BC821DD");

			entity.HasOne(d => d.Supporter).WithMany(p => p.RatingSupporters)
				.HasForeignKey(d => d.SupporterId)
				.HasConstraintName("FK__Ratings__support__1CBC4616");
		});

		modelBuilder.Entity<Tool>(entity =>
		{
			entity.HasKey(e => e.ToolId).HasName("PK__Tools__28DE264FB28D074B");

			entity.Property(e => e.ToolId).HasColumnName("tool_id");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("created_at");
			entity.Property(e => e.SellerId).HasColumnName("seller_id");
			entity.Property(e => e.ToolCategoryId).HasColumnName("tool_category_id");
			entity.Property(e => e.ToolDescription).HasColumnName("tool_description");
			entity.Property(e => e.ToolName)
				.HasMaxLength(255)
				.HasColumnName("tool_name");
			entity.Property(e => e.ToolPrice)
				.HasColumnType("decimal(10, 2)")
				.HasColumnName("tool_price");
			entity.Property(e => e.UpdatedAt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("updated_at");

			entity.HasOne(d => d.Seller).WithMany(p => p.Tools)
				.HasForeignKey(d => d.SellerId)
				.HasConstraintName("FK__Tools__seller_id__1DB06A4F");

			entity.HasOne(d => d.ToolCategory).WithMany(p => p.Tools)
				.HasForeignKey(d => d.ToolCategoryId)
				.HasConstraintName("FK__Tools__tool_cate__1EA48E88");
		});

		modelBuilder.Entity<ToolCategory>(entity =>
		{
			entity.HasKey(e => e.ToolCategoryId).HasName("PK__Tool_Cat__77A0093DA0850FDD");

			entity.ToTable("Tool_Category");

			entity.HasIndex(e => e.ToolCategoryName, "UQ__Tool_Cat__E74C77BE8B4FAA40").IsUnique();

			entity.Property(e => e.ToolCategoryId).HasColumnName("tool_category_id");
			entity.Property(e => e.ToolCategoryName)
				.HasMaxLength(255)
				.HasColumnName("tool_category_name");
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F4036364C");

			entity.HasIndex(e => e.Username, "UQ__Users__F3DBC5726D062E2B").IsUnique();

			entity.Property(e => e.UserId).HasColumnName("user_id");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("created_at");
			entity.Property(e => e.Email)
				.HasMaxLength(255)
				.HasColumnName("email");
			entity.Property(e => e.Facebook)
				.HasMaxLength(255)
				.HasColumnName("facebook");
			entity.Property(e => e.Password)
				.HasMaxLength(255)
				.HasColumnName("password");
			entity.Property(e => e.UpdatedAt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("updated_at");
			entity.Property(e => e.UserType)
				.HasMaxLength(50)
				.HasColumnName("user_type");
			entity.Property(e => e.Username)
				.HasMaxLength(255)
				.HasColumnName("username");
		});

		modelBuilder.Entity<UserTool>(entity =>
		{
			entity.HasKey(e => e.UserToolId).HasName("PK__User_Too__2ED437F3017703F1");

			entity.ToTable("User_Tools");

			entity.Property(e => e.UserToolId).HasColumnName("user_tool_id");
			entity.Property(e => e.KeyCode)
				.HasMaxLength(255)
				.HasColumnName("key_code");
			entity.Property(e => e.ToolId).HasColumnName("tool_id");
			entity.Property(e => e.UserId).HasColumnName("user_id");

			entity.HasOne(d => d.Tool).WithMany(p => p.UserTools)
				.HasForeignKey(d => d.ToolId)
				.HasConstraintName("FK__User_Tool__tool___1F98B2C1");

			entity.HasOne(d => d.User).WithMany(p => p.UserTools)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__User_Tool__user___208CD6FA");
		});

		modelBuilder.Entity<WithdrawalRequest>(entity =>
		{
			entity.HasKey(e => e.WithdrawalRequestId).HasName("PK__Withdraw__199972BE7AB5E2AC");

			entity.Property(e => e.WithdrawalRequestId).HasColumnName("withdrawal_request_id");
			entity.Property(e => e.Comments).HasColumnName("comments");
			entity.Property(e => e.PaymentId).HasColumnName("payment_id");
			entity.Property(e => e.RequestDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("request_date");
			entity.Property(e => e.Status)
				.HasMaxLength(50)
				.HasColumnName("status");
			entity.Property(e => e.SupporterId).HasColumnName("supporter_id");

			entity.HasOne(d => d.Payment).WithMany(p => p.WithdrawalRequests)
				.HasForeignKey(d => d.PaymentId)
				.HasConstraintName("FK__Withdrawa__payme__2180FB33");

			entity.HasOne(d => d.Supporter).WithMany(p => p.WithdrawalRequests)
				.HasForeignKey(d => d.SupporterId)
				.HasConstraintName("FK__Withdrawa__suppo__22751F6C");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
