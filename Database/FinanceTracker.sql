/*
    Finance Tracker Database Setup
    --------------------------------
    Creates the FinanceTracker database, tables, relationships,
    constraints, indexes, and default transaction categories.
*/

CREATE DATABASE FinanceTracker;
GO

USE FinanceTracker;
GO


-- =========================================================
-- Users
-- =========================================================

CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Balance DECIMAL(10,2) NOT NULL,
    MonthlyIncome DECIMAL(10,2) NOT NULL,
    MonthlyExpenses DECIMAL(10,2) NOT NULL,

    CONSTRAINT PK_Users
        PRIMARY KEY (UserId)
);
GO


-- =========================================================
-- Categories
-- =========================================================

CREATE TABLE Categories
(
    CategoryId INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_Categories
        PRIMARY KEY (CategoryId),

    CONSTRAINT UQ_Categories_CategoryName
        UNIQUE (CategoryName)
);
GO


-- =========================================================
-- Transactions
-- =========================================================

CREATE TABLE Transactions
(
    TransactionId INT IDENTITY(1,1) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    UserId INT NOT NULL,
    CategoryId INT NOT NULL,
    TransactionDate DATE NOT NULL
        CONSTRAINT DF_Transactions_TransactionDate
        DEFAULT (CAST(GETDATE() AS DATE)),
    TransactionDescription NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_Transactions
        PRIMARY KEY (TransactionId),

    CONSTRAINT FK_Transactions_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId),

    CONSTRAINT FK_Transactions_Categories
        FOREIGN KEY (CategoryId)
        REFERENCES Categories(CategoryId),

    CONSTRAINT CHK_TransactionMin
        CHECK (Amount > 0)
);
GO


-- =========================================================
-- Indexes
-- =========================================================

CREATE INDEX IX_Transactions_UserId_TransactionDate
ON Transactions (UserId, TransactionDate);
GO


-- =========================================================
-- Default Categories
-- =========================================================

INSERT INTO Categories (CategoryName)
VALUES
    ('Food and Dining'),
    ('Housing and Living'),
    ('Health'),
    ('Personal Care or Lifestyle'),
    ('Financial or Debt'),
    ('Other');
GO