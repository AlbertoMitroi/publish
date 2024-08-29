using System.ComponentModel.DataAnnotations;

namespace InternshipTradingApp.AccountManagement.DTOs
{
    public class BankAccountDto
    {
        [Required]
        [StringLength(34, MinimumLength = 12, ErrorMessage = "IBAN must be between 12 and 34 characters.")]
        public string IBAN { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Bank Name must be between 10 and 100 characters.")]
        public string BankName { get; set; } = string.Empty;
    }
}
