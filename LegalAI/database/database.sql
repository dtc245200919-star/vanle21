PRAGMA foreign_keys = ON;
CREATE TABLE Departments(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);
CREATE TABLE Users(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL CHECK(Role IN ('Admin','Employee')),
    DepartmentId INTEGER NOT NULL,
    FOREIGN KEY(DepartmentId) REFERENCES Departments(Id) ON UPDATE CASCADE ON DELETE RESTRICT
);
CREATE TABLE LegalRecords(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    RecordType TEXT NOT NULL,
    ResponsiblePerson TEXT,
    DepartmentId INTEGER NOT NULL,
    Content TEXT NOT NULL,
    ExpiryDate TEXT,
    Status TEXT NOT NULL,
    FOREIGN KEY(DepartmentId) REFERENCES Departments(Id) ON UPDATE CASCADE ON DELETE RESTRICT
);
CREATE TABLE AuditLogs(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    Username TEXT NOT NULL,
    Action TEXT NOT NULL,
    Entity TEXT,
    EntityId INTEGER,
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE SET NULL
);
CREATE INDEX IX_LegalRecords_DepartmentId ON LegalRecords(DepartmentId);
CREATE INDEX IX_LegalRecords_ExpiryDate ON LegalRecords(ExpiryDate);
CREATE INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
