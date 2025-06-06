﻿namespace BankSystem.Server.Dtos
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? Role { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
