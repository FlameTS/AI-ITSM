CREATE TABLE Roles
(
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL
);

CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,

    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

CREATE TABLE Categories
(
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Incidents
(
    IncidentId INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Description VARCHAR(MAX) NOT NULL,
    CategoryId INT NOT NULL,
    Priority VARCHAR(50) NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Open',
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ResolvedAt DATETIME NULL,

    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);

CREATE TABLE IncidentAssignments
(
    AssignmentId INT IDENTITY(1,1) PRIMARY KEY,
    IncidentId INT NOT NULL,
    AssignedTo INT NOT NULL,
    AssignedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (IncidentId) REFERENCES Incidents(IncidentId),
    FOREIGN KEY (AssignedTo) REFERENCES Users(UserId)
);

CREATE TABLE IncidentComments
(
    CommentId INT IDENTITY(1,1) PRIMARY KEY,
    IncidentId INT NOT NULL,
    UserId INT NOT NULL,
    CommentText VARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (IncidentId) REFERENCES Incidents(IncidentId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE AIAnalysis
(
    AIAnalysisId INT IDENTITY(1,1) PRIMARY KEY,
    IncidentId INT NOT NULL,
    SuggestedCategory VARCHAR(100),
    SuggestedPriority VARCHAR(50),
    SuggestedResolution VARCHAR(MAX),
    RelatedIncidentId INT NULL,
    ConfidenceScore DECIMAL(5,2),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (IncidentId) REFERENCES Incidents(IncidentId),
    FOREIGN KEY (RelatedIncidentId) REFERENCES Incidents(IncidentId)
);

CREATE TABLE Notifications
(
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    IncidentId INT NULL,
    Message VARCHAR(500) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (IncidentId) REFERENCES Incidents(IncidentId)
);

CREATE TABLE Escalations
(
    EscalationId INT IDENTITY(1,1) PRIMARY KEY,
    IncidentId INT NOT NULL,
    EscalatedBy INT NULL,
    EscalatedTo INT NULL,
    Reason VARCHAR(500) NOT NULL,
    EscalatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ResolvedAt DATETIME NULL,

    FOREIGN KEY (IncidentId) REFERENCES Incidents(IncidentId),
    FOREIGN KEY (EscalatedBy) REFERENCES Users(UserId),
    FOREIGN KEY (EscalatedTo) REFERENCES Users(UserId)
);
