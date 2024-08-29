using InternshipTradingApp.AccountManagement.Data;
using InternshipTradingApp.AccountManagement.Entities;
using InternshipTradingApp.AccountManagement.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InternshipTradingApp.AccountManagement.Services
{
    public class FundsService : IFundsService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AccountDbContext _context;

        public FundsService(UserManager<AppUser> userManager, AccountDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task AddFundsAsync(int userId, decimal amount)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            user.Balance += amount;

            var transaction = new Transaction
            {
                Amount = amount,
                Date = DateTime.UtcNow,
                Type = "Deposit"
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task WithdrawFundsAsync(int userId, decimal amount)
        {
            var user = await _userManager.Users
                .Include(u => u.BankAccounts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            if (user.Balance < amount)
                throw new Exception("Insufficient funds");

            var bankAccount = user.BankAccounts.FirstOrDefault();
            if (bankAccount == null)
                throw new Exception("User has no bank accounts");

            user.Balance -= amount;

            var transaction = new Transaction
            {
                Amount = -amount,
                Date = DateTime.UtcNow,
                BankAccountId = bankAccount.Id,
                Type = "Withdraw",
                BankAccount = bankAccount
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
