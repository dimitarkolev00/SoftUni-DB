CREATE DATABASE Bitbucket
USE Bitbucket

--Problem 1 DDL
CREATE TABLE Users(
Id INT PRIMARY KEY IDENTITY,
Username NVARCHAR(30) NOT NULL,
[Password] NVARCHAR (30) NOT NULL,
Email NVARCHAR(30) NOT NULL
)

CREATE TABLE Repositories(
Id INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE RepositoriesContributors(
RepositoryId INT NOT NULL FOREIGN KEY REFERENCES Repositories(Id),
ContributorId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
PRIMARY KEY (RepositoryId,ContributorId)
)

CREATE TABLE Issues(
Id INT PRIMARY KEY IDENTITY,
Title NVARCHAR(255) NOT NULL,
IssueStatus NVARCHAR(6) NOT NULL,
RepositoryId INT NOT NULL FOREIGN KEY REFERENCES Repositories(Id),
AssigneeId INT NOT NULL FOREIGN KEY REFERENCES Users(Id)
)

CREATE TABLE Commits(
Id INT PRIMARY KEY IDENTITY,
[Message] NVARCHAR(50) NOT NULL,
IssueId INT FOREIGN KEY REFERENCES Issues(Id),
RepositoryId INT NOT NULL FOREIGN KEY REFERENCES Repositories(Id),
ContributorId INT NOT NULL FOREIGN KEY REFERENCES Users(Id)
)

CREATE TABLE Files(
Id INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(100) NOT NULL,
Size DECIMAL(10,2),
ParentId INT FOREIGN KEY REFERENCES Files(Id),
CommitId INT NOT NULL FOREIGN KEY REFERENCES Commits(Id)
)

--Problem 2
INSERT INTO Files([Name],Size,ParentId,CommitId)
       VALUES ('Trade.idk',2598.0,	1,1),
	          ('menu.net',9238.31,2,2),
			  ('Administrate.soshy',1246.93,3,3),
			  ('Controller.php',7353.15,4,4),
			  ('Find.java',	9957.86	,5,	5),
			  ('Controller.json',14034.87,3,6),
			  ('Operate.xix',7662.92,7,7)

INSERT INTO Issues(Title,IssueStatus,RepositoryId,AssigneeId)
       VALUES('Critical Problem with HomeController.cs file','open',1,4),
	         ('Typo fix in Judge.html',	'open',	4,	3),
			 ('Implement documentation for UsersService.cs','closed',8,2),
			 ('Unreachable code in Index.cs','open',9,8)

--Problem 3
UPDATE Issues
SET IssueStatus = 'closed'
WHERE AssigneeId = 6

--Problem 4
DELETE FROM RepositoriesContributors
WHERE RepositoryId IN (SELECT Id FROM Repositories
                       WHERE [Name] = 'Softuni-Teamwork')

DELETE FROM Issues
WHERE RepositoryId IN (SELECT Id FROM Repositories
                       WHERE [Name] = 'Softuni-Teamwork')

--Problem 5
SELECT Id,[Message],RepositoryId,ContributorId FROM Commits
ORDER BY Id ASC,
         [Message] ASC,
		 RepositoryId ASC,
		 ContributorId ASC

--Problem 6
SELECT Id,[Name],Size FROM Files
WHERE Size>1000 AND [Name]  LIKE '%html'
ORDER BY Size DESC, 
         Id ASC,
		 [Name] ASC

--Problem 7
SELECT i.Id,CONCAT(u.Username,' : ',i.Title) AS [IssueAssignee] FROM Issues AS i
JOIN Users AS u ON i.AssigneeId= u.Id
ORDER BY i.Id DESC, 
         [IssueAssignee] ASC

--Problem 8
WITH CTE_NewTable(Id, Name, Size) As
(
	SELECT f.Id, f.Name, f.Size
	FROM Files As f
	WHERE Id IN (SELECT ParentId FROM Files)
)
SELECT Id, Name, CAST(Size AS VARCHAR(20)) + 'KB' As Size FROM Files
WHERE Id NOT IN (SELECT Id FROM CTE_NewTable)

-- Problem 9 
SELECT TOP(5) r.Id,r.[Name],COUNT(c.ContributorId) AS [Commits] FROM Repositories AS r
JOIN RepositoriesContributors AS rc ON r.Id= rc.RepositoryId
JOIN Commits AS c ON r.Id = c.RepositoryId
GROUP BY r.Id,r.[Name]
ORDER BY [Commits] DESC,
         r.Id ASC,
		 r.[Name] ASC

--Problem 10
SELECT Username, AVG(Size) AS [Size] FROM Users AS u
JOIN Commits AS c ON u.Id=c.ContributorId
JOIN Files AS f ON c.Id = f.CommitId
GROUP BY u.Username
ORDER BY [Size] DESC,
         Username ASC

--Problem 11
GO
CREATE FUNCTION udf_UserTotalCommits(@username NVARCHAR(30)) 
RETURNS INT
AS
BEGIN
DECLARE @ret INT =
    (
	SELECT COUNT(ContributorId) FROM Commits AS c
	JOIN Users AS u ON u.Id = c.ContributorId
	WHERE u.Username = @username
	)
RETURN @ret
END
GO
SELECT dbo.udf_UserTotalCommits('UnderSinduxrein')

--Problem 12
GO
CREATE PROC usp_FindByExtension(@extension NVARCHAR(20))
AS
BEGIN
    SELECT Id,[Name],CONCAT(Size,'KB') AS [Size] FROM Files
	WHERE [Name] LIKE '%'+@extension+'%'
END
GO
EXEC usp_FindByExtension 'txt'
