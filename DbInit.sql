CREATE SCHEMA N;
GO
CREATE SCHEMA Audit;
GO

CREATE TABLE Audit.AuditType (
	Code varchar(20) CONSTRAINT PK_AuditType PRIMARY KEY,
	Name nvarchar(100) NOT NULL
);

INSERT INTO Audit.AuditType (Code, Name) VALUES
('Read', N'Четене'),
('Write', N'Запис'),
('LoginSuccess', N'Успешен вход'),
('LoginError', N'Грешка при вход'),
('RegiXRequest', N'Заявка към RegiX'),
('RegiXResponse', N'Отговор от RegiX'),
('RegiXError', N'Грешка от RegiX');

CREATE TABLE Audit.Audit (
	Id int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Audit PRIMARY KEY,
	UserId nvarchar(128) NULL CONSTRAINT FK_Audit_AspNetUsers FOREIGN KEY REFERENCES AspNetUsers (Id),
	DateTime datetime2(7) NOT NULL,
	IpAddress nvarchar(100),
	Url nvarchar(1000),
	Data nvarchar(max),
	Notes nvarchar(max),
	Duration bigint,
	UserName nvarchar(256),
	Controller nvarchar(1000),
	Action nvarchar(1000),
	SessionId nvarchar(1000),
	RequestMethod nvarchar(100),
	Hash varbinary(128),
	AuditTypeCode varchar(20) NOT NULL CONSTRAINT FK_Audit_AuditType FOREIGN KEY REFERENCES N.AuditType (Code),
	EntityName varchar(100),
	EntityRecordId nvarchar(100)
);

CREATE INDEX IX_Audit_DateTime ON Audit (DateTime);
CREATE INDEX IX_Audit_AuditTypeCode ON Audit (AuditTypeCode);

CREATE TABLE Audit.AuditDetail (
	Id int IDENTITY(1, 1) NOT NULL CONSTRAINT PK_AuditDetail PRIMARY KEY NONCLUSTERED,
	AuditId int NOT NULL CONSTRAINT FK_AuditDetail_Audit FOREIGN KEY (AuditId) REFERENCES Audit (Id) ON DELETE CASCADE,
	AuditDetailType char(1) NOT NULL, -- enum AuditLogDetailType (C, U, D)
	EntityName varchar(100) NOT NULL,
	RecordId nvarchar(100) NOT NULL,
	PropertyName varchar(100) NOT NULL,
	OriginalValue nvarchar(max),
	NewValue nvarchar(max),
	OriginalValueDescription nvarchar(max),
	NewValueDescription nvarchar(max)
);

CREATE TABLE Audit.AuditConfig (
	EntityName varchar(100) NOT NULL,
	PropertyName varchar(100) NOT NULL, -- Слагам string.Empty, ако преводът е име на таблица, а не на колона - за да има индекс
	Translation nvarchar(100),
	Mapping varchar(100),
	CONSTRAINT PK_AuditConfig PRIMARY KEY (EntityName, PropertyName)
);