using System;
using System.Collections.Generic;

namespace LibrarySystem
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public bool Available { get; set; }

        public bool IsAvailable() => Available;

        public double CalculateFine()
        {
            // Logic for fine calculation (for example, based on overdue days).
            // For now, returning a mock fine value.
            return 5.0;
        }

        public bool MarkReturned()
        {
            Available = true;
            return true;
        }
    }

    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public List<Book> BooksBorrowed { get; set; } = new List<Book>();
        public int MaxBooksAllowed { get; set; } = 3;

        public List<Book> ListBooksBorrowed()
        {
            return BooksBorrowed;
        }

        public bool CanBorrowMoreBooks()
        {
            return BooksBorrowed.Count < MaxBooksAllowed;
        }
    }

    public class Librarian : User
    {
        public string Role { get; set; }
        public string Course { get; set; }

        public Librarian(string name, string role, string course)
        {
            Name = name;
            Role = role;
            Course = course;
        }
    }

    public class Loan
    {
        public int LoanID { get; set; }
        public Book Book { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public User User { get; set; }
    }

    public class Reservation
    {
        public int ReservationID { get; set; }
        public Book Book { get; set; }
        public User User { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; }

        public void CancelReservation()
        {
            Status = "Cancelled";
        }

        public void FulfillReservation()
        {
            Status = "Fulfilled";
        }
    }

    public class Library
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
        public List<User> Users { get; set; } = new List<User>();
        public List<Loan> Loans { get; set; } = new List<Loan>();
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

        public void AddBook(Book book)
        {
            Books.Add(book);
        }

        public void RegisterMember(User user)
        {
            Users.Add(user);
        }

        public User SearchUserById(int userId)
        {
            return Users.Find(user => user.UserID == userId);
        }

        public List<Book> SearchBookByAuthor(string author)
        {
            return Books.FindAll(book => book.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
        }

        public List<Book> SearchBookByTitle(string title)
        {
            return Books.FindAll(book => book.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        public List<Book> ListAllBooks()
        {
            return Books;
        }

        public List<User> ListAllBorrowingUsers()
        {
            return Users.FindAll(user => user.BooksBorrowed.Count > 0);
        }

        public Loan BorrowBook(Book book, User user)
        {
            if (user.CanBorrowMoreBooks() && book.IsAvailable())
            {
                book.Available = false;
                var loan = new Loan
                {
                    LoanID = Loans.Count + 1,
                    Book = book,
                    User = user,
                    IssueDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14)
                };
                user.BooksBorrowed.Add(book);
                Loans.Add(loan);
                return loan;
            }
            return null;
        }

        public bool ReturnBook(Book book, User user)
        {
            if (user.BooksBorrowed.Contains(book))
            {
                user.BooksBorrowed.Remove(book);
                book.MarkReturned();
                var loan = Loans.Find(l => l.Book == book && l.User == user && l.ReturnDate == null);
                if (loan != null)
                {
                    loan.ReturnDate = DateTime.Now;
                    return true;
                }
            }
            return false;
        }

        public bool ExtendLoan(Book book)
        {
            var loan = Loans.Find(l => l.Book == book && l.ReturnDate == null);
            if (loan != null)
            {
                loan.DueDate = loan.DueDate.AddDays(7); // Extend by 7 days
                return true;
            }
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create library
            var library = new Library { Name = "Central Library", Address = "123 Main St" };

            // Add books to library
            var book1 = new Book { Title = "C# Programming", Author = "John Doe", Publisher = "Tech Books", Available = true };
            var book2 = new Book { Title = "Data Structures", Author = "Jane Smith", Publisher = "Study Press", Available = true };
            library.AddBook(book1);
            library.AddBook(book2);

            // Create users
            var user1 = new User { UserID = 1, Name = "Alice" };
            var user2 = new User { UserID = 2, Name = "Bob" };
            library.RegisterMember(user1);
            library.RegisterMember(user2);

            // Borrow a book
            Console.WriteLine("Borrowing a book...");
            var loan = library.BorrowBook(book1, user1);
            if (loan != null)
                Console.WriteLine($"Book '{book1.Title}' borrowed by {user1.Name}.");

            // Search for books by title
            var booksByTitle = library.SearchBookByTitle("C#");
            Console.WriteLine("Books found by title 'C#':");
            foreach (var b in booksByTitle)
                Console.WriteLine($"- {b.Title} by {b.Author}");

            // Return the book
            Console.WriteLine("\nReturning the book...");
            if (library.ReturnBook(book1, user1))
                Console.WriteLine($"Book '{book1.Title}' returned by {user1.Name}.");

            // Check for borrowed books
            Console.WriteLine("\nBorrowed books:");
            foreach (var b in user1.ListBooksBorrowed())
                Console.WriteLine($"- {b.Title}");

            Console.ReadKey();
        }
    }
}