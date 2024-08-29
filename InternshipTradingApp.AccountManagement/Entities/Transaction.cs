using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternshipTradingApp.AccountManagement.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;

        // Make BankAccount optional
        public int? BankAccountId { get; set; }
        public virtual BankAccount? BankAccount { get; set; }
    }

}
