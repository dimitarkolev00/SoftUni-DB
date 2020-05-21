CREATE DATABASE Minions

USE Minions

CREATE TABLE Minions(
  Id INT PRIMARY KEY NOT NULL,
  [Name] NVARCHAR(50) NOT NULL,
  Age TINYINT
)

CREATE TABLE Towns (
  Id INT PRIMARY KEY NOT NULL,
  [Name] NVARCHAR(50) NOT NULL,
)

ALTER TABLE Minions
ADD TownId INT FOREIGN KEY REFERENCES Towns(Id)

INSERT INTO Towns(Id,[Name])
       VALUES
	   (1,'Sofia'),
	   (2,'Plovdiv'),
	   (3,'Varna')

INSERT INTO Minions(Id,[Name],Age,TownId)
       VALUES 
	       (1,'Kevin',22,1),
		   (2,'Bob',15,3),
		   (3,'Steward',NULL,2)

TRUNCATE TABLE Minions

DROP TABLE Minions
DROP TABLE Towns

CREATE TABLE People (
  Id INT PRIMARY KEY IDENTITY,
  [Name] NVARCHAR(200) NOT NULL,
  Picture VARBINARY(MAX)
  CHECK(DATALENGTH(Picture)<= 2000*1024),
  Height DECIMAL(7,2),
  [Weight] DECIMAL(7,2),
  Gender CHAR(1) NOT NULL,
  Birthdate DATETIME2  NOT NULL,
  Biography NVARCHAR(MAX)
)

INSERT INTO People([Name],Height,[Weight],Gender,Birthdate,Biography)
      VALUES
	   ('Pesho0','1.49','59.34','m','05.30.2000','Ne znam'),
	   ('Pesho1','1.79','49.55','m','05.30.2000','Ne  dd znam'),
	   ('Pesho2','1.69','59.65','f','05.30.2000','Ne er znam'),
	   ('Pesho3','1.99','79.99','m','05.30.2000','Ne fd znam'),
	   ('Pesho4','1.39','99.23','f','05.30.2000','Ne ddd znam')

SELECT * FROM People

CREATE TABLE Users(
   Id BIGINT PRIMARY KEY IDENTITY NOT NULL,
   Username VARCHAR(30) UNIQUE NOT NULL,
   [Password] VARCHAR(26)  NOT NULL,
   ProfilePicture VARBINARY(MAX)
   CHECK(DATALENGTH(ProfilePicture)<= 900*1024),
   LastLoginTime DATETIME2 NOT NULL,
   IsDeleted BIT NOT NULL
)

INSERT INTO Users(Username,[Password],LastLoginTime,IsDeleted)
     VALUES 
	     ('Pesho0','123456','05.19.2020', 1),
		 ('Pesho1','123456','05.19.2020', 0),
		 ('Pesho2','123456','05.19.2020', 1),
		 ('Pesho3','123456','05.19.2020', 0),
		 ('Pesho4','123456','05.19.2020', 1)
SELECT * FROM Users	  