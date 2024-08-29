using AutoMapper;
using InternshipTradingApp.AccountManagement.DTOs;
using InternshipTradingApp.AccountManagement.Entities;
using InternshipTradingApp.AccountManagement.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InternshipTradingApp.Server.Controllers.AccountManagement
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")] // account/register
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Email)) return BadRequest("Email is taken");

            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = NormalizeUserName(registerDto.FullName);

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            if (user.Email == null) return Unauthorized("Invalid email");

            return new UserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Balance = user.Balance,
                Token = await tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized("Invalid email");

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized("Invalid password");

            if (user.Email == null) return Unauthorized("Invalid email");

            if (user.UserName == null) return Unauthorized("Invalid username");

            return new UserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Balance = user.Balance,
                Token = await tokenService.CreateToken(user)
            };
        }

        [HttpGet("current-user-details")]
        public async Task<ActionResult<UserDto>> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in the claims.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Email == null || user.UserName == null) return NotFound("Email or Username not found");

            return Ok(new UserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Balance = user.Balance,
                Token = string.Empty
            });
        }


        private async Task<bool> UserExists(string email)
        {
            return await userManager.FindByEmailAsync(email) != null;
        }

        private string NormalizeUserName(string fullName)
        {
            return fullName.Trim().Replace(" ", "_").ToLower();
        }   
    }
}
