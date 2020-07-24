using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XMLHelper;


namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //var fileXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            //var result = ImportCategoryProducts(context, fileXml);

            //Console.WriteLine(result);

            //RestartDatabase(context);

            var result = GetUsersWithProducts(context);
            File.WriteAllText("../../../results/users-and-products.xml", result);

        }

        //Problem 08
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersAndProducts = context.Users
                .ToArray()
                .Where(p => p.ProductsSold.Any())
                .Select(u => new ExportUserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportProductCountDto
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                            .Select(p => new ExportProductDto
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .OrderByDescending(p=>p.Price)
                            .ToArray()
                    }
                })
                .OrderByDescending(x=>x.SoldProducts.Count)
                .Take(10)
                .ToArray();

            var resultDto = new ExportUserCountDto
            {
                Count = context.Users.Count(p=>p.ProductsSold.Any()),
                Users = usersAndProducts
            };

            var result = XmlConverter.Serialize(resultDto, "Users");

            return result;
        }

        //Problem 07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            const string rootElement = "Categories";

            var categoriesDtos = context.Categories
                .Select(p => new ExportCategoriesByProductsCountDto
                {
                    Name = p.Name,
                    Count = p.CategoryProducts.Count,
                    AveragePrice = p.CategoryProducts.Average(c => c.Product.Price),
                    TotalRevenue = p.CategoryProducts.Sum(c => c.Product.Price)
                })
                .OrderByDescending(n => n.Count)
                .ThenBy(n => n.TotalRevenue)
                .ToArray();

            var result = XmlConverter.Serialize(categoriesDtos, rootElement);

            return result;

        }

        //Problem 06
        public static string GetSoldProducts(ProductShopContext context)
        {
            const string rootElement = "Users";

            var users = context.Users
                .Where(u => u.ProductsSold.Any())
                .Select(x => new ExportUserSoldProductsDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                        .Select(p => new UserProductDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        }).ToArray()
                })
                .OrderBy(l => l.LastName)
                .ThenBy(f => f.FirstName)
                .Take(5)
                .ToArray();

            string result = XmlConverter.Serialize(users, rootElement);

            return result;
        }

        //Problem 05
        public static string GetProductsInRange(ProductShopContext context)
        {
            const string rootElement = "Products";

            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ExportProductInfoDto()
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToList();

            var result = XmlConverter.Serialize(products, rootElement);
            return result;
        }

        //Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            const string rootElement = "CategoryProducts";

            var categoriesResult = XmlConverter.Deserializer<ImportCategoryProductsDto>
                (inputXml, rootElement);


            var categories = categoriesResult
                .Where(i => context.Categories.Any(s => s.Id == i.CategoryId)
                          && context.Products.Any(s => s.Id == i.ProductId))
                .Select(cr => new CategoryProduct
                {
                    CategoryId = cr.CategoryId,
                    ProductId = cr.ProductId
                })
                .ToArray();

            context.CategoryProducts.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";

        }

        //problem 03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Categories";

            var categoriesResult = XmlConverter.Deserializer
                <ImportCategoriesDto>(inputXml, rootElement);

            List<Category> categories = new List<Category>();

            foreach (var categoryDto in categoriesResult)
            {
                if (categoryDto.Name == null)
                {
                    continue;
                }

                var category = new Category
                {
                    Name = categoryDto.Name
                };

                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //Problem 02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Products";

            var productsResult = XmlConverter.Deserializer<ImportProductsDto>
                (inputXml, rootElement);


            var products = productsResult.Select(p => new Product
            {
                Name = p.Name,
                Price = p.Price,
                SellerId = p.SellerId,
                BuyerId = p.BuyerId
            })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //Problem 01
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Users";

            var usersResult = XmlConverter.Deserializer<ImportUserDto>(inputXml, rootElement);

            List<User> users = new List<User>();

            foreach (var importUserDto in usersResult)
            {
                User user = new User
                {
                    FirstName = importUserDto.FirstName,
                    LastName = importUserDto.LastName,
                    Age = importUserDto.Age
                };
                users.Add(user);
            }
            context.Users.AddRange(users);
            context.SaveChanges();


            return $"Successfully imported {users.Count}";
        }

        public static void RestartDatabase(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database was successfully deleted!");

            context.Database.EnsureCreated();
            Console.WriteLine("Database was successfully created!");
        }
    }
}