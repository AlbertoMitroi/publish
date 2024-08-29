using System.ComponentModel.DataAnnotations;

namespace InternshipTradingApp.AccountManagement.Entities
{
    public class BankAccount
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public required string IBAN { get; set; } 
        public required string BankName { get; set; }
        public required AppUser User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

}
