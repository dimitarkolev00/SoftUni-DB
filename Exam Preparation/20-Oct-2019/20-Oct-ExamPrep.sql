CREATE DATABASE [Service]

CREATE TABLE [Status](
Id INT PRIMARY KEY IDENTITY,
[Label] NVARCHAR(30) NOT NULL
)

CREATE TABLE Departments(
Id INT PRIMARY KEY IDENTITY ,
[Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE Users(
Id INT PRIMARY KEY IDENTITY ,
Username NVARCHAR(30) UNIQUE NOT NULL,
[Password] NVARCHAR(50) NOT NULL,
[Name] NVARCHAR(50),
Birthdate DATETIME2,
Age INT CHECK(Age>=14 AND Age<=110),
Email NVARCHAR(50) NOT NULL
)
 CREATE TABLE Categories(
Id INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(50) NOT NULL,
DepartmentId INT NOT NULL FOREIGN KEY REFERENCES Departments(Id)
)

CREATE TABLE Employees(
Id INT PRIMARY KEY IDENTITY,
FirstName NVARCHAR(25),
LastName NVARCHAR(25),
Birthdate DATETIME2,
Age INT CHECK(Age>=18 AND Age<=110),
DepartmentId INT FOREIGN KEY REFERENCES Departments(Id)
)

CREATE TABLE Reports(
Id INT PRIMARY KEY IDENTITY,
CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(Id),
StatusId INT NOT NULL FOREIGN KEY REFERENCES [Status](Id),
OpenDate DATETIME2 NOT NULL,
CloseDate DATETIME2,
[Description] NVARCHAR(200) NOT NULL,
UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
EmployeeId INT FOREIGN KEY REFERENCES Employees(Id)
)

INSERT INTO Employees(FirstName,LastName,Birthdate,DepartmentId)
       VALUES('Marlo','O''Malley','1958-9-21',1),
		     ('Niki','Stanaghan','1969-11-26',4),
		     ('Ayrton','Senna','1960-03-21',9),
			 ('Ronnie','Peterson','1944-02-14',9),
			 ('Giovanna','Amati','1959-07-20',5)
INSERT INTO Reports(CategoryId,StatusId,OpenDate,CloseDate,[Description],UserId,EmployeeId)
       VALUES (1,1,'2017-04-13',NULL,'Stuck Road on Str.133',6,2),
	          (6,3,'2015-09-05' ,'2015-12-06' ,'Charity trail running',3,5),
			  (14,2,'2015-09-07',NULL,'Falling bricks on Str.58',5,2),
			  (4,3,'2017-07-03','2017-07-06','Cut off streetlight on Str.11',1,1)

UPDATE Reports
SET CloseDate = GETDATE() 
WHERE CloseDate IS NULL

DELETE Reports
WHERE StatusId = 4

--Problem 5
USE [Service]

SELECT [Description],FORMAT(OpenDate,'dd-MM-yyyy') FROM Reports
WHERE EmployeeId IS NULL
ORDER BY OpenDate,[Description]
--Problem 6
SELECT r.[Description],c.Name FROM Reports AS r
JOIN Categories AS c ON r.CategoryId=c.Id
ORDER BY r.[Description], c.[Name]
--Problem 7
SELECT TOP(5) [Name],COUNT(r.CategoryId) AS [ReportsNumber] FROM Reports AS r
JOIN Categories AS c ON r.CategoryId=c.Id
GROUP BY c.Id, [Name]
ORDER BY COUNT(r.CategoryId) DESC, [Name]
--Problem 8
SELECT Username,c.[Name] FROM  Users AS u
JOIN Reports AS r ON u.Id = r.UserId
JOIN Categories AS c ON r.CategoryId = c.Id
WHERE DATEPART(DAY,u.Birthdate) = DATEPART(DAY,r.OpenDate) AND DATEPART(MONTH,u.Birthdate) = DATEPART(MONTH,r.OpenDate)
ORDER BY Username,c.[Name]
--Problem 9
SELECT  CONCAT(e.FirstName, ' ', e.LastName) AS [FullName], COUNT(u.Id) AS [UserCount] FROM Employees AS e
LEFT JOIN Reports AS r ON r.EmployeeId = e.Id
LEFT JOIN Users AS u ON u.Id = r.UserId
GROUP BY e.FirstName, e.LastName
ORDER BY UserCount DESC , FullName
--Problem 10
SELECT CASE
            WHEN CONCAT(e.FirstName, ' ', e.LastName) = '' THEN 'None'
            ELSE
                CONCAT(e.FirstName, ' ', e.LastName)
            END AS [Employee],
       ISNULL(d.Name, 'None') AS [Department],
       c.Name AS [Category],
       r.Description,
       FORMAT(r.OpenDate, 'dd.MM.yyyy') AS [OpenDate],
       s.Label AS [Status],
       u.Name AS [User]
 
FROM Reports AS r
LEFT JOIN Employees AS e ON e.Id = r.EmployeeId
LEFT JOIN Departments AS d ON d.Id = e.DepartmentId
LEFT JOIN Categories AS c ON c.Id = r.CategoryId
LEFT JOIN Status AS s ON s.Id = r.StatusId
LEFT JOIN Users AS u ON u.Id = r.UserId
ORDER BY e.FirstName DESC, e.LastName DESC, d.Name, c.Name, r.Description, r.OpenDate, s.Label, u.Name
--Problem 11
 GO
 CREATE FUNCTION udf_HoursToComplete(@StartDate DATETIME, @EndDate DATETIME) 
 RETURNS INT
 AS
 BEGIN
      DECLARE @totalHours INT
	  IF(@StartDate IS NULL OR @EndDate IS NULL)
	    BEGIN
	     SET @totalHours = NULL
	    END
	  ELSE
	    BEGIN
		 SET @totalHours = DATEDIFF(HOUR,@StartDate,@EndDate)
	    END
	RETURN @totalHours
 END
 
 SELECT dbo.udf_HoursToComplete(2020-01-20,2025-01-20)
 FROM Reports
--Problem 12
GO
CREATE PROC usp_AssignEmployeeToReport(@EmployeeId INT, @ReportId INT) 
AS
BEGIN
	DECLARE @employeeDepartmentId INT = (SELECT DepartmentId 
										   FROM Employees 
										  WHERE Id = @EmployeeId)

	DECLARE @reportDepartmentId INT = (SELECT TOP(1) d.Id 
											 FROM Reports AS r 
											 JOIN Categories AS c ON r.CategoryId = c.Id 
											 JOIN Departments AS d ON c.DepartmentId = d.Id 
											WHERE r.id = @ReportId) 

	IF(@employeeDepartmentId <> @reportDepartmentId)
	BEGIN
		RAISERROR('Employee doesn''t belong to the appropriate department!', 16, 1)
		RETURN
	END

	UPDATE Reports
	SET EmployeeId = @employeeDepartmentId
	WHERE Id = @ReportId
END