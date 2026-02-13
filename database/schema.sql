-- Client Contact Manager - Database Schema
-- Run against SQL Server / LocalDB

CREATE TABLE Clients (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(200)   NOT NULL,
    ClientCode  NVARCHAR(6)     NOT NULL
);

CREATE UNIQUE INDEX IX_Clients_ClientCode ON Clients (ClientCode);
CREATE INDEX IX_Clients_Name ON Clients (Name);

CREATE TABLE Contacts (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100)   NOT NULL,
    Surname     NVARCHAR(100)   NOT NULL,
    Email       NVARCHAR(254)   NOT NULL
);

CREATE UNIQUE INDEX IX_Contacts_Email ON Contacts (Email);
CREATE INDEX IX_Contacts_FullName ON Contacts (Surname, Name);

CREATE TABLE ClientContacts (
    ClientId    INT NOT NULL,
    ContactId   INT NOT NULL,
    CONSTRAINT PK_ClientContacts PRIMARY KEY (ClientId, ContactId),
    CONSTRAINT FK_ClientContacts_Client FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ClientContacts_Contact FOREIGN KEY (ContactId) REFERENCES Contacts(Id) ON DELETE CASCADE
);

CREATE INDEX IX_ClientContacts_ContactId ON ClientContacts (ContactId);
