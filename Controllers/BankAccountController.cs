using InternshipTradingApp.AccountManagement.DTOs;
using InternshipTradingApp.AccountManagement.Entities;
using InternshipTradingApp.AccountManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InternshipTradingApp.Server.Controllers.AccountManagement
{
    [Authorize]
    public class BankAccountController : BaseApiController
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly UserManager<AppUser> _userManager;

        public BankAccountController(IBankAccountService bankAccountService, UserManager<AppUser> userManager)
        {
            _bankAccountService = bankAccountService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<BankAccountResponseDto>>> GetBankAccounts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var accounts = await _bankAccountService.GetBankAccountsForUser(user.Id);
            return Ok(accounts);
        }

        [HttpPost]
        public async Task<ActionResult> AddBankAccount([FromBody] BankAccountDto bankAccountDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                await _bankAccountService.AddBankAccount(user.Id, bankAccountDto);
                return NoContent(); // Operația a avut succes, nu returnăm conținut
            }
            catch (ArgumentException ex) // Exemplu de excepție personalizată
            {
                // Erori legate de validare
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) // Capturăm alte erori neprevăzute
            {
                // Erori interne ale serverului
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

    }
}
