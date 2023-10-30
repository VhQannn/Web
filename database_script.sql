use master
go

drop database if exists Web
go
create database Web
go

use Web
go

-- Creating the Users table
CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(255) NOT NULL,
    password NVARCHAR(255) NOT NULL,
    email NVARCHAR(255) NOT NULL,
    user_type NVARCHAR(50) CHECK (user_type IN ('Admin', 'Customer', 'Supporter', 'Seller')),
	facebook NVARCHAR(255) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

-- Creating the Post Category table
CREATE TABLE Post_Category (
    post_category_id INT PRIMARY KEY IDENTITY(1,1),
    post_category_name NVARCHAR(255) NOT NULL
);

-- Creating the Posts table
CREATE TABLE Posts (
    post_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
	post_category_id INT FOREIGN KEY REFERENCES Post_Category(post_category_id),
    post_title NVARCHAR(255) NOT NULL,
    post_content TEXT,
    post_date DATE NOT NULL,
    time_slot NVARCHAR(255),
    status NVARCHAR(50) CHECK (status IN ('approved', 'pending', 'rejected','received', 'done'))
);

-- Creating the Tool Category table
CREATE TABLE Tool_Category (
    tool_category_id INT PRIMARY KEY IDENTITY(1,1),
    tool_category_name NVARCHAR(255) NOT NULL
);

-- Creating the Tools table
CREATE TABLE Tools (
    tool_id INT PRIMARY KEY IDENTITY(1,1),
	tool_category_id INT FOREIGN KEY REFERENCES Tool_Category(tool_category_id),
    seller_id INT FOREIGN KEY REFERENCES Users(user_id),
    tool_name NVARCHAR(255) NOT NULL,
    tool_description TEXT,
    tool_price DECIMAL(10, 2) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE User_Tools (
    user_tool_id INT PRIMARY KEY IDENTITY(1,1),
	tool_id INT FOREIGN KEY REFERENCES Tools(tool_id),
	user_id INT FOREIGN KEY REFERENCES Users(user_id),
	key_code NVARCHAR(255) NOT NULL
);

-- Creating the Purchases table
CREATE TABLE Purchases (
    purchase_id INT PRIMARY KEY IDENTITY(1,1),
    buyer_id INT FOREIGN KEY REFERENCES Users(user_id),
    tool_id INT FOREIGN KEY REFERENCES Tools(tool_id),
    purchase_date DATETIME DEFAULT GETDATE(),
    amount INT NOT NULL
);

-- Creating the Ratings table
CREATE TABLE Ratings (
    rating_id INT PRIMARY KEY IDENTITY(1,1),
    rater_id INT FOREIGN KEY REFERENCES Users(user_id),
    supporter_id INT FOREIGN KEY REFERENCES Users(user_id),
    rating_value INT CHECK (rating_value BETWEEN 1 AND 5),
    comments TEXT,
    rating_date DATETIME DEFAULT GETDATE()
);

-- Creating the Payments table
CREATE TABLE Payments (
    payment_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    amount DECIMAL(10, 2) NOT NULL,
    payment_date DATETIME DEFAULT GETDATE(),
	related_id INT,
	service_type NVARCHAR(50) CHECK (service_type IN ('Post', 'Tool', 'Assignment')),
    status NVARCHAR(50) CHECK (status IN ('completed', 'pending', 'failed'))
);

-- Creating the Assignments table (optional)
CREATE TABLE Assignments (
    assignment_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    title NVARCHAR(255) NOT NULL,
    description TEXT,
    deadline DATE,
    status NVARCHAR(50) CHECK (status IN ('open', 'in-progress', 'completed'))
);

-- Creating the Courses table (optional)
CREATE TABLE Courses (
    course_id INT PRIMARY KEY IDENTITY(1,1),
	user_id INT FOREIGN KEY REFERENCES Users(user_id),
    course_title NVARCHAR(255) NOT NULL,
    course_description TEXT,
	coursera_email NVARCHAR(255) NOT NULL,
	coursera_password NVARCHAR(255) NOT NULL,
    start_date DATE,
    end_date DATE
);

-- Creating the Parent Comment table
CREATE TABLE Parent_Comment (
    parent_comment_id INT PRIMARY KEY IDENTITY(1,1),
    post_id INT FOREIGN KEY REFERENCES Posts(post_id),
	user_id INT FOREIGN KEY REFERENCES Users(user_id),
	content TEXT NOT NULL,
    comment_date DATETIME DEFAULT GETDATE()
);

-- Creating the Comments table
CREATE TABLE Comments (
    comment_id INT PRIMARY KEY IDENTITY(1,1),
    parent_comment_id INT NULL FOREIGN KEY REFERENCES Parent_Comment(parent_comment_id),
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    content TEXT NOT NULL,
    comment_date DATETIME DEFAULT GETDATE()
);