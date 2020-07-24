using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CarDealer.Data;
using CarDealer.Dto.Export;
using CarDealer.Dto.Import;
using CarDealer.Models;
using CarDealer.XMLHelper;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            //string inputXml = File.ReadAllText("../../../Datasets/sales.xml");

            string result = GetSalesWithAppliedDiscount(context);
            File.WriteAllText("../../../results/sales-discounts.xml", result);

            //Console.WriteLine(result);

            //ResetDatabase(context);
        }

        //Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(c => new ExportSalesWithDiscount
                {
                    Car = new ExportCarDto
                    {
                        Make = c.Car.Make,
                        Model = c.Car.Model,
                        TravelledDistance = c.Car.TravelledDistance
                    },

                    CustomerName = c.Customer.Name,
                    Discount = c.Discount,
                    Price = c.Car.PartCars.Sum(cp => cp.Part.Price),
                    PriceWithDiscount = c.Car.PartCars.Sum(cp => cp.Part.Price) -
                                         c.Car.PartCars.Sum(p => p.Part.Price) * c.Discount / 100

                })
                .ToArray();

            string result = XmlConverter.Serialize(sales, "sales");
            return result;

        }

        //Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new ExportCustomerWithSalesDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.SelectMany(s => s.Car.PartCars).Sum(cp => cp.Part.Price)
                })
                .OrderByDescending(t => t.SpentMoney)
                .ToArray();

            var result = XmlConverter.Serialize(customers, "customers");
            return result;
        }

        //problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new ExportCarsWithPartsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(cr => new CarPartsDto
                    {
                        Name = cr.Part.Name,
                        Price = cr.Part.Price
                    })
                        .OrderByDescending(cp => cp.Price)
                        .ToArray()
                })
                .OrderByDescending(t => t.TravelledDistance)
                .ThenBy(m => m.Model)
                .Take(5)
                .ToArray();

            var result = XmlConverter.Serialize(cars, "cars");
            return result;
        }

        //Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportLocalSuppliersDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            var result = XmlConverter.Serialize(suppliers, "suppliers");

            return result;
        }
        //problem 15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new ExportBMWCarsDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();

            var result = XmlConverter.Serialize(cars, "cars");
            return result;
        }


        //Problem 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new ExportCarWithDistanceDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            var carsDto = XmlConverter.Serialize(cars, "cars");

            return carsDto;
        }


        //Problem 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var salesDtos = XmlConverter.Deserializer<ImportSalesDto>
                (inputXml, "Sales");

            var sales = salesDtos
                .Where(i => context.Cars.Any(c => c.Id == i.CarId))
                .Select(s => new Sale
                {
                    CarId = s.CarId,
                    CustomerId = s.CustomerId,
                    Discount = s.Discount
                })
                .ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }


        //Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var customersDtos = XmlConverter.Deserializer<ImportCustomersDto>
                (inputXml, "Customers");

            var customers = customersDtos
                .Select(c => new Customer
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        //Problem 11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var carsDtos = XmlConverter.Deserializer<ImportCarsDto>
                (inputXml, "Cars");

            List<Car> cars = new List<Car>();

            foreach (var carDto in carsDtos)
            {
                var uniqueParts = carDto.Parts.Select(s => s.Id).Distinct().ToArray();
                var realParts = uniqueParts.Where(id => context.Parts.Any(i => i.Id == id));


                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance,
                    PartCars = realParts.Select(p => new PartCar
                    {
                        PartId = p
                    })
                        .ToArray()
                };

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }


        //Problem 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {


            var xmlResult = XmlConverter.Deserializer<ImportPartsDto>
                (inputXml, "Parts");


            var result = xmlResult.Where(i => context.Suppliers.Any(s => s.Id == i.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                })
                .ToArray();

            context.Parts.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Length}";
        }


        //Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            const string rootElement = "Suppliers";

            var xmlResult = XmlConverter.Deserializer<ImportSuppliersDto>
                (inputXml, rootElement);

            var result = xmlResult
                .Select(x => new Supplier
                {
                    Name = x.Name,
                    IsImporter = x.IsImporter
                })
                .ToArray();

            context.Suppliers.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Length}";
        }


        public static void ResetDatabase(CarDealerContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was successfully deleted!");

            context.Database.EnsureCreated();
            Console.WriteLine("Database was successfully created!");
        }
    }
}