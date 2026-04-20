using System;
using SmartSeatAllocation.Core.Models;

namespace SmartSeatAllocation.Core.DTOs
{
    public class RegisterUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public DepartmentType Department { get; set; }
    }

    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public DateTime CreatedAt { get; set; }

        public static UserDto FromEntity(UserAccount user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Department = user.Department.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }

    public class AuthenticationResponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}
