namespace CommunityLibraryDesk.Models;

public class Member
{
    public int Id { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}