using InternshipTradingApp.AccountManagement.Data;
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
    public class TransactionController : BaseApiController
    {
        private readonly IFundsService _fundsService;
        private readonly UserManager<AppUser> _userManager;

        public TransactionController(IFundsService fundsService, UserManager<AppUser> userManager)
        {
            _fundsService = fundsService;
            _userManager = userManager;
        }

        [HttpPost("deposit")]
        public async Task<ActionResult> DepositFunds([FromBody] AddFundsDto depositFundsDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _fundsService.AddFundsAsync(user.Id, depositFundsDto.Amount);
            return NoContent();
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult> WithdrawFunds([FromBody] WithdrawFundsDto withdrawFundsDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _fundsService.WithdrawFundsAsync(user.Id, withdrawFundsDto.Amount);
            return NoContent();
        }
    }
}
