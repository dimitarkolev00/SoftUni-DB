USE SoftUni
--Problem 1
SELECT TOP(5) e.EmployeeID,e.JobTitle,e.AddressID,a.AddressText FROM Employees AS e
JOIN Addresses AS a ON e.AddressID=a.AddressID
ORDER BY e.AddressID 
--Problem 2
SELECT TOP(50) e.FirstName,e.LastName,t.[Name],a.AddressText FROM Employees AS e
JOIN Addresses AS a ON e.AddressID=a.AddressID
JOIN Towns AS t ON a.TownID=t.TownID
ORDER BY e.FirstName, e.LastName
--Problem 3
SELECT EmployeeID,FirstName,LastName,d.[Name] FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID=d.DepartmentID
WHERE d.Name = 'Sales'
ORDER BY EmployeeID
--Problem 4
SELECT TOP (5) EmployeeID,FirstName,Salary,d.[Name] FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID=d.DepartmentID
WHERE Salary > 15000
ORDER BY e.DepartmentID
--Problem 5
SELECT TOP(3) e.EmployeeID,FirstName FROM Employees AS e
LEFT JOIN EmployeesProjects AS ep ON e.EmployeeID=ep.EmployeeID
WHERE ep.ProjectID IS NULL
ORDER BY e.EmployeeID
--Problem 6
SELECT FirstName,LastName,HireDate,d.[Name] AS [Deptname] FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID=d.DepartmentID
WHERE HireDate>'1999-01-01' AND d.[Name] IN ('Sales','Finance')
ORDER BY HireDate
--Problem 7
SELECT TOP(5) e.EmployeeID,e.FirstName,p.[Name] AS [ProjectName] FROM Employees AS e
JOIN EmployeesProjects AS ep ON e.EmployeeID=ep.EmployeeID
JOIN Projects AS p ON ep.ProjectID=p.ProjectID
WHERE StartDate > '2002-08-13' AND EndDate IS NULL
ORDER BY e.EmployeeID
--Problem 8 
SELECT  e.EmployeeID,e.FirstName,
 CASE   
    WHEN DATEPART(YEAR,p.StartDate) >=2005 THEN NULL
	ELSE p.[Name]
 END AS [ProjectName]
FROM Employees AS e
JOIN EmployeesProjects AS ep ON e.EmployeeID=ep.EmployeeID
JOIN Projects AS p ON ep.ProjectID=p.ProjectID
WHERE e.EmployeeID = 24
--Problem 9 
SELECT e1.EmployeeID,e1.FirstName,e1.ManagerID,e2.FirstName AS [ManagerName] FROM Employees AS e1
JOIN Employees AS e2 ON e2.EmployeeID=e1.ManagerID
WHERE e1.ManagerID IN (3,7)
ORDER BY EmployeeID
--Problem 10
SELECT TOP(50) e1.EmployeeID,
        CONCAT(e1.FirstName,' ',e1.LastName) AS [EmployeeName],
		CONCAT(e2.FirstName,' ',e2.LastName) AS [ManagerName],
		d.[Name]
  FROM Employees AS e1
LEFT JOIN Employees AS e2 ON e1.ManagerID=e2.EmployeeID
JOIN Departments AS d ON e1.DepartmentID = d.DepartmentID
ORDER BY EmployeeID
--Problem 11
SELECT TOP(1) AVG(e.Salary) AS [MinAvarageSalary] FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
GROUP BY d.DepartmentID
ORDER BY MinAvarageSalary
--Problem 12
USE Geography
SELECT c.CountryCode,MountainRange,PeakName,Elevation FROM Countries AS c 
JOIN MountainsCountries AS mc ON c.CountryCode=mc.CountryCode
JOIN Mountains AS m ON mc.MountainId=m.Id
JOIN Peaks AS p ON p.MountainId=m.Id
WHERE c.CountryCode = 'BG' AND Elevation > 2835
ORDER BY Elevation DESC
--Problem 13 
SELECT mc.CountryCode, COUNT(m.MountainRange) AS [MountainRanges] FROM Mountains AS m
JOIN MountainsCountries AS mc ON mc.MountainId = m.Id
GROUP BY mc.CountryCode
HAVING mc.CountryCode IN ('BG', 'US', 'RU')
--Problem 14 
SELECT TOP(5) CountryName,RiverName FROM Countries AS c 
LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
LEFT JOIN  Rivers AS r ON cr.RiverId=r.Id
WHERE ContinentCode = 'AF'
ORDER BY CountryName 
--Problem 15
SELECT ContinentCode,CurrencyCode,CurrencyCount AS [CurrencyUsage]  FROM( SELECT ContinentCode,
       CurrencyCode,[CurrencyCount],
       DENSE_RANK() OVER(PARTITION BY ContinentCode ORDER BY CurrencyCount DESC) AS [CurrencyRank] 
FROM (SELECT ContinentCode,
                      CurrencyCode, 
                      COUNT(*) AS [CurrencyCount]
         FROM  Countries 
         GROUP BY ContinentCode,CurrencyCode
         )AS [CurrencyCountQuery]
WHERE CurrencyCount>1) AS [CurrencyRankingQuery]
WHERE CurrencyRank=1
ORDER BY ContinentCode
--Problem 16
SELECT COUNT(*) AS [Count]
FROM
     (SELECT mc.CountryCode AS [mc_country_code]
         FROM MountainsCountries AS mc
         GROUP BY mc.CountryCode) AS d
        RIGHT JOIN Countries AS c
        ON c.CountryCode = d.mc_country_code
WHERE d.mc_country_code IS NULL
--Problem 17
SELECT TOP(5) c.CountryName, MAX(p.Elevation)AS [HighestPeakElevation], MAX(r.Length) AS [LongestRiverLength]
FROM Countries AS c
JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
JOIN Mountains AS m On mc.MountainId = m.Id
JOIN Peaks AS p on p.MountainId = m.Id
JOIN Rivers AS r ON r.Id = cr.RiverId
GROUP BY c.CountryName
ORDER BY HighestPeakElevation DESC, LongestRiverLength DESC, c.CountryName
--Problem 18
SELECT TOP(5) WITH TIES c.CountryName AS [Country], ISNULL(p.PeakName, '(no highest peak)')
    AS [Highest Peak Name],
 
       ISNULL(MAX(p.Elevation), 0)
           AS [Highest Peak Elevation], ISNULL(m.MountainRange, '(no mountain)')
 
FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
LEFT JOIN Mountains AS m on mc.MountainId = m.Id
LEFT JOIN Peaks AS p ON m.Id = p.MountainId
GROUP BY c.CountryName, p.PeakName, m.MountainRange
ORDER BY c.CountryName, p.PeakName