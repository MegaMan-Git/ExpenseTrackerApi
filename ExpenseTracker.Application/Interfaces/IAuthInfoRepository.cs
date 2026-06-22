using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//تیک
namespace ExpenseTracker.Application.Interfaces
{
    public interface IAuthInfoRepository
    {
        public Task<UserDto> LoginUser(LoginDto loginDto);
        public Task<UserDto> RegisterUser(RegisterDto registerDto);
        public string PasswordHashing(string password);

    }

}
