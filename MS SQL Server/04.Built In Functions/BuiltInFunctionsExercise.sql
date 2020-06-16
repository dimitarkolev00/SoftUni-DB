USE SoftUni
--Problem 1
SELECT FirstName,LastName FROM Employees
WHERE FirstName LIKE 'Sa%'
--Probem 2
SELECT FirstName,LastName FROM Employees
WHERE LastName LIKE '%ei%'
--Problem 3
SELECT FirstName FROM Employees
WHERE DepartmentID IN (3,10) AND DATEPART(YEAR,HireDate) BETWEEN 1995 AND 2005
--Problem 4
SELECT FirstName,LastName FROM Employees
WHERE JobTitle NOT LIKE '%engineer%'
--Problem 5
SELECT [Name] FROM Towns
WHERE LEN([Name]) IN (5,6)  
ORDER BY [Name]
--Problem 6 
SELECT * FROM Towns
WHERE SUBSTRING([Name],1,1) IN ('M','K','B','E')
ORDER BY [Name] ASC
--Problem 7 
SELECT * FROM Towns
WHERE [Name] LIKE '[^RBD]%'
ORDER BY [Name] ASC
--Problem 8
CREATE VIEW V_EmployeesHiredAfter2000 AS
SELECT FirstName,LastName FROM Employees
WHERE YEAR(HireDate)>2000 
--Problem 9
SELECT FirstName,LastName FROM Employees
WHERE LEN(LastName) = 5
--Problem 10
SELECT EmployeeID,FirstName,LastName,Salary,
            DENSE_RANK() OVER(PARTITION BY Salary ORDER BY Salary DESC) AS [Rank] 
  FROM Employees
WHERE Salary BETWEEN 10000 AND 50000
ORDER BY Salary DESC
--Problem 11
SELECT * FROM (SELECT EmployeeID,FirstName,LastName,Salary,
            DENSE_RANK() OVER(PARTITION BY Salary ORDER BY EmployeeID) AS [Rank] 
  FROM Employees
WHERE Salary BETWEEN 10000 AND 50000) AS [Rank Table]
WHERE [Rank] = 2
ORDER BY Salary DESC
--Problem 12
USE Geography
SELECT CountryName,IsoCode FROM Countries
WHERE CountryName LIKE '%a%a%a%'
ORDER BY IsoCode
--Problem 13
SELECT PeakName, RiverName, LOWER (CONCAT(p.PeakName,SUBSTRING(r.RiverName,2,LEN(r.RiverName)-1))) AS [Mix]
   FROM Peaks AS p, Rivers AS r
WHERE RIGHT(p.PeakName,1) = LEFT(r.RiverName,1)
ORDER BY [Mix]
--Problem 14
SELECT TOP(50) [Name],FORMAT([Start],'yyyy-MM-dd') AS [Start]
FROM Games AS g
WHERE DATEPART(YEAR,[Start]) IN (2011,2012)
ORDER BY g.[Start],[Name] 
--Problem 15
SELECT Username, SUBSTRING(Email, CHARINDEX('@',Email,1)+1 ,LEN(Email)) AS [Email Provider]
FROM Users
ORDER BY [Email Provider], Username
--Problem 16
SELECT Username, IpAddress FROM Users
WHERE IpAddress LIKE '___.1_%._%.___'
ORDER BY Username
--Problem 17
SELECT [Name],
CASE
   WHEN DATEPART(HOUR,[Start]) BETWEEN 0AND 11 THEN 'Morning'
   WHEN DATEPART(HOUR,[Start]) BETWEEN 12AND 17 THEN 'Afternoon'
   Else 'Evening'
END AS [Part of the Day],
CASE
   WHEN Duration <=3 THEN 'Extra Short'
   WHEN Duration BETWEEN 4 AND 6 THEN 'Short'
   WHEN Duration >6 THEN 'Long'
   Else 'Extra Long'
END AS [Duration]
FROM Games
ORDER BY [Name],[Duration],[Part of the Day]
--Problem 18


