using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //int input = int.Parse(Console.ReadLine());
            //string input = Console.ReadLine();

            int result = RemoveBooks(db);

            Console.WriteLine(result);
        }

        //Problem 02
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {

            var books = context.Books
                .AsEnumerable()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(bt => bt)
                .ToList();

            return String.Join(Environment.NewLine, books);
        }

        //Problem 03
        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                .Where(b => (b.EditionType == EditionType.Gold) && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //Problem 04
        public static string GetBooksByPrice(BookShopContext context)
        {

            StringBuilder sb = new StringBuilder();

            var bookTitles = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .ToList();

            foreach (var bt in bookTitles)
            {
                sb.AppendLine($"{bt.Title} - ${bt.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 05
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookTitles = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //Problem 06
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            List<string> bookTitles = new List<string>();

            foreach (var cat in categories)
            {
                List<string> currentBookTitle = context
                    .Books
                    .Where(b => b.BookCategories.Any(bc => bc.Category.Name.ToLower() == cat))
                    .Select(b => b.Title)
                    .ToList();

                bookTitles.AddRange(currentBookTitle);
            }

            bookTitles = bookTitles
                .OrderBy(bt => bt)
                .ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //Problem 07
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime releaseDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                .Where(b => b.ReleaseDate < releaseDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new { b.Title, b.EditionType, b.Price })
                .ToList();

            return String.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"));
        }

        //Problem 8
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var fullName = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                })
                .ToList();

            foreach (var f in fullName)
            {
                sb.AppendLine($"{f.FirstName} {f.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 09
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookTitles = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //Problem 10
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {

            StringBuilder sb = new StringBuilder();

            var bookTitles = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    First = b.Author.FirstName,
                    Last = b.Author.LastName
                })
                .ToList();

            foreach (var b in bookTitles)
            {
                sb.AppendLine($"{b.Title} ({b.First} {b.Last})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 11
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books
                .Count(b => b.Title.Length > lengthCheck);

            return booksCount;
        }

        //Problem 12
        public static string CountCopiesByAuthor(BookShopContext context)
        {

            StringBuilder sb = new StringBuilder();

            var books = context.Authors
                .Select(a =>
                    new
                    {
                        FullName = a.FirstName + ' ' + a.LastName,
                        BooksCount = a.Books.Sum(b => b.Copies)
                    })
                .OrderByDescending(a => a.BooksCount)
                .ToList();

            foreach (var a in books)
            {
                sb.AppendLine($"{a.FullName} - {a.BooksCount}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var profits = context.Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks
                        .Select(cb => new
                        {
                            Bookprofit = cb.Book.Copies * cb.Book.Price
                        })
                        .Sum(cb => cb.Bookprofit)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (var p in profits)
            {
                sb.AppendLine($"{p.Name} ${p.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 14
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    MostRecentBooks = c.CategoryBooks
                        .OrderByDescending(cb => cb.Book.ReleaseDate)
                        .Take(3)
                        .Select(cb => new
                        {
                            BookTitle = cb.Book.Title,
                            ReleaseYear = cb.Book
                                .ReleaseDate.Value.Year
                        })
                        .ToList()
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            foreach (var c in categories)
            {
                sb.AppendLine($"--{c.CategoryName}");

                foreach (var b in c.MostRecentBooks)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.ReleaseYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 15
        public static void IncreasePrices(BookShopContext context)
        {
            var booksToUpdate = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var b in booksToUpdate)
            {
                b.Price += 5;
            }

            context.SaveChanges();

        }

        //Problem 16
        public static int RemoveBooks(BookShopContext context)
        {
            var booksForDelete = context.Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.RemoveRange(booksForDelete);
            context.SaveChanges();

            

            return booksForDelete.Count;
        }
    }
}
