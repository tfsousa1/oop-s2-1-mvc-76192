namespace CommunityLibraryDesk.Models;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Genre { get; set; } = "";
    public string ISBN { get; set; } = "";

    public int PublishedYear { get; set; }

    public bool IsAvailable { get; set; } = true;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
