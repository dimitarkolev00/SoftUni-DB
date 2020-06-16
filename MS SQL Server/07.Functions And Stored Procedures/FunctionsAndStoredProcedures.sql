--Problem 1
GO

CREATE PROC usp_GetEmployeesSalaryAbove35000
AS 
BEGIN
     SELECT FirstName,LastName FROM Employees
	 WHERE Salary > 35000
END

EXEC usp_GetEmployeesSalaryAbove35000

--Problem 2
GO

CREATE PROC usp_GetEmployeesSalaryAboveNumber(@minSalary DECIMAL(18,4))
AS
BEGIN
     SELECT FirstName,LastName FROM Employees
	 WHERE Salary >= @minSalary
END

EXEC usp_GetEmployeesSalaryAboveNumber 48100

--Problem 3
GO

CREATE PROC usp_GetTownsStartingWith(@stringParam NVARCHAR(MAX))
AS
BEGIN
    SELECT [Name] FROM Towns
	WHERE SUBSTRING([Name],1,LEN(@stringParam)) = @stringParam
END

--Problem 4
GO

CREATE PROC usp_GetEmployeesFromTown(@town NVARCHAR(50))
AS
BEGIN
     SELECT FirstName,LastName FROM Employees AS e 
	 JOIN Addresses AS a ON e.AddressID = a.AddressID
	 JOIN Towns AS t ON a.TownID = t.TownID
	 WHERE t.[Name] = @town
END

EXEC usp_GetEmployeesFromTown Sofia

--Problem 5
GO

CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS NVARCHAR(50)
AS
BEGIN
     DECLARE @level NVARCHAR(50)
	 IF(@salary<30000)
	    SET @level = 'Low'
     ELSE IF (@salary<=50000 )
	    SET @level = 'Average'
	 ELSE 
	   SET @level = 'High'
   RETURN @level
END

SELECT Salary,dbo.ufn_GetSalaryLevel(Salary) AS [Salary Level] FROM Employees

--Problem 6
GO

 CREATE PROC usp_EmployeesBySalaryLevel(@salaryLevel VARCHAR(7))
 AS
 BEGIN
    SELECT FirstName,LastName FROM Employees
	WHERE dbo.ufn_GetSalaryLevel(Salary) = @salaryLevel
 END

 EXEC usp_EmployeesBySalaryLevel 'High'

 --Problem 7
 GO

 CREATE FUNCTION ufn_IsWordComprised(@setOfLetters NVARCHAR(MAX), @word NVARCHAR(MAX)) 
 RETURNS BIT
 AS
 BEGIN
   DECLARE @counter INT = 1;
   DECLARE @currentLetter CHAR;

   WHILE @counter <= LEN(@word)
    BEGIN
	  SET @currentLetter = SUBSTRING(@word,@counter,1)
	  DECLARE @indexInSet INT = CHARINDEX(@currentLetter,@setOfLetters)

	  IF @indexInSet <= 0
	  BEGIN
	      RETURN 0
	  END
	  SET @counter +=1
	END

    RETURN 1
 END

 --Problem 8 
 GO

 CREATE PROC usp_DeleteEmployeesFromDepartment(@departmentId INT) 
 AS
 BEGIN
     DELETE EmployeesProjects 
     WHERE EmployeeID IN (SELECT e.EmployeeID
                          FROM Employees AS e
                          WHERE e.DepartmentID = @departmentId);
 
      UPDATE Employees
      SET ManagerID = NULL   
      WHERE ManagerID IN (SELECT e.EmployeeID
                          FROM Employees AS e
                          WHERE e.DepartmentID = @departmentId);
 
        ALTER TABLE Departments
        ALTER COLUMN ManagerID INT;
 
        UPDATE Departments
        SET ManagerID = NULL
        WHERE DepartmentID = @departmentId;
 
        DELETE Employees
        WHERE DepartmentID = @departmentId;
 
        DELETE Departments
        WHERE DepartmentID = @departmentId;
 
        SELECT COUNT(*)
        FROM Employees AS e
        WHERE e.DepartmentID = @departmentId
 END

 --Problem 9
 GO
 USE Bank

 CREATE PROC usp_GetHoldersFullName 
 AS
 BEGIN
      SELECT CONCAT(FirstName,' ',LastName) FROM AccountHolders AS [Full Name]
 END

 --Problem 10
 GO

 CREATE PROC usp_GetHoldersWithBalanceHigherThan(@number DECIMAL(18,4)) 
 AS
 BEGIN
      SELECT FirstName,LastName FROM Accounts AS a
	  JOIN AccountHolders AS ah ON a.AccountHolderId= ah.Id
	  GROUP BY FirstName,LastName 
	  HAVING SUM(Balance) > @number
	  ORDER BY FirstName,LastName
 END 

 --Problem 11
 GO
CREATE FUNCTION ufn_CalculateFutureValue(@sum DECIMAL(18,4),@yir FLOAT,@yearsCount INT)
RETURNS DECIMAL(18,4)
AS
BEGIN
     DECLARE @futureValue DECIMAL (18,4);

	 SET @futureValue = @sum * (POWER((1+@yir),@yearsCount));

	 RETURN @futureValue
END

SELECT dbo.ufn_CalculateFutureValue(1000,0.1,5)

--Problem 12
GO

CREATE PROC usp_CalculateFutureValueForAccount(@acId INT,@ir FLOAT) 
AS
BEGIN
     SELECT a.Id,ah.FirstName,ah.LastName,a.Balance, dbo.ufn_CalculateFutureValue(a.Balance,@ir,5) AS [Balance in 5 years]
	 FROM Accounts AS a
	 JOIN AccountHolders AS ah ON a.Id=ah.Id
	 WHERE a.Id = @acId
END

EXEC dbo.usp_CalculateFutureValueForAccount 1,0.1

--Problem 13 
GO
CREATE FUNCTION ufn_CashInUsersGames(@gameName NVARCHAR(50))
RETURNS TABLE
AS
RETURN SELECT(SELECT SUM(Cash) AS [SumCash] FROM ( SELECT g.[Name],
                       ug.Cash,
	                   ROW_NUMBER() OVER(PARTITION BY g.[Name] ORDER BY ug.Cash DESC) AS [RowNum]
                       FROM Games AS g 
                       JOIN UsersGames AS ug ON g.Id = ug.GameId
                       WHERE g.[Name] = @gameName) AS [RowNumQuery]
              WHERE [RowNum] % 2 <> 0) AS [SumCash]
