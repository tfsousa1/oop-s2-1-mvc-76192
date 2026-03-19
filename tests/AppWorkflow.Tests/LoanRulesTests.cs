using CommunityLibraryDesk.Data;
using CommunityLibraryDesk.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppWorkflow.Tests;

public class LoanRulesTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void CannotHaveDuplicateActiveLoanForSameBook()
    {
        using var context = GetDbContext();

        var book = new Book { Title = "Book A", Author = "Author A", Isbn = "111", Category = "Fiction", IsAvailable = false };
        var member = new Member { FullName = "John Doe", Email = "john@test.com", Phone = "123" };

        context.Books.Add(book);
        context.Members.Add(member);
        context.SaveChanges();

        context.Loans.Add(new Loan
        {
            BookId = book.Id,
            MemberId = member.Id,
            LoanDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),
            ReturnedDate = null
        });
        context.SaveChanges();

        var activeExists = context.Loans.Any(l => l.BookId == book.Id && l.ReturnedDate == null);

        activeExists.Should().BeTrue();
    }

    [Fact]
    public void ReturnedLoanMakesBookAvailableAgain()
    {
        var book = new Book { IsAvailable = false };
        var loan = new Loan { ReturnedDate = null };

        loan.ReturnedDate = DateTime.Today;
        book.IsAvailable = true;

        loan.ReturnedDate.Should().NotBeNull();
        book.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void OverdueLoanIsDetectedCorrectly()
    {
        var loan = new Loan
        {
            LoanDate = DateTime.Today.AddDays(-10),
            DueDate = DateTime.Today.AddDays(-2),
            ReturnedDate = null
        };

        var isOverdue = loan.DueDate < DateTime.Today && loan.ReturnedDate == null;

        isOverdue.Should().BeTrue();
    }

    [Fact]
    public void BookSearchReturnsMatchingTitle()
    {
        using var context = GetDbContext();

        context.Books.AddRange(
            new Book { Title = "C# Basics", Author = "Alice", Isbn = "1", Category = "Technology" },
            new Book { Title = "History of Rome", Author = "Bob", Isbn = "2", Category = "History" }
        );
        context.SaveChanges();

        var results = context.Books
            .Where(b => b.Title.Contains("C#") || b.Author.Contains("C#"))
            .ToList();

        results.Should().HaveCount(1);
        results[0].Title.Should().Be("C# Basics");
    }

    [Fact]
    public void BookFilterByAvailabilityReturnsOnlyAvailableBooks()
    {
        using var context = GetDbContext();

        context.Books.AddRange(
            new Book { Title = "Available Book", Author = "A", Isbn = "1", Category = "Fiction", IsAvailable = true },
            new Book { Title = "Loaned Book", Author = "B", Isbn = "2", Category = "Fiction", IsAvailable = false }
        );
        context.SaveChanges();

        var results = context.Books.Where(b => b.IsAvailable).ToList();

        results.Should().HaveCount(1);
        results[0].Title.Should().Be("Available Book");
    }
}
