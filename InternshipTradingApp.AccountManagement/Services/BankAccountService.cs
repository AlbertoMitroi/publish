using InternshipTradingApp.AccountManagement.Data;
using InternshipTradingApp.AccountManagement.DTOs;
using InternshipTradingApp.AccountManagement.Entities;
using InternshipTradingApp.AccountManagement.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InternshipTradingApp.AccountManagement.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly AccountDbContext _context;

        public BankAccountService(AccountDbContext context)
        {
            _context = context;
        }

        public async Task<List<BankAccountResponseDto>> GetBankAccountsForUser(int userId)
        {
            return await _context.BankAccounts
                .Where(b => b.UserId == userId)
                .Select(b => new BankAccountResponseDto
                {
                    Id = b.Id,
                    IBAN = b.IBAN,
                    BankName = b.BankName
                })
                .ToListAsync();
        }

        public async Task AddBankAccount(int userId, BankAccountDto bankAccountDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var bankAccount = new BankAccount
            {
                UserId = userId,
                User = user,
                IBAN = bankAccountDto.IBAN,
                BankName = bankAccountDto.BankName
            };

            _context.BankAccounts.Add(bankAccount);
            await _context.SaveChangesAsync();

            var addedBankAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(b => b.UserId == userId && b.IBAN == bankAccountDto.IBAN);

            if (addedBankAccount == null)
                throw new Exception("Failed to add bank account.");
        }
    }
}
