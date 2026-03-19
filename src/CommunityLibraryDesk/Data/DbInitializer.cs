using Bogus;
using CommunityLibraryDesk.Models;
using Microsoft.AspNetCore.Identity;

namespace CommunityLibraryDesk.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminEmail = "admin@library.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (!context.Books.Any())
            {
                var categories = new[] { "Fiction", "History", "Science", "Technology", "Biography" };

                var books = new Faker<Book>()
                    .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                    .RuleFor(b => b.Author, f => f.Name.FullName())
                    .RuleFor(b => b.Isbn, f => f.Random.ReplaceNumbers("978-1-####-#####"))
                    .RuleFor(b => b.Category, f => f.PickRandom(categories))
                    .RuleFor(b => b.IsAvailable, true)
                    .Generate(20);

                context.Books.AddRange(books);
                await context.SaveChangesAsync();
            }

            if (!context.Members.Any())
            {
                var members = new Faker<Member>()
                    .RuleFor(m => m.FullName, f => f.Name.FullName())
                    .RuleFor(m => m.Email, f => f.Internet.Email())
                    .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber())
                    .Generate(10);

                context.Members.AddRange(members);
                await context.SaveChangesAsync();
            }

            if (!context.Loans.Any())
            {
                var books = context.Books.ToList();
                var members = context.Members.ToList();

                var loans = new List<Loan>();

                for (int i = 0; i < 5; i++)
                {
                    var loanDate = DateTime.Today.AddDays(-(20 + i));
                    var dueDate = loanDate.AddDays(7);
                    var returnedDate = dueDate.AddDays(2);

                    loans.Add(new Loan
                    {
                        BookId = books[i].Id,
                        MemberId = members[i % members.Count].Id,
                        LoanDate = loanDate,
                        DueDate = dueDate,
                        ReturnedDate = returnedDate
                    });

                    books[i].IsAvailable = true;
                }

                for (int i = 5; i < 10; i++)
                {
                    var loanDate = DateTime.Today.AddDays(-2);
                    var dueDate = DateTime.Today.AddDays(5);

                    loans.Add(new Loan
                    {
                        BookId = books[i].Id,
                        MemberId = members[i % members.Count].Id,
                        LoanDate = loanDate,
                        DueDate = dueDate,
                        ReturnedDate = null
                    });

                    books[i].IsAvailable = false;
                }

                for (int i = 10; i < 15; i++)
                {
                    var loanDate = DateTime.Today.AddDays(-15);
                    var dueDate = DateTime.Today.AddDays(-5);

                    loans.Add(new Loan
                    {
                        BookId = books[i].Id,
                        MemberId = members[i % members.Count].Id,
                        LoanDate = loanDate,
                        DueDate = dueDate,
                        ReturnedDate = null
                    });

                    books[i].IsAvailable = false;
                }

                context.Loans.AddRange(loans);
                await context.SaveChangesAsync();
            }
        }
    }
}
