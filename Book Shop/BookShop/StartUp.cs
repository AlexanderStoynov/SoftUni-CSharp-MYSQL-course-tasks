
namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Text;
    using System.Xml.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            Console.WriteLine(GetMostRecentBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            bool hasParsed = Enum.TryParse(typeof(AgeRestriction), command, true, out object? ageRestrictionObj);
            AgeRestriction ageRestriction;
            if (hasParsed)
            {
                ageRestriction = (AgeRestriction)ageRestrictionObj;


                StringBuilder sb = new StringBuilder();
                var bookTitles = context.Books
                    .Where(b => b.AgeRestriction == ageRestriction)
                    .OrderBy(b => b.Title)
                    .Select(b => b.Title)
                    .ToArray();

                foreach (var bookTitle in bookTitles)
                {
                    sb.AppendLine(bookTitle);
                }

                return sb.ToString().TrimEnd();
            }

            return null;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, goldenBooks);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categories = new List<string>(input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => c.ToLower()).ToList());

            var bookTitles = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            string end = input;

            var names = context.Authors
                .Where(a => a.FirstName.EndsWith(end))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => $"{a.FirstName} {a.LastName}")
                .ToArray();

            return string.Join(Environment.NewLine, names);

        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var bookCopiesByAuthor = context.Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.TotalCopies)
                .ToArray();

            foreach (var bookCopies in bookCopiesByAuthor)
            {
                sb.AppendLine(bookCopies.FullName + " - " + bookCopies.TotalCopies);
            }

            return sb.ToString();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var bookProfitByCategory = context.Categories
                .Select(c => new
                {
                    c.Name,
                    Price = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)

                })
                .OrderByDescending(c => c.Price)
                .ThenBy(c => c.Name)
                .ToArray();

            foreach(var c in bookProfitByCategory)
            {
                sb.AppendLine($"{c.Name} ${c.Price:f2}");
            }

            return sb.ToString();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var mostRecentBook = context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Name,
                    Books = c.CategoryBooks
                    .OrderByDescending(cb => cb.Book.ReleaseDate)
                    .Take(3)
                    .Select(cb => new
                    {
                        BookTittle = cb.Book.Title,
                        BookReleaseDate = cb.Book.ReleaseDate.Value.Year
                    })
                    .ToArray()

                })
                .ToArray();

            foreach (var item in mostRecentBook)
            {
                sb.AppendLine($"--{item.Name}");
                foreach (var book in item.Books) 
                { 
                    sb.AppendLine($"{book.BookTittle} ({book.BookReleaseDate})");
                }
            }    
            
            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var increasePriceBy5 = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010 )
                .ToArray();

            foreach(var item in increasePriceBy5)
            {
                item.Price += 5;
            }

            context.SaveChanges();
        }
    }

    
}


