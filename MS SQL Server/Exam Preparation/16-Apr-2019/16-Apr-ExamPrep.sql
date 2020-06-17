CREATE DATABASE Airport
USE Airport

--Problem 1 DDL
CREATE TABLE Planes(
Id INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(30) NOT NULL,
Seats INT NOT NULL,
[Range] INT NOT NULL
)

CREATE TABLE Flights(
Id INT PRIMARY KEY IDENTITY,
DepartureTime DATETIME2,
ArrivalTime DATETIME2,
Origin NVARCHAR(50) NOT NULL,
Destination NVARCHAR(50) NOT NULL,
PlaneId INT NOT NULL FOREIGN KEY REFERENCES Planes(Id)
)

CREATE TABLE Passengers(
Id INT PRIMARY KEY IDENTITY,
FirstName NVARCHAR(30) NOT NULL,
LastName NVARCHAR(30) NOT NULL,
Age INT NOT NULL,
[Address] NVARCHAR(30) NOT NULL,
PassportId NVARCHAR(11) NOT NULL
)

CREATE TABLE LuggageTypes(
Id INT PRIMARY KEY IDENTITY,
[Type] NVARCHAR(30) NOT NULL
)

CREATE TABLE Luggages(
Id INT PRIMARY KEY IDENTITY,
LuggageTypeId INT NOT NULL FOREIGN KEY REFERENCES LuggageTypes(Id),
PassengerId INT NOT NULL FOREIGN KEY REFERENCES Passengers(Id)
)

CREATE TABLE Tickets(
Id INT PRIMARY KEY IDENTITY,
PassengerId INT NOT NULL FOREIGN KEY REFERENCES Passengers(Id),
FlightId INT NOT NULL FOREIGN KEY REFERENCES Flights(Id),
LuggageId INT NOT NULL FOREIGN KEY REFERENCES Luggages(Id),
Price DECIMAL(10,2) NOT NULL
)

--Problem 2 
INSERT INTO Planes([Name],Seats,[Range])
       VALUES ('Airbus 336',112,5132),
	          ('Airbus 330',432,5325),
			  ('Boeing 369',231,2355),
			  ('Stelt 297',	254,2143),
			  ('Boeing 338',165,5111),
			  ('Airbus 558',387,1342),
			  ('Boeing 128',345,5541)

INSERT INTO LuggageTypes([Type])
       VALUES ('Crossbody Bag'),
	          ('School Backpack'),
			  ('Shoulder Bag')
--Problem 3
UPDATE Tickets
SET Price = Price * 1.13
WHERE FlightId IN (SELECT Id FROM Flights
                   WHERE Destination = 'Carlsbad')

--Problem 4
DELETE FROM Tickets
WHERE FlightId IN (SELECT Id FROM Flights
                   WHERE Destination = 'Ayn Halagim')

DELETE FROM Flights
WHERE Destination= 'Ayn Halagim'

--Problem 5
SELECT * FROM Planes
WHERE [Name] LIKE '%tr%'

--Problem 6
SELECT FlightId,SUM(Price) AS [Price] FROM Tickets
GROUP BY FlightId
ORDER BY SUM(Price) DESC, FlightId

--Problem 7
SELECT CONCAT(FirstName,' ',LastName) AS [Full Name],
       f.Origin,
	   f.Destination
FROM Passengers AS p
JOIN Tickets AS t ON t.PassengerId=p.Id
JOIN Flights AS f ON t.FlightId=f.Id
ORDER BY [Full Name],f.Origin,f.Destination

--Problem 8
SELECT FirstName,LastName,Age FROM Passengers AS p
FULL JOIN Tickets AS t ON t.PassengerId=p.Id
WHERE t.Id IS NULL
ORDER BY Age DESC,
         FirstName,
		 LastName

--Problem 9
SELECT CONCAT(FirstName,' ',LastName) AS [Full Name],
       pl.[Name] AS [Plane Name],
	   CONCAT(Origin,' - ',Destination) AS [Trip],
	   lt.[Type] AS [Luggage Type]
FROM Passengers AS p
JOIN Tickets AS t ON p.Id=t.PassengerId
JOIN Flights AS f ON t.FlightId=f.Id
JOIN Planes AS pl ON f.PlaneId=pl.Id
JOIN Luggages AS l ON t.LuggageId=l.Id
JOIN LuggageTypes AS lt ON l.LuggageTypeId = lt.Id
WHERE t.Id IS NOT NULL
ORDER BY [Full Name],
         [Name],
		 Origin,
		 Destination,
		 [Luggage Type]

--Problem 10
SELECT p.Name,p.Seats,COUNT(t.PassengerId) AS [Passengers Count] FROM Planes AS p
LEFT JOIN Flights AS f ON p.Id=f.PlaneId
LEFT JOIN Tickets AS t ON f.Id=t.FlightId
GROUP BY P.[Name],p.Seats
ORDER BY [Passengers Count] DESC,
         p.Name,
		 p.Seats

--Problem 11
GO
CREATE FUNCTION udf_CalculateTickets(@origin NVARCHAR(50), @destination NVARCHAR(50), @peopleCount INT) 
RETURNS VARCHAR(70) 
AS
BEGIN
	IF(@peopleCount <= 0)
	BEGIN
	     RETURN 'Invalid people count!';
	END
	DECLARE @flightId INT= (SELECT TOP(1) Id FROM Flights
	                     WHERE @origin = Origin AND @destination = Destination)
	IF (@flightId IS NULL)
	BEGIN
	     RETURN 'Invalid flight!'
	END
	DECLARE @pricePerTicket DECIMAL(10,2) = (SELECT TOP(1) Price FROM Tickets
	                                         WHERE FlightId = @flightId)
    DECLARE @totalPrice DECIMAL(24,2) = @pricePerTicket * @peopleCount

	RETURN CONCAT('Total price ',@totalPrice)
END
GO
SELECT dbo.udf_CalculateTickets('Kolyshley','Rancabolang', 33)

--Problem 12
GO
CREATE PROC usp_CancelFlights 
AS
BEGIN
     UPDATE Flights
	 SET DepartureTime = NULL, ArrivalTime = NULL
	 WHERE DATEDIFF(SECOND,DepartureTime,ArrivalTime) < 0
END
GO