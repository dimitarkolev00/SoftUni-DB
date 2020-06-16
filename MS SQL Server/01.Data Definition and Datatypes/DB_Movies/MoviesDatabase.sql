CREATE DATABASE Movies

USE Movies

CREATE TABLE Directors(
Id INT PRIMARY KEY IDENTITY,
DirectiorName NVARCHAR(50) NOT NULL,
Notes NVARCHAR(MAX) 
)

CREATE TABLE Genres(
Id INT PRIMARY KEY IDENTITY,
GenreName NVARCHAR(50) NOT NULL,
Notes NVARCHAR(MAX) 
)

CREATE TABLE Categories(
Id INT PRIMARY KEY IDENTITY,
CategoryName NVARCHAR(50) NOT NULL,
Notes NVARCHAR(MAX)
)

CREATE TABLE Movies(
Id INT PRIMARY KEY IDENTITY,
Title NVARCHAR(100) NOT NULL,
DirectorId INT
FOREIGN KEY REFERENCES Directors(Id)NOT NULL,
CopyrightYear DATETIME2 NOT NULL,
[Length] TIME NOT NULL,
GenreId INT FOREIGN KEY REFERENCES Genres(Id) NOT NULL,
CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
Rating DECIMAL(3,1),
Notes NVARCHAR(MAX)
)

INSERT INTO Directors(DirectiorName,Notes)
      VALUES
	     ('Pesho', NULL),
		 ('Gosho', Null),
		 ('Pesho', NULL),
		 ('Ivo',Null),
		('Nasko', NULL)

INSERT INTO Genres(GenreName,Notes)
      VALUES 
	        ('POP', NULL),
			('FOLK',NULL),
			('JAZZ', NULL),
			('JAZZ', NULL),
			('JAZZ', NULL)

INSERT INTO Categories(CategoryName,Notes)
      VALUES 
	        ('Thriller', NULL),
			('Documentary',NULL),
			('Horror', NULL),
			('Horror', NULL),
			('Horror', NULL)

INSERT INTO Movies(Title, DirectorId,CopyrightYear,[Length],GenreId,CategoryId,Rating,Notes)
      VALUES 
	  ('Title1',1,'2020-05-20','02:33:08',2,3,'3.9',NULL),
	  ('Title1',1,'2020-05-20','02:33:08',2,3,'3.9',NULL),
	  ('Title1',1,'2020-05-20','02:33:08',2,3,'3.9',NULL),
	  ('Title1',1,'2020-05-20','02:33:08',2,3,'3.9',NULL),
	  ('Title1',1,'2020-05-20','02:33:08',2,3,'3.9',NULL)



