﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Web.DbConnection;

public partial class WebContext : DbContext
{
    public WebContext()
    {
    }

    public WebContext(DbContextOptions<WebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<ParentComment> ParentComments { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostCategory> PostCategories { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Tool> Tools { get; set; }

    public virtual DbSet<ToolCategory> ToolCategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserTool> UserTools { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-TEIM7A0\\SQLEXPRESS;Database=Web;uid=sa;pwd=admin;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_100_CI_AI_SC_UTF8");

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__DA8918143B4DF870");

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
                .HasConstraintName("FK__Assignmen__user___4E88ABD4");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__E7957687874DB425");

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
                .HasConstraintName("FK__Comments__parent__59FA5E80");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__user_i__5AEE82B9");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__8F1EF7AEC115DBF9");

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
                .HasConstraintName("FK__Courses__user_id__52593CB8");
        });

        modelBuilder.Entity<ParentComment>(entity =>
        {
            entity.HasKey(e => e.ParentCommentId).HasName("PK__Parent_C__D23CEB6D8917B076");

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
                .HasConstraintName("FK__Parent_Co__post___5535A963");

            entity.HasOne(d => d.User).WithMany(p => p.ParentComments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Parent_Co__user___5629CD9C");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EA2CA39D79");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
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
                .HasConstraintName("FK__Payments__user_i__48CFD27E");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__3ED78766EC37B20E");

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
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TimeSlot)
                .HasMaxLength(255)
                .HasColumnName("time_slot");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PostCategory).WithMany(p => p.Posts)
                .HasForeignKey(d => d.PostCategoryId)
                .HasConstraintName("FK__Posts__post_cate__2E1BDC42");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Posts__user_id__2D27B809");
        });

        modelBuilder.Entity<PostCategory>(entity =>
        {
            entity.HasKey(e => e.PostCategoryId).HasName("PK__Post_Cat__B2316F12886787D7");

            entity.ToTable("Post_Category");

            entity.HasIndex(e => e.PostCategoryName, "UQ__Post_Cat__8CA54ABE8DB81578").IsUnique();

            entity.Property(e => e.PostCategoryId).HasColumnName("post_category_id");
            entity.Property(e => e.PostCategoryName)
                .HasMaxLength(255)
                .HasColumnName("post_category_name");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__87071CB97370086A");

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
                .HasConstraintName("FK__Purchases__buyer__3E52440B");

            entity.HasOne(d => d.Tool).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.ToolId)
                .HasConstraintName("FK__Purchases__tool___3F466844");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Ratings__D35B278B564CD70C");

            entity.Property(e => e.RatingId).HasColumnName("rating_id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.RaterId).HasColumnName("rater_id");
            entity.Property(e => e.RatingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("rating_date");
            entity.Property(e => e.RatingValue).HasColumnName("rating_value");
            entity.Property(e => e.SupporterId).HasColumnName("supporter_id");

            entity.HasOne(d => d.Rater).WithMany(p => p.RatingRaters)
                .HasForeignKey(d => d.RaterId)
                .HasConstraintName("FK__Ratings__rater_i__4316F928");

            entity.HasOne(d => d.Supporter).WithMany(p => p.RatingSupporters)
                .HasForeignKey(d => d.SupporterId)
                .HasConstraintName("FK__Ratings__support__440B1D61");
        });

        modelBuilder.Entity<Tool>(entity =>
        {
            entity.HasKey(e => e.ToolId).HasName("PK__Tools__28DE264F6729316D");

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
                .HasConstraintName("FK__Tools__seller_id__35BCFE0A");

            entity.HasOne(d => d.ToolCategory).WithMany(p => p.Tools)
                .HasForeignKey(d => d.ToolCategoryId)
                .HasConstraintName("FK__Tools__tool_cate__34C8D9D1");
        });

        modelBuilder.Entity<ToolCategory>(entity =>
        {
            entity.HasKey(e => e.ToolCategoryId).HasName("PK__Tool_Cat__77A0093D635E12EC");

            entity.ToTable("Tool_Category");

            entity.HasIndex(e => e.ToolCategoryName, "UQ__Tool_Cat__E74C77BE891FFA4C").IsUnique();

            entity.Property(e => e.ToolCategoryId).HasColumnName("tool_category_id");
            entity.Property(e => e.ToolCategoryName)
                .HasMaxLength(255)
                .HasColumnName("tool_category_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FACE89A4D");

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572191C503C").IsUnique();

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
            entity.HasKey(e => e.UserToolId).HasName("PK__User_Too__2ED437F37E5CE049");

            entity.ToTable("User_Tools");

            entity.Property(e => e.UserToolId).HasColumnName("user_tool_id");
            entity.Property(e => e.KeyCode)
                .HasMaxLength(255)
                .HasColumnName("key_code");
            entity.Property(e => e.ToolId).HasColumnName("tool_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Tool).WithMany(p => p.UserTools)
                .HasForeignKey(d => d.ToolId)
                .HasConstraintName("FK__User_Tool__tool___3A81B327");

            entity.HasOne(d => d.User).WithMany(p => p.UserTools)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__User_Tool__user___3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
