using System.ComponentModel;
using System.Globalization;
using System.Text;
using BookShop.Models.Enums;

namespace BookShop;

using BookShop.Models;
using Data;
using Initializer;

public class StartUp
{
    public static void Main()
    {
        using var dbContext = new BookShopContext();
        //DbInitializer.ResetDatabase(dbContext);
        //int input = int.Parse(Console.ReadLine()!);
        
            
        int result = RemoveBooks(dbContext);
        Console.WriteLine(result);
        //Console.WriteLine($"There are {result} books with longer title than {} symbols");
    }

    //02. Age Restriction
    public static string GetBooksByAgeRestriction(BookShopContext dbContext, string command)
    {

        try
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);
            var bookTitles = dbContext.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    //03. Golden Books
    public static string GetGoldenBooks(BookShopContext context)
    {
        var bookTitles = context.Books
            .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
            .OrderBy(b => b.BookId)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, bookTitles);
    }

    //04. Books by Price
    public static string GetBooksByPrice(BookShopContext context)
    {
        var booksTitleAndPrice = context.Books
            .Where(b => b.Price > 40)
            .OrderByDescending(b => b.Price)
            .Select(b => new
            {
                b.Title,
                Price = b.Price.ToString("f2")
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var b in booksTitleAndPrice)
        {
            sb.AppendLine($"{b.Title} - ${b.Price}");
        }

        return sb.ToString().TrimEnd();
    }

    //05. Not Released In
    public static string GetBooksNotReleasedIn(BookShopContext context, int year)
    {
        var booksTitle = context.Books
            .Where(b => b.ReleaseDate!.Value.Year != year)
            .OrderBy(b => b.BookId)
            .Select(b => b.Title)
            .ToArray();

        return string.Join(Environment.NewLine, booksTitle);
    }

    //06. Book Titles by Category
    public static string GetBooksByCategory(BookShopContext context, string input)
    {
        List<string> categories = input
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(c =>c.ToLower())
            .ToList();

        var booksTitleByGivenCategories = context.Books
                .Where(bc => bc.BookCategories
                    .Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToList();
           
        
        return string.Join(Environment.NewLine, booksTitleByGivenCategories);
    }

    //07. Released Before Date
    public static string GetBooksReleasedBefore(BookShopContext context, string date)
    {
        DateTime dateConvert = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
       

        var bookInfo = context.Books
            .Where(b => b.ReleaseDate < dateConvert)
            .OrderByDescending(c => c.ReleaseDate)
            .Select(b => new
            {
                b.Title,
                b.EditionType,
                Price = b.Price.ToString("f2")

            })
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var b in bookInfo)
        {
            sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price}");
        }

        return sb.ToString().TrimEnd();
    }

    //08. Author Search
    public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
    {
        var authors = context.Authors
            .Where(a => a.FirstName.EndsWith(input))
            .Select(a => new
            {
                FullName = $"{a.FirstName} {a.LastName}"
            })
            .OrderBy(a => a.FullName)
            .ToList();

        StringBuilder sb = new StringBuilder();
        foreach (var a in authors)
        {
            sb.AppendLine(a.FullName);
        }

        return sb.ToString().TrimEnd();
    }

    //09. Book Search
    public static string GetBookTitlesContaining(BookShopContext context, string input)
    {
        var bookTitle = context.Books
            .Where(b => b.Title.ToLower().Contains(input.ToLower()))
            .OrderBy(t => t.Title)
            .ToList();

        StringBuilder sb = new StringBuilder();
        foreach (var b in bookTitle)
        {
            sb.AppendLine(b.Title);
        }

        return sb.ToString().TrimEnd();
    }

    //10. Book Search by Author
    public static string GetBooksByAuthor(BookShopContext context, string input)
    {
        var bookAuthorInfo = context.Books
            .Where(ba => ba.Author.LastName.ToLower().StartsWith(input.ToLower()))
            .OrderBy(b => b.BookId)
            .Select(ba => new
            {
                ba.Title,
                AuthorFullName = $"{ba.Author.FirstName} {ba.Author.LastName}"
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var ba in bookAuthorInfo)
        {
            sb.AppendLine($"{ba.Title} ({ba.AuthorFullName})");
        }

        return sb.ToString().TrimEnd();
    }

    //11. Count Books
    public static int CountBooks(BookShopContext context, int lengthCheck)
    {
        var booksCount = context.Books
            .Where(b => b.Title.Length > lengthCheck).Count();

        return booksCount;
    }

    //12. Total Book Copies
    public static string CountCopiesByAuthor(BookShopContext context)
    {
        var authors = context.Authors
            .Select(a => new
            {
                NumberOfCoppies = a.Books.Sum(b => b.Copies),
                AuthorFullName = $"{a.FirstName} {a.LastName}"
            })
            .OrderByDescending(a => a.NumberOfCoppies)
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var a in authors)
        {
            sb.AppendLine($"{a.AuthorFullName} - {a.NumberOfCoppies}");
        }

        return sb.ToString().TrimEnd();
    }

    //13. Profit by Category
    public static string GetTotalProfitByCategory(BookShopContext context)
    {
        var totalProfitByCategory = context.Categories
            //.ToArray()
            .Select(c => new
            {
                CategoryName = c.Name,
                PriceForEachCategory = c.CategoryBooks.Sum(b => b.Book.Copies * b.Book.Price)//.ToString("f2")
            })
            .OrderByDescending(p => p.PriceForEachCategory)
            .ThenBy(c => c.CategoryName)
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var p in totalProfitByCategory)
        {
            sb.AppendLine($"{p.CategoryName} ${p.PriceForEachCategory:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    //14. Most Recent Books
    public static string GetMostRecentBooks(BookShopContext context)
    {
        StringBuilder sb = new StringBuilder();

        var bookInfo = context.Categories
            .Select(a => new
            {
                Category = a.Name,
                Books = a.CategoryBooks.Select(b => new
                    {
                        BookName = b.Book.Title,
                        Date = b.Book.ReleaseDate
                    })
                    .OrderByDescending(b => b.Date)
                    .Take(3)
                    .ToArray()
            })
            .OrderBy(b => b.Category)
            .ToArray();

        foreach (var c in bookInfo)
        {
            sb.AppendLine($"--{c.Category}");

            foreach (var item in c.Books)
            {
                sb.AppendLine($"{item.BookName} ({item.Date!.Value.Year})");
            }
        }

        return sb.ToString().TrimEnd();
    }

    public static void IncreasePrices(BookShopContext context)
    {
        var booksToIncrease = context.Books
            .Where(b => b.ReleaseDate.Value.Year < 2010)
            .ToArray();

        foreach (var book in booksToIncrease)
        {
            book.Price += 5;
        }

        context.SaveChanges();
    }

    public static int RemoveBooks(BookShopContext context)
    {
        var bookToDelete = context.Books
            .Where(b => b.Copies < 4200)
            .ToArray();
        var bookCategories = context.BooksCategories
            .Where(b => b.Book.Copies < 4200)
            .ToArray();

        context.BooksCategories.RemoveRange(bookCategories);

        context.Books.RemoveRange(bookToDelete);
        context.SaveChanges();

        return bookToDelete.Length;

    }

}




