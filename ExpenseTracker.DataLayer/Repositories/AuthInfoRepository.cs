using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Auth;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.DataLayer.Entities;
using AutoMapper;
using ExpenseTracker.DataLayer.Mappings;
using Microsoft.AspNetCore.Http.HttpResults;
using ExpenseTracker.Application.CustomException;
//تیک
namespace ExpenseTracker.DataLayer.Repositories
{
    public class AuthInfoRepository : IAuthInfoRepository
    {
        #region Dependency Injection
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher = new();
        public AuthInfoRepository(AppDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            
        }
        #endregion

        #region Login
        public async Task<UserDto> LoginUser(LoginDto loginDto)
        {
            //آیا ایمیل وجود دارد؟
            var user = await _context.Users
                .FirstOrDefaultAsync(p => p.Email == loginDto.Email.Trim());
            if (user == null)
            {
                throw new UnauthorizedAccessException("ایمیل یا رمز عبور صحیح نیست");
            }

            //بررسی رمز عبور که آیا با مقدار هش شده در دیتابیس یکسان هست؟
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginDto.Password
                );
            // آیا رمز عبور غلط است؟
            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("ایمیل یا رمز عبور صحیح نیست");
            }

            //تبدیل به نوع قابل بازگشت
            return _mapper.Map<UserDto>(user);
        }
        #endregion

        #region Function PasswordHashing 
        public string PasswordHashing(string password)
        {
            var hash = _passwordHasher.HashPassword(new User(), password);

            return hash;
        }
        #endregion

        #region Register
        public async Task<UserDto> RegisterUser(RegisterDto registerDto)
        {
            //آیا ایمیل تکراریه؟
            var checkEmail = await _context.Users
                .AnyAsync(p => p.Email == registerDto.Email.Trim());
            if (checkEmail)
            {
                throw new UnauthorizedAccessException("با این ایمیل قبلا ثبت نام شده");
            }

            //استفاده از تابع رمزنگاری
            string PasswordHash = PasswordHashing(registerDto.Password);

            //افزودن کاربر
            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email.Trim(),
                PasswordHash = PasswordHash,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Users.AddAsync(user);
            
            //ذخیره تغییرات
            await _context.SaveChangesAsync();

            //بازگشت به رمز ساده برای کاربر
            user.PasswordHash = registerDto.Password;
            //تبدیل به نوع قابل بازگشت
            return _mapper.Map<UserDto>(user);
        }
        #endregion
    }
}
