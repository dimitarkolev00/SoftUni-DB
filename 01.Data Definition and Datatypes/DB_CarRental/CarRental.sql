CREATE DATABASE CarRental

USE CarRental

CREATE TABLE Categories(
  Id INT PRIMARY KEY IDENTITY,
  CategoryName NVARCHAR(50) NOT NULL,
  DailyRate DECIMAL(3,1) NOT NULL,
  WeeklyRate DECIMAL(3,1) NOT NULL,
  MonthlyRate DECIMAL(3,1) NOT NULL,
  WeekendRate DECIMAL(3,1) NOT NULL
)

CREATE TABLE Cars(
 Id INT PRIMARY KEY IDENTITY,
 PlateNumber INT NOT NULL,
 Manufacturer NVARCHAR(50) NOT NULL,
 Model NVARCHAR(50) NOT NULL,
 CarYear INT NOT NULL,
 CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
 Doors INT ,
 Picture VARBINARY(MAX),
 Condition NVARCHAR(50),
 Avaliable BIT
)

CREATE TABLE Employees(
 Id INT PRIMARY KEY IDENTITY,
 FirstName NVARCHAR(30) NOT NULL,
 LastName NVARCHAR(30) NOT NULL,
 Title NVARCHAR(30),
 Notes NVARCHAR(30)
)

CREATE TABLE Customers(
 Id INT PRIMARY KEY IDENTITY,
 DriverLicenceNumber INT NOT NULL,
 FullName NVARCHAR(100) NOT NULL,
 Adress NVARCHAR(150),
 City NVARCHAR(50) NOT NULL,
 ZipCode INT NOT NULL,
 Notes NVARCHAR(MAX)
)

CREATE TABLE RentalOrders(
 Id INT PRIMARY KEY IDENTITY,
 EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
 CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
 CarId INT FOREIGN KEY REFERENCES Cars(Id),
 TankLevel INT NOT NULL,
 KilometrageStart DECIMAL(15, 3) NOT NULL,
 KilometrageEnd DECIMAL(15, 3) NOT NULL,
 TotalKilometrage DECIMAL(15, 3) NOT NULL,
 StartDate DATE NOT NULL,
 EndDate DATE NOT NULL,
 TotalDays INT NOT NULL,
 RateApplied DECIMAL(15, 3),
 TaxRate DECIMAL(15, 3),
 OrderStatus NVARCHAR(30),
 Notes NVARCHAR(MAX)
)

INSERT INTO Categories(CategoryName, DailyRate, WeeklyRate, MonthlyRate ,WeekendRate)
      VALUES 
	       ('Bus',3.2,3.6,5.6,3.4),
		   ('Van',3.2,3.6,5.6,3.4),
		   ('Car',3.2,3.6,5.6,3.4)

INSERT INTO Cars(PlateNumber, Manufacturer, Model, CarYear, CategoryId, Doors, Picture, Condition, Avaliable)
      VALUES 
	        (2342,'Audi','A6',2017,2,4,NULL,NULL,NULL),
			(2342,'Audi','A6',2017,2,4,NULL,NULL,NULL),
			(2342,'Audi','A6',2017,2,4,NULL,NULL,NULL)

INSERT INTO Employees(FirstName, LastName, Title, Notes)
      VALUES 
	        ('Asen', 'Asenov', NULL,NULL),
			('Atans', 'Kolev', NULL,NULL),
			('Qsen', 'Asenov', NULL,NULL)

INSERT INTO Customers(DriverLicenceNumber, FullName, Adress, City, ZipCode, Notes)
       VALUES 
	        (689, 'Dimitar Kolev', NULL, 'Sofia', 1000, NULL),
			(689, 'Dimitar Kolev', NULL, 'Sofia', 1000, NULL),
			(689, 'Dimitar Kolev', NULL, 'Sofia', 1000, NULL)

INSERT INTO RentalOrders(EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart,
                        KilometrageEnd, TotalKilometrage, 
                        StartDate, EndDate, TotalDays, RateApplied, TaxRate, OrderStatus, Notes)
       VALUES 
	        (1,2,1,52,19999, 21000, 1001,'2020-05-09','2020-05-21', 12, NULL,NULL,NULL,NULL),
			 (1,2,1,52,19999, 21000, 1001,'2020-05-09','2020-05-21', 12, NULL,NULL,NULL,NULL),
			  (1,2,1,52,19999, 21000, 1001,'2020-05-09','2020-05-21', 12, NULL,NULL,NULL,NULL)

