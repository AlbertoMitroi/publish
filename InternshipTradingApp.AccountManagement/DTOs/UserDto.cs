﻿namespace InternshipTradingApp.AccountManagement.DTOs
{
    public class UserDto
    {
        public required string Username {  get; set; }
        public required string Email { get; set; }
        public required decimal Balance { get; set; }
        public required string Token { get; set; }
    }
}
