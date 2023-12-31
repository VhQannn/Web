﻿using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? UserType { get; set; }

    public string Facebook { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<ParentComment> ParentComments { get; set; } = new List<ParentComment>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<Rating> RatingRaters { get; set; } = new List<Rating>();

    public virtual ICollection<Rating> RatingSupporters { get; set; } = new List<Rating>();

    public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();

    public virtual ICollection<UserTool> UserTools { get; set; } = new List<UserTool>();
}
